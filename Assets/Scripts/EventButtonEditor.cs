using UnityEngine;
using System.Collections;

public class EventButtonEditor : MonoBehaviour {
	
	public EditorLevelManager elm;
	
	public bool isRight;
	
	void OnClick()
	{
		if(isRight)
		{
			elm.nextEnemeyAction();	
		}else{
			elm.prevEnemeyAction();	
		}
	}
}
