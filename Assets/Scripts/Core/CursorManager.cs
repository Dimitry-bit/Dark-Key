using UnityEngine;

namespace DarkKey.Core
{
    public class CursorManager : MonoBehaviour
    {
        public static void ShowCursor()
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }

        public static void HideCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}