using System.Collections.Generic;
using System;
using Mirror;
using UnityEngine;

public class NetworkedPlayer : NetworkBehaviour
{
    [SyncVar(hook = nameof(ClientHandleResourcesUpdated))]
    private int _myResources = 500;
    public event Action<int> ClientOnResourcesUpdated;

    private List<Unit> _myUnits = new List<Unit>();
    private List<Building> _myBuildings = new List<Building>();
    [SerializeField] private Building[] _buildings = new Building[0];
    public List<Unit> GetMyUnits() { return _myUnits; }
    public List<Building> GetMyBuildings() { return _myBuildings; }
    public int GetResources() { return _myResources; }
    [Server]
    public void SetResources(int newResources) => _myResources = newResources;
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

       GameObject buildingInstance = Instantiate(buildingToPlace.gameObject, position, buildingToPlace.transform.rotation);

        NetworkServer.Spawn(buildingInstance, connectionToClient);
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
