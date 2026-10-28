using System.Collections.Generic;
using UnityEngine;
using TMPro;   // for TextMeshPro display

/// <summary>
/// A simple conversational NPC agent.
/// Attach to a GameObject. Assign the LLMClient and a TMP text display.
/// Call Speak(playerInput) from your interaction system.
/// </summary>
public class NPCAgent : MonoBehaviour
{
    [Header("References")]
    public LLMClient llmClient;
    public TextMeshProUGUI speechBubble;   // assign in Inspector

    [Header("Persona")]
    [TextArea(3, 8)]
    // Prompt de base qui est changé dans l'éditeur Unity
    public string systemPrompt = @"You are Ma'am Annie, a kind and hardworking farmer. Speak in a simple, rustic way with folksy charm. Mention your garden, your hens, or the weather. Be warm, grounded, and keep your answers very brief (3-4 sentences).";

    private List<ChatMessage> _history = new List<ChatMessage>();
    private bool _isThinking = false;
    public KokoroTTS tts; // Optional Text-to-Speech component

    /// <summary>Call this from your player interaction / voice input system.</summary>
    public void Speak(string playerInput)
    {
        if (_isThinking) return;

        // Add to conversation history
        _history.Add(new ChatMessage { role = "user", content = playerInput });

        // Show "thinking" state
        speechBubble.text = "...";
        _isThinking = true;

        StartCoroutine(llmClient.Chat(
            _history.ToArray(),
            systemPrompt,
            onComplete: (response) =>
            {
                _history.Add(new ChatMessage { role = "assistant", content = response });
                speechBubble.text = response;
                _isThinking = false;

                // Optional: also feed to TTS
                if (tts != null) tts.Speak(response);
            },
            onError: (err) =>
            {
                speechBubble.text = "[The guide seems distracted...]";
                _isThinking = false;
            }
        ));
    }

    /// <summary>Reset conversation — e.g., when player walks away.</summary>
    public void ResetConversation()
    {
        _history.Clear();
        speechBubble.text = "";
    }
}