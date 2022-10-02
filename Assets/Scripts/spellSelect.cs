using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spellSelect : MonoBehaviour
{
    public static spellSelect instance;

    [SerializeField] public List<spellStats> spellInventory = new List<spellStats>();

    [SerializeField] int damage;
    [SerializeField] int castRate;
    [SerializeField] int manaCost;
    [SerializeField] int spellDistance;
    [SerializeField] GameObject spellPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void startingSpells(spellStats spell)
    {
        damage = spell.damage;
        castRate = spell.castRate;
        manaCost = spell.manaCost;
        spellDistance = spell.spellDistance;

        
    }

    void spellSelection ()
    {

    }
}
