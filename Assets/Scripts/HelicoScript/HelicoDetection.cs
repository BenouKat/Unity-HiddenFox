using UnityEngine;
using System.Collections;

public class HelicoDetection : MonoBehaviour {
	
	private Gameplay gameplay;
	private GameObject PSDetection;
	
	void Awake()
	{
		PSDetection = transform.FindChild("PSDetection").gameObject;
		if(!Application.loadedLevelName.Contains("Editor")) gameplay = GameObject.Find("Engine").GetComponent<Gameplay>();	
	}
	
	void OnTriggerStay(Collider c)
	{
		if(c.name.Contains("Player"))
		{
			gameplay.isDiscovered(PSDetection, "Helicopter");
		}
	}
}
