using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class SerializableEnemy{
	
	public int startX;
	public int startY;
	public List<EnemyAction> ea;
	public LOOK defaultLook;
	public bool isHelicopter;
	
	public SerializableEnemy(int tstartX, int tstartY, List<EnemyAction> tea, LOOK tdefaultLook, bool tisHelicopter)
	{
		startX = tstartX;
		startY = tstartY;
		ea = tea;
		defaultLook = tdefaultLook;
		isHelicopter = tisHelicopter;
	}
	
}
