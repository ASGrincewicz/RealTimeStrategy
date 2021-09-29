using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameOverHandler : NetworkBehaviour
{
    [SerializeField] private List<UnitBase> _unitBases = new List<UnitBase>();


    #region Server
    public override void OnStartServer()
    {
        UnitBase.ServerOnBaseSpawned += ServerHandleBaseSpawned;
        UnitBase.ServerOnBaseDespawned += ServerHandleBaseDespawned;
    }

    public override void OnStopServer()
    {
        UnitBase.ServerOnBaseSpawned += ServerHandleBaseSpawned;
        UnitBase.ServerOnBaseDespawned -= ServerHandleBaseDespawned;
    }
    [Server]
    private void ServerHandleBaseSpawned(UnitBase unitBase)
    {
        _unitBases.Add(unitBase);
    }

    [Server]
    private void ServerHandleBaseDespawned(UnitBase unitBase)
    {
        _unitBases.Remove(unitBase);

        if (_unitBases.Count != 1) return;

        Debug.Log("Game Over, Man!");
    }
    #endregion
}
