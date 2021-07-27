using DarkKey.Gameplay.Interaction;
using UnityEngine;

namespace DarkKey.Rooms.FirstRoom.Item_Frame
{
    public class PictureItem : GenericItem
    {
        [SerializeField] private Material picture;
        public Material _picture => picture;
        public Material GetPicture()
        {
            return picture;
        }

    }
}
