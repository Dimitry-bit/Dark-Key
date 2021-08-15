using System;
using Mirror;
using TMPro;
using UnityEngine;

namespace DarkKey.DeveloperTools.DeveloperPanels.InfoPlugins
{
    public class DisplayPing : MonoBehaviour, IInfoPlugin
    {
        private TMP_Text _pingTMP;

        public void InitializePlugin(TMP_Text textMeshPro)
        {
            _pingTMP = textMeshPro;
            DisplayPingUi();
        }

        public void UpdateUi() => DisplayPingUi();

        private void DisplayPingUi()
        {
            if (_pingTMP == null) return;

            if (NetworkManager.singleton == null || !NetworkManager.singleton.isNetworkActive)
            {
                if (_pingTMP.gameObject.activeSelf)
                    _pingTMP.gameObject.SetActive(false);

                return;
            }

            if (!_pingTMP.gameObject.activeSelf)
                _pingTMP.gameObject.SetActive(true);

            var rtt = $"RTT: {Math.Round(NetworkTime.rtt * 1000)}ms";
            _pingTMP.text = rtt;
        }
    }
}