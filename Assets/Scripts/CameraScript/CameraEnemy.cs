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
	
	private LOOK startLOOK;
	
	private Gameplay gameplay;
	private GameObject PSDetection;
	
	// Use this for initialization
	void Start () {
		capteur = new Ray(Vector3.zero, Vector3.zero);
		positionDecal = new Vector3(transform.position.x, transform.position.y - 2f, transform.position.z);
		rotationChecked = 0f;
		if(!Application.loadedLevelName.Contains("Editor")) gameplay = GameObject.Find("Engine").GetComponent<Gameplay>();
		PSDetection = transform.FindChild("PSDetection").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void go()
	{
		StartCoroutine("Camera_Coroutine");
	}
	
	public void reset()
	{
		StopCoroutine("Camera_Coroutine");
		rotationChecked = 0f;
		switch(startLOOK)
		{
			case LOOK.DOWN:
				transform.eulerAngles = new Vector3(0f, 90f, 0f);
				break;
			case LOOK.UP:
				transform.eulerAngles = new Vector3(0f, -90f, 0f);
				break;
			case LOOK.RIGHT:
				transform.eulerAngles = new Vector3(0f, 0f, 0f);
				break;
			case LOOK.LEFT:
				transform.eulerAngles = new Vector3(0f, 180f, 0f);
				break;	
		}
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
					StopCoroutine("Camera_Coroutine");
					gameplay.isDiscovered(PSDetection, "Camera");
				}
			}
		}
	}
}
