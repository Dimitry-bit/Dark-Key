using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkKey
{
    public class Cassette : MonoBehaviour
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
    }
}
