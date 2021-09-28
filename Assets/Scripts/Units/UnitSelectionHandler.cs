using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler : MonoBehaviour
{
    private Camera _mainCamera;

    private List<Unit> _selectedUnits = new List<Unit>();
    public List<Unit> SelectedUnits { get => _selectedUnits; }
    [SerializeField] private LayerMask _layerMask = new LayerMask();

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if(Mouse.current.leftButton.wasPressedThisFrame)
        {
            foreach(Unit selectedUnit in _selectedUnits)
            {
                selectedUnit.Deselect();
            }
            _selectedUnits.Clear();
        }
        else if(Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ClearSelectionArea();
        }
    }

    private void ClearSelectionArea()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _layerMask)) return;

        if (!hit.collider.TryGetComponent<Unit>(out Unit unit)) return;

        if (!unit.hasAuthority) return;

        _selectedUnits.Add(unit);

        foreach(Unit selectedUnit in _selectedUnits)
        {
            selectedUnit.Select();
        }
    }
}
