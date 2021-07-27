using UnityEngine;
using UnityEngine.EventSystems;

namespace DarkKey.Ui.Utilities
{
    public class DraggableUi : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        private Vector2 _offset;
        [SerializeField] private GameObject debugPanelGameObject;

        private void Start()
        {
            if (debugPanelGameObject == null) debugPanelGameObject = gameObject;
        }

        public void OnBeginDrag(PointerEventData eventData) =>
            _offset = eventData.position - new Vector2(debugPanelGameObject.transform.position.x,
                debugPanelGameObject.transform.position.y);

        public void OnDrag(PointerEventData eventData)
        {
            if (Input.GetKey(KeyCode.Mouse0))
                DragUi();
            else if (Input.GetKey(KeyCode.Mouse1)) 
                ResizeUi();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
        }

        private void DragUi()
        {
            var newPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - _offset;

            // Min value is '0' because we are calculating position in world_space not local_space.
            var xClamped = Mathf.Clamp(newPosition.x, 0, Screen.width);
            var yClamped = Mathf.Clamp(newPosition.y, 0, Screen.height);

            debugPanelGameObject.transform.position = new Vector3(xClamped, yClamped, 0);
        }

        private void ResizeUi()
        {
        }
    }
}