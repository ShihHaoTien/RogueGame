using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    //public bool gameStart=false;
    public static GameController instance = null;
    [HideInInspector] public bool playerTurn=true;
    public float turnDelay=0.1f;
    BoardManager boardManager;
    int level=1;
    public int playerHP=30;
    public int startHP=30;
    List<Enemy> enemies;
    bool enemyMoving;
    public float levelStartDelay=2f;
    bool doingSetup;
    GameObject levelImage;
    Text levelText;
    CanvasManager canvasManager;
    //bool gameStart=false;

    //StartPage
    StartPage startPage;
    public GameObject playerOBJ;
    public bool gameStart;
    GameObject tempPlayer;

    //Game Over Page
    public Button backBtn;


    void Start()
    {
        SceneManager.sceneLoaded += LevelWasLoaded;
    }

    //Pressed start button, start game.
    void StartGame()
    {
        
    }




    void LevelWasLoaded(Scene scene,LoadSceneMode mode)
    {
        level++;
        //Debug.Log("Level: "+level);
        InitGame();
    }
    void UpDate()
    {
        Debug.Log("GM Update");
        if(playerTurn||enemyMoving||doingSetup)
            return;
        StartCoroutine(MoveEnemys());
    }

    /*============================
    *REQUESTS BEGIN
    *============================*/
    public void MoveEnemyReq()
    {
        //Debug.Log("REQ");
        if(playerTurn||enemyMoving||doingSetup)
            return;
        StartCoroutine(MoveEnemys());   
    }

    public void StartGameReq()
    {
        if(startPage==null)
        {
            GetStartPage();
        }
        gameStart=true;
        if(startPage!=null) startPage.gameObject.SetActive(false);
        //Destroy(startPage.gameObject);
        //Instantiate(playerOBJ);//Init Player
        level=1;
        playerHP=startHP;
        Debug.Log("Now start game");
        InitGame();
    }

    public void GoBackStartPageReq()
    {
        GetStartPage();
        //Debug.Log("GM:"+startPage);
        gameStart=false;
        startPage.gameObject.SetActive(true);
        levelText.text="";
        //SceneManager.LoadScene(0);
        SoundManager.instance.musicSource.Play();
        //Destroy(boardManager.gameObject);
        boardManager.DeleteBoard();

        enemies.Clear();
    }

    //Get current startpage from CanvasManager
    public void UpdateStartPageReq(StartPage ss)
    {
        startPage=ss;
    }

    public void AddHPReq(int add)
    {
        playerHP=playerHP+add;
        tempPlayer.GetComponent<Player>().RecieveFromGM();
    }
    
    /*============================
    *REQUESTS END
    *============================*/
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
            GetStartPage();//init the start page object.
            startHP=playerHP;
        }
        else if(instance!=null)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        enemies=new List<Enemy>();
        //Get instance of component 'GameManager'
        boardManager=GetComponent<BoardManager>();
        //InitGame();
        if(gameStart && startPage!=null)
        {
            startPage.gameObject.SetActive(false);
        }
        canvasManager=GameObject.Find("Canvas").GetComponent<CanvasManager>();
    }

    //get start page,do it when first init GM
    void GetStartPage()
    {
        gameStart=false;
        GameObject stpOBJ=GameObject.FindWithTag("StartPage");
        if(stpOBJ!=null) startPage=stpOBJ.GetComponent<StartPage>();
    }

    public void GameOver()
    {
        Debug.Log("GM Game over");
        //base.enabled=false;
        levelText.text="You Survived "+level+" days";
        levelImage.SetActive(true);
        //base.enabled=false;
        canvasManager.ShowBackBtn();
        SaveData();
    }

    void SaveData()
    {
        if(level>PlayerPrefs.GetInt("DaysRecord",level))
        {
            PlayerPrefs.SetInt("DaysRecord",level);
        }
    }
    void GetObjects()
    {
        canvasManager=GameObject.Find("Canvas").GetComponent<CanvasManager>();
    }

    void InitGame()
    {
        Debug.Log("Init Game");
        //gameStart=true;
        GetObjects();
        doingSetup=true;
        levelImage=GameObject.Find("LevelImage");
        levelText=GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text="Day "+level;
        levelImage.SetActive(true);
        Invoke("HideLevelImage",levelStartDelay);
        //HideLevelImage();
        boardManager.SetupScene(level);
        enemies.Clear();
        //InitPlayer
        tempPlayer=Instantiate(playerOBJ);
        //Debug.Log("GM HP: "+playerHP);
    }

    void HideLevelImage()
    {
        //Debug.Log("Hide LevelImage");
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
