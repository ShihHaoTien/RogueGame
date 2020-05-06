using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Enemy : MovingObject
{
    public int playerDmg;
    Transform target;
    bool skipMove;
    Animator animator;
    //public BloodBar bloodBar;
    Slider bloodBar;
    EnemyCanvas enemyCanvas;
    public int HP;
    //Init
    protected override void Start()
    {
        HP=50;
        GameController.instance.addEnemyToList(this);
        target=GameObject.FindGameObjectWithTag("Player").transform;
        animator=GetComponent<Animator>();
        bloodBar=GetComponentInChildren<Canvas>().GetComponentInChildren<Slider>();
        enemyCanvas=GetComponentInChildren<EnemyCanvas>();
        //bloodBar.transform.position=this.transform.position;
        bloodBar.maxValue=HP;
        bloodBar.value=HP;
        //Debug.Log("this: "+this.transform.position+"Bar: "+bloodBar.transform.position);
        //Debug.Log(bloodBar.slider.maxValue);
        //Debug.Log("mas:"+bloodBar.master.transform.position.x+" this: "+this.transform.position.x);
        base.Start();
    }

    void Awake()
    {
        //bloodBar=GetComponent<BloodBar>();
        //Debug.Log(bloodBar.name);
        //Debug.Log(bloodBar.slider.maxValue);
    }

    public void MoveEnemy()
    {
        int xdir=0;
        int ydir=0;
        //Same x, move at y axis
        if(Mathf.Abs(target.position.x-transform.position.x)<float.Epsilon)
        {
            ydir=target.position.y>transform.position.y?1:-1;
        }
        //same y, move at x axis
        else xdir=target.position.x>transform.position.x?1:-1;
        AttemptMove<Player>(xdir,ydir);
        enemyCanvas.UpdatePosition();
    }

    protected override void AttemptMove<T>(int xdir, int ydir)
    {
        //Debug.Log(skipMove);
        if(skipMove)
        {
            skipMove=false;
            return;
        }
        base.AttemptMove<T>(xdir,ydir);
        skipMove=true;
    }

    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer=component as Player;
        if(hitPlayer!=null)
        {
            animator.SetTrigger("EnemyAttack");
            hitPlayer.LoseHP(playerDmg);
            //show bloodBar
            //enemyCanvas.ShowCanvas();
        }
       
    }

    public void GetAttack(int atk)
    {
        enemyCanvas.ShowCanvas();
        HP-=atk;
        bloodBar.value=HP;
        if(HP<=0)
        {
            gameObject.SetActive(false);
            GameController.instance.RemoveEnemy(this);
        }
    }
}
