﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject
{
    public int wallDmg=1;
    public int atk=10;
    public int HPperFood=11;
    public int HPperSoda=21;
    public float restartLevelDelay=1f;
    Animator animator;
    public int HP;
    public Text HPText;
    public AudioClip move1;
    public AudioClip move2;
    public AudioClip eat1;
    public AudioClip eat2;
    public AudioClip drink1;
    public AudioClip drink2;
    public AudioClip gameoverSound;
    public AudioClip attackSound1;
    public AudioClip attackSound2;

    int xdir;
    int ydir;

    //Invincible
    [HideInInspector] public bool invincible;
    [HideInInspector] public int invinDistance;

    //get hit lose HP
    public void LoseHP(int loss)
    {
        if(invincible==false)
        {
            animator.SetTrigger("playerHit");
            HP-=loss;
            CheckGameOver();
        }
    }

    //detection collider TAG
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag=="Food")
        {
            HP+=HPperFood;
            other.gameObject.SetActive(false);
            showDetails(true,HPperFood);
            SoundManager.instance.RandomizeSfx(eat1,eat2);
            showDetails(true,HPperFood);
        }
        else if(other.tag=="Soda")
        {
            HP+=HPperSoda;
            other.gameObject.SetActive(false);
            showDetails(true,HPperSoda);
            SoundManager.instance.RandomizeSfx(drink1,drink2);
            showDetails(true,HPperSoda);
        }
        else if(other.tag=="Exit")
        {
            Invoke("Restart",restartLevelDelay);
            this.enabled=false;//ban scripts
        }
    }

    protected override void AttemptMove<T>(int xdir, int ydir)
    {
        if(invincible==false) {HP--;}
        //If move, invindistance sub 1. But if cant move, shall add 1. 
        base.AttemptMove<T>(xdir,ydir);
        if(invincible==true) invinDistance-=1;
        CheckGameOver();
        //Debug.Log(HP);
        GameController.instance.playerTurn=false;
        //Debug.Log(GameController.instance.playerTurn);
        GameController.instance.MoveEnemyReq();
        //GameController.instance.playerHP=HP;
        showDetails();
        //Debug.Log(invinDistance);
    }


    void CheckGameOver()
    {
        if(HP<=0)
        {
            Debug.Log("Player died");
            SoundManager.instance.RandomizeSfx(gameoverSound);
            GameController.instance.GameOver();
            SoundManager.instance.musicSource.Stop();
            PlayerDied();
            //this.enabled=false;
        }
    }

    void PlayerDied()
    {
        Destroy(gameObject);
    }
    void Restart()
    {
        SceneManager.LoadScene(0);
    }

    protected override void Start()
    {
        invincible=false;
        invinDistance=0;
        animator=GetComponent<Animator>();
        base.Start();
        GetDataFromGM();
        //HP=GameController.instance.playerHP;
        //HPText.text="HP: "+HP;
        //HPText=GetComponentInChildren<Canvas>().GetComponentInChildren<Text>();
        showDetails();
    }

    void GetDataFromGM()
    {
        HP=GameController.instance.player.HP;
        invincible=GameController.instance.player.invinState;
        invinDistance=GameController.instance.player.invinDistance;
    }

    //return HP now
    void OnDisable()
    {
        //GameController.instance.playerHP=HP;
        GameController.instance.GetPlayerDataReq(gameObject);
    }

    protected override void OnCantMove<T>(T component)
    {
        //Debug.Log("player cant move");
        Wall hitWall=component as Wall;
        if(hitWall!=null)
        {
            //Debug.Log("hitwall not null");
            hitWall.DamageWall(wallDmg);
            animator.SetTrigger("playerChop");
        }
        else if(hitWall==null)
        {
            //Debug.Log("edge!!");
            if(invincible==false) {HP++;}
            //HPText.text="HP: "+HP;
        }
        //check whether attack enemy
        HitEnemy();
        //Cant move then add invin distance
        if(invincible==true) invinDistance+=1;
    }

    void HitEnemy()
    {
        RaycastHit2D hit;
        bool canMove = Move(xdir,ydir,out hit);
        //Debug.Log(canMove);
        if(hit.transform==null)
        {
            //Debug.Log("hit is null");
            return;
        }
        Enemy hitComponent=hit.transform.GetComponent<Enemy>();
        if(!canMove&&hitComponent!=null)
        {
            //attack enemy
            if(!invincible)
            {
                HP--;
            }
            //Debug.Log("Atk Enemy");
            hitComponent.GetAttack(this.atk);
            animator.SetTrigger("playerChop");
            SoundManager.instance.RandomizeSfx(attackSound1,attackSound2);
        }
        //Debug.Log("No enemy");
    }

    void showDetails()
    {
        //Debug.Log(HPText.name);
        HPText.text="HP: "+HP.ToString();
        //GameController.instance.playerHP=HP;
        GameController.instance.GetPlayerDataReq(gameObject);
    }

    void showDetails(bool add,int d)
    {
        //add HP
        if(add==true)
        {
            HPText.text="+"+d.ToString()+" HP: "+HP.ToString();
        }
        else if(add==false)
        {
            HPText.text="-"+d.ToString()+" HP: "+HP.ToString();
        }
        //GameController.instance.playerHP=HP;
        GameController.instance.GetPlayerDataReq(gameObject);
    }
    //int index=0;
    void Update()
    {
        //Debug.Log("Player Update");
        if(!GameController.instance.playerTurn)
            return;

        int hor=0;
        int ver=0;

        hor=(int)Input.GetAxisRaw("Horizontal");
        ver=(int)Input.GetAxisRaw("Vertical");

        //Debug.Log(hor.ToString()+ver.ToString());
        if(hor!=0)
            ver=0;
        if(hor!=0||ver!=0)
        {
            xdir=hor;
            ydir=ver;
           // GameController.instance.playerTurn=false;
            AttemptMove<Wall>(hor,ver);
        }
        //End invincible state
        if(invinDistance<=0) 
        {
            invincible=false;
        }
    }

    public void RecieveFromGM()
    {
        //HP=GameController.instance.playerHP;
        GetDataFromGM();
        showDetails(true,30);
        //showDetails();
    }

    public void GetInvincible(int distance)
    {
        invinDistance=invinDistance+distance;
        invincible=true;
    }


    void Awake()
    {
        Debug.Log("Player inited");
        HPText=GameObject.Find("HPText").GetComponent<Text>();
    }
}
