using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Tile
{
    Floor,
    Wall,//can be destoryed
    OuterWall//cannot be destroyed
}


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

    public int row=31;
    public int col=31;
    public GameObject[] floorTiles;
    public GameObject[] outerWallTiles;
    public GameObject exitTile;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] potionTiles;
    Transform itemsHolder;
    Transform boardHolder;
    List<Vector3> gridPositions = new List<Vector3>();
    GameObject tempExit;
    Tile[,] mapArray;
    GameObject map;
    Transform maps;
    int level;
    //Generate one room
    void InitARoom(Room troom)
    {
        int roomCol=troom.horR*2+1;
        int roomRow=troom.verR*2+1;
        //Set the southwest point as zero point
        Vector3Int southwest=troom.middle-new Vector3Int(troom.horR,troom.verR,0);
        int minX=southwest.x;
        int minY=southwest.y;
        int maxX=troom.middle.x+troom.horR;
        int maxY=troom.middle.y+troom.verR;
        //send player position
        if(troom.roomType==RoomType.startRoom)
        {
            GameController.instance.SetPlayerPositon(minX+1,minY+1);
        }
        //Debug.Log("Board Init Level "+level);
        //Init outer wall
        for(int i=minX;i<maxX+1;++i){
           for(int j=minY;j<maxY+1;++j)
           {
               GameObject toInstantiate=floorTiles[UnityEngine.Random.Range(0,floorTiles.Length)];
               if(i==minX||i==maxX||j==minY||j==maxY)
                   toInstantiate=outerWallTiles[UnityEngine.Random.Range(0,outerWallTiles.Length)];
               GameObject instance=Instantiate(toInstantiate,new Vector3(i,j,0f),Quaternion.identity) as GameObject;
               instance.transform.SetParent(boardHolder);
           }
        }
        //Init exit
        if(troom.roomType==RoomType.exitRoom)
        {
            tempExit=Instantiate(exitTile, new Vector3(maxX-1, maxY-1, 0f), Quaternion.identity);
            tempExit.transform.SetParent(itemsHolder);
        }

        
        //Init random wall, food and enemy
        InitGridList(minX,minY,maxX,maxY);
        InitRandomObject(wallTiles, wallCount.min, wallCount.max);
        InitRandomObject(foodTiles, foodCount.min, foodCount.max);
        int enemyCount = (int)Math.Log(level, 2f);
        InitRandomObject(enemyTiles, enemyCount, enemyCount);
        

    }


    //Main entrance, for outer call.
    public void SetupScene(int llevel)
    {
        itemsHolder=new GameObject("Items").transform;
        boardHolder = new GameObject("Board").transform;
        level=llevel;
        mapArray=new Tile[row,col];//Save map
        for(int i=0;i<row;++i)
        {
            for(int j=0;j<col;++j)
            {
                mapArray[i,j]=Tile.Wall;
            }
        }
        maps=GameObject.Find("Map").transform;
        map=new GameObject();
        map.transform.SetParent(maps);

        //GenerateMap();
        FloodGenerate();

    }

    public enum RoomType
    {
        startRoom,
        exitRoom,
        chestRoom,//linked one normal room
        normalRoom,
        none
    }
    class Room
    {
        public Vector3Int middle;
        public int horR;//radius in hor dir
        public int verR;//radius in ver dir
        public RoomType roomType;
        //public Vector3Int southwest;
        public Room(Vector3Int pos,int hr,int vr,RoomType type)
        {
            middle=pos;
            horR=hr;
            verR=vr;
            roomType=type;
        }

        public void SetRoomType(RoomType type)
        {
            roomType=type;
        }
    }

    public float doubleChance=0.2f;//the chance that appear a branch
    public float chestChance=1f;// the chance that appear a chest
    List<Room> rooms=new List<Room>();
    public int horRoomCount=4;
    public int verRoomCount=4;
    void FloodGenerate()
    {
        //Generated matrix 4x4
        RoomType[,] gm=new RoomType[horRoomCount,verRoomCount];
        for(int i=0;i<horRoomCount;i++)
        {
            for(int j=0;j<verRoomCount;j++)
            {gm[i,j]=RoomType.none;}
        }
        //choose a start room
        int startRoomX=UnityEngine.Random.Range(0,horRoomCount);
        gm[startRoomX,0]=RoomType.startRoom;
        GameController.instance.SetPlayerPositon(startRoomX,0);
        int curY=0;
        int curX=startRoomX;
        int lastX=curX;
        int lastY=curY;
        bool twice=false;
        while(true)
        {
            int dir;
            int prob=UnityEngine.Random.Range(0,11);
            if(prob<4) dir=0;
            else if(prob>6) dir=1;
            else dir=2;
            //three dir, 0 means west, 1 means east, 2 means north.
            //if goes west or east and out of edge, set dir to north
            if((dir==0 && curX==0)||(dir==1 && curX==horRoomCount-1))
            {
                dir=2;
            }
            if(dir==0) curX=curX-1;
            else if(dir==1) curX=curX+1;
            else if(dir==2) curY=curY+1;
            //decide when to end
            //if Y out of edge, set (curX,curY-1) as exit room and break.
            if(curY==verRoomCount)
            {
                gm[curX,curY-1]=RoomType.exitRoom;
                break;
            }
            //set (curX,curY) as normal room.
            if(gm[curX,curY]==RoomType.none)
            {
                gm[curX,curY]=RoomType.normalRoom;
            }
            //decide whehter be a chest room
            if(twice==true && UnityEngine.Random.Range(0,100)<=chestChance*100)
            {
                gm[curX,curY]=RoomType.chestRoom;
               // Debug.Log("Bonus!");
            } 
            //if have second chance, go back to last state.
            if(twice==false && UnityEngine.Random.Range(0,100)<=doubleChance*100)
            {
                curX=lastX;
                curY=lastY;
                //Debug.Log("Twice!");
                twice=true;
            }
            else
            {
                twice=false;
                lastX=curX;
                lastY=curY;
            }   
        }//while ends
        //For each in gm matrix, add a room to room list
        //calculate standard room radius
        int horRadius=(((col-horRoomCount+1)/horRoomCount)-1)/2;
        int verRadius=(((row-verRoomCount+1)/verRoomCount)-1)/2;
        for(int i=0;i<verRoomCount;++i)
        {
            for(int j=0;j<horRoomCount;++j)
            {
                int roomX=(2*horRadius+1)*(i)+i+horRadius;
                int roomY=(2*verRadius+1)*(j)+j+verRadius;
                int roomXR=horRadius;
                int roomYR=verRadius;
                //Random smaller room
                if(UnityEngine.Random.Range(0,100)<=10)
                {
                    roomXR=roomXR-1;
                    roomYR=roomYR-1;
                }
                //Init a room
                Room tempRoom=new Room(new Vector3Int(roomX,roomY,0),roomXR,roomYR,RoomType.none);
                if(gm[i,j]==RoomType.startRoom)
                {
                    tempRoom.SetRoomType(RoomType.startRoom);
                }
                else if(gm[i,j]==RoomType.exitRoom)
                {
                    tempRoom.SetRoomType(RoomType.exitRoom);
                }
                else if(gm[i,j]==RoomType.chestRoom)
                {
                    tempRoom.SetRoomType(RoomType.chestRoom);
                }
                else if(gm[i,j]==RoomType.normalRoom)
                {
                    tempRoom.SetRoomType(RoomType.normalRoom);
                }
                //Add to room list
                if(tempRoom.roomType!=RoomType.none)
                {
                    rooms.Add(tempRoom);
                }
            }//for ends
        }//for ends
        //Init each room
        for(int i=0;i<rooms.Count;++i)
        {
            InitARoom(rooms[i]);
        }

        /*
        //For test
        for(int i=0;i<horRoomCount;++i)
        {
            for(int j=0;j<verRoomCount;++j)
            {
                if(gm[i,j]==RoomType.none)
                {
                    PutObjectAt(outerWallTiles,i,j);
                }
                else if(gm[i,j]==RoomType.exitRoom)
                {
                    tempExit=Instantiate(exitTile, new Vector3(i,j, 0f), Quaternion.identity);
                    tempExit.transform.SetParent(itemsHolder);
                }
                else if(gm[i,j]==RoomType.chestRoom)
                {
                    PutObjectAt(floorTiles,i,j);
                    PutObjectAt(potionTiles,i,j);
                }
                else
                {
                    PutObjectAt(floorTiles,i,j);
                }
            }
        }*/
    }

    class mPostion
    {
        public int x;
        public int y;
        public bool stepped;
        public mPostion(int xx,int yy)
        {
            x=xx;
            y=yy;
            stepped=true;
        }
    }
    public int chooseStartTimes=3;
    public int initPathTimes=3;
    public int PathLength=3;

    void GenerateMap()
    {
        List<mPostion> paths=new List<mPostion>();//save stepped positions. Means this is PATH.
        List<mPostion> walls=new List<mPostion>();//save wall positions.
        int startX=UnityEngine.Random.Range(0,row);
        int startY=UnityEngine.Random.Range(0,col);
        //Generate paths
        for(int i=0;i<chooseStartTimes;++i)
        {
            //after first loop, choose one path tile as start point.
            if(i!=0 && paths.Count!=0) 
            {
                mPostion tempStart=paths[UnityEngine.Random.Range(paths.Count*99/100,paths.Count)];
                startX=tempStart.x;
                startY=tempStart.y;
            }    
            int curX=startX,curY=startY;
            for(int j=0;j<initPathTimes;++j)
            {
                int dir=UnityEngine.Random.Range(0,4);
                for(int k=0;k<PathLength;++k)
                {
                    //random dir
                    if(dir==0) curX=curX-1;
                    else if(dir==1) curX=curX+1;
                    else if(dir==2) curY=curY-1;
                    else if(dir==3) curY=curY+1;
                    //detect out of edge
                    if(curY<0)
                    {
                        curY=0;
                        break;
                    }
                    if(curY>=col)
                    {
                        curY=col-1;
                        break;
                    }
                    if(curX<0)
                    {
                        curX=0;
                        break;
                    }
                    if(curX>=row)
                    {
                        curX=row-1;
                        break;
                    }
                    //Init Path and put it in Paths list
                    mPostion temp=new mPostion(curX,curY);
                    paths.Add(temp);
                }
            }
        }
        //Mark mapArray
        for(int i=0;i<paths.Count;++i)
        {
            mPostion tempPos=paths[i];
            mapArray[tempPos.x,tempPos.y]=Tile.Floor;
            if(i==0)
            {
                GameController.instance.SetPlayerPositon(tempPos.x,tempPos.y);
            }
        }
        //Init Map
        for(int i=0;i<row;++i)
        {
            for(int j=0;j<col;++j)
            {
                if(mapArray[i,j]==Tile.Floor)
                {
                    PutObjectAt(floorTiles,i,j);
                }
                else if(mapArray[i,j]==Tile.Wall)
                {
                    PutObjectAt(outerWallTiles,i,j);
                }
            }
        }

    }
    void PutObjectAt(GameObject[] tileArray,int x,int y)
    {
        Vector3 pos=new Vector3(x,y,0);
        GameObject tileChoice = tileArray[UnityEngine.Random.Range(0, tileArray.Length)];
        tileChoice=Instantiate(tileChoice, pos, Quaternion.identity);
        tileChoice.transform.SetParent(boardHolder);
    }

    

    public void DeleteBoard()
    {
        Destroy(boardHolder.gameObject);
        Destroy(itemsHolder.gameObject);
        //Destroy(exitTile);
    }

    //Init Grid List
    public void InitGridList(int minX,int minY,int maxX,int maxY)
    {
        gridPositions.Clear();
        for (int i = minX+1; i < maxX; ++i)
        {
            for (int j = minY+1; j < maxY; ++j)
            {
                if(i==minX+1 && j==minY+1)
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



    public void AddOnePotion(Vector3 pos)
    {
        GameObject tileChoice = potionTiles[UnityEngine.Random.Range(0, potionTiles.Length)];
        tileChoice=Instantiate(tileChoice, pos, Quaternion.identity);
        tileChoice.transform.SetParent(itemsHolder);
    }
}
