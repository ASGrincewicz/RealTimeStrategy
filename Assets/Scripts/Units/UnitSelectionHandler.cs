using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask = new LayerMask();
    [SerializeField] private RectTransform _unitSelectionArea = null;

    private Vector2 _startPosition;
    private NetworkedPlayer _player;
    private Camera _mainCamera;
    private List<Unit> _selectedUnits = new List<Unit>();

    public List<Unit> SelectedUnits { get => _selectedUnits; }


    private void Start()
    {
        _mainCamera = Camera.main;

        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;

        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }

    private void OnDestroy()
    {
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }

    private void Update()
    {
        if (_player == null)
        {
            _player = NetworkClient.connection.identity.GetComponent<NetworkedPlayer>();
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartSelectionArea();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ClearSelectionArea();
        }
        else if (Mouse.current.leftButton.isPressed)
        {
            UpdateSelectionArea();
        }
    }
    private void StartSelectionArea()
    {

        if (!Keyboard.current.leftShiftKey.isPressed)
        {
            foreach (Unit selectedUnit in _selectedUnits)
            {
                selectedUnit.Deselect();
            }
            _selectedUnits.Clear();
        }

        _unitSelectionArea.gameObject.SetActive(true);

        _startPosition = Mouse.current.position.ReadValue();

        UpdateSelectionArea();
    }

    private void UpdateSelectionArea()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        float areaWidth = mousePosition.x - _startPosition.x;
        float areaHeight = mousePosition.y - _startPosition.y;

        _unitSelectionArea.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Mathf.Abs(areaHeight));

        _unitSelectionArea.anchoredPosition = _startPosition + new Vector2(areaWidth / 2, areaHeight / 2);
    }

    private void ClearSelectionArea()
    {
        _unitSelectionArea.gameObject.SetActive(false);

        if (_unitSelectionArea.sizeDelta.magnitude == 0)
        {
            Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _layerMask)) return;

            if (!hit.collider.TryGetComponent<Unit>(out Unit unit)) return;

            if (!unit.hasAuthority) return;

            _selectedUnits.Add(unit);

            foreach (Unit selectedUnit in _selectedUnits)
            {
                selectedUnit.Select();
            }
            return;
        }

        Vector2 min = _unitSelectionArea.anchoredPosition - (_unitSelectionArea.sizeDelta / 2);
        Vector2 max = _unitSelectionArea.anchoredPosition + (_unitSelectionArea.sizeDelta / 2);

        foreach (Unit unit in _player.GetMyUnits())
        {
            if (SelectedUnits.Contains(unit)) continue;

            Vector3 screenPosition = _mainCamera.WorldToScreenPoint(unit.transform.position);

            if (screenPosition.x > min.x &&
                screenPosition.x < max.x &&
                screenPosition.y > min.y &&
                screenPosition.y < max.y)
            {
                SelectedUnits.Add(unit);

                unit.Select();
            }
        }
    }
    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        SelectedUnits.Remove(unit);
    }

    private void ClientHandleGameOver(string player)
    {
        enabled = false;
    }
}
