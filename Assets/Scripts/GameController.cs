using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance = null;
    [HideInInspector] public bool playerTurn=true;
    public float turnDelay=0.1f;
    BoardManager boardManager;
    int level=1;
    public int playerHP=30;
    List<Enemy> enemies;
    bool enemyMoving;
    

    public float levelStartDelay=2f;
    
    bool doingSetup;
    GameObject levelImage;
    Text levelText;

    void Start()
    {
        SceneManager.sceneLoaded += LevelWasLoaded;
    }

    void LevelWasLoaded(Scene scene,LoadSceneMode mode)
    {
        level++;
        InitGame();
    }
    void UpDate()
    {
        Debug.Log("GM Update");
        if(playerTurn||enemyMoving||doingSetup)
            return;
        StartCoroutine(MoveEnemys());
    }

    public void MoveEnemyReq()
    {
        //Debug.Log("REQ");
        if(playerTurn||enemyMoving||doingSetup)
            return;
        StartCoroutine(MoveEnemys());   
    }

    IEnumerator MoveEnemys()
    {
        enemyMoving=true;
        yield return new WaitForSeconds(turnDelay);
        if(enemies.Count==0)
        {
            yield return new WaitForSeconds(turnDelay);
        }
        for(int i=0;i<enemies.Count;++i)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime);
        }
        enemyMoving=false;
        playerTurn=true;

    }

    //Initial
    void Awake()
    {
        Debug.Log("GM AWAKE");
        if(instance==null)
        {
            instance = this;
        }
        else if(instance!=null)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        enemies=new List<Enemy>();
        //Get instance of component 'GameManager'
        boardManager=GetComponent<BoardManager>();
        InitGame();
    }

    public void GameOver()
    {
        Debug.Log("GM DIED");
        //base.enabled=false;
        levelText.text="You Survived "+level+" days";
        levelImage.SetActive(true);
        base.enabled=false;
    }

    void InitGame()
    {
        doingSetup=true;
        levelImage=GameObject.Find("LevelImage");
        levelText=GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text="Day "+level;
        levelImage.SetActive(true);
        Invoke("HideLevelImage",levelStartDelay);
        boardManager.SetupScene(level);
        enemies.Clear();
    }

    void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup=false;
    }

    public void addEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    public void RemoveEnemy(Enemy enemy)
    {
        this.enemies.Remove(enemy);
    }
}
