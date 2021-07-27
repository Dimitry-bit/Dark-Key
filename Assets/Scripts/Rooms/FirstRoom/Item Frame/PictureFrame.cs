using DarkKey.Gameplay.Interaction;
using UnityEngine;

namespace DarkKey.Rooms.FirstRoom.Item_Frame
{
    public class PictureFrame : ItemHolder
    {
        [SerializeField] private Material requiredPicture;
        private Material _currentMaterial;
        private Renderer _currentPicture;
        private PictureItem _pictureObject;
        private bool _hasPicture;
        private bool _hasRequiredPicture;
        public bool hasRequiredPicture => _hasRequiredPicture;

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
                    CheckPicture(pictureScript);
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
        private void CheckPicture(PictureItem pictureScript)
        {
            if (pictureScript.GetPicture() == requiredPicture)
            {
                _hasRequiredPicture = true;
            }
        }
        #endregion
    }
}
