using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	
	public GameObject cameraPlayer;
	public ParticleSystem dashPS;
	public ManaBar manabar;
	private Ray capteur;
	private RaycastHit info;
	
	private float speed;
	public float basicSpeed;
	public float speedDash;
	public float secureDistance;
	
	private Vector3 velocity = Vector3.zero;
	private int layerMask = 0;
	
	private Vector3 up = new Vector3(0f, 180f, 0f);
	private Vector3 down = new Vector3(0f, 0f, 0f);
	private Vector3 right = new Vector3(0f, -90f, 0f);
	private Vector3 left = new Vector3(0f, 90f, 0f);
	
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
	
	// Use this for initialization
	void Start () {
		layerMask = ~(1 << LayerMask.NameToLayer("Player"));
		capteur = new Ray(Vector3.zero, Vector3.zero);
		speed = basicSpeed;
		actualMana = maxMana;
		manabar.transform.gameObject.SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {
		
		if(onDash)
		{
			capteur.origin = transform.position + transform.right;
			capteur.direction = -transform.right;
			var theDistance = 100f;
			if(Physics.Raycast(capteur, out info, 10f, layerMask))
			{
				theDistance = info.distance;
			}
			
			if(time >= timeDash || theDistance < secureDistance + speed*Time.deltaTime)
			{
				speed = basicSpeed;
				onDash = false;
			}
			time += Time.deltaTime;
		}
		
		
		if(Input.GetKey(KeyCode.UpArrow)){
			if(Input.GetKeyDown(KeyCode.UpArrow)){
				transform.eulerAngles = new Vector3(0f, 180f, 0f);
			}
			transform.Translate(speed*Time.deltaTime, 0f , 0f, Space.World);
			if(transform.eulerAngles != up) transform.eulerAngles = up;
			if(!animation.IsPlaying("AnimHero"))
			{
				animation.Stop("idleHero");
				animation.Play("AnimHero");
			}
			
			if(Input.GetKeyDown(KeyCode.Space) && Time.time > timeCooldown + coolDownDash && actualMana >= manaCost){
				enteringDash = true;	
			}
			
		}else if(Input.GetKey(KeyCode.DownArrow)){
			if(Input.GetKeyDown(KeyCode.DownArrow)){
				transform.eulerAngles = new Vector3(0f, 0f, 0f);
			}
			transform.Translate(-speed*Time.deltaTime, 0f, 0f, Space.World);
			if(transform.eulerAngles != down) transform.eulerAngles = down;
			if(!animation.IsPlaying("AnimHero"))
			{
				animation.Stop("idleHero");
				animation.Play("AnimHero");
			}
			
			if(Input.GetKeyDown(KeyCode.Space) && Time.time > timeCooldown + coolDownDash && actualMana >= manaCost){
				enteringDash = true;	
			}
			
		}else if(Input.GetKey(KeyCode.RightArrow)){
			if(Input.GetKeyDown(KeyCode.RightArrow)){
				transform.eulerAngles = new Vector3(0f, -90f, 0f);
			}
			transform.Translate(0f, 0f, -speed*Time.deltaTime, Space.World);
			if(transform.eulerAngles != right) transform.eulerAngles = right;
			if(!animation.IsPlaying("AnimHero"))
			{
				animation.Stop("idleHero");
				animation.Play("AnimHero");
			}
			
			if(Input.GetKeyDown(KeyCode.Space) && Time.time > timeCooldown + coolDownDash && actualMana >= manaCost){
				enteringDash = true;	
			}
			
		}else if(Input.GetKey(KeyCode.LeftArrow)){
			if(Input.GetKeyDown(KeyCode.UpArrow)){
				transform.eulerAngles = new Vector3(0f, 90f, 0f);
			}
			transform.Translate(0f, 0f, speed*Time.deltaTime, Space.World);
			if(transform.eulerAngles != left) transform.eulerAngles = left;
			if(!animation.IsPlaying("AnimHero"))
			{
				animation.Stop("idleHero");
				animation.Play("AnimHero");
			}
			
			if(Input.GetKeyDown(KeyCode.Space) && Time.time > timeCooldown + coolDownDash && actualMana >= manaCost){
				enteringDash = true;	
			}
		}else{
			if(!animation.IsPlaying("idleHero"))
			{
				animation.Stop("AnimHero");
				animation.Play("idleHero");
			}
		}
		
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
		
		if(actualMana < 100f && Time.time > timeCooldown + coolDownDash)
		{
			actualMana += recoverManaPerSeconds*Time.deltaTime;
			manabar.recoverMana(recoverManaPerSeconds*Time.deltaTime);
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
}
