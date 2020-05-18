using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
    public LayerMask blockingLayer;
    public float moveTime=0.1f;//time per moving
    BoxCollider2D boxCollider;
    Rigidbody2D rd2D;
    float inverseMoveTime;

    //init
    protected virtual void Start()
    {
        boxCollider=GetComponent<BoxCollider2D>();
        rd2D=GetComponent<Rigidbody2D>();
        inverseMoveTime=1f/moveTime;//speed=(dis=1/time=moveTime)
    }

    //Corutine smooth movement
    protected IEnumerator Smoothmove(Vector3 end)
    {
        float sqrRemainDist=(transform.position-end).sqrMagnitude;
        while(sqrRemainDist>float.Epsilon)
        {
            Vector3 newPos=Vector3.MoveTowards(rd2D.position,end,inverseMoveTime*Time.deltaTime);
            rd2D.position=newPos;
            sqrRemainDist=(transform.position-end).sqrMagnitude;
            yield return null;//stop program till next frame
        }
       // GameController.instance.playerTurn=true;
    }

    //not need implementation
    protected abstract void OnCantMove<T>(T component)
        where T:Component;

    //Obstacle detection
    protected bool Move(int xdir,int ydir,out RaycastHit2D hit)
    {
        Vector2 start=transform.position;
        Vector2 end=start+new Vector2(xdir,ydir);
        
        boxCollider.enabled=false;
        hit=Physics2D.Linecast(start,end,blockingLayer);
        boxCollider.enabled=true;

        /*if(end.x<0 || end.x>7 || end.y>7 ||end.y<0)
        {

            return false;
        }*/

        if(hit.transform==null)
        {
            //Debug.Log("start moving");
            StartCoroutine(Smoothmove(end));
            return true;
        }
        return false;
    }

    //Override Move,Add a paramenter to decide whether sub HP
    protected bool Move(int xdir,int ydir,out bool loseHP,out RaycastHit2D hit)
    {
        Vector2 start=transform.position;
        Vector2 end=start+new Vector2(xdir,ydir);

        boxCollider.enabled=false;
        hit=Physics2D.Linecast(start,end,blockingLayer);
        boxCollider.enabled=true;

       /* if(end.x<0 || end.x>7 || end.y>7 ||end.y<0)
        {
            loseHP=false;
            return false;
        }*/

        if(hit.transform==null)
        {
            //Debug.Log("start moving");
            StartCoroutine(Smoothmove(end));
            loseHP=true;
            return true;
        }
        loseHP=true;
        return false;
    }

    //Just detect whether play sound, no implement of move
    protected bool MoveCheck(int xdir,int ydir,out RaycastHit2D hit)
    {
        Vector2 start=transform.position;
        Vector2 end=start+new Vector2(xdir,ydir);
        //RaycastHit2D hit;

        boxCollider.enabled=false;
        hit=Physics2D.Linecast(start,end,blockingLayer);
        boxCollider.enabled=true;

        if(end.x<0 || end.x>7 || end.y>7 ||end.y<0)
        {
            return false;
        }

        if(hit.transform==null)
        {
            //Debug.Log("start moving");
            //StartCoroutine(Smoothmove(end));
            return true;
        }
        return false;
    }

    protected virtual void AttemptMove <T>(int xdir,int ydir)
        where T:Component
    {
        RaycastHit2D hit;
        bool canMove = Move(xdir,ydir,out hit);
        //Debug.Log(canMove);
        if(hit.transform==null)
        {
            //Debug.Log("hit is null");
            return;
        }
        T hitComponent=hit.transform.GetComponent<T>();
        //Enemy hitEnemy=hit.transform.GetComponent<Enemy>();
        //Has hit component then can't move
        //if(!canMove&&hitComponent!=null)
        if(!canMove)
        {
            //Debug.Log("Cant Move");
            //Debug.Log(hitComponent);
            OnCantMove(hitComponent);
        }
        /*
        else if(!canMove)
        {
            Debug.Log("Edge!!");
            Debug.Log(hitComponent);
        }*/
    }
   
}
