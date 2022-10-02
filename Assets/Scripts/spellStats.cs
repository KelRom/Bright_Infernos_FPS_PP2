using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class spellStats : ScriptableObject
{
    public int damage;
    public int castRate;
    public int manaCost;
    public int spellDistance;
    public GameObject spellPos;
}
