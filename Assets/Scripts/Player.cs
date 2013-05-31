using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	
	private AnimationSprite spriteAnim;
	
	private UISprite sprite;
	
	public float speed;
	
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
	}
}
