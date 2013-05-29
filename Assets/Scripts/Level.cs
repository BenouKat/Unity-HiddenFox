using UnityEngine;
using System.Collections;

public enum BLOCSTATE{
	EMPTY,
	CUBE
}

public class Level{
	
	public BLOCSTATE[,,] levelState;
	
	public GameObject[,,] gameObjectList;
	
	
	public Level(int maxWidth, int maxHeight, int maxVolume)
	{
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
		gameObjectList[w,h,v] = null;
	}
	
	public GameObject getGameObjectAt(int w, int h, int v)
	{
			
	}
	
	public bool isEmpty(int w, int h, int v)
	{
		return levelState[w, h, v] == BLOCSTATE.EMPTY;
	}	
}
