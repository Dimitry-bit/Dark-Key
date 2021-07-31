using UnityEngine;

namespace DarkKey.Rooms.FirstRoom.Item_Frame
{
    public class RandomPictures : MonoBehaviour
    {
        [SerializeField] private Material[] listOfMaterials;
        private PictureFrame[] itemFrames;
        private int _TakenMaterialIndex;
        void Start()
        {
            itemFrames = GetComponentsInChildren<PictureFrame>();
            Randomizer();
        }
        private void Randomizer()
        {
            foreach (var itemFrame in itemFrames)
            {
                for (var x = 0; x < 1; x--)
                {
                    _TakenMaterialIndex = Random.Range(0, listOfMaterials.Length);
                    if (listOfMaterials[_TakenMaterialIndex] != null)
                        break;
                }
                itemFrame.requiredPicture = listOfMaterials[_TakenMaterialIndex];
                listOfMaterials[_TakenMaterialIndex] = null;
            }
        }
    }
}
