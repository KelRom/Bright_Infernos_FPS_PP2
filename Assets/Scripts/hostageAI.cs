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
    }

    void Update()
    {
        StartCoroutine(hostageDamage());
    }

    IEnumerator hostageDamage()
    {
        hostageHP -= 1;
        updateHostageHP();
        yield return new WaitForSeconds(30);
        if (hostageHP <= 0)
        {
            gameManager.instance.playerIsDead(); //make mission failed menu??
        }
    }
    public void updateHostageHP()
    {
        gameManager.instance.HPBar.fillAmount = (float)hostageHP / (float)hostageHPOriginal;
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
