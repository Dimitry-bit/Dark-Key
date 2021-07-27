using System.Collections.Generic;
using DarkKey.Core.Network;
using MLAPI;
using MLAPI.Configuration;
using TMPro;
using UnityEngine;

namespace DarkKey.Ui.Debug_Panels
{
    public class GameJumpStart : BaseDebugPanel
    {
        [Header("JumpStart Options")]
        [SerializeField] private NetworkPrefab spawnAblePlayerPrefab;
        [SerializeField] private GameObject networkManagerPrefab;
        [SerializeField] private GameObject netPortalPrefab;
        [SerializeField] private TMP_InputField inputField;
        private bool _hasInitialized;

        protected override void InitializePanel()
        {
        }

        #region Public Methods

        public void StartHosting()
        {
            if (IsHost) return;

            ValidateNetworkObjects();

            if (!_hasInitialized)
                InitializeNetworkManagerNetworkPrefabs();

            var password = inputField.text;
            NetPortal.Instance.Host(password);
        }

        public void StopHosting()
        {
            if (!IsHost) return;

            NetworkManager.Singleton.StopHost();
        }

        #endregion

        #region Private Methods

        private void InitializeNetworkManagerNetworkPrefabs()
        {
            bool doesPlayerAlreadyExists = false;
            List<NetworkPrefab> networkPrefabs = NetworkManager.Singleton.NetworkConfig.NetworkPrefabs;

            foreach (var networkPrefab in networkPrefabs)
            {
                networkPrefab.PlayerPrefab = false;
                if (networkPrefab.Prefab != spawnAblePlayerPrefab.Prefab) continue;

                networkPrefab.PlayerPrefab = true;
                doesPlayerAlreadyExists = true;
            }

            if (!doesPlayerAlreadyExists)
                NetworkManager.Singleton.NetworkConfig.NetworkPrefabs.Add(spawnAblePlayerPrefab);

            _hasInitialized = true;
        }

        private void ValidateNetworkObjects()
        {
            if (NetworkManager.Singleton == null)
                Instantiate(networkManagerPrefab, Vector3.zero, Quaternion.identity);

            if (NetPortal.Instance == null)
                Instantiate(netPortalPrefab, Vector3.zero, Quaternion.identity);
        }

        #endregion
    }
}