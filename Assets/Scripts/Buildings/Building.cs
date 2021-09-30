using System;
using UnityEngine;
using Mirror;

public class Building : NetworkBehaviour
{
    [SerializeField] private GameObject _buildingPreview = null;
    [SerializeField] private Sprite _icon = null;
    [SerializeField] private int _id = -1;
    [SerializeField] private int _price = 100;
    [SerializeField] private Health _health = null;

    public static event Action<Building> ServerOnBuildingSpawned;
    public static event Action<Building> ServerOnBuildingDespawned;

    public static event Action<Building> AuthorityOnBuildingSpawned;
    public static event Action<Building> AuthorityOnBuildingDespawned;

    public GameObject GetBuildingPreview() { return _buildingPreview; }
    public Sprite GetIcon() { return _icon; }
    public int GetID() { return _id; }
    public int GetPrice() { return _price; }

    #region Server
    public override void OnStartServer()
    {
        ServerOnBuildingSpawned?.Invoke(this);
        _health.ServerOnDie += ServerHandleDie;

    }
    public override void OnStopServer()
    {
        ServerOnBuildingDespawned?.Invoke(this);
        _health.ServerOnDie -= ServerHandleDie;
    }

    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }
    #endregion

    #region Client
    public override void OnStartAuthority()
    {
        AuthorityOnBuildingSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if (!hasAuthority) return;
        AuthorityOnBuildingDespawned?.Invoke(this);
    }
    #endregion
}
