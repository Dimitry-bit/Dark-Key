using UnityEngine;

namespace DarkKey.Core
{
    public class InputHandler : MonoBehaviour
    {
        public Vector2 InputVector { get; set; }

        private void Update()
        {
            GetMovementInput();
        }

        private void GetMovementInput()
        {
            var x = Input.GetAxis("Horizontal");
            var y = Input.GetAxis("Vertical");

            InputVector = new Vector3(x,y);
        }
    }
}