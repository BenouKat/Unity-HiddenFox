using UnityEngine;
using System.Collections;



public class Canon : MonoBehaviour {
	
	private GameObject cercle;
	private GameObject bullet;
	private ParticleSystem chargementCanon;
	private ParticleSystem explosionCanon;
	
	private Vector3 bulletPositionBase;
	
	private LOOK startLOOK;
	
	
	public float maxSpeedRotate;
	private float speedRotate;
	public float speedUpRotate;
	public float speedDownRotate;
	public float speedBullet;
	public float speedTranslationRotation;
	
	private bool bulletShot;
	private bool prepareBullet;
	
	public bool goOnStart = false;
	
	private bool activeCanon;
	// Use this for initialization
	void Start () {
		cercle = transform.FindChild("WholeCanon").FindChild("Cercle").gameObject;
		chargementCanon = transform.FindChild("WholeCanon").FindChild("ChargementCanon").particleSystem;
		explosionCanon = transform.FindChild("WholeCanon").FindChild("ExplosionCanon").particleSystem;
		bullet = transform.FindChild("Bullet").gameObject;
		bulletPositionBase = bullet.transform.localPosition;
		chargementCanon.gameObject.SetActive(false);
		explosionCanon.gameObject.SetActive(false);
		bullet.SetActive(false);
		speedRotate = 0f;
		bulletShot = false;
		prepareBullet = false;
		
		
		if(goOnStart)
		{
			go ();
		}
		
	}
	
	void Update()
	{
		if(activeCanon)
		{
			cercle.transform.Rotate(new Vector3(speedRotate*Time.deltaTime, 0f, 0f));
			
			if(prepareBullet && speedBullet < maxSpeedRotate)
			{
				speedRotate += speedUpRotate*Time.deltaTime;
			}else if(!prepareBullet && speedRotate > 0f){
				speedRotate -= speedDownRotate*Time.deltaTime;	
			}
		}
	}
	
	public void reset()
	{
		activeCanon = false;
		StopCoroutine("Canon_Coroutine");
		speedRotate = 0f;
		switch(startLOOK)
		{
		case LOOK.DOWN:
			transform.eulerAngles = new Vector3(0f, 0f, 0f);
			break;
		case LOOK.UP:
			transform.eulerAngles = new Vector3(0f, 180f, 0f);
			break;
		case LOOK.RIGHT:
			transform.eulerAngles = new Vector3(0f, -90f, 0f);
			break;
		case LOOK.LEFT:
			transform.eulerAngles = new Vector3(0f, 90f, 0f);
			break;
		}
		stopBullet();
		reloadBullet();
		prepareBullet = false;
	}
	
	void OnEnable()
	{
		if(activeCanon)
		{
			Quaternion oldrotation = transform.rotation;
			reset();
			transform.rotation = oldrotation;
			go ();
		}
	}
	
	public void go()
	{
		activeCanon = true;
		StartCoroutine("Canon_Coroutine");
	}
	
	public void reloadBullet()
	{
		chargementCanon.gameObject.SetActive(false);
		explosionCanon.gameObject.SetActive(false);
		bullet.SetActive(false);
		bullet.GetComponent<Bullet>().reset();
		bullet.transform.localPosition = bulletPositionBase;
		
	}
	
	public void stopBullet(){
		bulletShot = false;	
	}
	
	public void setLook(LOOK l)
	{
		startLOOK = l;	
	}
	
	public LOOK getLook()
	{
		return startLOOK;	
	}
	
	IEnumerator Canon_Coroutine()
	{
		while(true)
		{
			yield return new WaitForSeconds(1f);
			
			prepareBullet = true;
			
			yield return new WaitForSeconds(1f);
			
			chargementCanon.gameObject.SetActive(true);
			
			yield return new WaitForSeconds(2f);
			
			prepareBullet = false;
			explosionCanon.gameObject.SetActive(true);
			bullet.SetActive(true);
			bulletShot = true;
			
			while(bulletShot)
			{
				bullet.transform.Translate(new Vector3(-speedBullet*Time.deltaTime, 0f, 0f), Space.Self);
				yield return 0;
			}
			
			yield return new WaitForSeconds(3f);
		}
	}
}
