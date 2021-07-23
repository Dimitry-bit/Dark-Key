// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using DarkKey.Gameplay;
// using DarkKey.Gameplay.Interfaces;
//
// namespace DarkKey
// {
//     public class CassetteTest : MonoBehaviour, IInteractable
//     {
//         private CD _cdObject;
//         private ItemHolder _itemHolderObject;
//         private bool hasCD;
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
