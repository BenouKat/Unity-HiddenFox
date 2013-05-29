using UnityEngine;
using System.Collections;

public enum BLOCSTATE{
	EMPTY,
	CUBE,
	PLAYERSTART
}

public class Level{
	
	public BLOCSTATE[,,] levelState;
	
	public GameObject[,,] gameObjectList;
	
	public Vector2 startPlayerPosition;
	
	public bool playerPositionSet;
	
	public Level(int maxWidth, int maxHeight, int maxVolume)
	{
		startPlayerPosition = new Vector2(0f,0f);
		levelState = new BLOCSTATE[maxWidth,maxHeight,maxVolume];
		gameObjectList = new GameObject[maxWidth, maxHeight, maxVolume];
	}
	
	public void setCube(int w, int h, int v, GameObject go)
	{
		levelState[w,h,v] = BLOCSTATE.CUBE;
		gameObjectList[w,h,v] = go;
	}
	
	public void removeCube(int w, int h, int v)
	{
		levelState[w,h,v] = BLOCSTATE.EMPTY;
		Destroy(gameObjectList[w,h,v]);
		gameObjectList[w,h,v] = null;
	}
	
	public GameObject getGameObjectAt(int w, int h, int v)
	{
		return gameObjectList[w,h,v];
	}
	
	public bool getBlocState(int w, int h, int v)
	{
		return levelState[w, h, v];
	}	
	
	public bool isDestroyable(int w, int h, int v)
	{
		return v != 0 || w != startPlayerPosition.x || h != startPlayerPosition.y;
	}
	
	public void setDisplayUpperBlocs(bool show, int maxWidth, int maxHeight, int maxVolume, int actualVolume)
	{
		for(int i=0; i<maxWidth; i++)
		{
			for(int j=0; j<maxHeight; j++)
			{
				for(int h=actualVolume+1; h<maxVolume; h++)
				{
					if(!isEmpty(i,j,h))
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
				if(!isEmpty(i,j,actualVolume))
				{
					gameObjectList[i,j,actualVolume].renderer.enabled = show;
				}
			}
		}
	}
	
	public void setStartPlayerPosition(int width, int height)
	{
		levelState[w,h,1] = BLOCSTATE.PLAYERSTART;
		startPlayerPosition = new Vector2((float)width, (float)height);
		playerPositionSet = true;
	}
	
	public void removePlayerPosition()
	{
		levelState[startPlayerPosition.x,startPlayerPosition.y,1] = BLOCSTATE.EMPTY;
		playerPositionSet = false;
	}
}
