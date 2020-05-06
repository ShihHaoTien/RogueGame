using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    public GameObject gameManager;

    void Awake()
    {
        Debug.Log("Loader Awake");
        enabled=true;
        if(GameController.instance==null)
        {
            Debug.Log("first GM");
            Instantiate(gameManager);
        }
    }
}
