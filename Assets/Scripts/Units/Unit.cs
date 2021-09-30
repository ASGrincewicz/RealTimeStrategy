using System;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    [SerializeField] private int _resourceCost = 10;
    [SerializeField] private Health _health = null;
    [SerializeField] private Targeter _targeter = null;
    [SerializeField] private UnitMovement _unitMovement = null;
    [SerializeField] private UnityEvent onSelected = null;
    [SerializeField] private UnityEvent onDeselected = null;

    public static event Action<Unit> ServerOnUnitSpawned;
    public static event Action<Unit> ServerOnUnitDespawned;

    public static event Action<Unit> AuthorityOnUnitSpawned;
    public static event Action<Unit> AuthorityOnUnitDespawned;

    public int GetResourceCost() { return _resourceCost; }
    public UnitMovement GetUnitMovement() { return _unitMovement; }
    public Targeter GetTargeter() { return _targeter; }

    #region Server
    public override void OnStartServer()
    {
        ServerOnUnitSpawned?.Invoke(this);
        _health.ServerOnDie += ServerHandleDie;
    }

    public override void OnStopServer()
    {
        ServerOnUnitDespawned?.Invoke(this);
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
        AuthorityOnUnitSpawned?.Invoke(this);
    }
    public override void OnStopClient()
    {
        if (!hasAuthority) return;
       
        AuthorityOnUnitDespawned?.Invoke(this);
    }
    public void Select()
    {
        if (!hasAuthority) return;
        onSelected?.Invoke();
    }

    public void Deselect()
    {
        if (!hasAuthority) return;
        onDeselected?.Invoke();
    }
    #endregion
}
