using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Reflection;

public class DynamicLoader : MonoBehaviour {
	
	public GameObject cubeBase;
	public GameObject cubeBloc;
	public GameObject cubeWall;
	public GameObject player;
	public GameObject enemyWalk;
	public GameObject canon;
	public GameObject cameraEnemy;
	public GameObject helicopter;
	public GameObject playerFinish;
	
	private List<Enemy> enemyList;
	private List<CameraEnemy> cameraList;
	private List<Canon> canonList;
	
	private Vector3 playerStart;
	
	public int maxWidth;
	public int maxHeight;
	public int maxVolume;
	
	// Use this for initialization
	void Start () {
		enemyList = new List<Enemy>();
		cameraList = new List<CameraEnemy>();
		canonList = new List<Canon>();
		BDDLevelTime.Inst.actualLevel = "LevelTest"; //TEMPO
		loadLevel(BDDLevelTime.Inst.actualLevel);
	}
	
	
	public void startTheLevel()
	{
		for(int i=0; i<enemyList.Count; i++)
		{
			enemyList.ElementAt(i).go();	
		}
		for(int i=0; i<cameraList.Count; i++)
		{
			cameraList.ElementAt(i).go();	
		}
		for(int i=0; i<canonList.Count; i++)
		{
			canonList.ElementAt(i).go();	
		}
	}
	
	public void resetTheLevel()
	{
		for(int i=0; i<enemyList.Count; i++)
		{
			enemyList.ElementAt(i).reset();	
		}
		for(int i=0; i<cameraList.Count; i++)
		{
			cameraList.ElementAt(i).reset();	
		}
		for(int i=0; i<canonList.Count; i++)
		{
			canonList.ElementAt(i).reset();	
		}
		player.transform.position = playerStart;
		player.transform.eulerAngles = new Vector3(0f, 90f, 0f);
	}
	
	
	void loadLevel(string name)
	{
		Stream stream = File.Open(Application.dataPath + "/Levels/" + name + ".lvl", FileMode.Open);
		BinaryFormatter bformatter = new BinaryFormatter();
		bformatter.Binder = new VersionDeserializationBinder();
		Level l = (Level)bformatter.Deserialize(stream);
		stream.Close();
		
		var levelState = l.getLevelState();
		
		var listCubeWall = new List<Vector2>();
		//Optimisation
		for(int i=0; i<maxWidth; i++)
		{
			for(int j=0; j<maxHeight; j++)
			{
				for(int h=0; h<maxVolume; h++)
				{
					if((i >= 1 && i<maxWidth-1) && (j >= 1 && j<maxHeight-1) && (h >= 1 && h<maxVolume-1))
					{
						if(levelState[i-1,j,h] == BLOCSTATE.CUBE && levelState[i+1,j,h] == BLOCSTATE.CUBE && levelState[i,j+1,h] == BLOCSTATE.CUBE 
						&& levelState[i,j-1,h] == BLOCSTATE.CUBE && levelState[i,j,h-1] == BLOCSTATE.CUBE && levelState[i,j,h+1] == BLOCSTATE.CUBE)
						{
							levelState[i,j,h] = BLOCSTATE.EMPTY;
						}
					}
					if(h == 0)
					{
						if(levelState[i, j, h] == BLOCSTATE.CUBE)
						{
							if(i == 0)
							{
								listCubeWall.Add(new Vector2(-1, j));
							}else if(i == maxWidth-1)
							{
								listCubeWall.Add(new Vector2(maxWidth, j));
							}
							if(j == 0)
							{
								listCubeWall.Add(new Vector2(i, -1));
							}else if(j == maxHeight-1)
							{
								listCubeWall.Add(new Vector2(i, maxHeight));
							}
							
							if(i > 0 && levelState[i-1, j, h+1] == BLOCSTATE.EMPTY && levelState[i-1, j, h] == BLOCSTATE.EMPTY)
							{
								levelState[i-1, j, h+1] = BLOCSTATE.WALL;
							}
							
							if(i < maxWidth-1 && levelState[i+1, j, h+1] == BLOCSTATE.EMPTY && levelState[i+1, j, h] == BLOCSTATE.EMPTY)
							{
								levelState[i+1, j, h+1] = BLOCSTATE.WALL;
							}
							
							if(j > 0 && levelState[i, j-1, h+1] == BLOCSTATE.EMPTY && levelState[i, j-1, h] == BLOCSTATE.EMPTY)
							{
								levelState[i, j-1, h+1] = BLOCSTATE.WALL;
							}
							
							if(j < maxHeight-1 && levelState[i, j+1, h+1] == BLOCSTATE.EMPTY && levelState[i, j+1, h] == BLOCSTATE.EMPTY)
							{
								levelState[i, j+1, h+1] = BLOCSTATE.WALL;
							}
						}
					}
				}
			}
		}
		
		//Creation
		for(int i=0; i<maxWidth; i++)
		{
			for(int j=0; j<maxHeight; j++)
			{
				for(int h=0; h<maxVolume; h++)
				{	
					if(levelState[i,j,h] == BLOCSTATE.CUBE)
					{
						var go = (GameObject)Instantiate(h == 0 ? cubeBase : cubeBloc, new Vector3(j*2, h*2, i*2), cubeBase.transform.rotation);
						go.SetActive(true);
					}else if(levelState[i,j,h] == BLOCSTATE.WALL){
						var wall = (GameObject)Instantiate(cubeWall, new Vector3(j*2, h*2, i*2), cubeWall.transform.rotation);
						wall.SetActive(true);
					}
				}
			}
		}
		
		for(int i=0; i<listCubeWall.Count; i++)
		{
			var go = (GameObject)Instantiate(cubeWall, new Vector3(listCubeWall.ElementAt(i).y*2f, 2f, listCubeWall.ElementAt(i).x*2f), cubeWall.transform.rotation);
			go.SetActive(true);
		}
		
		player.transform.position = new Vector3(l.getPlayerSpawn().y*2f, 1.68f, l.getPlayerSpawn().x*2f);
		playerStart = player.transform.position;
		playerFinish.transform.position = new Vector3(l.getPlayerFinish().y*2f, 1.3f, l.getPlayerFinish().x*2f);
		playerFinish.SetActive(true);
		
		//Recréation de la liste d'ennemy
		for(int i=0; i<l.getEnemies().Count; i++)
		{
			var enemy = l.getEnemies().ElementAt(i);
			var go = (GameObject)Instantiate(enemy.isHelicopter ? helicopter : enemyWalk, new Vector3(enemy.startY*2f, enemy.isHelicopter ? 6f : 2f, enemy.startX*2f), enemyWalk.transform.rotation);
			go.SetActive(true);
			go.GetComponent<Enemy>().loadEnemy(enemy, false);
			enemyList.Add(go.GetComponent<Enemy>());
		}
		
		//Recreation de la liste d'objet (Canon / Camera)
		for(int i=0; i<l.getObjects().Count; i++)
		{
			var theObj = l.getObjects().ElementAt(i);
			if(theObj.isCanon)
			{
				var go = (GameObject)Instantiate(canon, new Vector3(theObj.positionY*2f, 2.2f, theObj.positionX*2f), canon.transform.rotation);
				go.SetActive(true);
				canonList.Add(go.GetComponent<Canon>());
				switch(theObj.theLook)
				{
				case LOOK.DOWN:
					go.transform.eulerAngles = new Vector3(0f, 0f, 0f);
					break;
				case LOOK.UP:
					go.transform.eulerAngles = new Vector3(0f, 180f, 0f);
					break;
				case LOOK.RIGHT:
					go.transform.eulerAngles = new Vector3(0f, -90f, 0f);
					break;
				case LOOK.LEFT:
					go.transform.eulerAngles = new Vector3(0f, 90f, 0f);
					break;
				}
			}else{
				var go = (GameObject)Instantiate(cameraEnemy, new Vector3(theObj.positionY*2f, 4f, theObj.positionX*2f), cameraEnemy.transform.rotation);
				go.GetComponent<CameraEnemy>().rightFirst = theObj.isRightFirst;
				go.SetActive(true);
				cameraList.Add(go.GetComponent<CameraEnemy>());
				switch(theObj.theLook)
				{
				case LOOK.DOWN:
					go.transform.eulerAngles = new Vector3(0f, 90f, 0f);
					break;
				case LOOK.UP:
					go.transform.eulerAngles = new Vector3(0f, -90f, 0f);
					break;
				case LOOK.RIGHT:
					go.transform.eulerAngles = new Vector3(0f, 0f, 0f);
					break;
				case LOOK.LEFT:
					go.transform.eulerAngles = new Vector3(0f, 180f, 0f);
					break;
				}
			}
		}
	}
}
