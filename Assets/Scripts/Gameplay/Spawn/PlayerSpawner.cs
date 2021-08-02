using System.Collections.Generic;
using System.Linq;
using DarkKey.Core.Debugger;
using DarkKey.Core.Network;
using DarkKey.Gameplay.CorePlayer;
using Mirror;
using UnityEngine;

namespace DarkKey.Gameplay.Spawn
{
    public class PlayerSpawner : NetworkBehaviour
    {
        private static readonly DebugLogLevel[] ScriptLogLevel = {DebugLogLevel.Core};

        [SerializeField] private NetworkIdentity playerPrefab;

        private static List<Transform> _spawnPoints = new List<Transform>();
        private int _spawnPointIndex;

        #region Unity Methods

        public void NetworkStart()
        {
            CustomDebugger.LogInfo("Network Start", ScriptLogLevel);
            NetPortal.Instance.OnSceneSwitch += CmdSpawnPlayer;
        }

        private void OnDestroy()
        {
            if (NetPortal.Instance != null)
                NetPortal.Instance.OnSceneSwitch -= CmdSpawnPlayer;
        }

        #endregion

        #region Public Methods

        public static void AddSpawnPoint(Transform transform)
        {
            _spawnPoints.Add(transform);
            _spawnPoints = _spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
        }

        public static void RemoveSpawnPoint(Transform transform) => _spawnPoints.Remove(transform);

        #endregion

        #region Private Methods

        [Command]
        private void CmdSpawnPlayer(PlayerData playerData)
        {
            DestroyPlayerObject(playerData.ClientId);

            var spawnPoint = GetSpawnPoint();

            InstantiatePlayer(playerData, spawnPoint);

            _spawnPointIndex++;

            CustomDebugger.LogInfo("Player spawned successfully.", ScriptLogLevel);
        }

        private Transform GetSpawnPoint()
        {
            var spawnPoint = _spawnPoints[_spawnPointIndex];

            if (spawnPoint != null) return spawnPoint;

            CustomDebugger.LogError($"Missing spawn point for player {_spawnPointIndex}",
                ScriptLogLevel);

            return null;
        }

        private void InstantiatePlayer(PlayerData playerData, Transform spawnPointTransform)
        {
            var position = spawnPointTransform.position;
            var rotation = spawnPointTransform.rotation;

            NetworkIdentity playerInstance = Instantiate(playerPrefab, position, rotation);
            // TODO: Add Player.Name and Player.Role
            playerInstance.GetComponent<NetworkIdentity>().SpawnAsPlayerObject(playerData.ClientId);
            playerInstance.GetComponent<ScenePlayer>().InitializePlayerData(playerData);
        }

        private void DestroyPlayerObject(ulong clientId)
        {
            if (NetworkSpawnManager.GetPlayerNetworkObject(clientId) != null)
                NetworkSpawnManager.GetPlayerNetworkObject(clientId).Despawn(true);
        }

        #endregion
    }
}