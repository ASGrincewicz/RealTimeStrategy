using Mirror;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private Health _health = null;
    [SerializeField] private Unit _unitPrefab = null;
    [SerializeField] private Transform _unitSpawnPoint;
    [SerializeField] private TMP_Text _remainingUnitsText = null;
    [SerializeField] private Image _unitProgressImage = null;
    [SerializeField] private int _maxUnitQueue = 5;
    [SerializeField] private float _spawnMoveRange = 7f;
    [SerializeField] private float _unitSpawnDuration = 5f;

    [SyncVar (hook =nameof(ClientHandleQueuedUnitsUpdated))]
    private int _queuedUnits;
    [SyncVar]
    private float _unitTimer;
    private float _deltaTime;

    private float _progressImageVelocity;
    private NetworkedPlayer _player = null;

    private void Update()
    {
        _deltaTime = Time.deltaTime;

        if (isServer)
        {
            ProduceUnits();
        }

        if (isClient)
        {
            UpdateTimerDisplay();
        }
    }

    #region Server
    public override void OnStartServer() => _health.ServerOnDie += ServerHandleDeath;

    public override void OnStopServer() => _health.ServerOnDie -= ServerHandleDeath;

    [Server]
    private void ProduceUnits()
    {
        if (_queuedUnits == 0) return;

        _unitTimer += _deltaTime;

        if (_unitTimer < _unitSpawnDuration) return;

        var unitInstance = Instantiate(_unitPrefab.gameObject, _unitSpawnPoint.position, _unitSpawnPoint.rotation);

        NetworkServer.Spawn(unitInstance, connectionToClient);

        var spawnOffset = Random.insideUnitSphere * _spawnMoveRange;
        spawnOffset.y = _unitSpawnPoint.position.y;

        var unitMovement = unitInstance.GetComponent<UnitMovement>();
        unitMovement.ServerMove(_unitSpawnPoint.position + spawnOffset);

        _queuedUnits--;

        _unitTimer = 0f;
    }

    [Server]
    private void ServerHandleDeath()
    {
       NetworkServer.Destroy(gameObject);
    }

    [Command]
    private void CmdSpawnUnit()
    {
        if (_queuedUnits == _maxUnitQueue) return;

        _player = connectionToClient.identity.GetComponent<NetworkedPlayer>();

        if (_player.GetResources() < _unitPrefab.GetResourceCost()) return;

        _queuedUnits++;

        _player.SetResources(_player.GetResources() - _unitPrefab.GetResourceCost());
        
    }

    #endregion
    #region Client
    private void UpdateTimerDisplay()
    {
        var newProgress = _unitTimer / _unitSpawnDuration;

        if(newProgress < _unitProgressImage.fillAmount)
        {
            _unitProgressImage.fillAmount = newProgress;
        }
        else
        {
            _unitProgressImage.fillAmount = Mathf.SmoothDamp(_unitProgressImage.fillAmount, newProgress,
                ref _progressImageVelocity, 0.1f);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        if (!hasAuthority) return;

        CmdSpawnUnit();
    }

    private void ClientHandleQueuedUnitsUpdated(int oldQueue, int newQueue)
    {
        _remainingUnitsText.text = $"{newQueue}";
    }
    #endregion
}
