using UnityEngine;

namespace DarkKey.Gameplay.Spawn
{
    public class PlayerSpawnPoint : MonoBehaviour
    {
        private void Awake() => PlayerSpawner.AddSpawnPoint(transform);
        private void OnDestroy() => PlayerSpawner.RemoveSpawnPoint(transform);

        private void OnDrawGizmos()
        {
            var currentPosition = transform.position;

            Gizmos.color = Color.black;
            Gizmos.DrawSphere(currentPosition, 1f);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(currentPosition, currentPosition + transform.forward * 2);
        }
    }
}