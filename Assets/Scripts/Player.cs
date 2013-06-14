using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	
	public GameObject cameraPlayer;
	
	private AnimationSprite spriteAnim;
	
	private UISprite sprite;
	
	public float speed;
	
	private Vector3 velocity = Vector3.zero;
	
	// Use this for initialization
	void Start () {
	
		sprite = GetComponent<UISprite>();
		spriteAnim = new AnimationSprite(sprite, "");
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.UpArrow)){
			transform.parent.Translate(speed*Time.deltaTime, 0f , 0f, Space.World);
			spriteAnim.refreshPosition(LOOK.UP);
			spriteAnim.anim();
			
		}else if(Input.GetKey(KeyCode.DownArrow)){
			transform.parent.Translate(-speed*Time.deltaTime, 0f, 0f, Space.World);
			spriteAnim.refreshPosition(LOOK.DOWN);
			spriteAnim.anim();
		}else if(Input.GetKey(KeyCode.RightArrow)){
			transform.parent.Translate(0f, 0f, -speed*Time.deltaTime, Space.World);
			spriteAnim.refreshPosition(LOOK.RIGHT);
			spriteAnim.anim();
		}else if(Input.GetKey(KeyCode.LeftArrow)){
			transform.parent.Translate(0f, 0f, speed*Time.deltaTime, Space.World);
			spriteAnim.refreshPosition(LOOK.LEFT);
			spriteAnim.anim();
		}else{
			spriteAnim.idle();
		}
		cameraPlayer.transform.position = Vector3.SmoothDamp(cameraPlayer.transform.position, transform.parent.position, ref velocity, 0.1f);
	}
}
