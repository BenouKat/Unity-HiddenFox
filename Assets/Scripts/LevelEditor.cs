using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public enum BLOCSTATE{
	EMPTY,
	CUBE,
	PLAYERSTART,
	ENEMYSTART
}

public class LevelEditor{
	
	public BLOCSTATE[,,] levelState;
	
	public GameObject[,,] gameObjectList;
	
	public Vector2 startPlayerPosition;
	
	public LevelEditor(int maxWidth, int maxHeight, int maxVolume)
	{
		startPlayerPosition = new Vector2(0f, 0f);
		levelState = new BLOCSTATE[maxWidth,maxHeight,maxVolume];
		gameObjectList = new GameObject[maxWidth, maxHeight, maxVolume];
	}
	
	public void purge(int maxWidth, int maxHeight, int maxVolume)
	{
		for(int i=0; i<maxWidth; i++)
		{
			for(int j=0; j<maxHeight; j++)
			{
				for(int h=0; h<maxVolume; h++)
				{
					if(levelState[i,j,h] == BLOCSTATE.CUBE || levelState[i,j,h] == BLOCSTATE.ENEMYSTART)
					{
						GameObject.Destroy(gameObjectList[i,j,h]);
						gameObjectList[i,j,h] = null;
					}
					levelState[i,j,h] = BLOCSTATE.EMPTY;
				}
			}
		}
		startPlayerPosition = new Vector2(0f, 0f);
	}
	
	public void setCube(int w, int h, int v, GameObject go)
	{
		levelState[w,h,v] = BLOCSTATE.CUBE;
		gameObjectList[w,h,v] = go;
	}
	
	public void removeCube(int w, int h, int v)
	{
		levelState[w,h,v] = BLOCSTATE.EMPTY;
		GameObject.Destroy(gameObjectList[w,h,v]);
		gameObjectList[w,h,v] = null;
	}
	
	public GameObject getGameObjectAt(int w, int h, int v)
	{
		return gameObjectList[w,h,v];
	}
	
	public BLOCSTATE getBlocState(int w, int h, int v)
	{
		return levelState[w, h, v];
	}	
	
	public bool isDestroyable(int w, int h, int v)
	{
		return v != 0 || ((w != startPlayerPosition.x || h != startPlayerPosition.y) && levelState[w,h,1] != BLOCSTATE.ENEMYSTART);
	}
	
	public void setDisplayUpperBlocs(bool show, int maxWidth, int maxHeight, int maxVolume, int actualVolume)
	{
		for(int i=0; i<maxWidth; i++)
		{
			for(int j=0; j<maxHeight; j++)
			{
				for(int h=actualVolume+1; h<maxVolume; h++)
				{
					if(getBlocState(i,j,h) == BLOCSTATE.CUBE)
					{
						gameObjectList[i,j,h].renderer.enabled = show;
					}
				}	
			}
		}
	}
	
	public void setDisplayThisLevel(bool show, int maxWidth, int maxHeight, int actualVolume)
	{
		for(int i=0; i<maxWidth; i++)
		{
			for(int j=0; j<maxHeight; j++)
			{
				if(getBlocState(i,j,actualVolume) == BLOCSTATE.CUBE)
				{
					gameObjectList[i,j,actualVolume].renderer.enabled = show;
				}
			}
		}
	}
	
	public void setStartPlayerPosition(int width, int height)
	{
		levelState[width, height, 1] = BLOCSTATE.PLAYERSTART;
		startPlayerPosition = new Vector2((float)width, (float)height);
	}
	
	public void setStartEnemyPosition(int width, int height, GameObject go)
	{
		levelState[width, height, 1] = BLOCSTATE.ENEMYSTART;
		gameObjectList[width, height, 1] = go;
	}
	
	public void removePlayerPosition()
	{
		levelState[(int)startPlayerPosition.x,(int)startPlayerPosition.y,1] = BLOCSTATE.EMPTY;
	}
	
	public void removeEnemyPosition(int width, int height)
	{
		levelState[width, height, 1] = BLOCSTATE.EMPTY;
		GameObject.Destroy(gameObjectList[width, height, 1]);
		gameObjectList[width, height, 1] = null;
	}
	
	
	public Level saveLevel(string name, string ID, List<Enemy> listEnemy)
	{
		var listSE = new List<SerializableEnemy>();
		for(int i=0; i<listEnemy.Count; i++)
		{
			listSE.Add(listEnemy.ElementAt(i).saveEnemy());	
		}
		return new Level(name, ID, levelState, listSE, (int)startPlayerPosition.x, (int) startPlayerPosition.y);
	}
	
	public void loadLevel(Level l)
	{
		levelState = l.getLevelState();
		startPlayerPosition = l.getPlayerSpawn();
	}
	
	public BLOCSTATE[,,] getEntireBlocState()
	{
		return levelState;
	}
}
