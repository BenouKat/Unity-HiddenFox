using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Gameplay : MonoBehaviour {
	
	public GameObject thePlayer;
	private DynamicLoader dl;
	public UIManager uim;
	public GameObject playerDeath;
	
	public ParticleSystem cubeIncoming;
	public ParticleSystem cubeBouclier;
	public ParticleSystem cubeExplode;
	public GameObject particleOutro;
	
	private float speedRotation;
	
	private bool triggerLevelStarted;
	private bool levelStarted;
	private float time;
	
	private float globalLevelTime;
	
	private bool gameOver;
	private bool clear;
	// Use this for initialization
	void Start () {
		dl = GetComponent<DynamicLoader>();
		triggerLevelStarted = false;
		levelStarted = false;
		gameOver = false;
		globalLevelTime = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		//Provisoire
		if(!triggerLevelStarted)
		{
			if(time >= 0.2f)
			{
				startTheLevel();
			}
			time += Time.deltaTime;
		}
		
		if(levelStarted && !clear && !gameOver)
		{	
			globalLevelTime += Time.deltaTime;
		}
		
		if(gameOver)
		{
			if(time >= 0.2f)
			{
				if(thePlayer.activeSelf) thePlayer.SetActive(false);	
				if(Input.GetKeyDown(KeyCode.R))
				{
					resetTheLevel();	
				}
			}
			time += Time.deltaTime;
		}
		
		if(clear)
		{
			if(Input.GetKeyDown(KeyCode.R))
			{
				resetTheLevel();	
			}
		}
	}
	
	void startTheLevel()
	{
		triggerLevelStarted = true;
		globalLevelTime = 0f;
		StartCoroutine("startPlayer");
		uim.launchState(UISTATE.READY);
	}
	
	void resetTheLevel()
	{
		triggerLevelStarted = false;
		levelStarted = false;
		if(gameOver) uim.hideState(UISTATE.SPOT);
		if(clear) uim.hideState(UISTATE.CLEAR);
		clear = false;
		gameOver = false;
		time = 0f;
		dl.resetTheLevel();
		thePlayer.GetComponent<Player>().centerCamera();
		thePlayer.SetActive(false);
		desactiveAllDeathPlayer();
		for(int i=0; i<playerDeath.transform.childCount; i++)
		{
			playerDeath.transform.GetChild(i).gameObject.SetActive(false);	
		}
		particleOutro.SetActive(false);
	}
	
	public void isDiscovered()
	{
		if(!gameOver)
		{
			gameOver = true;
			uim.launchState(UISTATE.SPOT);
		}
	}
	
	public void isDiscovered(GameObject particuleDetection, string enemyType)
	{
		if(!gameOver)
		{
			gameOver = true;
			if(particuleDetection != null) particuleDetection.SetActive(true);
			activeSpecialTrigger(enemyType);
			time = 0f;
			uim.launchState(UISTATE.SPOT);
		}
	}
	
	public bool isLevelStarted()
	{
		return levelStarted;	
	}
	
	public void stageClear()
	{
		if(!clear)
		{
			clear = true;
			finishPlayer();
			uim.launchState(UISTATE.CLEAR);
		}
	}
	
	IEnumerator startPlayer()
	{
		cubeBouclier.transform.parent.transform.position = new Vector3(thePlayer.transform.position.x, 0f, thePlayer.transform.position.z);
		cubeIncoming.gameObject.SetActive (true);
		cubeBouclier.gameObject.SetActive(true);
		speedRotation = 180f;
		var theTime = 0f;
		yield return 0;
		while(theTime < 2f)
		{		
			cubeBouclier.transform.Rotate(new Vector3(0f, 0f, speedRotation*Time.deltaTime));
			speedRotation += 360f*Time.deltaTime;
			theTime += Time.deltaTime;
			yield return 0;
		}
		cubeIncoming.gameObject.SetActive(false);
		cubeExplode.gameObject.SetActive(true);
		thePlayer.SetActive(true);
		dl.startTheLevel();
		levelStarted = true;
		
		cubeBouclier.transform.Rotate(new Vector3(0f, 0f, speedRotation*Time.deltaTime));
		cubeExplode.transform.Rotate(new Vector3(0f, 0f, speedRotation*Time.deltaTime));
		yield return 0;
		
		theTime = 0f;
		while(theTime < 1f)
		{
			cubeBouclier.transform.Rotate(new Vector3(0f, 0f, speedRotation*Time.deltaTime));
			cubeExplode.transform.Rotate(new Vector3(0f, 0f, speedRotation*Time.deltaTime));
			speedRotation -= 360f*Time.deltaTime;
			theTime += Time.deltaTime;
			yield return 0;
		}
		
		cubeBouclier.gameObject.SetActive(false);
		cubeExplode.gameObject.SetActive(false);
	}
	
	void finishPlayer()
	{
		particleOutro.transform.position = new Vector3(thePlayer.transform.position.x, 2f, thePlayer.transform.position.z);
		particleOutro.SetActive(true);
		thePlayer.SetActive(false);
	}
	
	public float getTime()
	{
		return globalLevelTime;	
	}
	
	public void desactiveAllDeathPlayer()
	{
		playerDeath.transform.FindChild("Camera").particleSystem.Clear();
		playerDeath.transform.FindChild("Camera").particleSystem.Stop();
		playerDeath.transform.FindChild("Canon").FindChild("Explosion").particleSystem.Clear(true);
		playerDeath.transform.FindChild("Canon").FindChild("Explosion").particleSystem.Stop(true);
		playerDeath.transform.FindChild("Helicopter").FindChild("PlayerDead").transform.localPosition = new Vector3(0f, 1.68f, 0f);
		playerDeath.transform.FindChild("Helicopter").FindChild("PlayerDead").transform.eulerAngles = new Vector3(0f, 0f, 0f);
	}
	
	public void activeSpecialTrigger(string enemyType)
	{
		switch(enemyType)
		{
		case "Camera":
			playerDeath.transform.FindChild(enemyType).gameObject.SetActive(true);
			Invoke("callPlayerDeathCamera", 0.2f);
			break;
		case "Canon":
			thePlayer.SetActive(false);
			playerDeath.transform.position = new Vector3(thePlayer.transform.position.x, 0f, thePlayer.transform.position.z);
			playerDeath.transform.FindChild(enemyType).gameObject.SetActive(true);
			playerDeath.transform.rotation = thePlayer.transform.rotation;
			break;
		case "Helicopter":
			Invoke("callPlayerDeathHelico", 0.2f);
			break;
		}
	}
	
	public void callPlayerDeathCamera()
	{
		playerDeath.transform.position = new Vector3(thePlayer.transform.position.x, 0f, thePlayer.transform.position.z);	
	}
	
	public void callPlayerDeathHelico()
	{
		thePlayer.SetActive(false);
		playerDeath.transform.position = new Vector3(thePlayer.transform.position.x, 0f, thePlayer.transform.position.z);
		playerDeath.transform.rotation = thePlayer.transform.rotation;	
		playerDeath.transform.FindChild("Helicopter").gameObject.SetActive(true);
	}
}
