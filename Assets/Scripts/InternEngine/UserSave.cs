using UnityEngine;
using System.Collections;

public class UserSave {

	private UserSave()
	{
		
	}
	
	private static UserSave _inst;
	
	public static UserSave Inst{
		get{
			if(_inst == null)
			{
				_inst = new UserSave();	
			}
			return _inst;
		}
	}
	
	public void saveLevel(string levelName, float time)
	{
		PlayerPrefs.SetFloat(levelName, time);	
		PlayerPrefs.Save();
	}
	
	public float getLevelScore(string levelName)
	{
		
		return PlayerPrefs.HasKey(levelName) ? PlayerPrefs.GetFloat(levelName) : -1f;
	}
	
	public void clearSave()
	{
		PlayerPrefs.DeleteAll();
		PlayerPrefs.Save();
	}
}
