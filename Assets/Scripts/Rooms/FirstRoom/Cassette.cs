using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkKey.Gameplay;
using DarkKey.Gameplay.Interfaces;

namespace DarkKey
{
    public class Cassette : MonoBehaviour, IInteractable
    {
        private CD _cdObject;
        private bool hasCD;
        void OnCollisionEnter(Collision collidedObject)
        {
            if (hasCD) return;
            _cdObject = collidedObject.gameObject.GetComponent<CD>();

            //Checks if the collided object is a CD
            if (collidedObject.gameObject.layer == 10)
            {
                _cdObject.PlayAudio();
                hasCD = true;
            }
        }
        public virtual void Interact(Player player)
        {
            if (!hasCD)
            {
                if (player.IsHoldingItem() && player.ItemInHand.Value.gameObject.layer == 10)
                {
                    player.RemoveItemFromHand();
                    _cdObject = player.ItemInHand.Value.GetComponent<CD>();
                    _cdObject.PlayAudio();
                    hasCD = true;
                }
            }
            else
            {
                if (!player.IsHoldingItem())
                {
                    player.AssignItemToHand();
                }
            }
        }
    }
}
