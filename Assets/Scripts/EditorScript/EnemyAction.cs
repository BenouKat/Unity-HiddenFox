using UnityEngine;
using System.Collections;

[System.Serializable]
public enum MOVE{
	LEFT,
	RIGHT,
	UP,
	DOWN,
	WAIT,
	LOOKLEFT,
	LOOKRIGHT,
	LOOKUP,
	LOOKDOWN
}

[System.Serializable]
public class EnemyAction{
	
	
	public MOVE state;
	
	public float timeWait;
	
	public EnemyAction(MOVE action, float waiting)
	{
		if(action < MOVE.WAIT)
		{
			waiting = 0f;
		}else{
			timeWait = waiting;
		}
		state = action;
	}
	
	
}
