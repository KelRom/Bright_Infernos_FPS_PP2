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
    
    public float weaponFOV;
    [Range(10f,0f)]
    public float weaponZoomSpeed;
    public int currentGunCapacity;
    public int maxGunCapacity;
    public int currentAmmoCount;
    public int maxAmmoCount;
    public float reloadRate;


    public GameObject hitEffect;
    public AudioClip sound;
    public AudioClip reload;
}