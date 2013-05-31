using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum LOOK
{
	RIGHT,
	LEFT,
	DOWN,
	UP
}

public class Enemy : MonoBehaviour{
	
	
	private Vector2 startPosition;
	
	private Vector2 nextPosition;
	
	private List<EnemyAction> moveList;
	
	private int stateIndex;
	
	private float timeForAction;
	
	private bool isWaiting;
	
	private AnimationSprite spriteAnim;
	
	
	public float speed;
	
	public float distanceValid;
	
	private Vector3 poolVector3;
	
	private bool isInEditor;
	
	private LOOK defaultLookAt;
	
	void Start()
	{
		stateIndex = 0;
		timeForAction = 0f;
		isWaiting = false;
	}
	
	public void init(LOOK defaultLook, Vector2 tstartPosition, bool editorMode = true)
	{
		moveList = new List<EnemyAction>();
		startPosition = tstartPosition;
		nextPosition = startPosition;
		defaultLookAt = defaultLook;
		spriteAnim = new AnimationSprite(GetComponent<UISprite>(), "", defaultLook);
		isInEditor = editorMode;
	}
	
	void Update()
	{
		if(!isInEditor)
		{
			
		}
	}
	
	void UpdateAction()
	{
		if(!isWaiting)
		{
			switch(moveList[stateIndex].state)
			{
			case MOVE.UP:
				transform.Translate(speed*Time.deltaTime, 0f , 0f, Space.World);
				break;
			case MOVE.DOWN:
				transform.Translate(-speed*Time.deltaTime, 0f , 0f, Space.World);
				break;
			case MOVE.RIGHT:
				transform.Translate(0f, 0f , -speed*Time.deltaTime, Space.World);
				break;
			case MOVE.LEFT:
				transform.Translate(0f, 0f, speed*Time.deltaTime, Space.World);
			}
			spriteAnim.anim();
			if(Vector3.Distance(transform.position, convertToVector3(nextPosition)) <= distanceValid)
			{
				nextAction();	
			}
		}else if(timeForAction >= moveList[stateIndex].timeWait)
		{
			timeForAction = 0f;
			nextAction();
		}else{
			timeForAction += Time.deltaTime;	
		}
	}
	
	
	public bool isValidEnemy(Level thelevel)
	{
		var positionTMP = startPosition;
		for(int i=0; i<moveList.Count; i++)
		{
			switch(moveList[i].state)
			{
				case MOVE.LEFT:
					positionTMP.x++;
					break;
				case MOVE.RIGHT:
					positionTMP.x--;
					break;
				case MOVE.UP:
					positionTMP.y--;
					break;
				case MOVE.DOWN:
					positionTMP.y--;
					break;
				default:
					break;
			}
			
			if(thelevel.getBlocState((int)positionTMP.x, (int)positionTMP.y, 0) != BLOCSTATE.CUBE && thelevel.getBlocState((int)positionTMP.x, (int)positionTMP.y, 1) == BLOCSTATE.CUBE)
			{
				return false;
			}
		}
		
		return positionTMP.x == startPosition.x && positionTMP.y == startPosition.y;
	}
	
	public void addAction(MOVE action, float waiting)
	{
		moveList.Add(new EnemyAction(action, waiting));
	}
	
	public void removeLastAction()
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
		switch(moveList[stateIndex].state)
		{
			case MOVE.LEFT:
				isWaiting = false;
				transform.position = new Vector3(nextPosition.z*2f, 2f, nextPosition.x*2f);
				nextPosition.x++;
				spriteAnim.refreshPosition(LOOK.LEFT);
				break;
			case MOVE.RIGHT:
				isWaiting = false;
				transform.position = new Vector3(nextPosition.z*2f, 2f, nextPosition.x*2f);
				nextPosition.x--;
				spriteAnim.refreshPosition(LOOK.RIGHT);
				break;
			case MOVE.UP:
				isWaiting = false;
				transform.position = new Vector3(nextPosition.z*2f, 2f, nextPosition.x*2f);
				nextPosition.y++;
				spriteAnim.refreshPosition(LOOK.UP);
				break;
			case MOVE.DOWN:
				isWaiting = false;
				transform.position = new Vector3(nextPosition.z*2f, 2f, nextPosition.x*2f);
				nextPosition.y--;
				spriteAnim.refreshPosition(LOOK.DOWN);
				break;
			case MOVE.WAIT:
				isWaiting = true;
				spriteAnim.idle();
				break;
			case MOVE.LOOKLEFT:
				isWaiting = true;
				spriteAnim.refreshPosition(LOOK.LEFT);
				spriteAnim.idle();
				break;
			case MOVE.LOOKRIGHT:
				isWaiting = true;
				spriteAnim.refreshPosition(LOOK.RIGHT);
				spriteAnim.idle();
				break;
			case MOVE.LOOKUP:
				isWaiting = true;
				spriteAnim.refreshPosition(LOOK.UP);
				spriteAnim.idle();
				break;
			case MOVE.LOOKDOWN:
				isWaiting = true;
				spriteAnim.refreshPosition(LOOK.DOWN);
				spriteAnim.idle();
				break;
			default:
				isWaiting = true;
				spriteAnim.idle();
				break;
		}
	}
	
	Vector3 convertToVector3(Vector2 pos)
	{
		poolVector3.x = pos.z*2f;
		poolVector3.y = 2f;
		poolVector3.z = pos.x*2f;
		return poolVector3;
	}
	
	#region EDITOR
	//EDITOR
	public Vector2 getLastPosition()
	{
		var positionTMP = startPosition;
		for(int i=0; i<moveList.Count; i++)
		{
			switch(moveList[i].state)
			{
				case MOVE.LEFT:
					positionTMP.x++;
					break;
				case MOVE.RIGHT:
					positionTMP.x--;
					break;
				case MOVE.UP:
					positionTMP.y++;
					break;
				case MOVE.DOWN:
					positionTMP.y--;
					break;
				default:
					break;
			}
		}
		
		return positionTMP;
	}

	public TURN isMakingATurn()
	{
		var theActualTurn = moveList.Last().state;
		for(int i=moveList.Count - 2; i >=0; i--)
		{
			var el = moveList.ElementAt(i).state;
			if((int)el <= 3)
			{
				i = -1;
				if(((int)el <= 1 && (int)theActualTurn >= 2) || ((int)el >= 2 && (int)theActualTurn <= 1))
				{
					if(el == MOVE.LEFT && theActualTurn == MOVE.UP)
					{
						return TURN.RIGHTTOUP;
					}else if(el == MOVE.RIGHT && theActualTurn == MOVE.UP)
					{
						return TURN.LEFTTOUP;	
					}else if(el == MOVE.UP && theActualTurn == MOVE.LEFT)
					{
						return TURN.DOWNTOLEFT;	
					}else if(el == MOVE.DOWN && theActualTurn == MOVE.LEFT)
					{
						return TURN.UPTOLEFT;	
					}else if(el == MOVE.LEFT && theActualTurn == MOVE.DOWN)
					{
						return TURN.RIGHTTODOWN;
					}else if(el == MOVE.RIGHT && theActualTurn == MOVE.DOWN)
					{
						return TURN.LEFTTODOWN;	
					}else if(el == MOVE.UP && theActualTurn == MOVE.RIGHT)
					{
						return TURN.DOWNTORIGHT;	
					}else if(el == MOVE.DOWN && theActualTurn == MOVE.RIGHT)
					{
						return TURN.UPTORIGHT;	
					}
				}
			}
			
		}
		return TURN.NONE;
	}
	
	public MOVE getLastAction()
	{
		if(moveList.Any())
		{
			return moveList.Last().state;
		}
		return MOVE.UP;
	}
	
	public EnemyAction getLastActionComplete()
	{
		return moveList.Last();
	}
	
	public int getNumberOfAction()
	{
		return moveList.Count(c => (int)c.state <= 3);	
	}
	
	public void testEnemy()
	{
		isInEditor = false;
		startAction();
	}
	
	public void endTestEnemy()
	{
		isInEditor = true;
		transform.position = convertToVector3(startPosition);
		nextPosition = startPosition;
		spriteAnim.refreshPosition(defaultLookAt);
		stateIndex = 0;
		timeForAction = 0f;
		isWaiting = false;
	}
	#endregion
	
}
