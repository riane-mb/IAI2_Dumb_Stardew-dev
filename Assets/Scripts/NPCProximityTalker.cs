using UnityEngine;
 // Indispensable

public class NPCProximityTalker : MonoBehaviour
{
    public NPCAgent npc;
    public Transform player;
    public float talkDistance = 2f;
    
    [Header("XR Settings")]
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor handInteractor; 

    private bool _hasSpokenAboutThisObject = false;
    private GameObject _lastObject;

    void Update()
    {
        if (player == null || npc == null || handInteractor == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= talkDistance)
        {
            GameObject grabbedObject = GetGrabbedObject(); 

            if (grabbedObject != null && grabbedObject != _lastObject)
            {
                string itemName = grabbedObject.name;
                npc.Speak($"Oh ! C'est quoi ce que tu as là ? Dis-moi en plus sur ce/cette {itemName}."); // L'agent parle en fonction du nom de l'objet
                
                _lastObject = grabbedObject;
                _hasSpokenAboutThisObject = true;
            }
        }
        else
        {
            // Reset quand on s'éloigne
            _lastObject = null;
            _hasSpokenAboutThisObject = false;
        }
    }

    GameObject GetGrabbedObject()
    {
        // Vérifie si l'interrupteur a une sélection active
        if (handInteractor.interactablesSelected.Count > 0)
        {
            return handInteractor.interactablesSelected[0].transform.gameObject;
        }
        return null;
    }
}