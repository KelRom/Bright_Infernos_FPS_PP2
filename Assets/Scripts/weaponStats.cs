using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class weaponStats : ScriptableObject
{
    public int damage;
    public int range;
    public float fireRate;
    public GameObject model;
}
