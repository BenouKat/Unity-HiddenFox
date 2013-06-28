using UnityEngine;
using System.Collections;

public class ParticuleManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(!particleSystem.isPlaying)
		{
			gameObject.SetActive(false);	
		}
	}
}
