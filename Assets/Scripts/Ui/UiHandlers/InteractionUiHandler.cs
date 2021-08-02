using System.Collections;
using DarkKey.Gameplay.Interaction;
using Mirror;
using TMPro;
using UnityEngine;

namespace DarkKey.Ui.UiHandlers
{
    public class InteractionUiHandler : NetworkBehaviour
    {
        [SerializeField] private PlayerInteraction playerInteraction;
        [SerializeField] private GameObject interactionPanel;
        [SerializeField] private TMP_Text interactionText;

        private Coroutine _disableUiCoroutine;

        #region Unity Methods

        private void Start()
        {
            interactionPanel.SetActive(false);

            if (!isLocalPlayer) return;

            playerInteraction.OnInteractableSelected += EnableUi;
            playerInteraction.OnInteractableDeselected += DisableUi;
        }

        private void OnDestroy()
        {
            if (!isLocalPlayer) return;
            if (playerInteraction == null) return;

            playerInteraction.OnInteractableSelected += EnableUi;
            playerInteraction.OnInteractableDeselected += DisableUi;
        }

        #endregion

        #region Private Methods

        private void SetInteractionText(string text) => interactionText.text = $"Press 'E' To {text}";

        private void EnableUi(Interactable interactable)
        {
            interactionPanel.SetActive(true);
            SetInteractionText(interactable.InteractionDescription);
        }

        private void DisableUi()
        {
            if (_disableUiCoroutine != null) StopCoroutine(_disableUiCoroutine);
            _disableUiCoroutine = StartCoroutine(TimedDisableUi(0.2f));
        }

        private IEnumerator TimedDisableUi(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            interactionPanel.gameObject.SetActive(false);
        }

        #endregion
    }
}