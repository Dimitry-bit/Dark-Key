using DarkKey.Core;
using MLAPI;
using MLAPI.Messaging;
using UnityEngine;

namespace DarkKey.Guide
{
    public class NetworkPlayerGuide : NetworkBehaviour
    {
        public override void NetworkStart()
        {
            base.NetworkStart();

            NetworkingGuide.Instance.OnLocalConnection += Test;
            NetworkingGuide.Instance.OnAnyConnection += Test2;
        }

        private void OnDestroy()
        {
            NetworkingGuide.Instance.OnLocalConnection -= Test;
            NetworkingGuide.Instance.OnAnyConnection -= Test2;
        }

        private void Update()
        {
            // Stops the block from getting called from every 'NetworkPlayerGuide' script in the scene.
            if (!IsLocalPlayer) return;

            // Checks if Space is pressed.
            if (Input.GetKeyDown(KeyCode.Space))
                TestServerRpc();
        }

        /// The callback to invoke once a client connects. This callback is only ran on the server and on the local client that connects. 
        /// (Different SenderID) !!
        private void Test()
        {
            CustomDebugger.Instance.LogInfo($"LocalPlayerOnly: {OwnerClientId} Joined");
        }

        /// The callback to invoke once a client connects. This callback is only ran on the local client that connects.
        /// (Same SenderID)  !!
        private void Test2()
        {
            CustomDebugger.Instance.LogInfo($"AllPlayers: {OwnerClientId} Joined");
        }
        

        // ServerRpcs get called from owned objects only !! To override behaviour use [ServerRpc(RequireOwnership = false)]
        [ServerRpc]
        // Called from a client. Runs on the server only.
        private void TestServerRpc()
        {
            CustomDebugger.Instance.LogInfo(
                $"Player: {OwnerClientId} CalledServerRpc, HostStatus: {NetworkManager.ServerClientId == OwnerClientId})");

            if (NetworkManager.ServerClientId == OwnerClientId) TestClientRpc();
        }
        
        [ClientRpc]
        // Called from the server. Runs on every client.
        private void TestClientRpc()
        {
            CustomDebugger.Instance.LogInfo($"Player: {OwnerClientId} CalledClientRpc");
        }
    }
}