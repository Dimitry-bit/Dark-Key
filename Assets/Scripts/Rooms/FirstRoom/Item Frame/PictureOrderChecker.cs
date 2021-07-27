using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkKey.Rooms.FirstRoom
{
    [RequireComponent(typeof(AudioSource))]
    public class PictureOrderChecker : MonoBehaviour
    {
        private PictureFrame[] _frameObject;
        private AudioSource _audioSource;
        private bool[] _isOrderTrue;
        void Start()
        {
            _frameObject = GetComponentsInChildren<PictureFrame>();
            _audioSource = GetComponent<AudioSource>();
            _audioSource.playOnAwake = false;
        }

        void Update()
        {
                CheckOrder();
        }

        private void CheckOrder()
        {
            int _numberOfTruePictures = 0;

            foreach (PictureFrame pictureFrame in _frameObject)
            {
                if (pictureFrame.hasRequiredPicture == true)
                    _numberOfTruePictures++;
            }
            Debug.Log(_numberOfTruePictures);

            if (_numberOfTruePictures == _frameObject.Length)
            {
                PlayWinSound();
                Debug.Log("Sound Has Played");
            }
        }
        private void PlayWinSound()
        {
            _audioSource.Play();
        }

    }
}
