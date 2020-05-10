using System.Collections;
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
    int HP;
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



    //get hit lose HP
    public void LoseHP(int loss)
    {
        animator.SetTrigger("playerHit");
        HP-=loss;
        CheckGameOver();
    }

    //detection collider TAG
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag=="Food")
        {
            HP+=HPperFood;
            other.gameObject.SetActive(false);
            SoundManager.instance.RandomizeSfx(eat1,eat2);
            showDetails(true,HPperFood);
        }
        else if(other.tag=="Soda")
        {
            HP+=HPperSoda;
            other.gameObject.SetActive(false);
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
        HP--;
        //HPText.text="HP: "+HP;
        //bool loseHP;
        base.AttemptMove<T>(xdir,ydir);
        //if(loseHP)
        //{
        //    HP--;
        //}
       // RaycastHit2D hit2D;
       // if(base.canMove(xdir,ydir))
       // {
        //    SoundManager.instance.RandomizeSfx(move1,move2);
        //}
        
        CheckGameOver();
        //Debug.Log(HP);
        GameController.instance.playerTurn=false;
        //Debug.Log(GameController.instance.playerTurn);
        GameController.instance.MoveEnemyReq();
        GameController.instance.playerHP=HP;
        showDetails();
    }


    void CheckGameOver()
    {
        if(HP<=0)
        {
            Debug.Log("died");
            SoundManager.instance.RandomizeSfx(gameoverSound);
            GameController.instance.GameOver();
            SoundManager.instance.musicSource.Stop();
            this.enabled=false;
        }
    }

    void Restart()
    {
        SceneManager.LoadScene(0);
    }

    protected override void Start()
    {
        animator=GetComponent<Animator>();
        base.Start();
        HP=GameController.instance.playerHP;
        //HPText.text="HP: "+HP;
        //HPText=GetComponentInChildren<Canvas>().GetComponentInChildren<Text>();
        showDetails();
    }

    //return HP now
    void OnDisable()
    {
        GameController.instance.playerHP=HP;
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
            HP++;
            //HPText.text="HP: "+HP;
        }
        //check whether attack enemy
        HitEnemy();

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
            HP--;
            //Debug.Log("Atk Enemy");
            hitComponent.GetAttack(this.atk);
            animator.SetTrigger("playerChop");
            SoundManager.instance.RandomizeSfx(attackSound1,attackSound2);
        }
        //Debug.Log("No enemy");
    }

    void showDetails()
    {
        HPText.text="HP: "+HP.ToString();
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
        //Debug.Log("Player Update end");
    }

    void Awake()
    {
        Debug.Log("Player inited");
        HPText=GameObject.Find("HPText").GetComponent<Text>();
    }
}
