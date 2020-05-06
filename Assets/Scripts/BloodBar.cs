using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BloodBar : MonoBehaviour
{
    public Slider slider;
    public float HP;
    public Enemy master;
    public string barName="BloodBar";
    void Awake()
    {
        slider=GetComponent<Slider>();
        master=GetComponentInParent<Enemy>();
        slider.transform.position=master.transform.position;
        
        //master=GetComponent<Enemy>();
        //HP=master.
        slider.maxValue=500;
        HP=slider.maxValue;
        slider.value=HP;
    }

    public void UpdatePosition()
    {
        slider.transform.position=master.transform.position;
        Debug.Log(slider.transform.position);   
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(HP);
        HP--;
        if(HP<slider.minValue)
        {
            HP=slider.minValue;
        }
        slider.value=HP;
        //Debug.Log(HP);
    }
}
