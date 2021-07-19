using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkKey
{
    public class CD : MonoBehaviour
    {
        private AudioSource _audioClip;
        void Start()
        {
            _audioClip = GetComponent<AudioSource>();
        }
        //Plays the audio contined in the CD
        public void PlayAudio()
        {
            _audioClip.Play();
        }
    }
}
