using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerSO : ScriptableObject
{
    [field:SerializeField]public PlayerGroundedData PlayerGroundedData { get; private set; }
    [field:SerializeField]public PlayerAirborneData AirborneData { get; private set; }
   
}
