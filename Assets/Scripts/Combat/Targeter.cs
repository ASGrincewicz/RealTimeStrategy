using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Targeter : NetworkBehaviour
{
    private Targetable _target;
    public Targetable GetTarget() { return _target; }
    #region Server
    public override void OnStartServer()
    {
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    public override void OnStopServer()
    {
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }

    [Command]
    public void CmdSetTarget(GameObject targetGameObject)
    {
        if (!targetGameObject.TryGetComponent<Targetable>(out Targetable target )) return;

        _target = target;
    }

    [Server]
    public void ClearTarget() => _target = null;

    [Server]
    private void ServerHandleGameOver()
    {
        ClearTarget();
    }
    #endregion
}
