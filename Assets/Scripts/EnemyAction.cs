using UnityEngine;
using System.Collections;

public enum MOVE{
	LEFT,
	RIGHT,
	UP,
	DOWN,
	WAIT,
	LOOKLEFT,
	LOOKRIGHT,
	LOOKUP,
	LOOKDOWN,
	POWER1,
	POWER2,
	POWER3
}

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
