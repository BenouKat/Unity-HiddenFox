using UnityEngine;
using System.Collections;

public class CameraEnemy : MonoBehaviour {
	
	private Ray capteur;
	private RaycastHit info;
	private Vector3 positionDecal;
	
	public bool rightFirst;
	public float speedRotation;
	public float timePause;
	
	private float rotationChecked;
	
	public bool activeCamera;
	
	private LOOK startLOOK;
	
	// Use this for initialization
	void Start () {
	
		capteur = new Ray(Vector3.zero, Vector3.zero);
		positionDecal = new Vector3(transform.position.x, transform.position.y - 2f, transform.position.z);
		rotationChecked = 0f;
		if(activeCamera) StartCoroutine(Camera_Coroutine());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void activate()
	{
		activeCamera = true;
		StartCoroutine(Camera_Coroutine());
	}
	
	public void setLook(LOOK l)
	{
		startLOOK = l;	
	}
	
	public LOOK getLook()
	{
		return startLOOK;	
	}
	
	IEnumerator Camera_Coroutine()
	{
		while(true){
			while(rotationChecked <= 90f)
			{

				transform.Rotate(0f, rightFirst ? speedRotation*Time.deltaTime : -speedRotation*Time.deltaTime, 0f);
				rotationChecked += speedRotation*Time.deltaTime;

				yield return 0;
			}
			
			rotationChecked = 0f;
			yield return new WaitForSeconds(timePause);
			
			while(rotationChecked <= 90f)
			{

				transform.Rotate(0f, rightFirst ? -speedRotation*Time.deltaTime : speedRotation*Time.deltaTime, 0f);
				rotationChecked += speedRotation*Time.deltaTime;

				yield return 0;
			}
			
			rotationChecked = 0f;
			yield return new WaitForSeconds(timePause);
		}
	}
	
	void OnTriggerStay(Collider c)
	{
		if(c.transform.name.Contains("Player"))
		{
			capteur.origin = positionDecal;
			capteur.direction = (c.transform.position - positionDecal).normalized;
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
