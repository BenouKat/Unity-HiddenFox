using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BDDLevelTime {
	
	public Dictionary<string, Vector3> bdd;
	
	public string actualLevel;
	
	private BDDLevelTime()
	{
		bdd = new Dictionary<string, Vector3>();	
		
		bdd.Add("LevelTest", new Vector3(20f, 23f, 26f));
	}
	
	private static BDDLevelTime _inst;
	
	public static BDDLevelTime Inst{
		get{
			if(_inst == null)
			{
				_inst = new BDDLevelTime();	
			}
			return _inst;
		}
	}
	
	public float getNextGoal(float bestTime)
	{
		if(bestTime < 0f || bestTime > getBronzeTime()){
			return getBronzeTime();
		}else if(bestTime > getSilverTime())
		{
			return getSilverTime();	
		}else if(bestTime > getGoldTime())
		{
			return getGoldTime();	
		}else{
			return -1f;	
		}
	}
	
	public int getRank(float bestTime)
	{
		if(bestTime < 0f || bestTime > getBronzeTime()){
			return 3;
		}else if(bestTime > getSilverTime())
		{
			return 2;	
		}else if(bestTime > getGoldTime())
		{
			return 1;	
		}else{
			return 0;	
		}	
	}
	
	public float getBronzeTime()
	{
		return bdd[actualLevel].z;	
	}
	
	public float getSilverTime()
	{
		return bdd[actualLevel].y;	
	}
	
	public float getGoldTime()
	{
		return bdd[actualLevel].x;
	}
	
	public float getBronzeTime(string name)
	{
		return bdd[name].z;	
	}
	
	public float getSilverTime(string name)
	{
		return bdd[name].y;	
	}
	
	public float getGoldTime(string name)
	{
		return bdd[name].x;
	}
	
	
}
