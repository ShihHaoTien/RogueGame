using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyCanvas : MonoBehaviour
{
    Enemy master;
    Canvas enemyCanvas;
    Slider bloodBar;
    void Awake()
    {
        master=GetComponentInParent<Enemy>();
        enemyCanvas=GetComponent<Canvas>();
        bloodBar=GetComponentInChildren<Slider>();
        SetCanvasPosition();
        enemyCanvas.sortingLayerName="Default";
       // Debug.Log(master.transform.position);
       // Debug.Log(enemyCanvas.GetComponentInParent<Enemy>());
       // Debug.Log("local: "+enemyCanvas.transform.localPosition);
       // Debug.Log(enemyCanvas.transform.localPosition);

        //enemyCanvas.transform.position=master.transform.position;
        //bloodBar.transform.position=master.transform.position;
       // Debug.Log("new:"+enemyCanvas.transform.localPosition);
        //Debug.Log(bloodBar.transform.position);
    }
    // Start is called before the first frame update

    public void HideCanvas()
    {
        enemyCanvas.sortingLayerName="Default";
    }
    public void ShowCanvas()
    {
        enemyCanvas.sortingLayerName="Units";

    }
    public void UpdatePosition()
    {
        SetCanvasPosition();
        //Debug.Log("canvas: "+enemyCanvas.transform.position+" mas: "+master.transform.position);
    }

    void SetCanvasPosition()
    {
        enemyCanvas.transform.position=master.transform.position;
        enemyCanvas.transform.position= enemyCanvas.transform.position+new Vector3(0,0.2f,0);
    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
