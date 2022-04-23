using Rpg.Control;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rpg.Dialogue
{
    public class AiConversant : MonoBehaviour, IRaycastable
    {
        [SerializeField] private string conversantName;
        [SerializeField] private Dialogue dialogue;

        public CursorType GetCursorType()
        {
            return CursorType.Dialogue;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (dialogue == null) { return false; }

            if (Input.GetMouseButtonDown(0))
            {
                PlayerConversant conversant = callingController.GetComponent<PlayerConversant>();
                conversant.StartDialogue(this, dialogue);

            }

            return true;
        }

        public string GetName()
        {
            return conversantName;
        }
    }
}