using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour {

	public Color[] ambiantColor;
	public GameObject[] directionals;
	public GameObject[] levelAppercu;
	public Material[] SkyboxLevel;
	public GameObject[] levelTitle;
	public UISprite blackos;
	public float speedFade;
	
	public int oldLevel;
	public int actualLevel;
	public bool moving;
	
	private bool startPress;
	public UILabel pressAnyKey;
	public GameObject effectPressStart;
	
	// Use this for initialization
	void Start () {
		
		moving = false;
		actualLevel = 0;
		
	}
	
	// Update is called once per frame
	void Update () {
		if(!startPress && Input.anyKeyDown)
		{
			StartCoroutine(fadeOutSimple());
			startPress = true;
			pressAnyKey.gameObject.SetActive(false);
			effectPressStart.SetActive(false);
		}
	}
	
	
	void left()
	{
		if(!moving)
		{
			if(actualLevel > 0)
			{
				oldLevel = actualLevel;
				actualLevel -= 1;
				StartCoroutine(fadeInOut());
			}else{
				//error sound	
			}
		}
	}
	
	void right()
	{
		if(!moving)
		{
			if(actualLevel < levelAppercu.Length - 1)
			{
				oldLevel = actualLevel;
				actualLevel += 1;
				StartCoroutine(fadeInOut());
			}else{
				//error sound	
			}
		}
	}
	
	IEnumerator fadeInOut()
	{
		moving = true;
		blackos.gameObject.SetActive(true);
		while(blackos.alpha < 1f)
		{
			blackos.alpha += Time.deltaTime/speedFade;	
			yield return 0;
		}
		
		RenderSettings.ambientLight = ambiantColor[actualLevel];
		directionals[oldLevel].SetActive(false);
		directionals[actualLevel].SetActive(true);
		levelAppercu[oldLevel].SetActive(false);
		levelAppercu[actualLevel].SetActive(true);
		levelTitle[oldLevel].SetActive(false);
		levelTitle[actualLevel].SetActive(true);
		Camera.main.GetComponent<Skybox>().material = SkyboxLevel[actualLevel];
		Camera.main.GetComponent<DepthOfFieldScatter>().focalTransform = levelAppercu[actualLevel].transform.FindChild("Target");
		yield return new WaitForSeconds(0.5f);
		
		while(blackos.alpha > 0f)
		{
			blackos.alpha -= Time.deltaTime/speedFade;	
			yield return 0;
		}
		blackos.gameObject.SetActive(false);
		moving = false;
	}
	
	IEnumerator fadeOutSimple()
	{
		moving = true;
		while(blackos.alpha > 0f)
		{
			blackos.alpha -= Time.deltaTime/speedFade;	
			yield return 0;
		}
		blackos.gameObject.SetActive(false);
		
		blackos.alpha = 0f;
		moving = false;
	}
	
}
