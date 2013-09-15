using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	
	public GameObject cameraPlayer;
	public int positionSelected;
	public ParticleSystem dashPS;
	public ManaBar manabar;
	
	private Ray capteur;
	private float nextMove;
	private bool nextMoveForbidden;
	public float decalOriginCapteur = 0.551f;
	
	KeyCode lastKey;
	
	private float speed;
	public float basicSpeed;
	public float speedDash;
	public float secureDistance;
	
	private Vector3 velocity = Vector3.zero;
	private float velocityCam = 0f;
	private int layerMask = 0;
	
	private Vector3 up = new Vector3(0f, 180f, 0f);
	private Vector3 down = new Vector3(0f, 0f, 0f);
	private Vector3 right = new Vector3(0f, -90f, 0f);
	private Vector3 left = new Vector3(0f, 90f, 0f);
	
	private KeyCode keyInput;
	
	private GameObject realCamera;
	private GameObject[] cameraRotation;
	private bool moveCamera;
	private float slerpTime;
	
	private bool enteringDash;
	private bool onDash;
	private float time;
	public float timeDash;
	
	public float coolDownDash;
	private float timeCooldown;
	
	public float maxMana;
	private float actualMana;
	public float manaCost;
	public float recoverManaPerSeconds;
	
	private Vector3 fillVector = new Vector3(0f, 0f, 0f);
	
	// Use this for initialization
	void Start () {
		layerMask = (1 << LayerMask.NameToLayer("Wall")) | (1 << LayerMask.NameToLayer("LevelObject"));
		capteur = new Ray(Vector3.zero, Vector3.zero);
		speed = basicSpeed;
		actualMana = maxMana;
		manabar.transform.gameObject.SetActive(true);
		
		cameraRotation = new GameObject[4];
		
		for(int i=1; i<5; i++)
		{
			cameraRotation[i-1] = cameraPlayer.transform.FindChild("Position" + i).gameObject;
		}
		realCamera = cameraPlayer.transform.FindChild("CameraRotate").gameObject;
		
		positionSelected = 0;
		
		keyInput = InputManager.instance.getDirectionNow();
		
		if(keyInput == KeyCode.UpArrow){
			transform.eulerAngles = up;
		}else
		if(keyInput == KeyCode.DownArrow){
			transform.eulerAngles = down;
		}else
		if(keyInput == KeyCode.RightArrow){
			transform.eulerAngles = right;
		}else
		if(keyInput == KeyCode.LeftArrow){
			transform.eulerAngles = left;
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if(enteringDash)
		{
			enteringDash = false;
			onDash = true;
			speed = speedDash;
			time = 0f;
			if(!dashPS.gameObject.activeSelf) dashPS.gameObject.SetActive(true);
			dashPS.Play();
			
			timeCooldown = Time.time;
			actualMana -= manaCost;
			manabar.loseMana(manaCost);
		}
		
		
		if(onDash)
		{
			if(time >= timeDash || nextMoveForbidden)
			{
				speed = basicSpeed;
				onDash = false;
			}
			time += Time.deltaTime;
		}
		
		keyInput = InputManager.instance.getDirectionNow();
		
		if(isMoving())
		{
			nextMove = speed*Time.deltaTime;
			nextMoveForbidden = false;
			
		
			if(keyInput == KeyCode.UpArrow){
			transform.eulerAngles = up;
			}else
			if(keyInput == KeyCode.DownArrow){
				transform.eulerAngles = down;
			}else
			if(keyInput == KeyCode.RightArrow){
				transform.eulerAngles = right;
			}else
			if(keyInput == KeyCode.LeftArrow){
				transform.eulerAngles = left;
			}
			
			capteur.origin = transform.position - (transform.right*decalOriginCapteur);
			capteur.direction = -transform.right;
			
			if(Physics.Raycast(capteur, nextMove, layerMask))
			{
				nextMoveForbidden = true;
			}
			
			if(!nextMoveForbidden)
			{
				if(keyInput == KeyCode.UpArrow){
					
					
					if(transform.eulerAngles != up) transform.eulerAngles = up;
					
					transform.Translate(-transform.right*nextMove, Space.World);
					
					playAnim();
					
					checkDash();
					
				}else if(keyInput == KeyCode.DownArrow){
					
					if(transform.eulerAngles != down) transform.eulerAngles = down;
					
					transform.Translate(-transform.right*nextMove, Space.World);
					
					playAnim();
					
					checkDash();
					
				}else if(keyInput == KeyCode.RightArrow){
					
					if(transform.eulerAngles != right) transform.eulerAngles = right;
					
					transform.Translate(-transform.right*nextMove, Space.World);
					
					playAnim();
					
					checkDash();
					
				}else if(keyInput == KeyCode.LeftArrow){
					
					if(transform.eulerAngles != left) transform.eulerAngles = left;
					
					transform.Translate(-transform.right*nextMove, Space.World);
					
					playAnim();
					
					checkDash();
				}else{
					stopAnim();	
				}
			}else {
				stopAnim();	
			}
			
			
			
		}else
		{
			stopAnim();	
		}	
		
		if(actualMana < 100f && Time.time > timeCooldown + coolDownDash)
		{
			actualMana += recoverManaPerSeconds*Time.deltaTime;
			manabar.recoverMana(recoverManaPerSeconds*Time.deltaTime);
		}
		
		
		if(Input.GetKeyDown(KeyCode.LeftArrow))
		{
			positionSelected--;
			if(positionSelected < 0) positionSelected = 3;
			InputManager.instance.camGoLeft();
			velocityCam = 0f;
			slerpTime = 0f;
			moveCamera = true;
		}
		
		if(Input.GetKeyDown(KeyCode.RightArrow))
		{
			positionSelected++;
			if(positionSelected > 3) positionSelected = 0;
			InputManager.instance.camGoRight();
			velocityCam = 0f; 
			slerpTime = 0f;
			moveCamera = true;
		}
		
		if(moveCamera)
		{
			slerpTime = Mathf.SmoothDamp(slerpTime, 1f, ref velocityCam, 0.2f);
			realCamera.transform.localRotation = Quaternion.Slerp(realCamera.transform.localRotation, cameraRotation[positionSelected].transform.localRotation, slerpTime);
			
			if(slerpTime >= 0.999f)
			{
				realCamera.transform.localRotation = cameraRotation[positionSelected].transform.localRotation;
				moveCamera = false;
			}
		}
		
		
		cameraPlayer.transform.position = Vector3.SmoothDamp(cameraPlayer.transform.position, transform.position, ref velocity, 0.1f);
	}
	
	public void resetPlayer()
	{
		speed = basicSpeed;
		timeCooldown = 0f;
		actualMana = maxMana;
		manabar.resetMana();
		onDash = false;
		enteringDash = false;
		manabar.transform.gameObject.SetActive(true);
		cameraPlayer.transform.position = transform.position;
	}
	
	public Vector3 fillVector3(float x, float y, float z)
	{
		fillVector.x = x;
		fillVector.y = y;
		fillVector.z = z;
		return fillVector;
	}
	
	public bool isMoving()
	{
		return keyInput != KeyCode.None;
	}
	
	public void stopAnim()
	{
		if(!animation.IsPlaying("idleHero"))
		{
			animation.Stop("AnimHero");
			animation.Play("idleHero");
		}
	}
	
	public void playAnim()
	{
		if(!animation.IsPlaying("AnimHero"))
		{
			animation.Stop("idleHero");
			animation.Play("AnimHero");
		}
	}
	
	public void checkDash()
	{
		if(Input.GetKeyDown(KeyCode.Space) && Time.time > timeCooldown + coolDownDash && actualMana >= manaCost){
			enteringDash = true;	
		}
	}
}
