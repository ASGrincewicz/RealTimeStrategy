using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ResourceGenerator : NetworkBehaviour
{
    [SerializeField] private Health _health = null;

    [SerializeField] private int _resourcesPerInterval = 10;
    [SerializeField] private float _interval = 2f;

    private float _timer;
    private float _deltaTime;
    private NetworkedPlayer _player;

    #region Server
    public override void OnStartServer()
    {
        _timer = _interval;
        _player = connectionToClient.identity.GetComponent<NetworkedPlayer>();

        _health.ServerOnDie += ServerHandleDeath;
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    public override void OnStopServer()
    {
        _health.ServerOnDie -= ServerHandleDeath;
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }
    [ServerCallback]
    private void Update()
    {
        _deltaTime = Time.deltaTime;
        _timer -= _deltaTime;
        if(_timer <= 0)
        {
            _player.SetResources(_player.GetResources() + _resourcesPerInterval);

            _timer += _interval;
        }
        
    }
    [Server]
    private void ServerHandleDeath()
    {
        NetworkServer.Destroy(gameObject);
    }

    [Server]
    private void ServerHandleGameOver()
    {
        enabled = false;
    }
    #endregion
}
