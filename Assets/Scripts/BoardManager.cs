using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BoardManager : MonoBehaviour
{
    [Serializable]
    public class Count 
    {
        public int min;
        public int max;
        public Count(int mmin,int mmax)
        {
            min = mmin;
            max = mmax;
        }
    }

    public Count wallCount = new Count(5, 9);
    public Count foodCount = new Count(1, 5);

    public int row=8;
    public int col=8;
    public GameObject[] floorTiles;
    public GameObject[] outerWallTiles;
    public GameObject exitTile;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;

    Transform itemsHolder;
    Transform boardHolder;
    List<Vector3> gridPositions = new List<Vector3>();
    GameObject tempExit;

    //Generate level
    public void SetupScene(int level)
    {
        Debug.Log("Board Init Level "+level);

        BoardSetup();
        itemsHolder=new GameObject("Items").transform;
        //Init exit
        tempExit=Instantiate(exitTile, new Vector3(col - 1, row - 1, 0f), Quaternion.identity);
        tempExit.transform.SetParent(itemsHolder);
        //Init random wall, food and enemy
        InitGridList();
        InitRandomObject(wallTiles, wallCount.min, wallCount.max);
        InitRandomObject(foodTiles, foodCount.min, foodCount.max);
        int enemyCount = (int)Math.Log(level, 2f);
        InitRandomObject(enemyTiles, enemyCount, enemyCount);

    }

    //Generate outerwall floor
    public void BoardSetup()
    { 
        boardHolder = new GameObject("Board").transform;
        for(int i=-1;i<row+1;++i){
            for(int j=-1;j<col+1;++j)
            {
                GameObject toInstantiate=floorTiles[UnityEngine.Random.Range(0,floorTiles.Length)];
                if(i==-1||i==row||j==-1||j==col)
                    toInstantiate=outerWallTiles[UnityEngine.Random.Range(0,outerWallTiles.Length)];
                GameObject instance=Instantiate(toInstantiate,new Vector3(j,i,0f),Quaternion.identity) as GameObject;
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    public void DeleteBoard()
    {
        Destroy(boardHolder.gameObject);
        Destroy(itemsHolder.gameObject);
        //Destroy(exitTile);
    }

    //Init Grid List
    public void InitGridList()
    {
        gridPositions.Clear();
        for (int i = 0; i < col - 1; ++i)
        {
            for (int j = 0; j < row - 1; ++j)
            {
                if(i==0 && j==0)
                    continue;
                gridPositions.Add(new Vector3(i, j, 0f));
            }
        }
    }

    //Generate random grid, return a positon
    Vector3 RandomPosition()
    {
        int index = UnityEngine.Random.Range(0, gridPositions.Count);
        Vector3 pos = gridPositions[index];
        //Remove the selected position
        gridPositions.RemoveAt(index);
        return pos;
    }

    //Generate random objects at random positions
    void InitRandomObject(GameObject[] tileArray,int min,int max)
    {
        int count = UnityEngine.Random.Range(min, max + 1);
        for(int i=0;i<count;++i)
        {
            Vector3 pos = RandomPosition();
            GameObject tileChoice = tileArray[UnityEngine.Random.Range(0, tileArray.Length)];
            tileChoice=Instantiate(tileChoice, pos, Quaternion.identity);
            tileChoice.transform.SetParent(itemsHolder);
        }
    }
}
