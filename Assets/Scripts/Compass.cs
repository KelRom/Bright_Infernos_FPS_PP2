using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour
{
    // Update is called once per frame
    Vector3 rotation;

    void Update()
    {
        rotation.z = gameManager.instance.player.transform.eulerAngles.y;
        transform.localEulerAngles = rotation;
    }
}
