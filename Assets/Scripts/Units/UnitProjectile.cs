﻿using Mirror;
using UnityEngine;

public class UnitProjectile : NetworkBehaviour
{
    [SerializeField] private Rigidbody _rigidBody = null;

    [SerializeField] private float _destroyAfterSeconds = 5f;

    [SerializeField] private float _launchForce = 10f;

    [SerializeField] private int _damageToDeal = 20;

    private void Start() => _rigidBody.velocity = transform.forward * _launchForce;

    public override void OnStartServer() => Invoke(nameof(DestroySelf), _destroyAfterSeconds);


    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<NetworkIdentity>(out NetworkIdentity networkIdentity))
        {
            if (networkIdentity.connectionToClient == connectionToClient) return;
        }

        if(other.TryGetComponent<Health>(out Health health))
        {
            health.DealDamage(_damageToDeal);
        }
        DestroySelf();
    }

    [Server]
    private void DestroySelf() => NetworkServer.Destroy(this.gameObject);
}