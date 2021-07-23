// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using DarkKey.Gameplay;
// using DarkKey.Gameplay.Interfaces;
//
// namespace DarkKey
// {
//     public class Cassette : MonoBehaviour, IInteractable
//     {
//         private CD _cdObject;
//         private ItemHolder _itemHolderObject;
//         private bool hasCD;
//         void OnCollisionEnter(Collision collidedObject)
//         {
//             if (hasCD) return;
//             _cdObject = collidedObject.gameObject.GetComponent<CD>();
//
//             //Checks if the collided object is a CD
//             if (collidedObject.gameObject.layer == 10)
//             {
//                 _cdObject.PlayAudio();
//                 hasCD = true;
//             }
//         }
//         public virtual void Interact(Player player)
//         {
//             switch (player.IsHoldingItem())
//             {
//                 case (false):
//                     if (player.ItemInHand.Value.gameObject.layer == 10)
//                     {
//                         _itemHolderObject = GetComponent<ItemHolder>();
//                         _cdObject = player.ItemInHand.Value.GetComponent<CD>();
//
//                         player.RemoveItemFromHand();
//                         _itemHolderObject.HoldItem(player);
//                         _cdObject.PlayAudio();
//                         hasCD = true;
//                     }
//                     break;
//                 default:
//                     if (!player.IsHoldingItem())
//                         _itemHolderObject.GiveItemToPlayer(player);
//                     break;
//             }
//         }
//     }
// }
