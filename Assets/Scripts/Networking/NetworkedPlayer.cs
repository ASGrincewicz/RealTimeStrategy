using System.Collections.Generic;
using System;
using Mirror;
using UnityEngine;

public class NetworkedPlayer : NetworkBehaviour
{
    [SerializeField] private Building[] _buildings = new Building[0];
    [SerializeField] private LayerMask _buildingBlockMask = new LayerMask();
    [SerializeField] private float _buildingRangeLimit = 5f;
    [SyncVar(hook = nameof(ClientHandleResourcesUpdated))]
    private int _myResources = 500;
    public event Action<int> ClientOnResourcesUpdated;

    private List<Unit> _myUnits = new List<Unit>();
    private List<Building> _myBuildings = new List<Building>();
    private Color _teamColor = new Color();
    public List<Unit> GetMyUnits() { return _myUnits; }
    public List<Building> GetMyBuildings() { return _myBuildings; }
    public int GetResources() { return _myResources; }
    public Color GetTeamColor() { return _teamColor; }

    public bool CanPlaceBuilding(BoxCollider buildingCollider, Vector3 position)
    {
        if (Physics.CheckBox(position + buildingCollider.center,
                        buildingCollider.size / 2,
                        Quaternion.identity,
                        _buildingBlockMask)) return false;

        foreach (var building in _myBuildings)
        {
            if ((position - building.transform.position).sqrMagnitude <= _buildingRangeLimit * _buildingRangeLimit)
            {
                return true;
            }
        }
        return false;
    }
    #region Server
    public override void OnStartServer()
    {
        Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
        Building.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned += ServerHandleBuildingDespawned;
    }

    public override void OnStopServer()
    {
        Unit.ServerOnUnitDespawned -= ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
        Building.ServerOnBuildingSpawned -= ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned -= ServerHandleBuildingDespawned;
    }

    [Server]
    public void SetResources(int newResources) => _myResources = newResources;

    [Server]
    public void SetTeamColor(Color newColor) => _teamColor = newColor;

    [Command]
    public void CmdTryPlaceBuilding(int buildingID, Vector3 position)
    {
        Building buildingToPlace = null;

        foreach(var building in _buildings)
        {
            if(building.GetID()== buildingID)
            {
                buildingToPlace = building;
                break;
            }
        }
        if (buildingToPlace == null) return;

        if (_myResources < buildingToPlace.GetPrice()) return;

        var buildingCollider = buildingToPlace.GetComponent<BoxCollider>();

        
        if (!CanPlaceBuilding(buildingCollider, position)) return;


       var buildingInstance = Instantiate(buildingToPlace.gameObject, position, buildingToPlace.transform.rotation);

        NetworkServer.Spawn(buildingInstance, connectionToClient);

        SetResources(_myResources - buildingToPlace.GetPrice());
    }

    private void ServerHandleUnitSpawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;
        _myUnits.Add(unit);
    }
    private void ServerHandleUnitDespawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;
        _myUnits.Remove(unit);
    }

    private void ServerHandleBuildingSpawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) return;
        _myBuildings.Add(building);
    }
    private void ServerHandleBuildingDespawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) return;
        _myBuildings.Remove(building);
    }
    #endregion Server

    #region Client
    public override void OnStartAuthority()
    {
        if (NetworkServer.active) return;
        Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
        Building.AuthorityOnBuildingSpawned += AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned += AuthorityHandleBuildingDespawned;
    }

    public override void OnStopClient()
    {
        if (!isClientOnly || !hasAuthority) return;
        Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned -= AuthorityHandleBuildingDespawned;
    }

    private void AuthorityHandleUnitSpawned(Unit unit) => _myUnits.Add(unit);

    private void AuthorityHandleUnitDespawned(Unit unit) => _myUnits.Remove(unit);

    private void AuthorityHandleBuildingSpawned(Building building) => _myBuildings.Add(building);

    private void AuthorityHandleBuildingDespawned(Building building) => _myBuildings.Remove(building);

    private void ClientHandleResourcesUpdated(int oldResources, int newResources)
    {
        ClientOnResourcesUpdated?.Invoke(newResources);
    }

    #endregion Client
}
