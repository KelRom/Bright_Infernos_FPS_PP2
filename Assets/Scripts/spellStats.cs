using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class spellStats : ScriptableObject
{
    public float manaCost;
    public float damage;
    public float speed;
    public GameObject spellDisplay;
    public float Cooldown;
    public int knockbackStength;
}
