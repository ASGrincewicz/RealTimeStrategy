using UnityEngine;
using Mirror;

public class TeamColorSetter : NetworkBehaviour
{
    [SerializeField] private Renderer[] _colorRenderers = new Renderer[0];

    [SyncVar(hook = nameof(HandleTeamColorUpdated))]
    private Color _teamColor = new Color();

    #region Server
    public override void OnStartServer()
    {
        var player = connectionToClient.identity.GetComponent<NetworkedPlayer>();

        _teamColor = player.GetTeamColor();
    }


    #endregion
    #region Client
    private void HandleTeamColorUpdated(Color oldColor, Color newColor)
    {
        foreach(var renderer in _colorRenderers)
        {
            renderer.material.SetColor("_BaseColor", newColor);
        }
    }

    #endregion
}
