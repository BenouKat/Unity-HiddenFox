using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	
	private Canon theCanon;
	private Light pointLight;
	private LensFlare lensFlare;
	private ParticleSystem ps;
	
	private bool isDestroyed;
	private float time;
	public float timeDeath;
	public float timeNoResponse;
	
	private Gameplay gameplay;
	
	// Use this for initialization
	void Start () {
		theCanon = transform.parent.GetComponent<Canon>();
		pointLight = transform.FindChild("Light").light;
		lensFlare = transform.GetComponentInChildren<LensFlare>();
		ps = transform.FindChild("PS").particleSystem;
		time = 0f;
		if(!Application.loadedLevelName.Contains("Editor")) gameplay = GameObject.Find("Engine").GetComponent<Gameplay>();
	}
	
	void Update()
	{
		if(isDestroyed)
		{
			theCanon.stopBullet();
			ps.Stop();
			if(pointLight.intensity > 0)
			{
				pointLight.intensity -= 1f*Time.deltaTime;
			}
			if(lensFlare.brightness > 0)
			{
				lensFlare.brightness -= 0.8f*Time.deltaTime;
			}
			time += Time.deltaTime;
			if(time > timeDeath)
			{
				theCanon.reloadBullet();
			}
		}else{
			time += Time.deltaTime;
			if(time > timeNoResponse)
			{
				time = 0f;
				isDestroyed = true;
			}
		}
	}
	
	public void reset()
	{
		time = 0f;
		pointLight.intensity = 2f;
		lensFlare.brightness = 0.8f;
		isDestroyed = false;
		transform.collider.enabled = true;
	}
	
	void OnCollisionEnter(Collision c)
	{
		if(c.transform.name.Contains("Player"))
		{
			gameplay.isDiscovered(null, "Canon");
		}
		time = 0f;
		isDestroyed = true;
		transform.collider.enabled = false;	
	}
}
