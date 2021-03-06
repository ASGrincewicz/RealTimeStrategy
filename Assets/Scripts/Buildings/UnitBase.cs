using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Mirror;

public class UnitBase : NetworkBehaviour
{
    [SerializeField] private Health _health = null;
    public static event Action<UnitBase> ServerOnBaseSpawned;
    public static event Action<UnitBase> ServerOnBaseDespawned;

    public static event Action<int> ServerOnPlayerDeath;

    #region Server
    public override void OnStartServer()
    {
        _health.ServerOnDie += ServerHandleDie;

        ServerOnBaseSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        _health.ServerOnDie -= ServerHandleDie;

        ServerOnBaseDespawned?.Invoke(this);
    }

    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);

        ServerOnPlayerDeath?.Invoke(connectionToClient.connectionId);
    }


    #endregion

    #region Client



    #endregion
}
