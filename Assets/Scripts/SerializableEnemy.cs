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
	
	public SerializableEnemy(int tstartX, int tstartY, List<EnemyAction> tea, LOOK tdefaultLook)
	{
		startX = tstartX;
		startY = tstartY;
		ea = tea;
		defaultLook = tdefaultLook;
	}
	
}
