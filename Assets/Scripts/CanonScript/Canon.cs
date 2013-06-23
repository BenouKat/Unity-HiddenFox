using UnityEngine;
using System.Collections;



public class Canon : MonoBehaviour {
	
	private GameObject cercle;
	public Material materialTige;
	private GameObject bullet;
	private ParticleSystem chargementCanon;
	private ParticleSystem explosionCanon;
	private GameObject baseCanon;
	
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
	
	public bool activeCanon;
	// Use this for initialization
	void Start () {
		cercle = transform.FindChild("WholeCanon").FindChild("Cercle").gameObject;
		chargementCanon = transform.FindChild("WholeCanon").FindChild("ChargementCanon").particleSystem;
		explosionCanon = transform.FindChild("WholeCanon").FindChild("ExplosionCanon").particleSystem;
		bullet = transform.FindChild("Bullet").gameObject;
		baseCanon = transform.Find("Base").gameObject;
		bulletPositionBase = bullet.transform.localPosition;
		chargementCanon.gameObject.SetActive(false);
		explosionCanon.gameObject.SetActive(false);
		bullet.SetActive(false);
		
		bulletShot = false;
		prepareBullet = false;
		if(activeCanon) StartCoroutine(Canon_Coroutine());
		
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
	
	public void activeTheCanon()
	{
		activeCanon = true;
		StartCoroutine(Canon_Coroutine());
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
			
			while(transform.position.y > 1.8f){
				transform.Translate(new Vector3(0f, -speedTranslationRotation*Time.deltaTime, 0f));
				
				yield return 0;
			}
			transform.position = new Vector3(transform.position.x, 1.8f, transform.position.z);
			
			baseCanon.SetActive(false);
			
			var makeTurn = 0f;
			while(makeTurn < 90f)
			{
				makeTurn += 90f*speedTranslationRotation*Time.deltaTime;
				
				transform.Rotate(0f, 90f*speedTranslationRotation*Time.deltaTime - (makeTurn > 90f ? makeTurn - 90f : 0f), 0f);
				yield return 0;
			}
			
			baseCanon.SetActive(true);
			
			while(transform.position.y < 2.2f){
				transform.Translate(new Vector3(0f, speedTranslationRotation*Time.deltaTime, 0f));
				yield return 0;
			}
			transform.position = new Vector3(transform.position.x, 2.2f, transform.position.z);
		}
	}
}
