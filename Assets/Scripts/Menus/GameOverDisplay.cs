using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;


public class GameOverDisplay : MonoBehaviour
{
    [SerializeField] private GameObject _gameOverDisplayParent = null;
    [SerializeField] private TMP_Text _winnerNameText = null;

    private void Start()
    {
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }

    private void OnDestroy()
    {
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }

    private void ClientHandleGameOver(string winner)
    {
        _winnerNameText.text = $"{winner} Has Won!";

        _gameOverDisplayParent.SetActive(true);
    }

    public void LeaveGame()
    {
        if(NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
        }
    }
}
