using Mirror;
using TMPro;
using UnityEngine;

namespace DarkKey.Ui.Debug_Panels.InfoPlugins
{
    public class DisplayServerFps : NetworkBehaviour, IInfoPlugin
    {
        private TMP_Text _serverFpsTMP;
        private NetworkIdentity _networkIdentity;

        [SyncVar]
        private float _serverFPS;

        public void InitializePlugin(TMP_Text textMeshPro)
        {
            _networkIdentity = GetComponentInParent<NetworkIdentity>();
            _serverFpsTMP = textMeshPro;
            DisplayServerFPS();
        }

        public void UpdateUi() => DisplayServerFPS();

        private void DisplayServerFPS()
        {
            if (_serverFpsTMP == null) return;

            if (NetworkManager.singleton == null || !NetworkManager.singleton.isNetworkActive)
            {
                if (_serverFpsTMP.gameObject.activeSelf)
                    _serverFpsTMP.gameObject.SetActive(false);

                return;
            }

            if (_networkIdentity.isServer)
            {
                if (_serverFpsTMP.gameObject.activeSelf)
                    _serverFpsTMP.gameObject.SetActive(false);

                UpdateServerFps();
            }
            else
            {
                if (!_serverFpsTMP.gameObject.activeSelf)
                    _serverFpsTMP.gameObject.SetActive(true);

                _serverFpsTMP.text = $"ServerFPS: {_serverFPS}";
            }
        }

        [Server]
        private void UpdateServerFps() => _serverFPS = CalculateFPS();

        private float CalculateFPS() => 1 / Time.smoothDeltaTime;
    }
}