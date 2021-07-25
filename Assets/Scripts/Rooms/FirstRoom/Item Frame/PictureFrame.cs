using UnityEngine;
using DarkKey.Gameplay;

namespace DarkKey.Rooms.FirstRoom
{
    public class PictureFrame : ItemHolder
    {
        private Renderer _currentPicture;
        private PictureItem _pictureObject;
        private bool _hasPicture;

        #region Unity Methods
        void Start()
        {
            _currentPicture = GetComponent<Renderer>();
            _pictureObject = GetComponent<PictureItem>();
        }
        #endregion

        #region Public Methods
        public override void Interact(PlayerInteraction playerInteraction)
        {
            GenericItem ItemInHand = playerInteraction.GetItemType();

            if (_hasPicture)
            {
                if (ItemInHand != null) return;

                RemovePicture();
                AssignItemToPlayer(playerInteraction);
                _hasPicture = false;
            }
            else
            {
                if (ItemInHand.TryGetComponent(out PictureItem pictureScript))
                {
                    GetItemFromPlayer(playerInteraction);
                    ChangePicture(pictureScript);
                    _hasPicture = true;
                }
            }
        }
        #endregion

        #region Private Methods
        private void ChangePicture(PictureItem pictureScript)
        {
            _currentPicture.material = pictureScript.GetPicture();
        }
        private void RemovePicture()
        {
            _currentPicture.material = Resources.Load("ItemFrame", typeof(Material)) as Material;
        }
        #endregion
    }
}
