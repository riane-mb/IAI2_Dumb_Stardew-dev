using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class KokoroTTS : MonoBehaviour
{
    public AudioSource audioSource;
    
    [SerializeField] private string kokoroServerUrl = "http://192.168.1.61:8888/tts";
    [SerializeField] private string voiceName = "ff_siwis"; // Voix française de Kokoro

    private Coroutine currentVoiceCoroutine; // Pour gérer les appels concurrents

    public void Speak(string text)
    {
        if (string.IsNullOrEmpty(text)) return;

        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        if (currentVoiceCoroutine != null)
        {
            StopCoroutine(currentVoiceCoroutine);
        }
        
        currentVoiceCoroutine = StartCoroutine(GetKokoroVoice(text));
    }

    IEnumerator GetKokoroVoice(string text)
    {
        string url = $"{kokoroServerUrl}?text={UnityWebRequest.EscapeURL(text)}&voice={voiceName}";

        using var request = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            audioSource.clip = DownloadHandlerAudioClip.GetContent(request);
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("Petit souci avec Pierre : " + request.error);
        }

        currentVoiceCoroutine = null;
    }
}