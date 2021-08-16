// using DarkKey.Gameplay.Interaction;
// using UnityEngine;
//
// namespace DarkKey.Rooms.FirstRoom.Item_Frame
// {
//     public class PictureFrame : ItemHolder
//     {
//         public Material requiredPicture;
//         private Material _itemFrameMaterial;
//         private Renderer _currentPicture;
//         private bool _hasPicture;
//         private bool _hasRequiredPicture;
//         public bool hasRequiredPicture => _hasRequiredPicture;
//
//         #region Unity Methods
//
//         void Start()
//         {
//             _currentPicture = GetComponent<Renderer>();
//
//             if (_currentPicture != null)
//                 _itemFrameMaterial = _currentPicture.material;
//         }
//
//         #endregion
//
//         #region Public Methods
//
//         public void Interact(PlayerInteraction playerInteraction)
//         {
//             // GenericItem ItemInHand = playerInteraction.GetItemType();
//             //
//             // if (_hasPicture)
//             // {
//             //     if (ItemInHand != null) return;
//             //
//             //     RemovePicture();
//             //     AssignItemToPlayer(playerInteraction);
//             //     _hasPicture = false;
//             // }
//             // else
//             // {
//             //     if (ItemInHand.TryGetComponent(out PictureItem pictureScript))
//             //     {
//             //         GetItemFromPlayer(playerInteraction);
//             //         ChangePicture(pictureScript);
//             //         CheckPicture(pictureScript);
//             //         _hasPicture = true;
//             //     }
//             // }
//         }
//
//         #endregion
//
//         #region Private Methods
//
//         private void ChangePicture(PictureItem pictureScript)
//         {
//             _currentPicture.material = pictureScript.GetPicture();
//         }
//
//         private void RemovePicture()
//         {
//             _currentPicture.material = _itemFrameMaterial;
//         }
//
//         private void CheckPicture(PictureItem pictureScript)
//         {
//             if (pictureScript.GetPicture() == requiredPicture)
//
//                 _hasRequiredPicture = true;
//             else
//                 _hasRequiredPicture = false;
//         }
//
//         #endregion
//     }
// }