using System.Collections;
using DarkKey.Gameplay;
using MLAPI;
using TMPro;
using UnityEngine;

namespace DarkKey.Ui
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

            if (!IsLocalPlayer) return;

            playerInteraction.OnInteractableSelected += EnableUi;
            playerInteraction.OnInteractableDeselected += DisableUi;
        }

        private void OnDestroy()
        {
            if (!IsLocalPlayer) return;
            if (playerInteraction == null) return;

            playerInteraction.OnInteractableSelected += EnableUi;
            playerInteraction.OnInteractableDeselected += DisableUi;
        }

        #endregion

        #region Private Methods

        private void SetInteractionText(string text) => interactionText.text = $"Press 'E' To {text}";

        private void EnableUi()
        {
            interactionPanel.SetActive(true);
            SetInteractionText("Interact");
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