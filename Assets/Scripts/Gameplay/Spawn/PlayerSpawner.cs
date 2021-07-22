using System.Collections.Generic;
using System.Linq;
using DarkKey.Core.Debugger;
using DarkKey.Core.Network;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.Spawning;
using UnityEngine;

namespace DarkKey.Gameplay.Spawn
{
    public class PlayerSpawner : NetworkBehaviour
    {
        private static readonly DebugLogLevel[] ScriptLogLevel = {DebugLogLevel.Core};

        [SerializeField] private NetworkObject playerPrefab;

        private static List<Transform> _spawnPoints = new List<Transform>();
        private int _spawnPointIndex;

        #region Unity Methods

        public override void NetworkStart()
        {
            CustomDebugger.LogInfo("PlayerSpawner", "Network Start", ScriptLogLevel);
            NetPortal.Instance.OnSceneSwitch += SpawnPlayerServerRpc;
        }

        private void OnDestroy()
        {
            if (NetPortal.Instance != null)
                NetPortal.Instance.OnSceneSwitch -= SpawnPlayerServerRpc;
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

        [ServerRpc]
        private void SpawnPlayerServerRpc(PlayerData playerData)
        {
            DestroyPlayerObject(playerData.ClientId);

            var spawnPoint = GetSpawnPoint();

            InstantiatePlayer(playerData, spawnPoint);

            _spawnPointIndex++;

            CustomDebugger.LogInfo("PlayerSpawner", "Player spawned successfully.", ScriptLogLevel);
        }

        private Transform GetSpawnPoint()
        {
            var spawnPoint = _spawnPoints[_spawnPointIndex];

            if (spawnPoint != null) return spawnPoint;

            CustomDebugger.LogError("PlayerSpawner", $"Missing spawn point for player {_spawnPointIndex}",
                ScriptLogLevel);

            return null;
        }

        private void InstantiatePlayer(PlayerData playerData, Transform spawnPointTransform)
        {
            var position = spawnPointTransform.position;
            var rotation = spawnPointTransform.rotation;

            NetworkObject playerInstance = Instantiate(playerPrefab, position, rotation);
            // TODO: Add Player.Name and Player.Role
            playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(playerData.ClientId);
        }

        private void DestroyPlayerObject(ulong clientId)
        {
            NetworkSpawnManager.GetPlayerNetworkObject(clientId).Despawn(true);
        }

        #endregion
    }
}