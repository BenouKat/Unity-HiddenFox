using UnityEngine;
using System.Collections;

public class HelicoDetection : MonoBehaviour {

	void OnTriggerStay(Collider c)
	{
		if(c.name.Contains("Player"))
		{
			//Trouvé
		}
	}
}
