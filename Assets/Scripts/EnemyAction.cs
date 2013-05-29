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
}

public class EnemyAction{
	
	
	public MOVE state;
	
	public float timeWait;
	
	public EnemyAction(MOVE action, int waiting)
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
