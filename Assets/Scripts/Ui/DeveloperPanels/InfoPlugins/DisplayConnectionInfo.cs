using Mirror;
using TMPro;
using UnityEngine;

namespace DarkKey.Ui.DeveloperPanels.InfoPlugins
{
    public class DisplayConnectionInfo : MonoBehaviour, IInfoPlugin
    {
        private TMP_Text _clientIdTMP;
        private bool _hasInitialized;

        public void InitializePlugin(TMP_Text textMeshPro)
        {
            _clientIdTMP = textMeshPro;
            DisplayClientId();
        }

        public void UpdateUi() { }

        private void DisplayClientId()
        {
            if (_clientIdTMP == null) return;

            if (!_hasInitialized)
            {
                if (_clientIdTMP.gameObject.activeSelf)
                    _clientIdTMP.gameObject.SetActive(false);

                return;
            }

            if (NetworkManager.singleton == null || !NetworkManager.singleton.isNetworkActive) return;
            if (NetworkClient.localPlayer == null) return;

            if (!_clientIdTMP.gameObject.activeSelf)
                _clientIdTMP.gameObject.SetActive(true);

            _clientIdTMP.text = $"ID: {NetworkClient.localPlayer.netId}";
            _hasInitialized = true;
        }
    }
}