using UnityEngine;

public class LLMManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // --- C'EST ICI QU'ON ÉCRIT LA 3e ÉTAPE ---

        // 1. On charge le fichier depuis le dossier Assets/Resources/
        TextAsset configFile = Resources.Load<TextAsset>("llm_config");

        if (configFile != null)
        {
            Debug.Log("Fichier de config trouvé !");

            // 2. On affiche le contenu dans la console pour vérifier
            Debug.Log("Contenu : " + configFile.text);

            // C'est ici que vous ajouterez plus tard le code pour 
            // extraire l'API Key et l'envoyer au LLM.
        }
        else
        {
            Debug.LogError("Erreur : Le fichier llm_config.txt est introuvable dans Assets/Resources/"); 
        }
    }
}
