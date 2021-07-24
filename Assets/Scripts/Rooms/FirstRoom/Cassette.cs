using UnityEngine;
using DarkKey.Gameplay;

namespace DarkKey
{
    [RequireComponent(typeof(AudioSource))]
    public class Cassette : ItemHolder
    {
        private ItemHolder _itemHolderObject;
        private AudioSource _audioSource;
        private bool hasCD;
        void Start()
        {
            _itemHolderObject = GetComponent<ItemHolder>();
            _audioSource = GetComponent<AudioSource>();
            _audioSource.playOnAwake = false;
        }
        public override void Interact(PlayerInteraction playerInteraction)
        {
            GenericItem itemCDInHand = playerInteraction.GetItemType();

            if (hasCD)
            {
                if (itemCDInHand == null)
                {
                    AssignItemToPlayer(playerInteraction);
                    hasCD = false;
                }
            }
            else
            {
                if (itemCDInHand.TryGetComponent(out CD cdScript))
                {
                    HoldItem(itemCDInHand);
                    PlayAudio(cdScript);
                    hasCD = true;
                }
            }
        }
        private void PlayAudio(CD cdScript)
        {
            _audioSource.clip = cdScript.GetAudioClip();
            _audioSource.Play();
        }
    }
}
