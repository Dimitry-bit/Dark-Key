using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DarkKey.Gameplay.Interaction
{
    public static class ItemUtility
    {
        private static Dictionary<string, GameObject> _itemTable;

        private static void Initialize()
        {
            _itemTable = new Dictionary<string, GameObject>();

            // var spawnableItems = Resources.LoadAll("Spawnable", typeof(GenericItem)).Cast<GenericItem>().ToArray();
            var spawnableItems = Resources.LoadAll("Spawnable", typeof(GameObject)).Cast<GameObject>().ToArray();
            foreach (var spawnableItem in spawnableItems)
            {
                // if (!_itemTable.ContainsKey(spawnableItem.ItemName))
                // {
                //     _itemTable.Add(spawnableItem.ItemName, spawnableItem.gameObject);
                //     Debug.Log($"Added {spawnableItem.name} to ItemTable.");
                // }
                if (!_itemTable.ContainsKey(spawnableItem.name))
                {
                    _itemTable.Add(spawnableItem.name, spawnableItem.gameObject);
                    Debug.Log($"Added {spawnableItem.name} to ItemTable.");
                }
            }
        }

        public static GameObject GetPrefabByName(string prefabName)
        {
            if (_itemTable == null)
                Initialize();

            if (!_itemTable.ContainsKey(prefabName))
                return new GameObject("Prefab_Missing");

            return _itemTable[prefabName];
        }

        public static GameObject GetPrefabByType(ItemTypes types) =>
            GetPrefabByName(types.ToString());
    }
}