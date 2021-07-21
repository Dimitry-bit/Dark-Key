using UnityEngine;

namespace DarkKey.Gameplay.Spawn
{
    public class PlayerSpawnPoint : MonoBehaviour
    {
        private void Awake() => PlayerSpawner.AddSpawnPoint(transform);
        private void OnDestroy() => PlayerSpawner.RemoveSpawnPoint(transform);

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(transform.position, 1f);
            Gizmos.color = Color.green; 
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2);
        }
    }
}