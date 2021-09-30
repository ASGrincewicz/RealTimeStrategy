using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Health : NetworkBehaviour
{
    [SerializeField] private int _maxHealth = 100;

    [SyncVar(hook = nameof(HandleHealthUpdated))]
    private int _currentHealth;

    public event Action<int, int> ClientOnHealthUpdated;
    public event Action ServerOnDie;

    #region Server
    public override void OnStartServer()
    {
        _currentHealth = _maxHealth;

        UnitBase.ServerOnPlayerDeath += ServerHandlePlayerDeath;
    }

    public override void OnStopServer()
    {
        UnitBase.ServerOnPlayerDeath -= ServerHandlePlayerDeath;
    }

    [Server]
    public void DealDamage(int damageAmount)
    {
        if (_currentHealth == 0) return;

        _currentHealth = Mathf.Max(_currentHealth - damageAmount, 0);

        if (_currentHealth != 0) return;

        ServerOnDie?.Invoke();

        //Debug.Log("We died!");
    }
    [Server]
    private void ServerHandlePlayerDeath(int connectionId)
    {
        if (connectionToClient.connectionId != connectionId) return;

        DealDamage(_currentHealth);
    }

    #endregion
    #region Client
    private void HandleHealthUpdated(int oldHealth, int newHealth)
    {
        ClientOnHealthUpdated?.Invoke(newHealth, _maxHealth);
    }
    #endregion
}
