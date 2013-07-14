using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
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
	
	public float speed;
	public float distanceValid;
	
	private Vector3 velocity = Vector3.zero;
	
	private float distance;
	private float oldDistance;
	
	private Vector3 poolVector3;
	
	private bool isGameStarted;
	
	public LOOK defaultLookAt;
	private LOOK actualLook;
	private bool rotationRefreshed;
	
	private bool animPlayed;
	private bool animPlayedBack;
	
	private bool isHelicopter;
	
	void Start()
	{
		transform.collider.enabled = false;
		isGameStarted = false;
		stateIndex = 0;
		timeForAction = 0f;
		isWaiting = false;
		animPlayed = false;
		animPlayedBack = false;
	}
	
	public void init(LOOK defaultLook, Vector2 tstartPosition, bool editorMode = true, bool tisHelicopter = false)
	{
		moveList = new List<EnemyAction>();
		startPosition = tstartPosition;
		nextPosition = startPosition;
		defaultLookAt = defaultLook;
		turnEnemy(defaultLookAt, true);
		isHelicopter = tisHelicopter;
	}
	
	
	public void reset()
	{
		transform.collider.enabled = false;
		isGameStarted = false;
		stateIndex = 0;
		timeForAction = 0f;
		isWaiting = false;
		animPlayed = false;
		animPlayedBack = false;
		if(!isHelicopter)
		{
			GetComponent<EnemyDetection>().reset ();
			animation.Stop("AnimEnemyBack");
			animation.Stop("AnimEnemy");
			animation.Stop("AnimEnemySpot");
			transform.FindChild("Corps").rotation = Quaternion.identity;
			transform.FindChild("PivotBras").rotation = Quaternion.identity;
			transform.FindChild("PivotBras").FindChild("BrasGauche").rotation = Quaternion.identity;
			transform.FindChild("PivotBras").FindChild("BrasGauche").localPosition = new Vector3(-transform.FindChild("PivotBras").FindChild("BrasDroite").localPosition.x, 0f, 0f);
		}
		transform.position = convertToVector3(startPosition);	
		nextPosition = startPosition;
		turnEnemy(defaultLookAt, true);
	}
	
	public void go()
	{
		isGameStarted = true;
		transform.collider.enabled = true;
		startAction();
	}
	
	public void pause()
	{
		isGameStarted = false;
		transform.collider.enabled = false;
	}
	
	void Update()
	{
		if(isGameStarted)
		{
			UpdateAction();
		}
	}
	
	void UpdateAction()
	{
		if(!isWaiting)
		{
			if(!animPlayed && !isHelicopter){
				animPlayed = true;
				animPlayedBack = false;
				animation.Stop("AnimEnemyBack");
				animation.Play("AnimEnemy");
			}
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
				break;
			}
			distance = Vector3.Distance(transform.position, convertToVector3(nextPosition));
			if(distance <= distanceValid || distance > oldDistance)
			{
				nextAction();	
			}else{
				oldDistance = distance;	
			}
		}else if(timeForAction >= moveList[stateIndex].timeWait)
		{
			nextAction();
		}else{
			timeForAction += Time.deltaTime;	
		}
		
		if(isWaiting)
		{
			if(!animPlayedBack && !isHelicopter){
				animPlayedBack = true;
				animPlayed = false;
				animation.Stop("AnimEnemy");
				animation.Play("AnimEnemyBack");
			}	
		}
		
		if(!rotationRefreshed)
		{
			rotationRefreshed = turnEnemy(actualLook);	
		}
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
				transform.position = convertToVector3(nextPosition);
				nextPosition.x++;
				actualLook = LOOK.LEFT;
				break;
			case MOVE.RIGHT:
				isWaiting = false;
				transform.position = convertToVector3(nextPosition);
				nextPosition.x--;
				actualLook = LOOK.RIGHT;
				break;
			case MOVE.UP:
				isWaiting = false;
				transform.position = convertToVector3(nextPosition);
				nextPosition.y++;
				actualLook = LOOK.UP;
				break;
			case MOVE.DOWN:
				isWaiting = false;
				transform.position = convertToVector3(nextPosition);
				nextPosition.y--;
				actualLook = LOOK.DOWN;
				break;
			case MOVE.WAIT:
				isWaiting = true;
				break;
			case MOVE.LOOKLEFT:
				isWaiting = true;
				actualLook = LOOK.LEFT;
				break;
			case MOVE.LOOKRIGHT:
				isWaiting = true;
				actualLook = LOOK.RIGHT;
				break;
			case MOVE.LOOKUP:
				isWaiting = true;
				actualLook = LOOK.UP;
				break;
			case MOVE.LOOKDOWN:
				isWaiting = true;
				actualLook = LOOK.DOWN;
				break;
			default:
				isWaiting = true;
				break;
		}
		rotationRefreshed = false;
		oldDistance = Vector3.Distance(transform.position, convertToVector3(nextPosition));
	}
	
	bool turnEnemy(LOOK theLook, bool instant = false)
	{
		switch(theLook)
		{
		case LOOK.DOWN:
			transform.eulerAngles = Vector3.SmoothDamp(new Vector3(0f, transform.eulerAngles.y%360, 0f), new Vector3(0f, transform.eulerAngles.y%360 >= 350f ? 450f : 90f, 0f), ref velocity, instant ? 0f : 0.2f);
			return transform.eulerAngles.y%360 == 90f;
		case LOOK.UP:
			transform.eulerAngles = Vector3.SmoothDamp(new Vector3(0f, transform.eulerAngles.y%360, 0f), new Vector3(0f, transform.eulerAngles.y%360 >= 179f ? 270f : -90f, 0f), ref velocity, instant ? 0f : 0.2f);
			return transform.eulerAngles.y%360 == 270f;
		case LOOK.RIGHT:
			transform.eulerAngles = Vector3.SmoothDamp(new Vector3(0f, transform.eulerAngles.y%360, 0f), new Vector3(0f, transform.eulerAngles.y%360 >= 179f ? 360f : 0f, 0f), ref velocity, instant ? 0f : 0.2f);
			return transform.eulerAngles.y%360 == 0f;
		case LOOK.LEFT:
			transform.eulerAngles = Vector3.SmoothDamp(new Vector3(0f, transform.eulerAngles.y%360, 0f), new Vector3(0f, 180f, 0f), ref velocity, instant ? 0f : 0.2f);
			return transform.eulerAngles.y%360 == 180f;
		}
		return false;
	}
	
	Vector3 convertToVector3(Vector2 pos)
	{
		poolVector3.x = pos.y*2f;
		poolVector3.y = isHelicopter ? 6f : 2f;
		poolVector3.z = pos.x*2f;
		return poolVector3;
	}
	
	public Vector3 lookToVector()
	{
		return -transform.forward;
		/*switch(spriteAnim.getActualLook())
		{
		case LOOK.DOWN:
			return (-transform.forward - transform.right).normalized;
		case LOOK.LEFT:
			return (transform.forward - transform.right).normalized;
		case LOOK.UP:
			return (transform.forward + transform.right).normalized;
		case LOOK.RIGHT:
			return (-transform.forward + transform.right).normalized;
		default:
			return Vector3.zero;
		}*/
	}
	
	
	
	#region EDITOR
	//EDITOR
	public bool isValidEnemy(LevelEditor thelevel)
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
			
			if((thelevel.getBlocState((int)positionTMP.x, (int)positionTMP.y, isHelicopter ? 3 : 1) == BLOCSTATE.CUBE) || (!isHelicopter && (thelevel.getBlocState((int)positionTMP.x, (int)positionTMP.y, isHelicopter ? 2 : 0) != BLOCSTATE.CUBE)))
			{
				return false;
			}
		}
		
		return (int)positionTMP.x == (int)startPosition.x && (int)positionTMP.y == (int)startPosition.y;
	}
	
	public void addAction(MOVE action, float waiting)
	{
		moveList.Add(new EnemyAction(action, waiting < 0.2f ? 0.2f : waiting));
	}
	
	public void removeLastAction()
	{
		moveList.Remove(moveList.Last());
	}
	
	
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
	
	public bool isHelico()
	{
		return isHelicopter;
	}
	
	public void testEnemy()
	{
		go ();
	}
	
	public void endTestEnemy()
	{
		isGameStarted = false;
		transform.position = convertToVector3(startPosition);
		nextPosition = startPosition;
		turnEnemy(defaultLookAt, true);
		stateIndex = 0;
		timeForAction = 0f;
		isWaiting = false;
	}
	
	public SerializableEnemy saveEnemy()
	{
		return new SerializableEnemy((int) startPosition.x, (int) startPosition.y, moveList, defaultLookAt, isHelicopter);	
	}
	
	public void loadEnemy(SerializableEnemy se, bool inEditorLoad = true)
	{
		startPosition.x = se.startX;
		startPosition.y = se.startY;
		nextPosition = startPosition;
		if(inEditorLoad)
		{
			moveList = new List<EnemyAction>();
		}
		else{
			moveList = se.ea;
		}
		defaultLookAt = se.defaultLook;
		isHelicopter = se.isHelicopter;
		turnEnemy(defaultLookAt, true);
	}
	#endregion
	
	
}
