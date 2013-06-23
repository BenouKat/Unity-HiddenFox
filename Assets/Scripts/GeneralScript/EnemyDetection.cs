using UnityEngine;
using System.Collections;

public class EnemyDetection : MonoBehaviour {
	
	private Enemy enemyAssociated;
	private Ray capteur;
	private RaycastHit info;
	
	public float viewAngle;
	public float distanceMax;
	
	void Awake(){
		enemyAssociated = transform.GetComponent<Enemy>();
		capteur = new Ray(Vector3.zero, Vector3.zero);
	}
	
	void OnTriggerStay(Collider c)
	{
		if(Vector3.Distance(transform.position, c.transform.position) <= distanceMax)
		{
			//Trouvé
		}
		else if(Vector3.Angle(enemyAssociated.lookToVector(), (c.transform.position - transform.position).normalized) <= viewAngle){
			capteur.origin = transform.position;
			capteur.direction = (c.transform.position - transform.position).normalized;
			
			if(Physics.Raycast(capteur, out info))
			{
				if(info.transform.name.Contains("Player"))
				{
					//Trouvé
				}
			}
		}
	}
}
