using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartPage : MonoBehaviour
{
    public Text title;
    public Button startBtn;
    public Button recordBtn;
    public Button exitBtn;
    [HideInInspector] public bool gameStart;
    bool showingRecord;

    public void StartBtnClick()
    {
        
        //Debug.Log("clicked");
        GameController.instance.StartGameReq();
        gameStart=true;
        gameObject.SetActive(false);
        //Destroy(gameObject);
        //this.gameObject.SetActive(false);
    }

    public void ExitBtnClick()
    {
        Debug.Log("quit app");
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    public void RecordBtnClick()
    {
        if(showingRecord==false)
        {
            int rcd=PlayerPrefs.GetInt("DaysRecord",0);
            recordBtn.GetComponentInChildren<Text>().text=rcd.ToString()+" days";
            showingRecord=true;
        }
        else
        {
            recordBtn.GetComponentInChildren<Text>().text="Record";
            showingRecord=false;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        startBtn.onClick.AddListener(StartBtnClick);
        exitBtn.onClick.AddListener(ExitBtnClick);
        recordBtn.onClick.AddListener(RecordBtnClick);
        showingRecord=false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
