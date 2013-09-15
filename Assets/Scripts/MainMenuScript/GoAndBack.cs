using UnityEngine;
using System.Collections;

public class GoAndBack : MonoBehaviour {
	
	public float distance;
	
	private float move;
	public float speedMove;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(move < distance)
		{
			transform.Translate(-transform.forward*Time.deltaTime*speedMove, Space.World);
			move += Time.deltaTime*speedMove;
		}else{
			move = 0f - (move - distance);	
			transform.Rotate(new Vector3(0f, 180f, 0f));
		}
	}
}
