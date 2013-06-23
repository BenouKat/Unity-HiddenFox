using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	
	private Canon theCanon;
	private Light pointLight;
	private ParticleSystem ps;
	
	private bool isDestroyed;
	private float time;
	public float timeDeath;
	public float timeNoResponse;
	// Use this for initialization
	void Start () {
		theCanon = transform.parent.GetComponent<Canon>();
		pointLight = transform.FindChild("Light").light;
		ps = transform.FindChild("PS").particleSystem;
		time = 0f;
	}
	
	void Update()
	{
		if(isDestroyed)
		{
			theCanon.stopBullet();
			ps.Stop();
			if(pointLight.intensity > 0)
			{
				pointLight.intensity -= 1.5f*Time.deltaTime;
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
				isDestroyed = true;
			}
		}
	}
	
	public void reset()
	{
		time = 0f;
		pointLight.intensity = 3f;
		isDestroyed = false;
		transform.collider.enabled = true;
	}
	
	void OnCollisionEnter(Collision c)
	{
		if(c.transform.name.Contains("Player"))
		{
			//Trouvé
		}
		time = 0f;
		isDestroyed = true;
		transform.collider.enabled = false;	
	}
}
