using UnityEngine;
using DarkKey.Gameplay;

namespace DarkKey
{
    public class CD : GenericItem
    {
        [SerializeField] private AudioClip _audioclip;
        public AudioClip audioClip => _audioclip;
        public AudioClip GetAudioClip()
        {
            return audioClip;
        }
    }
}