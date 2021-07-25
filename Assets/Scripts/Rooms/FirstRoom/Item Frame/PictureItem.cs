using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkKey.Gameplay;


namespace DarkKey.Rooms.FirstRoom
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
