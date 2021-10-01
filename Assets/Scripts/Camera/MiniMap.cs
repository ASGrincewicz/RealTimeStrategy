using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using Mirror;

public class MiniMap : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] private RectTransform _miniMapRect = null;
    [SerializeField] private float _mapScale = 60f;
    [SerializeField] private float _offset = -30f;//Should be equal to mapScale but negative.

    private Transform _playerCameraTransform = null;

    private void Update()
    {
        if (_playerCameraTransform != null) return;

        if (NetworkClient.connection.identity == null) return;

        _playerCameraTransform = NetworkClient.connection.identity.GetComponent<NetworkedPlayer>().GetCameraTransform();
    }

    private void MoveCamera()
    {
        var mousePos = Mouse.current.position.ReadValue();

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_miniMapRect, mousePos, null, out Vector2 localPoint)) return;

        var lerp = new Vector2((localPoint.x - _miniMapRect.rect.x) / _miniMapRect.rect.width,
                               (localPoint.y - _miniMapRect.rect.y) / _miniMapRect.rect.height);

        var newCameraPos = new Vector3(Mathf.Lerp(-_mapScale, _mapScale, lerp.x),
                                        _playerCameraTransform.position.y, Mathf.Lerp(-_mapScale, _mapScale, lerp.y));

        _playerCameraTransform.position = newCameraPos + new Vector3(0, 0, _offset);
    }

    public void OnPointerDown(PointerEventData eventData) => MoveCamera();

    public void OnDrag(PointerEventData eventData) => MoveCamera();
}
