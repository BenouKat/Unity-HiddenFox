using UnityEngine;
using System.Collections;

public class FinishDetection : MonoBehaviour {
	
	private Gameplay gameplay;
	// Use this for initialization
	void Awake () {
		if(!Application.loadedLevelName.Contains("Editor")) gameplay = GameObject.Find("Engine").GetComponent<Gameplay>();
	}
	
	void OnTriggerStay(Collider c)
	{
		if(c.name.Contains("Player"))
		{
			gameplay.stageClear();
		}
	}
}
