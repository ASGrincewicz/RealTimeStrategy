using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent _navMeshAgent;
    [SerializeField] private Targeter _targeter;
    [SerializeField] private float _chaseRange = 10f;

    #region Server
    [ServerCallback]
    private void Update()
    {
        Targetable target = _targeter.GetTarget();

        if(_targeter.GetTarget() != null)
        {
            if((target.transform.position - transform.position).sqrMagnitude > _chaseRange * _chaseRange)
            {
                _navMeshAgent.SetDestination(target.transform.position);
            }
            else if(_navMeshAgent.hasPath)
            {
                _navMeshAgent.ResetPath();
            }
            return;
        }

        if (!_navMeshAgent.hasPath) return;
        if (_navMeshAgent.remainingDistance > _navMeshAgent.stoppingDistance) return;

        _navMeshAgent.ResetPath();
    }

    [Command]
    public void CmdMove(Vector3 position)
    {
        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) return;

        _navMeshAgent.SetDestination(hit.position);

        _targeter.ClearTarget();
    }
    #endregion
}
