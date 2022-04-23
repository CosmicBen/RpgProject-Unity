using Rpg.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Rpg.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        [SerializeField] private string playerName;

        private Dialogue currentDialogue;
        private DialogueNode currentNode;
        private bool isChoosing = false;
        private AiConversant currentConversant;

        public event Action OnConversationUpdated;

        public void StartDialogue(AiConversant newConversant, Dialogue newDialogue)
        {
            currentConversant = newConversant;
            currentDialogue = newDialogue;
            currentNode = currentDialogue.GetRootNode();
            TriggerEnterAction();
            OnConversationUpdated.Invoke();
        }

        public void Quit()
        {
            TriggerExitAction();
            currentConversant = null;
            currentDialogue = null;
            currentNode = null;
            isChoosing = false;
            OnConversationUpdated.Invoke();
        }

        public bool IsActive()
        {
            return currentDialogue != null;
        }

        public bool IsChoosing()
        {
            return isChoosing;
        }

        public string GetText()
        {
            if (currentNode == null) { return ""; }
            return currentNode.GetText();
        }

        public void SelectChoice(DialogueNode chosenNode)
        {
            currentNode = chosenNode;
            TriggerEnterAction();
            isChoosing = false;
            Next();
        }

        public void Next()
        {
            int numberPlayerResponses = FilterCondition(currentDialogue.GetPlayerChildren(currentNode)).Count();
            if (numberPlayerResponses > 0)
            {
                isChoosing = true;
                TriggerExitAction();
            }
            else
            {
                DialogueNode[] children = FilterCondition(currentDialogue.GetAiChildren(currentNode)).ToArray();
                int randomIndex = UnityEngine.Random.Range(0, children.Length);
                TriggerExitAction();
                currentNode = children[randomIndex];
                TriggerEnterAction();
            }
                        
            OnConversationUpdated.Invoke();
        }

        public bool HasNext()
        {
            return FilterCondition(currentDialogue.GetAllChildren(currentNode)).Count() > 0;
        }

        public string GetCurrentConversantName()
        {
            if (isChoosing)
            {
                return playerName;
            }
            else
            {
                return currentConversant.GetName();
            }
        }

        public IEnumerable<DialogueNode> GetChoices()
        {
            return FilterCondition(currentDialogue.GetPlayerChildren(currentNode));
        }

        private IEnumerable<DialogueNode> FilterCondition(IEnumerable<DialogueNode> inputNode)
        {
            foreach (DialogueNode node in inputNode)
            {
                if (node.CheckCondition(GetEvaluators()))
                {
                    yield return node;
                }
            }
        }

        private IEnumerable<IPredicateEvaluator> GetEvaluators()
        {
            return GetComponents<IPredicateEvaluator>();
        }

        private void TriggerEnterAction()
        {
            if (currentNode == null || currentNode.GetOnEnterAction() == "") { return; }
            TriggerAction(currentNode.GetOnEnterAction());
        }

        private void TriggerExitAction()
        {
            if (currentNode == null || currentNode.GetOnExitAction() == "") { return; }
            TriggerAction(currentNode.GetOnExitAction());
        }

        private void TriggerAction(string action)
        {
            foreach(DialogueTrigger trigger in currentConversant.GetComponents<DialogueTrigger>())
            {
                trigger.Trigger(action);
            }
        }
    }
}