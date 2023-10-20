using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportLast : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3 && !LevelManager.instance.inSequence)
        {
            LevelManager.instance.AnimLastBiome();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 3)
        {
            if (!LevelManager.instance.inSequence)
            {
                LevelManager.instance.GoLastBiome();
            }
            else
            {
                LevelManager.instance.inSequence = false;
                LevelManager.instance.takeInput = true;
            }
        }
    }
}