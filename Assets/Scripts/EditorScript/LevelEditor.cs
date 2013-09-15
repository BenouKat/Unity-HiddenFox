using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public enum BLOCSTATE{
	EMPTY,
	CUBE,
	WALL,
	SPECIAL,
	PLAYERSTART,
	PLAYERFINISH,
	CANON,
	CAMERA,
	ENEMYSTART,
	HELICOSTART
}

public class LevelEditor{
	
	public BLOCSTATE[,,] levelState;
	
	public GameObject[,,] gameObjectList;
	
	public Vector2 startPlayerPosition;
	
	public Vector2 finishPlayerPosition;
	
	//Used for quick save
	public List<GameObject> listObjectsForSave;
	
	public LevelEditor(int maxWidth, int maxHeight, int maxVolume)
	{
		startPlayerPosition = new Vector2(0f, 0f);
		finishPlayerPosition = new Vector2(0f, 0f);
		levelState = new BLOCSTATE[maxWidth,maxHeight,maxVolume];
		gameObjectList = new GameObject[maxWidth, maxHeight, maxVolume];
		listObjectsForSave = new List<GameObject>();
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
	
	public void setSpecial(int w, int h, int v, GameObject go)
	{
		levelState[w,h,v] = BLOCSTATE.SPECIAL;
		gameObjectList[w,h,v] = go;
	}
	
	
	public void setCanon(int w, int h, GameObject go)
	{
		levelState[w,h,1] = BLOCSTATE.CANON;
		gameObjectList[w,h,1] = go;
		listObjectsForSave.Add(go);
	}
	
	public void setCameraEnemy(int w, int h, GameObject go)
	{
		levelState[w,h,2] = BLOCSTATE.CAMERA;
		gameObjectList[w,h,2] = go;
		listObjectsForSave.Add(go);
	}
	
	
	public void removeCube(int w, int h, int v)
	{
		levelState[w,h,v] = BLOCSTATE.EMPTY;
		GameObject.Destroy(gameObjectList[w,h,v]);
		gameObjectList[w,h,v] = null;
	}
	
	public void removeSpecial(int w, int h, int v)
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
					if(getBlocState(i,j,h) == BLOCSTATE.CUBE || getBlocState(i,j,h) == BLOCSTATE.SPECIAL)
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
				if(getBlocState(i,j,actualVolume) == BLOCSTATE.CUBE || getBlocState(i,j,actualVolume) == BLOCSTATE.SPECIAL)
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
	
	public void setFinishPlayerPosition(int width, int height)
	{
		levelState[width, height, 1] = BLOCSTATE.PLAYERFINISH;
		finishPlayerPosition = new Vector2((float)width, (float)height);
	}
	
	public void setStartEnemyPosition(int width, int height, GameObject go)
	{
		levelState[width, height, 1] = BLOCSTATE.ENEMYSTART;
		gameObjectList[width, height, 1] = go;
	}
	
	public void setStartEnemyHelicoPosition(int width, int height, GameObject go)
	{
		levelState[width, height, 3] = BLOCSTATE.HELICOSTART;
		gameObjectList[width, height, 3] = go;
	}
	
	public void removePlayerPosition()
	{
		levelState[(int)startPlayerPosition.x,(int)startPlayerPosition.y,1] = BLOCSTATE.EMPTY;
	}
	
	public void removeFinishPosition()
	{
		levelState[(int)finishPlayerPosition.x,(int)finishPlayerPosition.y,1] = BLOCSTATE.EMPTY;
	}
	
	public void removeCanonPosition(int width, int height)
	{
		levelState[width, height, 1] = BLOCSTATE.EMPTY;
		listObjectsForSave.Remove(gameObjectList[width, height, 1]);
		GameObject.Destroy(gameObjectList[width, height, 1]);
		gameObjectList[width, height, 1] = null;
	}
	
	public void removeCameraEnemyPosition(int width, int height)
	{
		levelState[width, height, 2] = BLOCSTATE.EMPTY;
		listObjectsForSave.Remove(gameObjectList[width, height, 2]);
		GameObject.Destroy(gameObjectList[width, height, 2]);
		gameObjectList[width, height, 2] = null;
	}
	
	public void removeEnemyPosition(int width, int height)
	{
		levelState[width, height, 1] = BLOCSTATE.EMPTY;
		GameObject.Destroy(gameObjectList[width, height, 1]);
		gameObjectList[width, height, 1] = null;
	}
	
	public void removeEnemyHelicoPosition(int width, int height)
	{
		levelState[width, height, 3] = BLOCSTATE.EMPTY;
		GameObject.Destroy(gameObjectList[width, height, 3]);
		gameObjectList[width, height, 3] = null;
	}
	
	
	public Level saveLevel(string name, string ID, List<Enemy> listEnemy)
	{
		var listSE = new List<SerializableEnemy>();
		var listSO = new List<SerializableObject>();
		for(int i=0; i<listEnemy.Count; i++)
		{
			listSE.Add(listEnemy.ElementAt(i).saveEnemy());	
		}
		for(int i=0; i<listObjectsForSave.Count; i++)
		{
			if(listObjectsForSave.ElementAt(i).GetComponent("Canon") != null)
			{
				var theCanon = listObjectsForSave.ElementAt(i).GetComponent<Canon>();
				listSO.Add(new SerializableObject(true, theCanon.getLook(), (int)((theCanon.transform.position.z+0.5f)/2f), (int)((theCanon.transform.position.x+0.5f)/2f)));   
			}else{
				var theCamera = listObjectsForSave.ElementAt(i).GetComponent<CameraEnemy>();
				listSO.Add(new SerializableObject(false, theCamera.getLook(), (int)((theCamera.transform.position.z+0.5f)/2f), (int)((theCamera.transform.position.x+0.5f)/2f), theCamera.rightFirst));
			}
		}

		return new Level(name, ID, levelState, listSE, listSO, (int)startPlayerPosition.x, (int) startPlayerPosition.y, (int) finishPlayerPosition.x, (int) finishPlayerPosition.y);
	}
	
	public void loadLevel(Level l)
	{
		levelState = l.getLevelState();
		startPlayerPosition = l.getPlayerSpawn();
		finishPlayerPosition = l.getPlayerFinish();
	}
	
	public BLOCSTATE[,,] getEntireBlocState()
	{
		return levelState;
	}
}
