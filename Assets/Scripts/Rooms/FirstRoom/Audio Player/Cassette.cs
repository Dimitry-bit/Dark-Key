using DarkKey.Gameplay.Interaction;
using UnityEngine;

namespace DarkKey.Rooms.FirstRoom.Audio_Player
{
    [RequireComponent(typeof(AudioSource))]
    public class Cassette : ItemHolder
    {
        private AudioSource _audioSource;
        private bool _hasCd;

        #region Unity Methods
        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.playOnAwake = false;
        }
        #endregion

        #region Public Methods
        public override void Interact(PlayerInteraction playerInteraction)
        {
            GenericItem ItemInHand = playerInteraction.GetItemType();

            if (_hasCd)
            {
                if (ItemInHand != null) return;
                StopAudio();
                AssignItemToPlayer(playerInteraction);
                _hasCd = false;
            }
            else
            {
                if (ItemInHand == null) return;

                if (ItemInHand.TryGetComponent(out CD cdScript))
                {
                    GetItemFromPlayer(playerInteraction);
                    PlayAudio(cdScript);
                    _hasCd = true;
                    
                }
            }
        }
        #endregion

        #region Private Methods
        private void PlayAudio(CD cdScript)
        {
            _audioSource.clip = cdScript.GetAudioClip();
            _audioSource.Play();
        }

        private void StopAudio()
        {
            _audioSource.Stop();
        }
        #endregion
    }
}