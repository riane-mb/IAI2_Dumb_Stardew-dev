using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Thin wrapper around the OpenAI Chat Completions REST API.
/// Works with Groq, LM Studio, or any OpenAI-compatible endpoint.
/// </summary>
public class LLMClient : MonoBehaviour
{
    [Header("Config (leave blank to load from Resources/llm_config.txt)")]
    public string baseUrl  = "";
    public string apiKey   = "";
    public string model    = "";

    private string _baseUrl;
    private string _apiKey;
    private string _model;

    void Awake()
    {
        // Load from inspector fields first, then fall back to config file
        _baseUrl = !string.IsNullOrEmpty(baseUrl) ? baseUrl : LoadConfig("baseUrl");
        _apiKey  = !string.IsNullOrEmpty(apiKey)  ? apiKey  : LoadConfig("apiKey");
        _model   = !string.IsNullOrEmpty(model)   ? model   : LoadConfig("model");
    }

    // ── Public API ────────────────────────────────────────────────────

    /// <summary>
    /// Send a list of messages and call onComplete with the response string.
    /// Use this from any MonoBehaviour: StartCoroutine(client.Chat(...))
    /// </summary>
    public IEnumerator Chat(
        ChatMessage[] messages,
        string systemPrompt,
        Action<string> onComplete,
        Action<string> onError = null)
    {
        string url  = _baseUrl.TrimEnd('/') + "/chat/completions";
        string body = BuildRequestBody(messages, systemPrompt);
        byte[] bodyBytes = Encoding.UTF8.GetBytes(body);

        using var request = new UnityWebRequest(url, "POST");
        request.uploadHandler   = new UploadHandlerRaw(bodyBytes);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + _apiKey);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            string err = $"LLMClient error: {request.responseCode} {request.error}";
            Debug.LogError(err);
            onError?.Invoke(err);
            yield break;
        }

        string content = ParseResponse(request.downloadHandler.text);
        onComplete?.Invoke(content);
    }

    // ── Private helpers ───────────────────────────────────────────────

    string BuildRequestBody(ChatMessage[] messages, string systemPrompt)
    {
        // Hand-build JSON to avoid requiring a JSON library dependency
        var sb = new StringBuilder();
        sb.Append("{");
        sb.Append($"\"model\":\"{_model}\",");
        sb.Append("\"messages\":[");

        // System prompt first
        if (!string.IsNullOrEmpty(systemPrompt))
        {
            sb.Append("{\"role\":\"system\",\"content\":");
            sb.Append(JsonString(systemPrompt));
            sb.Append("},");
        }

        // Conversation messages
        for (int i = 0; i < messages.Length; i++)
        {
            sb.Append("{\"role\":\"");
            sb.Append(messages[i].role);
            sb.Append("\",\"content\":");
            sb.Append(JsonString(messages[i].content));
            sb.Append("}");
            if (i < messages.Length - 1) sb.Append(",");
        }

        sb.Append("],");
        sb.Append("\"max_tokens\":512,");
        sb.Append("\"temperature\":0.7");
        sb.Append("}");
        return sb.ToString();
    }

    string ParseResponse(string json)
    {
        // Minimal JSON extraction — find content field in first choice
        // For production use, import Newtonsoft.Json (available via Package Manager)
        const string marker = "\"content\":\"";
        int start = json.IndexOf(marker);
        if (start < 0) return "[no content in response]";
        start += marker.Length;

        var sb = new StringBuilder();
        for (int i = start; i < json.Length; i++)
        {
            if (json[i] == '"' && json[i - 1] != '\\') break;
            if (json[i] == '\\' && i + 1 < json.Length)
            {
                i++;
                switch (json[i])
                {
                    case 'n': sb.Append('\n'); break;
                    case 't': sb.Append('\t'); break;
                    case '"': sb.Append('"');  break;
                    case '\\': sb.Append('\\'); break;
                    default: sb.Append(json[i]); break;
                }
            }
            else sb.Append(json[i]);
        }
        return sb.ToString();
    }

    string JsonString(string s)
    {
        // Escape a string for embedding in JSON
        return "\"" + s.Replace("\\","\\\\").Replace("\"","\\\"")
                        .Replace("\n","\\n").Replace("\r","\\r")
                        .Replace("\t","\\t") + "\"";
    }

    string LoadConfig(string key)
    {
        var file = Resources.Load<TextAsset>("llm_config");
        if (file == null) { Debug.LogError("llm_config.txt not found in Resources/"); return ""; }
        foreach (var line in file.text.Split('\n'))
        {
            var parts = line.Trim().Split('=');
            if (parts.Length == 2 && parts[0].Trim() == key)
                return parts[1].Trim();
        }
        return "";
    }
}

// Simple data struct — no MonoBehaviour needed
[Serializable]
public struct ChatMessage
{
    public string role;      // "user" | "assistant" | "system"
    public string content;
}