using DarkKey.Gameplay;
using UnityEngine;

namespace DarkKey.Rooms.FirstRoom
{
    [RequireComponent(typeof(AudioSource))]
    public class Cassette : ItemHolder
    {
        private AudioSource _audioSource;
        private bool _hasCd;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.playOnAwake = false;
        }

        public override void Interact(PlayerInteraction playerInteraction)
        {
            GenericItem disk = playerInteraction.GetItemType();

            if (_hasCd)
            {
                if (disk != null) return;

                AssignItemToPlayer(playerInteraction);
                _hasCd = false;
                
                // StopAudio
            }
            else
            {
                if (disk == null) return;

                if (disk.TryGetComponent(out CD cdScript))
                {
                    HoldItem(disk);
                    PlayAudio(cdScript);
                    _hasCd = true;
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