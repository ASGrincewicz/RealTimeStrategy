using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class CameraController :NetworkBehaviour
{
    [SerializeField] private Transform _playerCameraTransform = null;
    [SerializeField] private float _speed = 20f;
    [SerializeField] private float _screenBorderThickness = 10f;
    [SerializeField] private Vector2 _screenXLimits = Vector2.zero;
    [SerializeField] private Vector2 _screenZLimits = Vector2.zero;
    private Vector2 _previousInput;
    private Controls _controls;
    private float _deltaTime;

    public override void OnStartAuthority()
    {
        _playerCameraTransform.gameObject.SetActive(true);

        _controls = new Controls();

        _controls.Player.MoveCamera.performed += SetPreviousInput;
        _controls.Player.MoveCamera.canceled += SetPreviousInput;

        _controls.Enable();
    }

    [ClientCallback]

    private void Update()
    {
        if (!hasAuthority || !Application.isFocused) return;

        _deltaTime = Time.deltaTime;

        UpdateCameraPosition();
    }
    private void UpdateCameraPosition()
    {
        var pos = _playerCameraTransform.position;
        if(_previousInput == Vector2.zero)
        {
            var cursorMovement = Vector3.zero;

            var cursorPosition = Mouse.current.position.ReadValue();

            if(cursorPosition.y > Screen.height - _screenBorderThickness)
            {
                cursorMovement.z += 1;
            }
            else if(cursorPosition.y <= _screenBorderThickness )
            {
                cursorMovement.z -= 1;
            }

            if (cursorPosition.x > Screen.width - _screenBorderThickness)
            {
                cursorMovement.x += 1;
            }
            else if (cursorPosition.x <= _screenBorderThickness)
            {
                cursorMovement.x -= 1;
            }

            pos += cursorMovement.normalized * _speed * _deltaTime;
        }
        else
        {
            pos += new Vector3(_previousInput.x, 0f, _previousInput.y) * _speed * _deltaTime;
        }

        pos.x = Mathf.Clamp(pos.x, _screenXLimits.x, _screenXLimits.y);
        pos.z = Mathf.Clamp(pos.z, _screenZLimits.x, _screenZLimits.y);

        _playerCameraTransform.position = pos;
    }
    private void SetPreviousInput(InputAction.CallbackContext ctx)
    {
        _previousInput = ctx.ReadValue<Vector2>();
    }
}
