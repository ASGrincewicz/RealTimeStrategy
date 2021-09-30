using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using System;

public class ResourcesDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text _resourcesText = null;
    private NetworkedPlayer _player;

    private void Update()
    {
        if (_player == null)
        {
            _player = NetworkClient.connection.identity.GetComponent<NetworkedPlayer>();

            if(_player != null)
            {
                ClientHandleResourcesUpdated(_player.GetResources());

                _player.ClientOnResourcesUpdated += ClientHandleResourcesUpdated;
            }
        }
    }

    private void OnDestroy()
    {
        _player.ClientOnResourcesUpdated -= ClientHandleResourcesUpdated;
    }

    private void ClientHandleResourcesUpdated(int resources)
    {
        _resourcesText.text = $"Resources: {resources}";
    }
}
