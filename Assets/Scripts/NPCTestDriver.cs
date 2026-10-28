using UnityEngine;
using UnityEngine.InputSystem;

public class NPCTestDriver : MonoBehaviour
{
    public NPCAgent npc;
    public string testInput = "Parle moi de ta journée !";

    void Update()
    {
        // Vérification pour la Manette (Bouton A / South)
        bool buttonAPressed = Gamepad.current != null && Gamepad.current.aButton.wasPressedThisFrame;

        if (buttonAPressed)
        {
            npc.Speak(testInput);
        }
    }
}