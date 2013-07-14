using UnityEngine;
using System.Collections;

public class EnemyDetection : MonoBehaviour {
	
	private Enemy enemyAssociated;
	private Ray capteur;
	private RaycastHit info;
	
	public float viewAngle;
	public float distanceMax;
	
	private Gameplay gameplay;
	private GameObject PSDetection;
	
	private bool discoverHero;
	private Vector3 heroPosition;
	public float speedLerp;
	
	void Awake(){
		enemyAssociated = transform.GetComponent<Enemy>();
		capteur = new Ray(Vector3.zero, Vector3.zero);
		if(!Application.loadedLevelName.Contains("Editor")) gameplay = GameObject.Find("Engine").GetComponent<Gameplay>();
		PSDetection = transform.FindChild("PSDetection").gameObject;
		discoverHero = false;
	}
	
	void Update()
	{
		if(discoverHero)
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(transform.position - heroPosition), speedLerp*Time.deltaTime);
		}
	}
	
	public void reset()
	{
		discoverHero = false;	
	}
	
	void OnTriggerStay(Collider c)
	{
		if(Vector3.Distance(transform.position, c.transform.position) <= distanceMax)
		{
			gameplay.isDiscovered(PSDetection, "Enemy");
			heroPosition = c.transform.position;
			enemyAssociated.pause();
			discoverHero = true;
			animation.Stop("AnimEnemyBack");
			animation.Stop("AnimEnemy");
			animation.Play("AnimEnemySpot");
		}
		else if(Vector3.Angle(enemyAssociated.lookToVector(), (c.transform.position - transform.position).normalized) <= viewAngle){
			capteur.origin = transform.position;
			capteur.direction = (c.transform.position - transform.position).normalized;
			
			if(Physics.Raycast(capteur, out info))
			{
				if(info.transform.name.Contains("Player"))
				{
					gameplay.isDiscovered(PSDetection, "Enemy");
					heroPosition = c.transform.position;
					enemyAssociated.pause();
					discoverHero = true;
					animation.Stop("AnimEnemyBack");
					animation.Stop("AnimEnemy");
					animation.Play("AnimEnemySpot");
				}
			}
		}
	}
}
