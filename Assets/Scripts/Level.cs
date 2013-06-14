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
	
	private int startXPlayer;
	private int startYPlayer;
	
	public Level(string tname, string tID, BLOCSTATE[,,] tlevelState, List<SerializableEnemy> tlistEnemy, int startX, int startY)
	{
		name = tname;
		ID = tID;
		levelState = tlevelState;
		listEnemy = tlistEnemy;
		startXPlayer = startX;
		startYPlayer = startY;
	}
	
	public BLOCSTATE[,,] getLevelState()
	{
		return levelState;
	}
	
	public List<SerializableEnemy> getEnemies()
	{
		return listEnemy;	
	}
	
	public Vector2 getPlayerSpawn()
	{
		return new Vector2(startXPlayer, startYPlayer);
	}
	
}
