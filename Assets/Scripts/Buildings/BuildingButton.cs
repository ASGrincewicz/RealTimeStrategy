using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;
using Mirror;

public class BuildingButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Building _building = null;
    [SerializeField] private Image _icon = null;
    [SerializeField] private TMP_Text _priceText = null;
    [SerializeField] private LayerMask _floorMask;

    private Camera _mainCamera = null;
    private NetworkedPlayer _player = null;
    private GameObject _buildingPreviewInstance = null;
    private Renderer _buildingRendererInstance = null;

    private void Start()
    {
        _mainCamera = Camera.main;
        _icon.sprite = _building.GetIcon();
        _priceText.text = $"{_building.GetPrice()}";
    }

    private void Update()
    {
        if(_player == null)
        {
            _player = NetworkClient.connection.identity.GetComponent<NetworkedPlayer>();
        }

        if (_buildingPreviewInstance == null) return;

        UpdateBuildingPreview();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        _buildingPreviewInstance = Instantiate(_building.GetBuildingPreview());

        _buildingRendererInstance = _buildingPreviewInstance.GetComponentInChildren<Renderer>();

        _buildingPreviewInstance.SetActive(false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_buildingPreviewInstance == null) return;

        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _floorMask))
        {
            _player.CmdTryPlaceBuilding(_building.GetID(), hit.point);
        }

        Destroy(_buildingPreviewInstance);
    }

    private void UpdateBuildingPreview()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _floorMask)) return;

        _buildingPreviewInstance.transform.position = hit.point;

        if(!_buildingPreviewInstance.activeSelf)
        {
            _buildingPreviewInstance.SetActive(true);
        }
    }
}
