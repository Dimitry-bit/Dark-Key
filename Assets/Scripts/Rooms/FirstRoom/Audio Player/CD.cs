using DarkKey.Gameplay.Interaction;
using UnityEngine;

namespace DarkKey.Rooms.FirstRoom.Audio_Player
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