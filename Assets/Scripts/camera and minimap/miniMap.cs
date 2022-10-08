using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class miniMap : MonoBehaviour
{
    // Update is called once per frame
    Vector3 position;
    private void LateUpdate()
    {
        position = gameManager.instance.player.transform.position;
        position.y += 30;
        transform.position = position;
    }
}
