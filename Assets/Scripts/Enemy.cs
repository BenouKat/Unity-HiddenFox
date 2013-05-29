using UnityEngine;
using System.Collections;

public enum LOOK
{
	RIGHT,
	LEFT,
	DOWN,
	UP
}

public class Enemy{
	
	
	public Vector2 startPosition;
	
	public Vector2 nextPosition;
	
	public LOOK actualLook;
	
	public List<EnemyAction> moveList;
	
	public int stateIndex;
	
	public float timeForAction;
	
	public bool isWaiting;
	
	public Enemy(LOOK defaultLook)
	{
		moveList = new List<EnemyAction>();
		stateIndex = 0;
		timeForAction = 0f;
		isWaiting = false;
		actualLook = defaultLook;
	}
	
	public bool isValidEnemy(Level thelevel)
	{
		var positionTMP = startPosition;
		for(int i=0; i<moveList.Count; i++)
		{
			switch(moveList[i].state)
			{
				case MOVE.LEFT:
					positionTMP.x--;
					break;
				case MOVE.RIGHT:
					positionTMP.x++;
					break;
				case MOVE.UP:
					positionTMP.y--;
					break;
				case MOVE.DOWN:
					positionTMP.y--;
				case default:
					break;
			}
			
			if(thelevel.getBlocState(positionTMP.x, positionTMP.y, 0) != BLOCSTATE.CUBE && thelevel.getBlocState(positionTMP.x, positionTMP.y, 1) == BLOCSTATE.CUBE)
			{
				return false;
			}
		}
		
		return positionTMP.x == startPosition.x && positionTMP.y == startPosition.y;
	}
	
	public bool addAction(MOVE action, int waiting)
	{
		moveList.Add(new EnemyAction(action, waiting));
	}
	
	public bool removeLastAction()
	{
		moveList.Remove(moveList.Last());
	}
	
	public void nextAction()
	{
		stateIndex++;
		if(stateIndex >= moveList.Count)
		{
			stateIndex = 0;
		}
		startAction();
	}
	
	public void startAction()
	{
		timeForAction = 0f;
		isWaiting = false;
		switch(moveList[stateIndex])
		{
			case MOVE.LEFT:
				nextPosition.x--;
				actualLook = LOOK.LEFT;
				break;
			case MOVE.RIGHT:
				nextPosition.x++;
				actualLook = LOOK.RIGHT;
				break;
			case MOVE.UP:
				nextPosition.y--;
				actualLook = LOOK.UP;
				break;
			case MOVE.DOWN:
				nextPosition.y--;
				actualLook = LOOK.DOWN;
				break;
			case MOVE.WAIT:
				isWaiting = true;
				break;
			case MOVE.LOOKLEFT:
				actualLook = LOOK.LEFT;
				isWaiting = true;
				break;
			case MOVE.LOOKRIGHT:
				actualLook = LOOK.RIGHT;
				isWaiting = true;
				break;
			case MOVE.LOOKUP:
				actualLook = LOOK.UP;
				isWaiting = true;
				break;
			case MOVE.LOOKDOWN:
				actualLook = LOOK.DOWN;
				isWaiting = true;
				break;
		}
	}
	
	
	
	
}
