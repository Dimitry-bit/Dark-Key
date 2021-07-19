using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkKey
{
    public class Cassette : MonoBehaviour
    {
        private CD _cdObject;
        void Start()
        {
            _cdObject = new CD();
        }
        void OnCollisionEnter(Collision collidedObject)
        {
            //Checks if the collided object is a CD
            if (collidedObject.gameObject.layer == 10)
            {
                _cdObject.PlayAudio();
            }
        }
    }
}
