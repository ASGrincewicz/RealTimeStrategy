using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Health : NetworkBehaviour
{
    [SerializeField] private int _maxHealth = 100;

    [SyncVar]
    private int _currentHealth;

    public event Action ServerOnDie;

    #region Server
    public override void OnStartServer()
    {
        _currentHealth = _maxHealth;

    }

    [Server]
    public void DealDamage(int damageAmount)
    {
        if (_currentHealth == 0) return;

        _currentHealth = Mathf.Max(_currentHealth - damageAmount, 0);

        if (_currentHealth != 0) return;

        ServerOnDie?.Invoke();

        Debug.Log("We died!");
    }

    #endregion
    #region Client



    #endregion
}
