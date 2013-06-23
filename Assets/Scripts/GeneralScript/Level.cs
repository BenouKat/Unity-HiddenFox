using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Level {

	private string name;
	private string ID;
	
	private BLOCSTATE[,,] levelState;
	
	private List<SerializableEnemy> listEnemy;
	private List<SerializableObject> listObject;
	
	private int startXPlayer;
	private int startYPlayer;
	
	private int finishXPlayer;
	private int finishYPlayer;
	
	public Level(string tname, string tID, BLOCSTATE[,,] tlevelState, List<SerializableEnemy> tlistEnemy, List<SerializableObject> tlistObject, int startX, int startY, int finishX, int finishY)
	{
		name = tname;
		ID = tID;
		levelState = tlevelState;
		listEnemy = tlistEnemy;
		listObject = tlistObject;
		startXPlayer = startX;
		startYPlayer = startY;
		finishXPlayer = finishX;
		finishYPlayer = finishY;
	}
	
	public BLOCSTATE[,,] getLevelState()
	{
		return levelState;
	}
	
	public List<SerializableEnemy> getEnemies()
	{
		return listEnemy;	
	}
	
	public List<SerializableObject> getObjects(){
		return listObject;	
	}
	
	public Vector2 getPlayerSpawn()
	{
		return new Vector2(startXPlayer, startYPlayer);
	}
	
	public Vector2 getPlayerFinish()
	{
		return new Vector2(finishXPlayer, finishYPlayer);
	}
	
}
