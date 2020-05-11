using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CanvasManager : MonoBehaviour
{
    //public static CanvasManager instance=null;
    public GameObject startPageCanvas;
    public Button backBtn;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(GameController.instance.gameStart==true && startPageCanvas!=null)
        {
            startPageCanvas.gameObject.SetActive(false);
        }       
    }

    public void ShowBackBtn()
    {
        backBtn=Instantiate(backBtn);
        backBtn.transform.SetParent(this.gameObject.transform);
        backBtn.transform.position=this.transform.position-new Vector3(0,200,0);
        //backBtn.gameObject.SetActive(false);
        backBtn.onClick.AddListener(BackBtnClick);
        GameController.instance.UpdateStartPageReq(startPageCanvas.GetComponent<StartPage>());
    }

    void BackBtnClick()
    {
        backBtn.gameObject.SetActive(false);
        GameController.instance.GoBackStartPageReq();
    }

    void Awake()
    {
        //if(instance==null)
        //{
          //  instance=this;
            startPageCanvas=Instantiate(startPageCanvas);
            startPageCanvas.transform.SetParent(this.gameObject.transform);//SET PARENT
            //Debug.Log("old:"+startPageCanvas.transform.position);
            startPageCanvas.transform.position=this.transform.position;
            //Debug.Log("new:"+startPageCanvas);
        
        /*
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);*/
        
    }

}
