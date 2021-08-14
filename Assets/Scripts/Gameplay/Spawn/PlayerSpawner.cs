using System.Collections.Generic;
using System.Linq;
using DarkKey.Core.Debugger;
using DarkKey.Core.Managers;
using DarkKey.Core.Network;
using Mirror;
using UnityEngine;

namespace DarkKey.Gameplay.Spawn
{
    public class PlayerSpawner : NetworkBehaviour
    {
        private static readonly DebugLogLevel[] ScriptLogLevel = {DebugLogLevel.Core};

        [SerializeField] private bool autoSpawnPlayerOnSceneStart = true;

        [SerializeField] private NetworkIdentity playerPrefab;
        private static List<Transform> _spawnPoints = new List<Transform>();
        private int _spawnPointIndex;

        #region Unity Methods

        public override void OnStartServer()
        {
            if (!autoSpawnPlayerOnSceneStart) return;
            Invoke(nameof(ReplaceAllPlayersGameObjects), 1f);
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

        private Transform GetSpawnPoint()
        {
            var spawnPoint = _spawnPoints[_spawnPointIndex];

            if (spawnPoint != null) return spawnPoint;

            ServiceLocator.Instance.GetDebugger().LogError($"Missing spawn point for player {_spawnPointIndex}",
                ScriptLogLevel);

            return null;
        }

        private void ReplacePlayerGameObject(NetworkIdentity oldPlayer, NetworkIdentity newPlayer)
        {
            Transform spawnPoint = GetSpawnPoint();
            var position = spawnPoint.position;
            var rotation = spawnPoint.rotation;

            if (newPlayer == null || oldPlayer == null)
            {
                var messageDiff = oldPlayer == null ? "oldPlayer" : "newPlayer";

                ServiceLocator.Instance.GetDebugger().LogError(
                    $"Failed to spawn player ({messageDiff} can't be null).", ScriptLogLevel);
                return;
            }

            var isSpawned = NetworkServer.ReplacePlayerForConnection(oldPlayer.connectionToClient,
                Instantiate(newPlayer.gameObject, position, rotation));

            if (!isSpawned)
            {
                ServiceLocator.Instance.GetDebugger().LogError("Failed to spawn player.", ScriptLogLevel);
                return;
            }

            ServiceLocator.Instance.GetDebugger().LogInfo("Player spawned successfully.", ScriptLogLevel);

            // TODO: Add Player.Name && Player.Role.
            // TODO: Initialize PlayerData in newly created playerGameObject. 

            _spawnPointIndex++;
        }

        private void DestroyPlayer(GameObject playerGameObject)
        {
            string playerGameObjectName = playerGameObject.name;
            NetworkServer.Destroy(playerGameObject);

            ServiceLocator.Instance.GetDebugger().LogInfo($"{playerGameObjectName} Destroyed successfully.",
                ScriptLogLevel);
        }

        private void ReplaceAllPlayersGameObjects()
        {
            ServiceLocator.Instance.GetDebugger().LogInfo("Started replacing all players GameObjects.",
                ScriptLogLevel);

            foreach (var lobbyPlayer in NetPortal.Instance.LobbyPlayers)
            {
                ReplacePlayerGameObject(lobbyPlayer.netIdentity, playerPrefab);
                DestroyPlayer(lobbyPlayer.gameObject);
            }

            ServiceLocator.Instance.GetDebugger().LogInfo("Finished replacing all players GameObjects.",
                ScriptLogLevel);
        }

        #endregion
    }
}