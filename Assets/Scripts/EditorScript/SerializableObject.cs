using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class SerializableObject{
	
	public bool isCanon;
	public LOOK theLook;
	public bool isRightFirst;
	public int positionX;
	public int positionY;
	
	public SerializableObject(bool tisCanon, LOOK ttheLook, int posX, int posY, bool tisRightFirst = false)
	{
		isCanon = tisCanon;
		theLook = ttheLook;
		isRightFirst = tisRightFirst;
		positionX = posX;
		positionY = posY;
	}
	
}