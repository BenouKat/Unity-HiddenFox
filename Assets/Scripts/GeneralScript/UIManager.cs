using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum UISTATE{
	READY,
	GO,
	SPOT,
	CLEAR
}

public class UIManager : MonoBehaviour {
	
	public List<UISprite> bandeau;
	public List<UISprite> labelSprite;
	public List<Color> colorBandeau;

	
	public UILabel timeDisplayed;
	public UILabel timeText;
	private float timeContener;
	
	private Vector3[] posbandeauBase;
	private Vector3[] postagBase;
	private Vector3[] scalebandeauBase;
	
	private Vector3 velocity = Vector3.zero;
	private Vector3 velocity2 = Vector3.zero;
	private Vector3 velocity3 = Vector3.zero;
	
	private Gameplay gameplay;
	//public float speedBandeau;
	public float speedLabel;
	
	
	public List<GameObject> medals;
	
	public UIPanel panelFinish;
	public UISprite cache;
	public UILabel levelName;
	public UILabel newTime;
	public UILabel bestTime;
	public UILabel goalTime;
	public UISprite yellowBandbest;
	public UISprite yellowBandgoal;
	public List<UILabel> infoMedal;
	public List<Color> colorButeeMedal;
	public UILabel retry;
	public GameObject manaBar;
	
	public float speedFade;
	
	void Start(){
		gameplay = GameObject.Find("Engine").GetComponent<Gameplay>();
		posbandeauBase = new Vector3[bandeau.Count];
		postagBase = new Vector3[labelSprite.Count];
		scalebandeauBase = new Vector3[bandeau.Count];
		
		for(int i=0; i<bandeau.Count; i++)
		{
			posbandeauBase[i] = bandeau[i].transform.position;
			postagBase[i] = labelSprite[i].transform.position;
			scalebandeauBase[i] = bandeau[i].transform.localScale;
		}
	}
	
	void Update()
	{
		if(gameplay.isLevelStarted())
		{
			timeContener = gameplay.getTime();
			timeDisplayed.text = ((int)(timeContener/60f)).ToString("00") + "'" + ((int)(timeContener%60f)).ToString("00") + "''" + ((timeContener - (int)timeContener)*100f).ToString("00");
		}
	}
	
	public void launchState(UISTATE state)
	{
		switch(state)
		{
		case UISTATE.READY:
			StartCoroutine("launchGetReady");
			break;
		case UISTATE.SPOT:
			StartCoroutine("launchSpotted");
			break;
		case UISTATE.CLEAR:
			StartCoroutine("launchClear");
			break;
		}
	}
	
	public void hideState(UISTATE state)
	{
		switch(state)
		{
		case UISTATE.SPOT:
			bandeau[(int)UISTATE.SPOT].gameObject.SetActive(false);
			labelSprite[(int)UISTATE.SPOT].gameObject.SetActive(false);
			break;
		case UISTATE.CLEAR:
			StopCoroutine("displayEndingScreen");
			bandeau[(int)UISTATE.CLEAR].gameObject.SetActive(false);
			labelSprite[(int)UISTATE.CLEAR].gameObject.SetActive(false);
			panelFinish.gameObject.SetActive(false);
			resetSpriteColor(cache);
			resetLabelColor(levelName);
			resetLabelColor(newTime);
			resetLabelColor(bestTime);
			resetLabelColor(goalTime);
			yellowBandbest.color = new Color(1f, 1f, 1f, 0f);
			yellowBandbest.transform.localScale = new Vector3(2048f, 125f, 1f);
			yellowBandgoal.color = new Color(1f, 1f, 1f, 0f);
			yellowBandgoal.transform.localScale = new Vector3(2048f, 125f, 1f);
			for(int i=0; i<infoMedal.Count; i++)
			{
				resetLabelColor(infoMedal[i]);
			}
			resetLabelColor(retry);
			resetMedalsPosition();
			timeText.color = new Color(0.83f, 0f, 0f, 1f);
			timeDisplayed.color = new Color(0.72f, 1f, 1f, 1f);
			break;
		}
	}
	
	void resetState(UISTATE state)
	{
		bandeau[(int)state].transform.position = posbandeauBase[(int)state];
		labelSprite[(int)state].transform.position = postagBase[(int)state];
		bandeau[(int)state].transform.localScale = scalebandeauBase[(int)state];
		
		bandeau[(int)state].color = new Color(1f, 1f, 1f, 0f);
		labelSprite[(int)state].color = new Color(1f, 1f, 1f, 0f);
	}
			
	void resetSpriteColor(UISprite theSprite)
	{
		theSprite.color = new Color(theSprite.color.r, theSprite.color.g, theSprite.color.r, 0f);	
	}
	
	void resetLabelColor(UILabel theSprite)
	{
		theSprite.color = new Color(theSprite.color.r, theSprite.color.g, theSprite.color.r, 0f);	
	}
	
	void resetMedalsPosition()
	{
		for(int i=0; i<4; i++)
		{
			medals[i].transform.localPosition = new Vector3(0f, 5f, 0f);	
			medals[i].SetActive(false);
		}
	}
	
	IEnumerator launchGetReady()
	{
		resetState(UISTATE.READY);
		resetState(UISTATE.GO);
		
		bandeau[(int)UISTATE.READY].gameObject.SetActive(true);
		labelSprite[(int)UISTATE.READY].gameObject.SetActive(true);
		var theTime = 0f;
		bandeau[(int)UISTATE.READY].color = new Color(1f, 1f, 1f, 1f);
		yield return 0;
		while(theTime < 0.5f)
		{
			bandeau[(int)UISTATE.READY].color = Color.Lerp(bandeau[(int)UISTATE.READY].color, colorBandeau[(int)UISTATE.READY], theTime*2f);
			labelSprite[(int)UISTATE.READY].color = Color.Lerp(labelSprite[(int)UISTATE.READY].color, new Color(1f, 1f, 1f, 1f), theTime*2f);
			theTime += Time.deltaTime;
			bandeau[(int)UISTATE.READY].transform.localScale = new Vector3(2048f, 250f - 125f*theTime*2f, 1f);
			labelSprite[(int)UISTATE.READY].transform.Translate(15f*speedLabel*Time.deltaTime, 0f, 0f);
			yield return 0;
		}
		bandeau[(int)UISTATE.READY].color = colorBandeau[(int)UISTATE.READY];

		bandeau[(int)UISTATE.READY].transform.localScale = new Vector3(2048f, 125f, 1f);
			
		theTime = 0f;
		while(theTime < 1.5f)
		{
			theTime += Time.deltaTime;
			labelSprite[(int)UISTATE.READY].transform.Translate(speedLabel*Time.deltaTime, 0f, 0f);
			bandeau[(int)UISTATE.READY].transform.localScale = new Vector3(2048f, 125f - 93f*(theTime/1.5f), 1f);
			yield return 0;
		}
		
		bandeau[(int)UISTATE.READY].gameObject.SetActive(false);
		labelSprite[(int)UISTATE.READY].gameObject.SetActive(false);
		
		bandeau[(int)UISTATE.GO].gameObject.SetActive(true);
		bandeau[(int)UISTATE.GO].color = new Color(1f, 1f, 1f, 1f);
		labelSprite[(int)UISTATE.GO].gameObject.SetActive(true);
		labelSprite[(int)UISTATE.GO].color = new Color(1f, 1f, 1f, 1f);
		
		theTime = 0f;
		while(theTime < 0.5f)
		{
			bandeau[(int)UISTATE.GO].color = Color.Lerp(bandeau[(int)UISTATE.GO].color, colorBandeau[(int)UISTATE.GO], theTime*2f);
			theTime += Time.deltaTime;
			yield return 0;
		}
		
		yield return new WaitForSeconds(0.5f);
		
		theTime = 0f;
		var col = bandeau[(int)UISTATE.GO].color;
		while(theTime < 0.5f)
		{
			bandeau[(int)UISTATE.GO].color = Color.Lerp(bandeau[(int)UISTATE.GO].color, new Color(col.r, col.g, col.b, 0f), theTime*2f);
			labelSprite[(int)UISTATE.GO].color = Color.Lerp(labelSprite[(int)UISTATE.GO].color, new Color(1f, 1f, 1f, 0f), theTime*2f);
			theTime += Time.deltaTime;
			yield return 0;
		}
		
		bandeau[(int)UISTATE.GO].gameObject.SetActive(false);
		labelSprite[(int)UISTATE.GO].gameObject.SetActive(false);
	}
	
	IEnumerator launchSpotted()
	{
		resetState(UISTATE.SPOT);
		
		bandeau[(int)UISTATE.SPOT].gameObject.SetActive(true);
		labelSprite[(int)UISTATE.SPOT].gameObject.SetActive(true);
		var theTime = 0f;
		bandeau[(int)UISTATE.SPOT].color = new Color(1f, 1f, 1f, 1f);
		yield return 0;
		while(theTime < 0.5f)
		{
			theTime += Time.deltaTime;
			bandeau[(int)UISTATE.SPOT].color = Color.Lerp(bandeau[(int)UISTATE.SPOT].color, colorBandeau[(int)UISTATE.SPOT], theTime*2f);
			labelSprite[(int)UISTATE.SPOT].color = Color.Lerp(labelSprite[(int)UISTATE.SPOT].color, new Color(1f, 1f, 1f, 1f), theTime*2f);
			bandeau[(int)UISTATE.SPOT].transform.localScale = new Vector3(2048f, 250f - 218f*((theTime > 0.5f ? 0.5f : theTime)/0.5f), 1f);
			labelSprite[(int)UISTATE.SPOT].transform.localPosition = Vector3.SmoothDamp(labelSprite[(int)UISTATE.SPOT].transform.localPosition, new Vector3(162f, labelSprite[(int)UISTATE.SPOT].transform.localPosition.y, 0f), ref velocity, 0.5f);
			yield return 0;
		}
	}
	
	IEnumerator launchClear()
	{
		resetState(UISTATE.CLEAR);
		
		manaBar.SetActive(false);
		
		bandeau[(int)UISTATE.CLEAR].gameObject.SetActive(true);
		labelSprite[(int)UISTATE.CLEAR].gameObject.SetActive(true);
		var theTime = 0f;
		bandeau[(int)UISTATE.CLEAR].color = new Color(1f, 1f, 1f, 1f);
		yield return 0;
		while(theTime < 0.5f)
		{
			labelSprite[(int)UISTATE.CLEAR].transform.localPosition = Vector3.SmoothDamp(labelSprite[(int)UISTATE.CLEAR].transform.localPosition, new Vector3(labelSprite[(int)UISTATE.CLEAR].transform.localPosition.x, 0f, 0f), ref velocity, 0.2f);
			theTime += Time.deltaTime;
			bandeau[(int)UISTATE.CLEAR].color = Color.Lerp(bandeau[(int)UISTATE.CLEAR].color, colorBandeau[(int)UISTATE.CLEAR], theTime*2f);
			labelSprite[(int)UISTATE.CLEAR].color = Color.Lerp(labelSprite[(int)UISTATE.CLEAR].color, new Color(1f, 1f, 1f, 1f), theTime*2f);
			bandeau[(int)UISTATE.CLEAR].transform.localScale = new Vector3(2048f, 250f - 210f*((theTime > 0.5f ? 0.5f : theTime)/0.5f), 1f);
			yield return 0;
		}
		
		yield return new WaitForSeconds(1f);
		
		theTime = 0f;
		while(theTime < 1.5f)
		{
			
			bandeau[(int)UISTATE.CLEAR].transform.localPosition = Vector3.SmoothDamp(labelSprite[(int)UISTATE.CLEAR].transform.localPosition, new Vector3(bandeau[(int)UISTATE.CLEAR].transform.localPosition.x, 200f, 0f), ref velocity, 0.7f);
			labelSprite[(int)UISTATE.CLEAR].transform.localPosition = Vector3.SmoothDamp(labelSprite[(int)UISTATE.CLEAR].transform.localPosition, new Vector3(labelSprite[(int)UISTATE.CLEAR].transform.localPosition.x, 200f, 0f), ref velocity, 0.7f);
			theTime += Time.deltaTime;
			yield return 0;
		}
		
		StartCoroutine("displayEndingScreen");
	}
	
	IEnumerator displayEndingScreen()
	{
		panelFinish.gameObject.SetActive(true);
		var theTime = 0f;
		while(timeText.color.a > 0)
		{
			timeText.color = Color.Lerp(timeText.color, new Color(0.8f, 0f, 0f, 0f), theTime*speedFade);
			timeDisplayed.color = Color.Lerp(timeDisplayed.color, new Color(1f, 1f, 1f, 0f), theTime*speedFade);
			cache.color = Color.Lerp(cache.color, new Color(cache.color.r, cache.color.g, cache.color.b, 0.7f), theTime*speedFade);
			theTime += Time.deltaTime;
			
			yield return 0;
		}
		
		yield return new WaitForSeconds(0.5f);
		
		theTime = 0f;
		var theBestTime = UserSave.Inst.getLevelScore(BDDLevelTime.Inst.actualLevel);
		var rank = BDDLevelTime.Inst.getRank(theBestTime);
		medals[rank].SetActive(true);
		levelName.text = "Level : " + BDDLevelTime.Inst.actualLevel;
		while(levelName.color.a < 1f)
		{
			levelName.color = Color.Lerp(levelName.color, new Color(levelName.color.r, levelName.color.g, levelName.color.b, 1f), theTime*speedFade);
			medals[rank].transform.localPosition = Vector3.SmoothDamp(medals[rank].transform.localPosition, new Vector3(0f, 0f, 0f), ref velocity, 0.2f);
			theTime += Time.deltaTime;
			yield return 0;
		}
		
		medals[rank].transform.localPosition = new Vector3(0f, 0f, 0f);
		
		theTime = 0f;
		newTime.text = "Time : " + ((int)(timeContener/60f)).ToString("00") + "'" + ((int)(timeContener%60f)).ToString("00") + "''" + ((timeContener - (int)timeContener)*100f).ToString("00");
		while(newTime.color.a < 1f)
		{
			newTime.color = Color.Lerp(newTime.color, new Color(newTime.color.r, newTime.color.g, newTime.color.b, 1f), theTime*speedFade);
			theTime += Time.deltaTime;
			yield return 0;
		}
		
		theTime = 0f;
		
		bestTime.text = "Best : " + (theBestTime < 0f ? "--'--''--" : ((int)(theBestTime/60f)).ToString("00") + "'" + ((int)(theBestTime%60f)).ToString("00") + "''" + ((theBestTime - (int)theBestTime)*100f).ToString("00"));
		while(bestTime.color.a < 1f)
		{
			bestTime.color = Color.Lerp(bestTime.color, new Color(bestTime.color.r, bestTime.color.g, bestTime.color.b, 1f), theTime*speedFade);
			theTime += Time.deltaTime;
			yield return 0;
		}
		
		theTime = 0f;
		var theGoalTime = BDDLevelTime.Inst.getNextGoal(theBestTime);
		goalTime.text = theGoalTime < 0f ? "Level mastered" : "Goal : " + ((int)(theGoalTime/60f)).ToString("00") + "'" + ((int)(theGoalTime%60f)).ToString("00") + "''" + ((theGoalTime - (int)theGoalTime)*100f).ToString("00");
		while(goalTime.color.a < 1f)
		{
			goalTime.color = Color.Lerp(goalTime.color, new Color(goalTime.color.r, goalTime.color.g, goalTime.color.b, 1f), theTime*speedFade);
			theTime += Time.deltaTime;
			yield return 0;
		}
		
		yield return new WaitForSeconds(1f);
		
		var newRank = BDDLevelTime.Inst.getRank(timeContener);
		var rankToClign = 3;
		if(newRank < rank || timeContener < theBestTime || theBestTime < 0f)
		{
			if(newRank < rank)
			{
				goalTime.color = new Color(1f, 0.8f, 0.5f, 1f);
				var newGoal = BDDLevelTime.Inst.getNextGoal(timeContener);
				goalTime.text = newGoal < 0f ? "Level mastered" : "Goal : " + ((int)(newGoal/60f)).ToString("00") + "'" + ((int)(newGoal%60f)).ToString("00") + "''" + ((newGoal - (int)newGoal)*100f).ToString("00");
				rankToClign = newRank;
				medals[newRank].SetActive(true);
			}
			if(timeContener < theBestTime || theBestTime < 0f)
			{
				bestTime.color = new Color(1f, 0.8f, 0.5f, 1f);
				bestTime.text = "Best : " + ((int)(timeContener/60f)).ToString("00") + "'" + ((int)(timeContener%60f)).ToString("00") + "''" + ((timeContener - (int)timeContener)*100f).ToString("00");
			}
			
			theTime = 0f;
			velocity = Vector3.zero;
			while(theTime < 1f)
			{
				theTime += Time.deltaTime;
				
				if(newRank < rank)
				{
					yellowBandgoal.color = Color.Lerp(yellowBandgoal.color, colorBandeau[(int)UISTATE.READY], theTime);
					yellowBandgoal.transform.localScale = new Vector3(2048f, 125f - (110f*theTime), 1f);
					medals[rank].transform.localPosition = Vector3.SmoothDamp(medals[rank].transform.localPosition, new Vector3(0f, -5f, 0f), ref velocity2, 0.2f);
					medals[newRank].transform.localPosition = Vector3.SmoothDamp(medals[newRank].transform.localPosition, new Vector3(0f, 0f, 0f), ref velocity3, 0.2f);
					infoMedal[newRank].color = Color.Lerp(infoMedal[newRank].color, new Color(infoMedal[newRank].color.r, infoMedal[newRank].color.g, infoMedal[newRank].color.b, 1f), theTime);
				}else{
					infoMedal[3].color = Color.Lerp(infoMedal[3].color, new Color(infoMedal[3].color.r, infoMedal[3].color.g, infoMedal[3].color.b, 1f), theTime);
				}
				
				if(timeContener < theBestTime || theBestTime < 0f)
				{
					yellowBandbest.color = Color.Lerp(yellowBandgoal.color, colorBandeau[(int)UISTATE.READY], theTime);
					yellowBandbest.transform.localScale = new Vector3(2048f, 125f - (110f*theTime), 1f);
				}
				theTime += Time.deltaTime;
				yield return 0;
			}
			
			if(newRank < rank)
			{
				medals[rank].transform.localPosition = new Vector3(0f, -5f, 0f);
				medals[newRank].transform.localPosition = new Vector3(0f, 0f, 0f);
			}
		}else{
			theTime = 0f;
			while(theTime < 1f)	
			{
				infoMedal[3].color = Color.Lerp(infoMedal[3].color, new Color(infoMedal[3].color.r, infoMedal[3].color.g, infoMedal[3].color.b, 1f), theTime);
				theTime += Time.deltaTime;
				yield return 0;
			}
		}
		
		if(timeContener < theBestTime || theBestTime < 0f) UserSave.Inst.saveLevel(BDDLevelTime.Inst.actualLevel, timeContener);
		theTime = 0f;
		var colorNormalEffectClign = infoMedal[rankToClign].effectColor;
		while(true)
		{
			retry.color = Color.Lerp(retry.color, new Color(1f, 1f, 1f, 1f), theTime);
			infoMedal[rankToClign].effectColor = Color.Lerp(colorNormalEffectClign, colorButeeMedal[rankToClign], Mathf.PingPong(theTime, 1f));
			theTime += Time.deltaTime;
			yield return 0;
		}
	
	}
}
