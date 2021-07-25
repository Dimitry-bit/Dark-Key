using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkKey.Gameplay;


namespace DarkKey.Rooms.FirstRoom
{
    public class PictureFrame : ItemHolder
    {
        private Material _currentPicture;
        private PictureItem _pictureObject;
        // Start is called before the first frame update
        void Start()
        {
            // _pictureObject = GetComponent<PictureItem>();
            //_currentPicture = GetComponent<Renderer>().material;
            _pictureObject = GetComponent<PictureItem>();

        }

        // Update is called once per frame

        public override void Interact(PlayerInteraction playerInteraction)
        {
            GenericItem ItemInHand = playerInteraction.GetItemType();

            if (ItemInHand == null) return;

            if (ItemInHand.TryGetComponent(out PictureItem pictureScript))
            {
                GetItemFromPlayer(playerInteraction);
                ChangePicture(pictureScript);
            }



        }

        private void ChangePicture(PictureItem pictureScript)
        {
            GetComponent<Renderer>().material = pictureScript.GetPicture();

        }

    }
}
