using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class RTSNetworkManager : NetworkManager
{
    [SerializeField] private GameObject _unitSpawnerPrefab;

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        var unitSpawnerInstance = Instantiate(_unitSpawnerPrefab,
            conn.identity.transform.position,
            conn.identity.transform.rotation);

        NetworkServer.Spawn(unitSpawnerInstance, conn);

    }
}
