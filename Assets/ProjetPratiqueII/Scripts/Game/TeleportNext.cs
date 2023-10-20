using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class TeleportNext : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3 && !LevelManager.instance.inSequence)
        {
            LevelManager.instance.AnimNextBiome();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 3)
        {
            if (!LevelManager.instance.inSequence)
            {
                LevelManager.instance.GoNextBiome();
            }
            else
            {
                LevelManager.instance.inSequence = false;
                LevelManager.instance.takeInput = true;
            }
        }
    }
}