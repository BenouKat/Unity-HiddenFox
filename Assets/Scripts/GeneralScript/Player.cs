using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	
	public GameObject cameraPlayer;
	
	public float speed;
	
	private Vector3 velocity = Vector3.zero;
	
	private Vector3 up = new Vector3(0f, 180f, 0f);
	private Vector3 down = new Vector3(0f, 0f, 0f);
	private Vector3 right = new Vector3(0f, -90f, 0f);
	private Vector3 left = new Vector3(0f, 90f, 0f);
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
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
		}else{
			if(!animation.IsPlaying("idleHero"))
			{
				animation.Stop("AnimHero");
				animation.Play("idleHero");
			}
		}
		
		cameraPlayer.transform.position = Vector3.SmoothDamp(cameraPlayer.transform.position, transform.position, ref velocity, 0.1f);
	}
}
