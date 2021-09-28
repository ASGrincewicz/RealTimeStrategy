using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Targetable : NetworkBehaviour
{
   [SerializeField] private Transform _aimAtPoint = null;
    public Transform GetAimAtPoint() { return _aimAtPoint; }
}
