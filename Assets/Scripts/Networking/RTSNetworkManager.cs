using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RTSNetworkManager : NetworkManager
{
    [SerializeField] private GameObject _unitSpawnerPrefab = null;
    [SerializeField] private GameOverHandler _gameOverHandlerPrefab = null;

    #region Server
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        var unitSpawnerInstance = Instantiate(_unitSpawnerPrefab,
            conn.identity.transform.position,
            conn.identity.transform.rotation);

        NetworkServer.Spawn(unitSpawnerInstance, conn);
    }

    public override void OnServerChangeScene(string newSceneName)
    {
        if (SceneManager.GetActiveScene().buildIndex.Equals(1))
        {
            GameOverHandler gameOverHandlerInstance = Instantiate(_gameOverHandlerPrefab);

            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);
        }
    }
    #endregion
    
}
