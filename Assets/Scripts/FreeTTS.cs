using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class FreeTTS : MonoBehaviour
{
    public AudioSource audioSource;

    public void Speak(string text)
    {
        if (string.IsNullOrEmpty(text)) return;
        string shortText = text.Length > 200 ? text.Substring(0, 200) : text;
        StartCoroutine(GetVoice(shortText));
    }

    IEnumerator GetVoice(string text)
    {
        string url = "https://translate.google.com/translate_tts?ie=UTF-8&total=1&idx=0&textlen=" + 
                     text.Length + "&client=tw-ob&q=" + UnityWebRequest.EscapeURL(text) + "&tl=fr";

        using var request = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            audioSource.clip = DownloadHandlerAudioClip.GetContent(request);
            audioSource.Play();
        }
    }
}