using UnityEngine;

namespace DarkKey.Rooms.FirstRoom.Item_Frame
{
    [RequireComponent(typeof(AudioSource))]
    public class PictureOrderChecker : MonoBehaviour
    {
        private PictureFrame[] _frameObject;
        private AudioSource _audioSourceObject;
        private bool _playedSound;

        #region Unity Methods
        void Start()
        {
            _frameObject = GetComponentsInChildren<PictureFrame>();
            _audioSourceObject = GetComponent<AudioSource>();
            _audioSourceObject.playOnAwake = false;
        }
        void Update()
        {
            if (CheckOrder())
                PlayWinSound();
        }
        #endregion

        #region Private Methods
        private bool CheckOrder()
        {
            if (TruePictures() == _frameObject.Length)
                return true;
            else
            {
                _playedSound = false;
                return false;
            }
        }
        private void PlayWinSound()
        {
            if (_playedSound) return;

            _audioSourceObject.Play();
            _playedSound = true;
        }
        private int TruePictures()
        {
            int _numberOfTruePictures = 0;

            foreach (PictureFrame pictureFrame in _frameObject)
            {
                if (pictureFrame.hasRequiredPicture == true)
                    _numberOfTruePictures++;
            }
            return _numberOfTruePictures;
        }
        #endregion
    }
}
