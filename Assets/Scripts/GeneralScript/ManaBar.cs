using UnityEngine;
using System.Collections;

public class ManaBar : MonoBehaviour {
	

	public enum STATEMANA { NORMAL, USED, COOLDOWN, RECOVER }
	private STATEMANA state;
	
	public Color ready;
	public Color coolDown;
	public Color empty;
	
	public float timeChute;
	public float timeRecover;
	
	public GameObject cube1;
	public GameObject cube2;
	public GameObject cube3;
	public UILabel power1;
	public UILabel power2;
	public UILabel power3;
	
	private bool cube1recovered;
	private bool cube2recovered;
	private bool cube3recovered;
	
	public UISprite bandeau;
	
	private float actualLenght;
	private Vector3 zero = Vector3.zero;
	private Vector3 baseScale;
	private float oldLenght;
	
	private float time;
	private float timingRecover = 1f;
	// Use this for initialization
	void Start () {
		actualLenght = 100f;
		baseScale = bandeau.transform.localScale;
		state = STATEMANA.NORMAL;
	}
	
	// Update is called once per frame
	void Update () {
		switch(state)
		{
		case STATEMANA.USED :
			if(time < timeChute)
			{
				
				bandeau.transform.localScale = Vector3.Lerp(new Vector3(oldLenght*2f, baseScale.y, baseScale.z), new Vector3(0.01f, baseScale.y, bandeau.transform.localScale.z), time/timeChute);
				bandeau.color = Color.Lerp(bandeau.color, empty, time/timeChute);
				time += Time.deltaTime;
			}else{
				time = 0f;
				bandeau.transform.localScale = new Vector3(0.01f, baseScale.y, bandeau.transform.localScale.z);
				bandeau.color = coolDown;
				state = STATEMANA.COOLDOWN;	
			}
			break;
		case STATEMANA.COOLDOWN:
			if(time < timeRecover)
			{
				
				bandeau.transform.localScale = Vector3.Lerp(new Vector3(0.01f, baseScale.y, bandeau.transform.localScale.z), new Vector3(actualLenght*2f, bandeau.transform.localScale.y, bandeau.transform.localScale.z), time/timeRecover);
				bandeau.color = Color.Lerp(bandeau.color, coolDown, time/timeRecover);
				time += Time.deltaTime;
			}else{
				time = 0f;
				bandeau.transform.localScale = new Vector3(actualLenght*2f, bandeau.transform.localScale.y, bandeau.transform.localScale.z);
				if(actualLenght < 40f)
				{
					bandeau.color = empty;	
				}else{
					bandeau.color = ready;	
				}
				state = STATEMANA.RECOVER;	
			}
			break;
		case STATEMANA.RECOVER:
			bandeau.transform.localScale = Vector3.SmoothDamp(bandeau.transform.localScale, new Vector3(actualLenght*2f, bandeau.transform.localScale.y, bandeau.transform.localScale.z), ref zero, 0.5f);
			break;
		}
		
		if(state == STATEMANA.RECOVER || state == STATEMANA.COOLDOWN)
		{
			if(!cube1recovered && bandeau.transform.localScale.x >= 80f)
			{
				StartCoroutine("recoverColor", cube1);
				cube1recovered = true;
				power1.enabled = true;
			}else if(!cube2recovered && bandeau.transform.localScale.x >= 140f) {
				StartCoroutine("recoverColor", cube2);
				cube2recovered = true;
				power2.enabled = true;
			}else if(!cube3recovered && bandeau.transform.localScale.x >= 200f) {
				StartCoroutine("recoverColor", cube3);
				cube3recovered = true;
				power3.enabled = true;
			}
		}
		
		if(state == STATEMANA.RECOVER)
		{
			if(actualLenght >= 100f)
			{
				bandeau.transform.localScale = new Vector3(actualLenght*2f, bandeau.transform.localScale.y, bandeau.transform.localScale.z);
				state = STATEMANA.NORMAL;
			}else if(actualLenght >= 40f && bandeau.color != ready)
			{
				bandeau.color = Color.Lerp(bandeau.color, ready, Mathf.Clamp(actualLenght - 41f, 40f, 41f));
			}
			
		}
	}
	
	public void loseMana(float cost)
	{
		StopCoroutine("recoverColor");
		cube1recovered = false;
		cube2recovered = false;
		cube3recovered = false;
		bandeau.transform.localScale += new Vector3(0f, bandeau.transform.localScale.y, 0f);
		bandeau.color = new Color(1f, 1f, 1f, 1f);
		time = 0f;
		oldLenght = actualLenght;
		actualLenght -= cost;
		cube1.renderer.material.SetColor("_OutlineColor", new Color(0.5f, 0.5f, 0.5f));
		cube2.renderer.material.SetColor("_OutlineColor", new Color(0.5f, 0.5f, 0.5f));
		cube3.renderer.material.SetColor("_OutlineColor", new Color(0.5f, 0.5f, 0.5f));
		power1.enabled = false;
		power2.enabled = false;
		power3.enabled = false;
		state = STATEMANA.USED;
	}
	
	public void recoverMana(float amount)
	{
		actualLenght += amount;
		actualLenght = Mathf.Clamp(actualLenght, 0f, 100f);
	}
	
	public void resetMana()
	{
		StopCoroutine("recoverColor");
		actualLenght = 100f;	
		cube1.renderer.material.SetColor("_OutlineColor", ready);
		cube2.renderer.material.SetColor("_OutlineColor", ready);
		cube3.renderer.material.SetColor("_OutlineColor", ready);
		cube1recovered = true;
		cube2recovered = true;
		cube3recovered = true;
		power1.enabled = true;
		power2.enabled = true;
		power3.enabled = true;
		bandeau.color = ready;
		bandeau.transform.localScale = baseScale;
		time = 0f;
		state = STATEMANA.NORMAL;
	}
	
	IEnumerator recoverColor(GameObject cube)
	{
		var theTime = 0f;
		while(theTime <= timingRecover)
		{
			cube.renderer.material.SetColor("_OutlineColor", Color.Lerp(cube.renderer.material.GetColor("_OutlineColor"), ready, theTime));
			theTime += Time.deltaTime;	
			yield return 0;
		}
	}
	
	
}
