using UnityEngine;
using System.Collections;

public class EventButtonEditor : MonoBehaviour {
	
	public EditorLevelManager elm;
	
	public bool isRight;
	
	public bool editEnemy;
	
	void OnClick()
	{
		if(editEnemy)
		{
			if(isRight)
			{
				elm.nextEnemeyAction();	
			}else{
				elm.prevEnemeyAction();	
			}
		}else{
			if(isRight)
			{
				elm.nextFile();	
			}else{
				elm.prevFile();	
			}	
		}
	}
}
