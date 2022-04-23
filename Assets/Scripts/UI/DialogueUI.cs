using Rpg.Dialogue;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rpg.Ui
{
    public class DialogueUI : MonoBehaviour
    {
        private PlayerConversant playerConversant;
        [SerializeField] private TextMeshProUGUI aiText;
        [SerializeField] private Button nextButton;
        [SerializeField] private Transform choiceRoot;
        [SerializeField] private GameObject choicePrefab;
        [SerializeField] private Transform aiResponse;
        [SerializeField] private Button quitButton;
        [SerializeField] private TextMeshProUGUI conversantName;

        private void Awake()
        {
            nextButton.onClick.AddListener(Next);
        }

        private void Start()
        {
            playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
            playerConversant.OnConversationUpdated += UpdateUi;

            quitButton.onClick.AddListener(() => { playerConversant.Quit(); });

            UpdateUi();
        }

        private void Next()
        {
            playerConversant.Next();
        }

        private void UpdateUi()
        {
            gameObject.SetActive(playerConversant.IsActive());
            if (!playerConversant.IsActive()) { return; }

            conversantName.text = playerConversant.GetCurrentConversantName();
            aiResponse.gameObject.SetActive(!playerConversant.IsChoosing());
            choiceRoot.gameObject.SetActive(playerConversant.IsChoosing());

            if (playerConversant.IsChoosing())
            {
                BuildChoiceList();
            }
            else
            {
                aiText.text = playerConversant.GetText();
                nextButton.gameObject.SetActive(playerConversant.HasNext());
            }
        }

        private void BuildChoiceList()
        {
            foreach (Transform choice in choiceRoot)
            {
                Destroy(choice.gameObject);
            }

            foreach (DialogueNode choice in playerConversant.GetChoices())
            {
                GameObject choiceInstance = Instantiate(choicePrefab, choiceRoot);
                TextMeshProUGUI textComponent = choiceInstance.GetComponentInChildren<TextMeshProUGUI>();
                textComponent.text = choice.GetText();
                Button button = choiceInstance.GetComponentInChildren<Button>();
                button.onClick.AddListener(() => { playerConversant.SelectChoice(choice); });
            }
        }
    }
}