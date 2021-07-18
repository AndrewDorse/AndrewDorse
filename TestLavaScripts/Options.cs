using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Options : ScriptableObject
{
    [SerializeField]
    private float movementSpeedPlayer;
    [SerializeField]
    private float impulsePower;
    [SerializeField]
    private float bulletSpeed;


    public float GetMovementSpeedPlayer
    {
        get
        {
            return movementSpeedPlayer;
        }
    }
    public float GetImpulsePower
    {
        get
        {
            return impulsePower;
        }
    }
    public float GetBulletSpeed
    {
        get
        {
            return bulletSpeed;
        }
    }
}
