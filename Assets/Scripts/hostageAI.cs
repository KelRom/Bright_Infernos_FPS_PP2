using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hostageAI : MonoBehaviour
{
    [SerializeField] int hostageHP;
    private int hostageHPOriginal;

    private void Start()
    {
        hostageHPOriginal = hostageHP;
        InvokeRepeating("hostageDamage",1, 5);
    }

    void Update()
    {

    }

    void hostageDamage()
    {
        hostageHP -= 1;
        updateHostageHP();
        if (hostageHP <= 0)
        {
            gameManager.instance.hostageRescued.faceColor = Color.red;
        }
    }
    public void updateHostageHP()
    {
        gameManager.instance.hostageHPBar.fillAmount = (float)hostageHP / (float)hostageHPOriginal;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.hostageInRange = true;
            gameManager.instance.hostageRescued.faceColor = Color.green;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.hostageInRange = false;
            gameManager.instance.hostageRescued.faceColor = Color.white;
        }
    }
}
