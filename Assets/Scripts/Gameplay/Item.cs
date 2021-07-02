using MLAPI;
using Unity.Collections;
using UnityEngine;

namespace DarkKey.Gameplay
{
    public class Item : NetworkBehaviour
    {
        [ReadOnly] public Vector3 InHandOffset;
        [ReadOnly] public Vector3 InItemHolderOffset;
    }
}