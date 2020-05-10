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


    public void StartBtnClick()
    {
        //gameObject.SetActive(false);
        //Debug.Log("clicked");
        GameController.instance.StartGameReq();
        gameStart=true;
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
        return;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        startBtn.onClick.AddListener(StartBtnClick);
        exitBtn.onClick.AddListener(ExitBtnClick);
        recordBtn.onClick.AddListener(RecordBtnClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
