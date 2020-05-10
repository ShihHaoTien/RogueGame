/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Loader : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject playerGO;
    //public GameObject mainCanvas;
    StartPage startPage;
    GameObject startPageGO;
    Text levelText;
    void Awake()
    {
        //Instantiate(mainCanvas);
        startPageGO=GameObject.Find("StartPageCanvas");
        if(startPageGO!=null) startPage=startPageGO.GetComponent<StartPage>();
        levelText=GameObject.Find("LevelText").GetComponent<Text>();
        //levelText.gameObject.SetActive(false);
        Debug.Log("Loader Awake");
        //gameStart=false;
        //Instantiate(startPage);
        //enabled=true;
     
    }

    void Update()
    {
        //Debug.Log("LOADER UPDATE");
        //Debug.Log(startPage.gameStart);
        if(startPage!=null && startPage.isActiveAndEnabled)
        {
        
            if(startPage.gameStart==true && GameController.instance==null)
            {
                //levelText.gameObject.SetActive(true);
                Debug.Log("Game Start");
                Instantiate(gameManager);
                startPage.gameStart=false;
                startPage.gameObject.SetActive(false); 
                InitPlayer();
            }
        }
        
    }

    void InitPlayer()
    {
        Instantiate(playerGO);
        playerGO.transform.position= new Vector2(0,0);
        //Debug.Log(GameObject.Find("Player").GetComponent<Player>().isActiveAndEnabled);
    }
}*/
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

