using System.Collections;
using Mirror;
using UnityEngine;

namespace DarkKey.Gameplay.Interaction
{
    public class SceneObject : NetworkBehaviour
    {
        [SyncVar(hook = nameof(OnChangeEquipment))]
        public ItemTypes equippedItem;

        private void OnChangeEquipment(ItemTypes oldEquippedItem, ItemTypes newEquippedItem) =>
            StartCoroutine(ChangeEquipment(newEquippedItem));

        private IEnumerator ChangeEquipment(ItemTypes newEquippedItem)
        {
            while (transform.childCount > 0)
            {
                Destroy(transform.GetChild(0).gameObject);
                yield return null;
            }

            SetItem(newEquippedItem);
        }

        public void SetItem(ItemTypes newItem)
        {
            var newItemPrefab = ItemUtility.GetPrefabByType(newItem);
            Instantiate(newItemPrefab, transform);
        }
    }
}