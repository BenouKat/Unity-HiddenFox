using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Reflection;

public enum EDITMODE{
	CUBE,
	STARTPOSITION,
	FINISHPOSITION,
	ENEMY,
	PLACECANON,
	PLACECAMERA,
	EDITENEMY,
	HELICO,
	EDITHELICO,
	TESTENEMY,
	SAVELEVEL,
	LOADLEVEL
}

public enum TURN{
	UPTORIGHT,
	UPTOLEFT,
	DOWNTORIGHT,
	DOWNTOLEFT,
	RIGHTTOUP,
	RIGHTTODOWN,
	LEFTTOUP,
	LEFTTODOWN,
	NONE
}

public class EditorLevelManager : MonoBehaviour {
	
	//debug
	public bool EnableCameraEditor;
	//Cube du niveau 0
	public GameObject cubeBase;
	//Cube du niveau autre
	public GameObject cubeBloc;
	//Collider Raycast
	public GameObject rayCastCollider;
	//Curseur
	public GameObject cursor;
	//Element du curseur
	private GameObject placementCube;
	private GameObject deleteCube;
	
	//Modele Enemy
	public GameObject enemyModel;
	//Curseur Enemy
	public GameObject enemyCursor;
	//Element du curseur
	private GameObject enemyValid;
	private GameObject enemyHover;
	private GameObject enemyInvalid;
	
	//Modele Enemy
	public GameObject enemyHelicoModel;
	//Curseur Enemy
	public GameObject enemyHelicoCursor;
	//Element du curseur
	private GameObject enemyHelicoValid;
	private GameObject enemyHelicoHover;
	private GameObject enemyHelicoInvalid;
	
	//Modele Edit Action
	public GameObject lineObject;
	public GameObject angleHObject;
	public GameObject angleAHObject;
	public GameObject actionObject;
	public GameObject possibleActionObject;
	//Curseur
	public GameObject cursorEdit;
	//Element du curseur
	private GameObject cursorWayValid;
	private GameObject cursorWayAction;
	
	//UI edit enemy
	public GameObject panelUIEdit;
	
	//UI load file
	public GameObject panelUILoad;
	
	//UI save file
	public GameObject panelUISave;
	
	private Dictionary<Enemy, List<GameObject>> listUIEnemy;
	
	//PlayerSpawn
	public GameObject playerSpawn;
	//Element du playerSpawn
	private GameObject playerSpawnValid;
	private GameObject playerSpawnInvalid;
	
	//PlayerSpawn
	public GameObject playerFinish;
	//Element du playerSpawn
	private GameObject playerFinishValid;
	private GameObject playerFinishInvalid;
	
	public GameObject canonModel;
	//Canon
	public GameObject canonCursor;
	//Element du curseur Canon
	private GameObject canonCursorValid;
	private GameObject canonCursorInvalid;
	
	public GameObject cameraEnModel;
	//CameraEnemy
	public GameObject cameraEnCursor;
	//Element du curseur CameraEnemy
	private GameObject cameraEnValid;
	private GameObject cameraEnInvalid;
	private GameObject cameraRotationCursor;
	private int cameraLook;
	
	//Taille du terrain par défaut (sera changé pour être rentré par l'utilisateur)
	public int widthLevelDefault;
	public int heightLevelDefault;
	
	//Taille max du terrain (à choisir selon les performances)
	public int maxWidth;
	public int maxHeight;
	public int maxVolume;
	
	//Vitesse déplacement caméra
	public float speedCameraMove;
	
	//Hauteur selectionnée
	private int actualSizeSelected;
	
	//Level construction
	private LevelEditor actualLevel;
	
	//Coordonnée selectionnées
	private int actualWidthSelected;
	private int actualHeightSelected;
	private int gridWidthSelected;
	private int gridHeightSelected;
	private int oldWidthSelected;
	private int oldHeightSelected;
	private int oldSizeSelected;
	
	//Bloc déjà en place selectionné
	private GameObject previousBlocSelected;
	
	//Look de l'ennemi en cours
	private LOOK enemyLook;
	private Enemy actualEnemyEdit;
	private Vector2 lastEnemyEditPosition;
	private int actualNumberAction;
	private int actualActionSelected;
	private bool inEnemyTest;
	
	//List des levels disponibles
	private List<string> listLevelSaved;
	private int actualFileSelected;
	
	//Mode de construction
	private EDITMODE actualMode;
	
	private bool isInHidingMode;
	
	// Use this for initialization
	void Start () {
		
		isInHidingMode = false;
		inEnemyTest = false;
		actualMode = EDITMODE.CUBE;
		placementCube = cursor.transform.FindChild("PlacementCube").gameObject;
		deleteCube = cursor.transform.FindChild("DeleteCube").gameObject;
		playerSpawnValid = playerSpawn.transform.FindChild("SpawnValid").gameObject;
		playerSpawnInvalid = playerSpawn.transform.FindChild("SpawnInvalid").gameObject;
		playerFinishValid = playerFinish.transform.FindChild("FinishValid").gameObject;
		playerFinishInvalid = playerFinish.transform.FindChild("FinishInvalid").gameObject;
		canonCursorValid = canonCursor.transform.FindChild("CanonValid").gameObject;
		canonCursorInvalid = canonCursor.transform.FindChild("CanonInvalid").gameObject;
		cameraEnValid = cameraEnCursor.transform.FindChild("CameraValid").gameObject;
		cameraEnInvalid = cameraEnCursor.transform.FindChild("CameraInvalid").gameObject;
		cameraRotationCursor = cameraEnCursor.transform.FindChild("CubePosition").gameObject;
		enemyHelicoValid = enemyHelicoCursor.transform.FindChild("HelicoValid").gameObject;
		enemyHelicoHover = enemyHelicoCursor.transform.FindChild("HelicoHover").gameObject;
		enemyHelicoInvalid = enemyHelicoCursor.transform.FindChild("HelicoInvalid").gameObject;
		enemyValid = enemyCursor.transform.FindChild("EnemyValid").gameObject;
		enemyHover = enemyCursor.transform.FindChild("EnemyHover").gameObject;
		enemyInvalid = enemyCursor.transform.FindChild("EnemyInvalid").gameObject;
		cursorWayValid = cursorEdit.transform.FindChild("WayEditValid").gameObject;
		cursorWayAction = cursorEdit.transform.FindChild("ActionEdit").gameObject;
		listUIEnemy = new Dictionary<Enemy, List<GameObject>>();
		listLevelSaved = new List<string>();
		
		actualSizeSelected = 0;
		actualActionSelected = (int)MOVE.WAIT;
		actualFileSelected = 0;
		
		actualLevel = new LevelEditor(maxWidth, maxHeight, maxVolume);
		
		
		for(int i = 0; i<widthLevelDefault; i++)
		{
			for(int j=0; j<heightLevelDefault; j++)
			{
				var go = (GameObject)Instantiate(cubeBase, new Vector3(j*2, 0f, i*2), cubeBase.transform.rotation);
				actualLevel.setCube(i, j, 0, go);
				go.SetActive(true);
			}
		}
	}
	
	void Update()
	{
		switch(actualMode)
		{
			case EDITMODE.CUBE:
				UpdateCube();
				break;
			case EDITMODE.STARTPOSITION:
				UpdateStartPosition();
				break;
			case EDITMODE.FINISHPOSITION:
				UpdateFinishPosition();
				break;
			case EDITMODE.PLACECANON:
				UpdateCanon();
				break;
			case EDITMODE.PLACECAMERA:
				UpdateCameraEnemy();
				break;
			case EDITMODE.ENEMY:
				UpdateEnemy();
				break;
			case EDITMODE.EDITENEMY:
				UpdateEnemyEdit();
				break;
			case EDITMODE.HELICO:
				UpdateHelico();
				break;
			case EDITMODE.EDITHELICO:
				UpdateHelicoEdit();
				break;
			case EDITMODE.TESTENEMY:
				UpdateCamera();
				break;
			case EDITMODE.LOADLEVEL:
				UpdateSaveLoad();
				break;
			case EDITMODE.SAVELEVEL:
				UpdateSaveLoad();
				break;
		}
		
		//Debug inputs
		if(Input.GetKeyDown(KeyCode.I) && actualMode == EDITMODE.CUBE) //SPAWN
		{
			goOnSpawnPlayerMode();	
		}
		
		if(Input.GetKeyDown(KeyCode.K) && actualMode == EDITMODE.CUBE) //FINISH
		{
			goOnFinishPlayerMode();	
		}
		
		if(Input.GetKeyDown(KeyCode.C) && actualMode == EDITMODE.CUBE) //CANON
		{
			goOnCanonMode();	
		}
		
		if(Input.GetKeyDown(KeyCode.V) && actualMode == EDITMODE.CUBE) //CANON
		{
			goOnCameraMode();	
		}
		
		if(Input.GetKeyDown(KeyCode.B) && actualMode == EDITMODE.CUBE) //HELICO PUT
		{
			goOnSpawnHelico();
		}
		
		if(Input.GetKeyDown(KeyCode.U) && actualMode == EDITMODE.CUBE) //ENEMY PUT
		{
			goOnSpawnEnemy();
		}
		
		if(Input.GetKeyDown(KeyCode.O))
		{
			if(isInHidingMode)
			{
				showUpperLevels();
				isInHidingMode = false;
			}else{
				hideUpperLevels();
				isInHidingMode = true;
			}
		}
		
		//TEST
		if(Input.GetKeyDown(KeyCode.T) && (((actualMode == EDITMODE.EDITENEMY || actualMode == EDITMODE.EDITHELICO) && actualEnemyEdit.isValidEnemy(actualLevel)) || (actualMode == EDITMODE.TESTENEMY)))
		{
			if(!inEnemyTest)
			{
				inEnemyTest = true;
				actualEnemyEdit.testEnemy();
				actualMode = EDITMODE.TESTENEMY;
				disableEnemyWay(actualEnemyEdit);	
			 	cursorWayValid.SetActive(false);
				cursorWayAction.SetActive(false);
				possibleActionObject.SetActive(false);
				panelUIEdit.SetActive(false);
				oldWidthSelected = -1;
				oldHeightSelected = -1;
			}else{
				inEnemyTest = false;
				actualEnemyEdit.endTestEnemy();
				enemyHover.SetActive(false);
				if(actualEnemyEdit.isHelico())
				{
					refreshPossiblePositionActionHelico();
				}else{
					refreshPossiblePositionAction();
				}
				panelUIEdit.SetActive(true);
				actualMode = EDITMODE.EDITENEMY;
				enableEnemyWay(actualEnemyEdit);
				oldWidthSelected = -1;
				oldHeightSelected = -1;
			}
		}
		
		//SAVE
		if(Input.GetKeyDown(KeyCode.S) && actualMode == EDITMODE.CUBE)
		{
			goOnSaveLevel();	
		}else if(Input.GetKeyDown(KeyCode.Return) && actualMode == EDITMODE.SAVELEVEL)
		{
			if(!String.IsNullOrEmpty(panelUISave.transform.FindChild("Input").GetComponent<UIInput>().text))
			{
				saveLevel();
			}
		}
		
		//LOAD
		if(Input.GetKeyDown(KeyCode.L) && actualMode == EDITMODE.CUBE)
		{
			goOnLoadLevel();
		}else if(Input.GetKeyDown(KeyCode.Return) && actualMode == EDITMODE.LOADLEVEL)
		{
			if(panelUILoad.transform.FindChild("LabelFile").GetComponent<UILabel>().text != "EMPTY")
			{
				loadLevel();
			}
		}
		
	}
	
	
	#region Cube
	void UpdateCube () {
	

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		
		if(Physics.Raycast(ray, out hit)){
			//Detection de la position curseur
			UpdatePositionMouse(hit);
			
			//Verification de la nouvelle position
			if(actualWidthSelected != oldWidthSelected || actualHeightSelected != oldHeightSelected)
			{
				oldWidthSelected = actualWidthSelected;
				oldHeightSelected = actualHeightSelected;
				gridWidthSelected = (int)actualWidthSelected/2;
				gridHeightSelected = (int)actualHeightSelected/2;
			
				//Actualisation
				cursor.transform.position = new Vector3(actualHeightSelected, actualSizeSelected*2, actualWidthSelected);
				
				//Activation des curseur
				var blocState = actualLevel.getBlocState(gridWidthSelected, gridHeightSelected, actualSizeSelected);
				switch(blocState)
				{
				case BLOCSTATE.EMPTY: //Case vide
					if(previousBlocSelected != null)
					{
						previousBlocSelected.renderer.enabled = appearCubeCondition();
						previousBlocSelected = null;
					}
					deleteCube.SetActive(false);
					placementCube.SetActive(true);
					break;
					
				case BLOCSTATE.CUBE: //Case pleine
					if(previousBlocSelected != null)
					{
						previousBlocSelected.renderer.enabled = appearCubeCondition();
					}
					previousBlocSelected = actualLevel.getGameObjectAt(gridWidthSelected, gridHeightSelected, actualSizeSelected);
					previousBlocSelected.renderer.enabled = false;
					deleteCube.SetActive(true);
					placementCube.SetActive(false);
					break;
					
				case BLOCSTATE.PLAYERSTART: //Case du respawn
					if(previousBlocSelected != null)
					{
						previousBlocSelected.renderer.enabled = appearCubeCondition();
						previousBlocSelected = null;
					}
					deleteCube.SetActive(true);
					placementCube.SetActive(false);
					break;
				case BLOCSTATE.PLAYERFINISH: //Case du finish
					if(previousBlocSelected != null)
					{
						previousBlocSelected.renderer.enabled = appearCubeCondition();
						previousBlocSelected = null;
					}
					deleteCube.SetActive(true);
					placementCube.SetActive(false);
					break;
				case BLOCSTATE.CANON: //Case canon
					if(previousBlocSelected != null)
					{
						previousBlocSelected.renderer.enabled = appearCubeCondition();
						previousBlocSelected = null;
					}
					deleteCube.SetActive(true);
					placementCube.SetActive(false);
					break;
				case BLOCSTATE.CAMERA: //Case camera
					if(previousBlocSelected != null)
					{
						previousBlocSelected.renderer.enabled = appearCubeCondition();
						previousBlocSelected = null;
					}
					deleteCube.SetActive(true);
					placementCube.SetActive(false);
					break;
				case BLOCSTATE.ENEMYSTART:
					if(previousBlocSelected != null)
					{
						previousBlocSelected.renderer.enabled = appearCubeCondition();
						previousBlocSelected = null;
					}
					deleteCube.SetActive(true);
					placementCube.SetActive(false);
					break;
				case BLOCSTATE.HELICOSTART:
					if(previousBlocSelected != null)
					{
						previousBlocSelected.renderer.enabled = appearCubeCondition();
						previousBlocSelected = null;
					}
					deleteCube.SetActive(true);
					placementCube.SetActive(false);
					break;
				}
				
				oldSizeSelected = actualSizeSelected;
			}
			
		}else{
			//Desactivation du curseur
			if(placementCube.activeSelf)
			{
				placementCube.SetActive(false);
			}else
			if(deleteCube.activeSelf)
			{
				deleteCube.SetActive(false);	
			}
			if(previousBlocSelected != null)
			{
				previousBlocSelected.renderer.enabled = appearCubeCondition();
				previousBlocSelected = null;
			}
			oldWidthSelected = -1;
			oldHeightSelected = -1;
			
		}

		
		//Clic souris
		if(Input.GetMouseButton(0) && placementCube.activeSelf)
		{
			var go = (GameObject)Instantiate(actualSizeSelected == 0 ? cubeBase : cubeBloc, new Vector3(actualHeightSelected, actualSizeSelected*2, actualWidthSelected), cubeBase.transform.rotation);
			actualLevel.setCube(gridWidthSelected, gridHeightSelected, actualSizeSelected, go);
			go.SetActive(true);
			oldWidthSelected = -1;
			oldHeightSelected = -1;
			
		}else if(Input.GetMouseButton(1) && deleteCube.activeSelf){
			var blocState = actualLevel.getBlocState(gridWidthSelected, gridHeightSelected, actualSizeSelected);
			if(actualSizeSelected != 0 || actualLevel.getBlocState(gridWidthSelected, gridHeightSelected, 1) <= BLOCSTATE.CUBE){
				switch(blocState)
				{
					case BLOCSTATE.CUBE:
						actualLevel.removeCube(gridWidthSelected, gridHeightSelected, actualSizeSelected);
						break;
					case BLOCSTATE.PLAYERSTART:
						actualLevel.removePlayerPosition();
						playerSpawnValid.SetActive(false);
						playerSpawnInvalid.SetActive(false);
						break;
					case BLOCSTATE.PLAYERFINISH:
						actualLevel.removeFinishPosition();
						playerFinishValid.SetActive(false);
						playerFinishInvalid.SetActive(false);
						break;
					case BLOCSTATE.CANON:
						actualLevel.removeCanonPosition(gridWidthSelected, gridHeightSelected);
						break;
					case BLOCSTATE.CAMERA:
						actualLevel.removeCameraEnemyPosition(gridWidthSelected, gridHeightSelected);
						break;
					case BLOCSTATE.ENEMYSTART:
						removeEnemyObject(actualLevel.getGameObjectAt(gridWidthSelected, gridHeightSelected, 1).transform.GetComponent<Enemy>());
						actualLevel.removeEnemyPosition(gridWidthSelected, gridHeightSelected);
						break;
					case BLOCSTATE.HELICOSTART:
						removeEnemyObject(actualLevel.getGameObjectAt(gridWidthSelected, gridHeightSelected, 3).transform.GetComponent<Enemy>());
						actualLevel.removeEnemyHelicoPosition(gridWidthSelected, gridHeightSelected);
						break;
				}
			}
			oldWidthSelected = -1;
			oldHeightSelected = -1;
		}
		
		//Wheel
		if((Input.GetAxis("Mouse ScrollWheel") > 0 || Input.GetKeyDown(KeyCode.P)) && actualSizeSelected < maxVolume - 1){
			actualSizeSelected++;
			if(isInHidingMode)
			{
				actualLevel.setDisplayThisLevel(true, maxWidth, maxHeight, actualSizeSelected);
			}
			rayCastCollider.transform.Translate(0f, 2f, 0f);
			oldWidthSelected = -1;
			oldHeightSelected = -1;
		}else if((Input.GetAxis("Mouse ScrollWheel") < 0 || Input.GetKeyDown(KeyCode.M)) && actualSizeSelected > 0){
			if(isInHidingMode)
			{
				actualLevel.setDisplayThisLevel(false, maxWidth, maxHeight, actualSizeSelected);
			}
			actualSizeSelected--;
			rayCastCollider.transform.Translate(0f, -2f, 0f);
			oldWidthSelected = -1;
			oldHeightSelected = -1;
		}
		
		
		UpdateCamera();
		
	}
	#endregion
	
	#region Spawn
	void UpdateStartPosition () {
	
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		
		if(Physics.Raycast(ray, out hit)){
			//Detection de la position curseur
			UpdatePositionMouse(hit);
			
			//Verification de la nouvelle position
			if(actualWidthSelected != oldWidthSelected || actualHeightSelected != oldHeightSelected)
			{
				oldWidthSelected = actualWidthSelected;
				oldHeightSelected = actualHeightSelected;
				gridWidthSelected = (int)actualWidthSelected/2;
				gridHeightSelected = (int)actualHeightSelected/2;
			
				//Actualisation
				playerSpawn.transform.position = new Vector3(actualHeightSelected, actualSizeSelected*2, actualWidthSelected);
				
				//Activation des curseur
				var blocStateStage = actualLevel.getBlocState(gridWidthSelected, gridHeightSelected, actualSizeSelected);
				var blocStateGround = actualLevel.getBlocState(gridWidthSelected, gridHeightSelected, 0);
				if(blocStateStage == BLOCSTATE.EMPTY && blocStateGround == BLOCSTATE.CUBE)
				{
					playerSpawnValid.SetActive(true);
					playerSpawnInvalid.SetActive(false);
				}else{
					playerSpawnInvalid.SetActive(true);
					playerSpawnValid.SetActive(false);
				}
			}
			
		}else{
			playerSpawnInvalid.SetActive(false);
			playerSpawnValid.SetActive(false);
		}
		
		//Clic souris
		if(Input.GetMouseButton(0) && playerSpawnValid.activeSelf)
		{
			actualLevel.setStartPlayerPosition(gridWidthSelected, gridHeightSelected);
			actualMode = EDITMODE.CUBE;
			oldWidthSelected = -1;
			oldHeightSelected = -1;
			
		}
		
		
		UpdateCamera();
		
	}
	#endregion
	
	#region Finish
	void UpdateFinishPosition () {
	
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		
		if(Physics.Raycast(ray, out hit)){
			//Detection de la position curseur
			UpdatePositionMouse(hit);
			
			//Verification de la nouvelle position
			if(actualWidthSelected != oldWidthSelected || actualHeightSelected != oldHeightSelected)
			{
				oldWidthSelected = actualWidthSelected;
				oldHeightSelected = actualHeightSelected;
				gridWidthSelected = (int)actualWidthSelected/2;
				gridHeightSelected = (int)actualHeightSelected/2;
			
				//Actualisation
				playerFinish.transform.position = new Vector3(actualHeightSelected, actualSizeSelected*2, actualWidthSelected);
				
				//Activation des curseur
				var blocStateStage = actualLevel.getBlocState(gridWidthSelected, gridHeightSelected, actualSizeSelected);
				var blocStateGround = actualLevel.getBlocState(gridWidthSelected, gridHeightSelected, 0);
				if(blocStateStage == BLOCSTATE.EMPTY && blocStateGround == BLOCSTATE.CUBE)
				{
					playerFinishValid.SetActive(true);
					playerFinishInvalid.SetActive(false);
				}else{
					playerFinishInvalid.SetActive(true);
					playerFinishValid.SetActive(false);
				}
			}
			
		}else{
			playerFinishInvalid.SetActive(false);
			playerFinishValid.SetActive(false);
		}
		
		//Clic souris
		if(Input.GetMouseButton(0) && playerFinishValid.activeSelf)
		{
			actualLevel.setFinishPlayerPosition(gridWidthSelected, gridHeightSelected);
			actualMode = EDITMODE.CUBE;
			oldWidthSelected = -1;
			oldHeightSelected = -1;
			
		}
		
		
		UpdateCamera();
		
	}
	#endregion
	
	#region Canon
	void UpdateCanon()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		
		if(Physics.Raycast(ray, out hit)){
			//Detection de la position curseur
			UpdatePositionMouse(hit);
			
			//Verification de la nouvelle position
			if(actualWidthSelected != oldWidthSelected || actualHeightSelected != oldHeightSelected)
			{
				oldWidthSelected = actualWidthSelected;
				oldHeightSelected = actualHeightSelected;
				gridWidthSelected = (int)actualWidthSelected/2;
				gridHeightSelected = (int)actualHeightSelected/2;
			
				//Actualisation
				canonCursor.transform.position = new Vector3(actualHeightSelected, 2f, actualWidthSelected);
				
				//Activation des curseur
				var blocStateStage = actualLevel.getBlocState(gridWidthSelected, gridHeightSelected, actualSizeSelected);
				var blocStateGround = actualLevel.getBlocState(gridWidthSelected, gridHeightSelected, actualSizeSelected-1);
				if(blocStateStage == BLOCSTATE.EMPTY && blocStateGround == BLOCSTATE.CUBE)
				{
					canonCursorValid.SetActive(true);	
					canonCursorInvalid.SetActive(false);
				}else{
					canonCursorInvalid.SetActive(true);
					canonCursorValid.SetActive(false);
				}
			}
			
		}else{
			canonCursorInvalid.SetActive(false);
			canonCursorValid.SetActive(false);
		}
		
		
		if(Input.GetMouseButton(0))
		{
			if(canonCursorValid.activeSelf){
				var go = (GameObject)Instantiate(canonModel, new Vector3(actualHeightSelected, actualSizeSelected*2 + 0.2f, actualWidthSelected), canonCursorValid.transform.rotation);
				actualLevel.setCanon(gridWidthSelected, gridHeightSelected, go);
				go.SetActive(true);
				go.GetComponent<Canon>().setLook((LOOK)enemyLook);
				canonCursorValid.SetActive(false);
				actualMode = EDITMODE.CUBE;
				oldWidthSelected = -1;
				oldHeightSelected = -1;
			}
			
		}else if(Input.GetMouseButtonDown(1) && canonCursorValid.activeSelf){
		
			enemyLook++;
			if((int) enemyLook >= 4)
			{
				enemyLook = (LOOK) 0;
			}
			switch(enemyLook)
			{
			case LOOK.DOWN:
				canonCursorValid.transform.eulerAngles = new Vector3(0f, 0f, 0f);
				canonCursorInvalid.transform.eulerAngles = new Vector3(0f, 0f, 0f);
				break;
			case LOOK.UP:
				canonCursorValid.transform.eulerAngles = new Vector3(0f, 180f, 0f);
				canonCursorInvalid.transform.eulerAngles = new Vector3(0f, 180f, 0f);
				break;
			case LOOK.RIGHT:
				canonCursorValid.transform.eulerAngles = new Vector3(0f, -90f, 0f);
				canonCursorInvalid.transform.eulerAngles = new Vector3(0f, -90f, 0f);
				break;
			case LOOK.LEFT:
				canonCursorValid.transform.eulerAngles = new Vector3(0f, 90f, 0f);
				canonCursorInvalid.transform.eulerAngles = new Vector3(0f, 90f, 0f);
				break;
			}
			oldWidthSelected = -1;
			oldHeightSelected = -1;
			
		}
		
		UpdateCamera();
	}
	#endregion
	
	#region CameraEnemy
	void UpdateCameraEnemy()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		
		if(Physics.Raycast(ray, out hit)){
			//Detection de la position curseur
			UpdatePositionMouse(hit);
			
			//Verification de la nouvelle position
			if(actualWidthSelected != oldWidthSelected || actualHeightSelected != oldHeightSelected)
			{
				oldWidthSelected = actualWidthSelected;
				oldHeightSelected = actualHeightSelected;
				gridWidthSelected = (int)actualWidthSelected/2;
				gridHeightSelected = (int)actualHeightSelected/2;
			
				//Actualisation
				cameraEnCursor.transform.position = new Vector3(actualHeightSelected, 4f, actualWidthSelected);
				
				//Activation des curseur
				var blocStateStage = actualLevel.getBlocState(gridWidthSelected, gridHeightSelected, actualSizeSelected);
				var blocStateGround = actualLevel.getBlocState(gridWidthSelected, gridHeightSelected, actualSizeSelected-1);
				if(blocStateStage == BLOCSTATE.EMPTY && blocStateGround == BLOCSTATE.CUBE)
				{
					cameraEnValid.SetActive(true);	
					cameraEnInvalid.SetActive(false);
				}else{
					cameraEnInvalid.SetActive(true);
					cameraEnValid.SetActive(false);
				}
				cameraRotationCursor.SetActive(true);
			}
			
		}else{
			cameraEnInvalid.SetActive(false);
			cameraEnValid.SetActive(false);
		}
		
		
		if(Input.GetMouseButton(0))
		{
			if(cameraEnValid.activeSelf){
				var go = (GameObject)Instantiate(cameraEnModel, new Vector3(actualHeightSelected, actualSizeSelected*2f, actualWidthSelected), cameraEnValid.transform.rotation);
				actualLevel.setCameraEnemy(gridWidthSelected, gridHeightSelected, go);
				go.GetComponent<CameraEnemy>().rightFirst = cameraLook%2 == 0;
				go.GetComponent<CameraEnemy>().setLook(enemyLook);
				go.SetActive(true);
				cameraEnValid.SetActive(false);
				cameraRotationCursor.SetActive(false);
				actualMode = EDITMODE.CUBE;
				oldWidthSelected = -1;
				oldHeightSelected = -1;
			}
			
		}else if(Input.GetMouseButtonDown(1) && cameraEnValid.activeSelf){
		
			cameraLook++;
			if((int) cameraLook >= 8)
			{
				cameraLook = 0;
			}
			enemyLook = (LOOK)((int)(cameraLook/2));
			if(cameraLook%2 == 0)
			{
				cameraRotationCursor.transform.eulerAngles = new Vector3(0f, 45f, 0f);
				switch(enemyLook)
				{
				case LOOK.DOWN:
					cameraEnValid.transform.eulerAngles = new Vector3(0f, 0f, 0f);
					cameraEnInvalid.transform.eulerAngles = new Vector3(0f, 0f, 0f);
					break;
				case LOOK.UP:
					cameraEnValid.transform.eulerAngles = new Vector3(0f, 180f, 0f);
					cameraEnInvalid.transform.eulerAngles = new Vector3(0f, 180f, 0f);
					break;
				case LOOK.RIGHT:
					cameraEnValid.transform.eulerAngles = new Vector3(0f, -90f, 0f);
					cameraEnInvalid.transform.eulerAngles = new Vector3(0f, -90f, 0f);
					break;
				case LOOK.LEFT:
					cameraEnValid.transform.eulerAngles = new Vector3(0f, 90f, 0f);
					cameraEnInvalid.transform.eulerAngles = new Vector3(0f, 90f, 0f);
					break;
				}
			}else{
				cameraRotationCursor.transform.eulerAngles = new Vector3(0f, -45f, 0f);
			}
			oldWidthSelected = -1;
			oldHeightSelected = -1;
			
		}
		
		UpdateCamera();
	}
	#endregion
	
	#region EnemySpawn
	void UpdateEnemy()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		
		if(Physics.Raycast(ray, out hit)){
			//Detection de la position curseur
			UpdatePositionMouse(hit);
			
			//Verification de la nouvelle position
			if(actualWidthSelected != oldWidthSelected || actualHeightSelected != oldHeightSelected)
			{
				oldWidthSelected = actualWidthSelected;
				oldHeightSelected = actualHeightSelected;
				gridWidthSelected = (int)actualWidthSelected/2;
				gridHeightSelected = (int)actualHeightSelected/2;
			
				//Actualisation
				enemyCursor.transform.position = new Vector3(actualHeightSelected, 0f, actualWidthSelected);
				
				//Activation des curseur
				var blocStateStage = actualLevel.getBlocState(gridWidthSelected, gridHeightSelected, actualSizeSelected);
				var blocStateGround = actualLevel.getBlocState(gridWidthSelected, gridHeightSelected, 0);
				if((blocStateStage == BLOCSTATE.EMPTY || blocStateStage == BLOCSTATE.ENEMYSTART) && blocStateGround == BLOCSTATE.CUBE)
				{
					if(blocStateStage == BLOCSTATE.ENEMYSTART)
					{
						enemyHover.SetActive(true);
						enemyValid.SetActive(false);
					}else{
						enemyHover.SetActive(false);
						enemyValid.SetActive(true);	
					}
					
					enemyInvalid.SetActive(false);
				}else{
					enemyInvalid.SetActive(true);
					enemyValid.SetActive(false);
					enemyHover.SetActive(false);
				}
			}
			
		}else{
			enemyInvalid.SetActive(false);
			enemyValid.SetActive(false);
		}
		
		
		if(Input.GetMouseButton(0))
		{
			if(enemyValid.activeSelf){
				var go = (GameObject)Instantiate(enemyModel, new Vector3(actualHeightSelected, actualSizeSelected*2, actualWidthSelected), enemyModel.transform.rotation);
				actualLevel.setStartEnemyPosition(gridWidthSelected, gridHeightSelected, go);
				go.SetActive(true);
				go.transform.GetComponent<Enemy>().init (enemyLook, new Vector2((float)gridWidthSelected, (float)gridHeightSelected));
				listUIEnemy.Add(go.transform.GetComponent<Enemy>(), new List<GameObject>());
				enemyValid.SetActive(false);
				actualMode = EDITMODE.CUBE;
				oldWidthSelected = -1;
				oldHeightSelected = -1;
			}else if(enemyHover.activeSelf)
			{
				enemyHover.SetActive(false);
				actualEnemyEdit = actualLevel.getGameObjectAt(gridWidthSelected, gridHeightSelected, 1).transform.GetComponent<Enemy>();
				lastEnemyEditPosition = actualEnemyEdit.getLastPosition();
				refreshPossiblePositionAction();
				panelUIEdit.SetActive(true);
				actualMode = EDITMODE.EDITENEMY;
				enableEnemyWay(actualEnemyEdit);
				oldWidthSelected = -1;
				oldHeightSelected = -1;
			}
			
		}else if(Input.GetMouseButtonDown(1) && enemyValid.activeSelf){
		
			enemyLook++;
			if((int) enemyLook >= 4)
			{
				enemyLook = (LOOK) 0;
			}
			switch(enemyLook)
			{
			case LOOK.DOWN:
				enemyValid.transform.eulerAngles = new Vector3(0f, 90f, 0f);
				enemyInvalid.transform.eulerAngles = new Vector3(0f, 90f, 0f);
				enemyHover.transform.eulerAngles = new Vector3(0f, 90f, 0f);
				break;
			case LOOK.UP:
				enemyValid.transform.eulerAngles = new Vector3(0f, -90f, 0f);
				enemyInvalid.transform.eulerAngles = new Vector3(0f, -90f, 0f);
				enemyHover.transform.eulerAngles = new Vector3(0f, -90f, 0f);
				break;
			case LOOK.RIGHT:
				enemyValid.transform.eulerAngles = new Vector3(0f, 0f, 0f);
				enemyInvalid.transform.eulerAngles = new Vector3(0f, 0f, 0f);
				enemyHover.transform.eulerAngles = new Vector3(0f, 0f, 0f);
				break;
			case LOOK.LEFT:
				enemyValid.transform.eulerAngles = new Vector3(0f, 180f, 0f);
				enemyInvalid.transform.eulerAngles = new Vector3(0f, 180f, 0f);
				enemyHover.transform.eulerAngles = new Vector3(0f, 180f, 0f);
				break;
			}
			oldWidthSelected = -1;
			oldHeightSelected = -1;
			
		}
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			 quitSpawnEnemy();	
		}
		
		UpdateCamera();
	}
	#endregion
	
	#region EnemyEdit
	void UpdateEnemyEdit()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		
		if(Physics.Raycast(ray, out hit)){
			//Detection de la position curseur
			UpdatePositionMouse(hit);
			
			//Verification de la nouvelle position
			if(actualWidthSelected != oldWidthSelected || actualHeightSelected != oldHeightSelected)
			{
				oldWidthSelected = actualWidthSelected;
				oldHeightSelected = actualHeightSelected;
				gridWidthSelected = (int)actualWidthSelected/2;
				gridHeightSelected = (int)actualHeightSelected/2;
			
				//Actualisation
				cursorEdit.transform.position = new Vector3(actualHeightSelected, 2f, actualWidthSelected);
				
				//Activation des curseur
				var blocStateStage = actualLevel.getBlocState(gridWidthSelected, gridHeightSelected, actualSizeSelected);
				var blocStateGround = actualLevel.getBlocState(gridWidthSelected, gridHeightSelected, 0);
				if(blocStateStage != BLOCSTATE.CUBE && blocStateGround == BLOCSTATE.CUBE)
				{
					if(gridWidthSelected == (int)lastEnemyEditPosition.x && gridHeightSelected == (int)lastEnemyEditPosition.y)
					{
						cursorWayValid.SetActive(false);
						cursorWayAction.SetActive(true);
					}else if(isPositionEditValid()){
						cursorWayValid.SetActive(true);
						cursorWayAction.SetActive(false);
					}else{
						cursorWayValid.SetActive(false);
						cursorWayAction.SetActive(false);
					}
				}else{
					cursorWayValid.SetActive(false);
					cursorWayAction.SetActive(false);
				}
			}
			
		}else{
			cursorWayValid.SetActive(false);
			cursorWayAction.SetActive(false);
		}
		
		
		if(Input.GetMouseButtonDown(0) && cursorWayValid.activeSelf)
		{
			computeAction();
			actualNumberAction = actualEnemyEdit.getNumberOfAction();
			placeUIWay();
			lastEnemyEditPosition = actualEnemyEdit.getLastPosition();
			refreshPossiblePositionAction();
			oldWidthSelected = -1;
			oldHeightSelected = -1;
		}
		if(Input.GetMouseButtonDown(0) && cursorWayAction.activeSelf)
		{
			
			var theValueParsed = panelUIEdit.transform.FindChild("Input").GetComponent<UIInput>().text;
			double result = -1;
			if(double.TryParse(theValueParsed, out result))
			{
 				if(result >= 0f)
				{
					panelUIEdit.transform.FindChild("Input").GetComponent<UIInput>().activeColor = new Color(0.18f, 0.588f, 1f, 1f);
					if((int)actualEnemyEdit.getLastAction() <= 3) //Deplacement
					{
						placeUIAction();
					}
					actualEnemyEdit.addAction((MOVE) actualActionSelected, (float) result);
					actualActionSelected = (int)MOVE.WAIT;
					refreshText(new EnemyAction((MOVE)actualActionSelected, 3f));
				}else{
					panelUIEdit.transform.FindChild("Input").GetComponent<UIInput>().activeColor = new Color(1f, 0.2f, 0.2f, 1f);
				}
				oldWidthSelected = -1;
				oldHeightSelected = -1;
			}
			
		}else
		if(Input.GetMouseButtonDown(1) && listUIEnemy[actualEnemyEdit].Any())
		{
			var isDeplacement = (int)actualEnemyEdit.getLastAction() <= 3;
			if(!isDeplacement)
			{
				refreshText(actualEnemyEdit.getLastActionComplete());
			}
			actualEnemyEdit.removeLastAction();
			if(isDeplacement || (int)actualEnemyEdit.getLastAction() <= 3)
			{
				Destroy(listUIEnemy[actualEnemyEdit].Last());
				listUIEnemy[actualEnemyEdit].RemoveAt(listUIEnemy[actualEnemyEdit].Count - 1);
			}
			
			lastEnemyEditPosition = actualEnemyEdit.getLastPosition();
			refreshPossiblePositionAction();
			oldWidthSelected = -1;
			oldHeightSelected = -1;
		}
		
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			quitEditEnemy();
		}
		
		UpdateCamera();
	}
	#endregion
	
	#region EnemyHelicoSpawn
	void UpdateHelico()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		
		if(Physics.Raycast(ray, out hit)){
			//Detection de la position curseur
			UpdatePositionMouse(hit);
			
			//Verification de la nouvelle position
			if(actualWidthSelected != oldWidthSelected || actualHeightSelected != oldHeightSelected)
			{
				oldWidthSelected = actualWidthSelected;
				oldHeightSelected = actualHeightSelected;
				gridWidthSelected = (int)actualWidthSelected/2;
				gridHeightSelected = (int)actualHeightSelected/2;
			
				//Actualisation
				enemyHelicoCursor.transform.position = new Vector3(actualHeightSelected, 6f, actualWidthSelected);
				
				//Activation des curseur
				var blocStateStage = actualLevel.getBlocState(gridWidthSelected, gridHeightSelected, actualSizeSelected);
				if((blocStateStage == BLOCSTATE.EMPTY || blocStateStage == BLOCSTATE.HELICOSTART))
				{
					if(blocStateStage == BLOCSTATE.HELICOSTART)
					{
						enemyHelicoHover.SetActive(true);
						enemyHelicoValid.SetActive(false);
					}else{
						enemyHelicoHover.SetActive(false);
						enemyHelicoValid.SetActive(true);	
					}
					
					enemyHelicoInvalid.SetActive(false);
				}else{
					enemyHelicoInvalid.SetActive(true);
					enemyHelicoValid.SetActive(false);
					enemyHelicoHover.SetActive(false);
				}
			}
			
		}else{
			enemyHelicoInvalid.SetActive(false);
			enemyHelicoValid.SetActive(false);
		}
		
		
		if(Input.GetMouseButton(0))
		{
			if(enemyHelicoValid.activeSelf){
				var go = (GameObject)Instantiate(enemyHelicoModel, new Vector3(actualHeightSelected, actualSizeSelected*2, actualWidthSelected), enemyHelicoModel.transform.rotation);
				actualLevel.setStartEnemyHelicoPosition(gridWidthSelected, gridHeightSelected, go);
				go.SetActive(true);
				go.transform.GetComponent<Enemy>().init (enemyLook, new Vector2((float)gridWidthSelected, (float)gridHeightSelected), true, true);
				listUIEnemy.Add(go.transform.GetComponent<Enemy>(), new List<GameObject>());
				enemyHelicoValid.SetActive(false);
				actualMode = EDITMODE.CUBE;
				oldWidthSelected = -1;
				oldHeightSelected = -1;
			}else if(enemyHelicoHover.activeSelf)
			{
				enemyHelicoHover.SetActive(false);
				actualEnemyEdit = actualLevel.getGameObjectAt(gridWidthSelected, gridHeightSelected, 3).transform.GetComponent<Enemy>();
				lastEnemyEditPosition = actualEnemyEdit.getLastPosition();
				refreshPossiblePositionActionHelico();
				panelUIEdit.SetActive(true);
				actualMode = EDITMODE.EDITHELICO;
				enableEnemyWay(actualEnemyEdit);
				oldWidthSelected = -1;
				oldHeightSelected = -1;
			}
			
		}else if(Input.GetMouseButtonDown(1) && enemyHelicoValid.activeSelf){
		
			enemyLook++;
			if((int) enemyLook >= 4)
			{
				enemyLook = (LOOK) 0;
			}
			switch(enemyLook)
			{
			case LOOK.DOWN:
				enemyHelicoValid.transform.eulerAngles = new Vector3(0f, 90f, 0f);
				enemyHelicoInvalid.transform.eulerAngles = new Vector3(0f, 90f, 0f);
				enemyHelicoHover.transform.eulerAngles = new Vector3(0f, 90f, 0f);
				break;
			case LOOK.UP:
				enemyHelicoValid.transform.eulerAngles = new Vector3(0f, -90f, 0f);
				enemyHelicoInvalid.transform.eulerAngles = new Vector3(0f, -90f, 0f);
				enemyHelicoHover.transform.eulerAngles = new Vector3(0f, -90f, 0f);
				break;
			case LOOK.RIGHT:
				enemyHelicoValid.transform.eulerAngles = new Vector3(0f, 0f, 0f);
				enemyHelicoInvalid.transform.eulerAngles = new Vector3(0f, 0f, 0f);
				enemyHelicoHover.transform.eulerAngles = new Vector3(0f, 0f, 0f);
				break;
			case LOOK.LEFT:
				enemyHelicoValid.transform.eulerAngles = new Vector3(0f, 180f, 0f);
				enemyHelicoInvalid.transform.eulerAngles = new Vector3(0f, 180f, 0f);
				enemyHelicoHover.transform.eulerAngles = new Vector3(0f, 180f, 0f);
				break;
			}
			oldWidthSelected = -1;
			oldHeightSelected = -1;
			
		}
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			 quitSpawnEnemy();	
		}
		
		UpdateCamera();
	}
	#endregion
	
	#region EnemyHelicoEdit
	void UpdateHelicoEdit()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		
		if(Physics.Raycast(ray, out hit)){
			//Detection de la position curseur
			UpdatePositionMouse(hit);
			
			//Verification de la nouvelle position
			if(actualWidthSelected != oldWidthSelected || actualHeightSelected != oldHeightSelected)
			{
				oldWidthSelected = actualWidthSelected;
				oldHeightSelected = actualHeightSelected;
				gridWidthSelected = (int)actualWidthSelected/2;
				gridHeightSelected = (int)actualHeightSelected/2;
			
				//Actualisation
				cursorEdit.transform.position = new Vector3(actualHeightSelected, 6f, actualWidthSelected);
				
				//Activation des curseur
				var blocStateStage = actualLevel.getBlocState(gridWidthSelected, gridHeightSelected, actualSizeSelected);
				if(blocStateStage != BLOCSTATE.CUBE)
				{
					if(gridWidthSelected == (int)lastEnemyEditPosition.x && gridHeightSelected == (int)lastEnemyEditPosition.y)
					{
						cursorWayValid.SetActive(false);
						cursorWayAction.SetActive(true);
					}else if(isPositionEditValid()){
						cursorWayValid.SetActive(true);
						cursorWayAction.SetActive(false);
					}else{
						cursorWayValid.SetActive(false);
						cursorWayAction.SetActive(false);
					}
				}else{
					cursorWayValid.SetActive(false);
					cursorWayAction.SetActive(false);
				}
			}
			
		}else{
			cursorWayValid.SetActive(false);
			cursorWayAction.SetActive(false);
		}
		
		
		if(Input.GetMouseButtonDown(0) && cursorWayValid.activeSelf)
		{
			computeAction();
			actualNumberAction = actualEnemyEdit.getNumberOfAction();
			placeUIWay();
			lastEnemyEditPosition = actualEnemyEdit.getLastPosition();
			refreshPossiblePositionActionHelico();
			oldWidthSelected = -1;
			oldHeightSelected = -1;
		}
		if(Input.GetMouseButtonDown(0) && cursorWayAction.activeSelf)
		{
			
			var theValueParsed = panelUIEdit.transform.FindChild("Input").GetComponent<UIInput>().text;
			double result = -1;
			if(double.TryParse(theValueParsed, out result))
			{
 				if(result >= 0f)
				{
					panelUIEdit.transform.FindChild("Input").GetComponent<UIInput>().activeColor = new Color(0.18f, 0.588f, 1f, 1f);
					if((int)actualEnemyEdit.getLastAction() <= 3) //Deplacement
					{
						placeUIAction();
					}
					actualEnemyEdit.addAction((MOVE) actualActionSelected, (float) result);
					actualActionSelected = (int)MOVE.WAIT;
					refreshText(new EnemyAction((MOVE)actualActionSelected, 3f));
				}else{
					panelUIEdit.transform.FindChild("Input").GetComponent<UIInput>().activeColor = new Color(1f, 0.2f, 0.2f, 1f);
				}
				oldWidthSelected = -1;
				oldHeightSelected = -1;
			}
			
		}else
		if(Input.GetMouseButtonDown(1) && listUIEnemy[actualEnemyEdit].Any())
		{
			var isDeplacement = (int)actualEnemyEdit.getLastAction() <= 3;
			if(!isDeplacement)
			{
				refreshText(actualEnemyEdit.getLastActionComplete());
			}
			actualEnemyEdit.removeLastAction();
			if(isDeplacement || (int)actualEnemyEdit.getLastAction() <= 3)
			{
				Destroy(listUIEnemy[actualEnemyEdit].Last());
				listUIEnemy[actualEnemyEdit].RemoveAt(listUIEnemy[actualEnemyEdit].Count - 1);
			}
			
			lastEnemyEditPosition = actualEnemyEdit.getLastPosition();
			refreshPossiblePositionActionHelico();
			oldWidthSelected = -1;
			oldHeightSelected = -1;
		}
		
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			quitEditEnemy();
		}
		
		UpdateCamera();
	}
	#endregion
	
	#region SaveLoad
	void UpdateSaveLoad()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			panelUILoad.SetActive(false);
			panelUISave.SetActive(false);
			actualMode = EDITMODE.CUBE;
			oldWidthSelected = -1;
			oldHeightSelected = -1;
		}
	}
	#endregion
	
	#region Position
	void UpdatePositionMouse(RaycastHit hit)
	{
		if(hit.point.z%2 > 1)
		{
			actualWidthSelected = (int) ((hit.point.z + (2 - (hit.point.z%2))) + 0.5f);
		}else{
			actualWidthSelected = (int) ((hit.point.z - (hit.point.z%2)) + 0.5f);	
		}
		
		if(hit.point.x%2 > 1)
		{
			actualHeightSelected = (int) ((hit.point.x + (2 - (hit.point.x%2))) + 0.5f);	
		}else{
			actualHeightSelected = (int) ((hit.point.x - (hit.point.x%2)) + 0.5f);
		}
		
	}
	#endregion
	
	#region Camera
	void UpdateCamera()
	{
		if(EnableCameraEditor)
		{
			UpdateCameraInEditor();
		}
	}
	
	void UpdateCameraInEditor()
	{
		if(Input.GetKey(KeyCode.LeftArrow))
		{
			Camera.main.transform.Translate(-speedCameraMove*Time.deltaTime, 0f, 0f);
		}
		if(Input.GetKey(KeyCode.RightArrow))
		{
			Camera.main.transform.Translate(speedCameraMove*Time.deltaTime, 0f, 0f);
		}
		if(Input.GetKey(KeyCode.UpArrow))
		{
			Camera.main.transform.Translate(0f, speedCameraMove*Time.deltaTime, 0f);
		}
		if(Input.GetKey(KeyCode.DownArrow))
		{
			Camera.main.transform.Translate(0f, -speedCameraMove*Time.deltaTime, 0f);
		}
	}
	#endregion
	
	
	#region TriggerMode
	public void goOnSpawnPlayerMode()
	{
		//Desactivation du curseur
		if(placementCube.activeSelf)
		{
			placementCube.SetActive(false);
		}else
		if(deleteCube.activeSelf)
		{
			deleteCube.SetActive(false);	
		}
		if(previousBlocSelected != null)
		{
			previousBlocSelected.renderer.enabled = appearCubeCondition();
			previousBlocSelected = null;
		}
		oldWidthSelected = -1;
		oldHeightSelected = -1;
		//Remove old position
		actualLevel.removePlayerPosition();
		actualMode = EDITMODE.STARTPOSITION;
		rayCastCollider.transform.position = new Vector3(39f, 2f, 39f);
		actualSizeSelected = 1;
		actualLevel.setDisplayThisLevel(true, maxWidth, maxHeight, actualSizeSelected);
		if(isInHidingMode)
		{
			actualLevel.setDisplayUpperBlocs(false, maxWidth, maxHeight, maxVolume, actualSizeSelected);
		}
		oldWidthSelected = -1;
		oldHeightSelected = -1;
	}
	
	public void goOnFinishPlayerMode()
	{
		//Desactivation du curseur
		if(placementCube.activeSelf)
		{
			placementCube.SetActive(false);
		}else
		if(deleteCube.activeSelf)
		{
			deleteCube.SetActive(false);	
		}
		if(previousBlocSelected != null)
		{
			previousBlocSelected.renderer.enabled = appearCubeCondition();
			previousBlocSelected = null;
		}
		oldWidthSelected = -1;
		oldHeightSelected = -1;
		//Remove old position
		actualLevel.removeFinishPosition();
		actualMode = EDITMODE.FINISHPOSITION;
		rayCastCollider.transform.position = new Vector3(39f, 2f, 39f);
		actualSizeSelected = 1;
		actualLevel.setDisplayThisLevel(true, maxWidth, maxHeight, actualSizeSelected);
		if(isInHidingMode)
		{
			actualLevel.setDisplayUpperBlocs(false, maxWidth, maxHeight, maxVolume, actualSizeSelected);
		}
		oldWidthSelected = -1;
		oldHeightSelected = -1;
	}
	
	public void goOnCanonMode()
	{
		//Desactivation du curseur
		if(placementCube.activeSelf)
		{
			placementCube.SetActive(false);
		}else
		if(deleteCube.activeSelf)
		{
			deleteCube.SetActive(false);	
		}
		if(previousBlocSelected != null)
		{
			previousBlocSelected.renderer.enabled = appearCubeCondition();
			previousBlocSelected = null;
		}
		oldWidthSelected = -1;
		oldHeightSelected = -1;
		//Remove old position
		enemyLook = LOOK.DOWN;
		actualMode = EDITMODE.PLACECANON;
		rayCastCollider.transform.position = new Vector3(39f, 2f, 39f);
		actualSizeSelected = 1;
		actualLevel.setDisplayThisLevel(true, maxWidth, maxHeight, actualSizeSelected);
		if(isInHidingMode)
		{
			actualLevel.setDisplayUpperBlocs(false, maxWidth, maxHeight, maxVolume, actualSizeSelected);
		}
		oldWidthSelected = -1;
		oldHeightSelected = -1;
	}
	
	public void goOnCameraMode()
	{
		//Desactivation du curseur
		if(placementCube.activeSelf)
		{
			placementCube.SetActive(false);
		}else
		if(deleteCube.activeSelf)
		{
			deleteCube.SetActive(false);	
		}
		if(previousBlocSelected != null)
		{
			previousBlocSelected.renderer.enabled = appearCubeCondition();
			previousBlocSelected = null;
		}
		oldWidthSelected = -1;
		oldHeightSelected = -1;
		//Remove old position
		cameraLook = (int)(LOOK.RIGHT)*2;
		actualMode = EDITMODE.PLACECAMERA;
		rayCastCollider.transform.position = new Vector3(39f, 4f, 39f);
		actualSizeSelected = 2;
		actualLevel.setDisplayThisLevel(true, maxWidth, maxHeight, actualSizeSelected);
		if(isInHidingMode)
		{
			actualLevel.setDisplayUpperBlocs(false, maxWidth, maxHeight, maxVolume, actualSizeSelected);
		}
		oldWidthSelected = -1;
		oldHeightSelected = -1;
	}
	
	
	public void goOnSpawnEnemy()
	{
		//Desactivation du curseur
		if(placementCube.activeSelf)
		{
			placementCube.SetActive(false);
		}else
		if(deleteCube.activeSelf)
		{
			deleteCube.SetActive(false);	
		}
		if(previousBlocSelected != null)
		{
			previousBlocSelected.renderer.enabled = appearCubeCondition();
			previousBlocSelected = null;
		}
		oldWidthSelected = -1;
		oldHeightSelected = -1;
		
		enemyLook = LOOK.RIGHT;
		actualMode = EDITMODE.ENEMY;
		rayCastCollider.transform.position = new Vector3(39f, 2f, 39f);
		actualSizeSelected = 1;
		actualLevel.setDisplayThisLevel(true, maxWidth, maxHeight, actualSizeSelected);
		if(isInHidingMode)
		{
			actualLevel.setDisplayUpperBlocs(false, maxWidth, maxHeight, maxVolume, actualSizeSelected);
		}
		oldWidthSelected = -1;
		oldHeightSelected = -1;
	}
	
	public void goOnSpawnHelico()
	{
		//Desactivation du curseur
		if(placementCube.activeSelf)
		{
			placementCube.SetActive(false);
		}else
		if(deleteCube.activeSelf)
		{
			deleteCube.SetActive(false);	
		}
		if(previousBlocSelected != null)
		{
			previousBlocSelected.renderer.enabled = appearCubeCondition();
			previousBlocSelected = null;
		}
		oldWidthSelected = -1;
		oldHeightSelected = -1;
		
		enemyLook = LOOK.RIGHT;
		actualMode = EDITMODE.HELICO;
		rayCastCollider.transform.position = new Vector3(39f, 6f, 39f);
		actualSizeSelected = 3;
		actualLevel.setDisplayThisLevel(true, maxWidth, maxHeight, actualSizeSelected);
		if(isInHidingMode)
		{
			actualLevel.setDisplayUpperBlocs(false, maxWidth, maxHeight, maxVolume, actualSizeSelected);
		}
		oldWidthSelected = -1;
		oldHeightSelected = -1;
	}
	
	public void goOnLoadLevel()
	{
		if(placementCube.activeSelf)
		{
			placementCube.SetActive(false);
		}else
		if(deleteCube.activeSelf)
		{
			deleteCube.SetActive(false);	
		}
		if(previousBlocSelected != null)
		{
			previousBlocSelected.renderer.enabled = appearCubeCondition();
			previousBlocSelected = null;
		}
		oldWidthSelected = -1;
		oldHeightSelected = -1;
		actualMode = EDITMODE.LOADLEVEL;
		
		foreach(var file in Directory.GetFiles(Application.dataPath + "/Levels/"))
		{
			if(file.Contains(".lvl") && !file.Contains(".meta"))
			{
				listLevelSaved.Add(file.Replace('\\', '/').Split('/').Last());
			}
		}
		actualFileSelected = 0;
		refreshTextLoad();
		
		panelUILoad.SetActive(true);
	}
	
	public void goOnSaveLevel()
	{
		if(placementCube.activeSelf)
		{
			placementCube.SetActive(false);
		}else
		if(deleteCube.activeSelf)
		{
			deleteCube.SetActive(false);	
		}
		if(previousBlocSelected != null)
		{
			previousBlocSelected.renderer.enabled = appearCubeCondition();
			previousBlocSelected = null;
		}
		oldWidthSelected = -1;
		oldHeightSelected = -1;
		actualMode = EDITMODE.SAVELEVEL;
		
		panelUISave.SetActive(true);
	}
	
	public void quitSpawnEnemy()
	{
		enemyValid.SetActive(false);
		enemyHover.SetActive(false);
		enemyInvalid.SetActive(false);
		enemyHelicoValid.SetActive(false);
		enemyHelicoHover.SetActive(false);
		enemyHelicoInvalid.SetActive(false);
		actualMode = EDITMODE.CUBE;
		oldWidthSelected = -1;
		oldHeightSelected = -1;
	}
	
	public void quitEditEnemy()
	{
		disableEnemyWay(actualEnemyEdit);	
	 	cursorWayValid.SetActive(false);
		cursorWayAction.SetActive(false);
		possibleActionObject.SetActive(false);
		panelUIEdit.SetActive(false);
		actualMode = EDITMODE.CUBE;
		oldWidthSelected = -1;
		oldHeightSelected = -1;
	}
	#endregion
	
	#region EDITCUBE
	bool appearCubeCondition()
	{
		return !isInHidingMode || (oldSizeSelected <= actualSizeSelected);
	}
	
	public void hideUpperLevels()
	{
		actualLevel.setDisplayUpperBlocs(false, maxWidth, maxHeight, maxVolume, actualSizeSelected);
		isInHidingMode = true;
	}
	
	public void showUpperLevels()
	{
		actualLevel.setDisplayUpperBlocs(true, maxWidth, maxHeight, maxVolume, actualSizeSelected);
		isInHidingMode = false;
	}
	#endregion
	
	#region EDITENEMY
	public void refreshPossiblePositionAction()
	{
		possibleActionObject.SetActive(true);
		var lastPosition = lastEnemyEditPosition;
		var left = possibleActionObject.transform.FindChild("LeftPossibleActionObject").gameObject;
		var right = possibleActionObject.transform.FindChild("RightPossibleActionObject").gameObject;
		var down = possibleActionObject.transform.FindChild("DownPossibleActionObject").gameObject;
		var up = possibleActionObject.transform.FindChild("UpPossibleActionObject").gameObject;
		var x = (int)lastPosition.x;
		var y = (int)lastPosition.y;
		
		left.SetActive(true);
		right.SetActive(true);
		down.SetActive(true);
		up.SetActive(true);
		possibleActionObject.transform.position = new Vector3(y*2, 2f, x*2);
		
		if(x + 1 < maxWidth && (actualLevel.getBlocState(x+1,y,1) == BLOCSTATE.CUBE || actualLevel.getBlocState(x+1, y, 0) == BLOCSTATE.EMPTY))
		{
			left.SetActive(false);	
		}
		if(x - 1 > 0 && (actualLevel.getBlocState(x-1,y,1) == BLOCSTATE.CUBE || actualLevel.getBlocState(x-1, y, 0) == BLOCSTATE.EMPTY))
		{
			right.SetActive(false);	
		}
		if(y - 1 > 0 && (actualLevel.getBlocState(x,y-1,1) == BLOCSTATE.CUBE || actualLevel.getBlocState(x, y-1, 0) == BLOCSTATE.EMPTY))
		{
			down.SetActive(false);	
		}
		if(y + 1 < maxHeight && (actualLevel.getBlocState(x,y+1,1) == BLOCSTATE.CUBE || actualLevel.getBlocState(x, y+1, 0) == BLOCSTATE.EMPTY))
		{
			up.SetActive(false);	
		}
	}
	
	
	public void refreshPossiblePositionActionHelico()
	{
		possibleActionObject.SetActive(true);
		var lastPosition = lastEnemyEditPosition;
		var left = possibleActionObject.transform.FindChild("LeftPossibleActionObject").gameObject;
		var right = possibleActionObject.transform.FindChild("RightPossibleActionObject").gameObject;
		var down = possibleActionObject.transform.FindChild("DownPossibleActionObject").gameObject;
		var up = possibleActionObject.transform.FindChild("UpPossibleActionObject").gameObject;
		var x = (int)lastPosition.x;
		var y = (int)lastPosition.y;
		
		left.SetActive(true);
		right.SetActive(true);
		down.SetActive(true);
		up.SetActive(true);
		possibleActionObject.transform.position = new Vector3(y*2, 6f, x*2);
		
		if(x + 1 < maxWidth && (actualLevel.getBlocState(x+1,y,3) == BLOCSTATE.CUBE))
		{
			left.SetActive(false);	
		}
		if(x - 1 > 0 && (actualLevel.getBlocState(x-1,y,3) == BLOCSTATE.CUBE))
		{
			right.SetActive(false);	
		}
		if(y - 1 > 0 && (actualLevel.getBlocState(x,y-1,3) == BLOCSTATE.CUBE))
		{
			down.SetActive(false);	
		}
		if(y + 1 < maxHeight && (actualLevel.getBlocState(x,y+1,3) == BLOCSTATE.CUBE))
		{
			up.SetActive(false);	
		}
	}
	
	
					
	public bool isPositionEditValid()
	{
		var lastPosition = lastEnemyEditPosition;
		var left = possibleActionObject.transform.FindChild("LeftPossibleActionObject").gameObject;
		var right = possibleActionObject.transform.FindChild("RightPossibleActionObject").gameObject;
		var down = possibleActionObject.transform.FindChild("DownPossibleActionObject").gameObject;
		var up = possibleActionObject.transform.FindChild("UpPossibleActionObject").gameObject;
		var x = (int)lastPosition.x;
		var y = (int)lastPosition.y;

		if(gridWidthSelected == (x - 1) && gridHeightSelected == y)
		{
			return right.activeSelf;	
		}else
		if(gridWidthSelected == (x + 1) && gridHeightSelected == y)
		{
			return left.activeSelf;	
		}else
		if(gridWidthSelected == x && gridHeightSelected == (y - 1))
		{
			return down.activeSelf;	
		}else
		if(gridWidthSelected == x && gridHeightSelected == (y + 1))
		{
			return up.activeSelf;		
		}
		
		return false;
	}
	
	public void computeAction()
	{
		var x = (int)lastEnemyEditPosition.x;
		var y = (int)lastEnemyEditPosition.y;
		if(gridWidthSelected < x && gridHeightSelected == y){
			actualEnemyEdit.addAction(MOVE.RIGHT, 0f);
		}else if(gridWidthSelected > x && gridHeightSelected == y){
			actualEnemyEdit.addAction(MOVE.LEFT, 0f);
		}else if(gridWidthSelected == x && gridHeightSelected < y){
			actualEnemyEdit.addAction(MOVE.DOWN, 0f);	
		}else if(gridWidthSelected == x && gridHeightSelected > y){
			actualEnemyEdit.addAction(MOVE.UP, 0f);
		}	
	}
	
	public void placeUIWay()
	{
		var theTurn = actualEnemyEdit.isMakingATurn();
		var x = (int)lastEnemyEditPosition.x;
		var y = (int)lastEnemyEditPosition.y;
		if(gridWidthSelected > x && gridHeightSelected == y){
			addGameObjectToList(lineObject, -90f);	
		}else if(gridWidthSelected < x && gridHeightSelected == y){
			addGameObjectToList(lineObject, 90f);	
		}else if(gridWidthSelected == x && gridHeightSelected > y){
			addGameObjectToList(lineObject, 0f);	
		}else if(gridWidthSelected == x && gridHeightSelected < y){
			addGameObjectToList(lineObject, 180f);	
		}
		
		if(theTurn != TURN.NONE)
		{
			replaceTurnObject(theTurn);	
		}
	}
	
	public void placeUIWay(MOVE action, bool helico)
	{
		var theTurn = actualEnemyEdit.isMakingATurn();
		if(action == MOVE.LEFT){
			addGameObjectToList(lineObject, -90f, actualEnemyEdit.getLastPosition(), helico);	
		}else if(action == MOVE.RIGHT){
			addGameObjectToList(lineObject, 90f, actualEnemyEdit.getLastPosition(), helico);	
		}else if(action == MOVE.UP){
			addGameObjectToList(lineObject, 0f, actualEnemyEdit.getLastPosition(), helico);	
		}else if(action == MOVE.DOWN){
			addGameObjectToList(lineObject, 180f, actualEnemyEdit.getLastPosition(), helico);	
		}
		
		if(theTurn != TURN.NONE)
		{
			replaceTurnObject(theTurn);	
		}
	}
	
	public void placeUIAction()
	{
		addGameObjectToList(actionObject, 0f);	
	}
	
	public void placeUIAction(Vector2 position, bool helico)
	{
		addGameObjectToList(actionObject, 0f, position, helico);	
	}
	
	
	public void replaceTurnObject(TURN t)
	{
		var positionToInsert = destroyLastGameObject();
		switch(t)
		{
		case TURN.DOWNTOLEFT:
			insertGameObjectToList(angleHObject, 90f, (int)positionToInsert.x, new Vector3(positionToInsert.y, positionToInsert.z, positionToInsert.w));
			break;
		case TURN.DOWNTORIGHT:
			insertGameObjectToList(angleAHObject, 0f, (int)positionToInsert.x, new Vector3(positionToInsert.y, positionToInsert.z, positionToInsert.w));
			break;
		case TURN.LEFTTODOWN:
			insertGameObjectToList(angleAHObject, 90f, (int)positionToInsert.x, new Vector3(positionToInsert.y, positionToInsert.z, positionToInsert.w));
			break;
		case TURN.LEFTTOUP:
			insertGameObjectToList(angleHObject, 180f, (int)positionToInsert.x, new Vector3(positionToInsert.y, positionToInsert.z, positionToInsert.w));
			break;
		case TURN.RIGHTTODOWN:
			insertGameObjectToList(angleHObject, 0f, (int)positionToInsert.x, new Vector3(positionToInsert.y, positionToInsert.z, positionToInsert.w));
			break;
		case TURN.RIGHTTOUP:
			insertGameObjectToList(angleAHObject, -90f, (int)positionToInsert.x, new Vector3(positionToInsert.y, positionToInsert.z, positionToInsert.w));
			break;
		case TURN.UPTOLEFT:
			insertGameObjectToList(angleAHObject, 180f, (int)positionToInsert.x, new Vector3(positionToInsert.y, positionToInsert.z, positionToInsert.w));
			break;
		case TURN.UPTORIGHT:
			insertGameObjectToList(angleHObject, -90f, (int)positionToInsert.x, new Vector3(positionToInsert.y, positionToInsert.z, positionToInsert.w));
			break;
		}
		
	}
	
	public void addGameObjectToList(GameObject model, float rotation) 
	{
		var go = (GameObject)Instantiate(model, cursorEdit.transform.position, cursorEdit.transform.rotation);
		go.transform.Rotate(0f, rotation, 0f);
		go.SetActive(true);
		if(model.name == "ActionObject") go.name = "Action";
		colorAllChild(go, actualNumberAction);
		listUIEnemy[actualEnemyEdit].Add(go);	
	}
	
	public void addGameObjectToList(GameObject model, float rotation, Vector2 position, bool helicoMode = false) 
	{
		var go = (GameObject)Instantiate(model, new Vector3(position.y*2f, helicoMode ? 6f : 2f, position.x*2f), cursorEdit.transform.rotation);
		go.transform.Rotate(0f, rotation, 0f);
		if(model.name == "ActionObject") go.name = "Action";
		colorAllChild(go, actualNumberAction);
		listUIEnemy[actualEnemyEdit].Add(go);	
	}
	
	public void insertGameObjectToList(GameObject model, float rotation, int positionToInsert, Vector3 oldPosition)
	{
		var go = (GameObject)Instantiate(model, oldPosition, cursorEdit.transform.rotation);
		go.transform.Rotate(0f, rotation, 0f);
		go.SetActive(actualMode == EDITMODE.EDITENEMY || actualMode == EDITMODE.EDITHELICO);
		colorAllChild(go, actualNumberAction - 1);
		listUIEnemy[actualEnemyEdit].Insert(positionToInsert, go);	
	}
	
	public Quaternion destroyLastGameObject()
	{
		var position = -1;
		var positionObject = new Vector3(0f, 0f, 0f);
		for(int i=listUIEnemy[actualEnemyEdit].Count - 2; i>=0; i--)
		{
			if(listUIEnemy[actualEnemyEdit].ElementAt(i).name != "Action")
			{
				position = i;
				positionObject = listUIEnemy[actualEnemyEdit].ElementAt(i).transform.position;
				Destroy(listUIEnemy[actualEnemyEdit].ElementAt(i));
				listUIEnemy[actualEnemyEdit].RemoveAt(i);
				i = -1;
			}
		}
		
		return new Quaternion((float)position, positionObject.x, positionObject.y, positionObject.z);
	}
	
	public void colorAllChild(GameObject go, int numberAction)
	{
		var theColor = colorMaterialWay(numberAction);
		for(int i=0; i<go.transform.childCount; i++)
		{
			if(go.transform.GetChild(i).gameObject.renderer != null)
			{
				go.transform.GetChild(i).gameObject.renderer.material.color = theColor;
			}else{
				go.transform.GetChild(i).light.color = theColor;	
			}
		}
	}
	
	public Color colorMaterialWay(int numberOfAction)
	{
		var c = new Color(1f, 1f, 1f, 1f);
		numberOfAction = numberOfAction%60;
		if(numberOfAction < 10)
		{
			c.r = 1f;
			c.g = 0f + 0.1f*(numberOfAction%10);
			c.b = 0f;
		}else if(numberOfAction < 20)
		{
			c.r = 1f - 0.1f*(numberOfAction%10);
			c.g = 1f;
			c.b = 0f;
		}else if(numberOfAction < 30)
		{
			c.r = 0f;
			c.g = 1f;
			c.b = 0f + 0.1f*(numberOfAction%10);
		}else if(numberOfAction < 40)
		{
			c.r = 0f;	
			c.g = 1f - + 0.1f*(numberOfAction%10);
			c.b = 1f;
		}else if(numberOfAction < 50)
		{
			c.r = 0f + 0.1f*(numberOfAction%10);
			c.g = 0f;
			c.b = 1f;
		}else if(numberOfAction < 60)
		{
			c.r = 1f;
			c.g = 0f;
			c.b = 1f - 0.1f*(numberOfAction%10);
		}
		
		return c;
	}
	
	public void removeEnemyObject(Enemy e)
	{
		
		for(int i=0; i<listUIEnemy[e].Count; i++)
		{
			Destroy (listUIEnemy[e].ElementAt(i));
		}
		listUIEnemy.Remove(e);
	}
	
	public void disableEnemyWay(Enemy e)
	{
		for(int i=0; i<listUIEnemy[e].Count; i++)
		{
			listUIEnemy[e].ElementAt(i).SetActive(false);
		}
	}
	
	public void enableEnemyWay(Enemy e)
	{
		for(int i=0; i<listUIEnemy[e].Count; i++)
		{
			listUIEnemy[e].ElementAt(i).SetActive(true);
		}
	}
	
	public string convertMoveToString(MOVE m)
	{
		switch(m)
		{
		case MOVE.WAIT:
			return "Wait";
		case MOVE.LOOKUP:
			return "Look Up";
		case MOVE.LOOKRIGHT:
			return "Look Right";
		case MOVE.LOOKLEFT:
			return "Look Left";
		case MOVE.LOOKDOWN:
			return "Look Down";
		}
		return "error";
	}
	
	public void refreshText(EnemyAction ea)
	{
		panelUIEdit.transform.FindChild("LabelAction").GetComponent<UILabel>().text = convertMoveToString(ea.state);
		panelUIEdit.transform.FindChild("Input").GetComponent<UIInput>().text =  ea.timeWait.ToString("0.0");
	}
	
	public void nextEnemeyAction()
	{
		actualActionSelected++;
		if(actualActionSelected > (int)MOVE.LOOKDOWN)
		{
			actualActionSelected = (int)MOVE.WAIT;	
		}
		refreshText(new EnemyAction((MOVE)actualActionSelected, 3f));
	}
	
	public void prevEnemeyAction()
	{
		actualActionSelected--;
		if(actualActionSelected < (int)MOVE.WAIT)
		{
			actualActionSelected = (int)MOVE.LOOKDOWN;	
		}
		refreshText(new EnemyAction((MOVE)actualActionSelected, 3f));
	}
	#endregion
	
	#region EDITSAVELOAD
	public void nextFile()
	{
		actualFileSelected++;
		if(actualFileSelected >= listLevelSaved.Count)
		{
			actualFileSelected = 0;	
		}
		refreshTextLoad();
	}
	
	public void prevFile()
	{
		actualFileSelected--;
		if(actualFileSelected < 0)
		{
			actualFileSelected = listLevelSaved.Count > 0 ? listLevelSaved.Count - 1 : 0;	
		}
		refreshTextLoad();
	}
	
	public void refreshTextLoad()
	{
		if(listLevelSaved.Any())
		{
			panelUILoad.transform.FindChild("LabelFile").GetComponent<UILabel>().text = listLevelSaved.ElementAt(actualFileSelected);
		}else{
			panelUILoad.transform.FindChild("LabelFile").GetComponent<UILabel>().text = "EMPTY";
		}
	}
	
	public void saveLevel()
	{
		var ID = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + 
			DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + 
				DateTime.Now.Millisecond.ToString();	
		
		var name = panelUISave.transform.FindChild("Input").GetComponent<UIInput>().text;
		
		var listEnemy = new List<Enemy>();
		for(int i=0; i<listUIEnemy.Count; i++)
		{
			listEnemy.Add(listUIEnemy.ElementAt(i).Key);	
		}
		
		
		var levelToSerialize = actualLevel.saveLevel(name, ID, listEnemy);
		
		if(File.Exists(Application.dataPath + "/Levels/" + name + ".lvl"))
		{
			if(File.Exists(Application.dataPath + "/Levels/" + name + ".bak"))
			{
				File.Delete(Application.dataPath + "/Levels/" + name + ".bak");
			}
			File.Move(Application.dataPath + "/Levels/" + name + ".lvl", Application.dataPath + "/Levels/" + name + ".bak");
		}
		
		Stream stream = File.Open(Application.dataPath + "/Levels/" + name + ".lvl", FileMode.Create);
		BinaryFormatter bformatter = new BinaryFormatter();
		bformatter.Binder = new VersionDeserializationBinder();
		
		try{
			bformatter.Serialize(stream, levelToSerialize);
			stream.Close();
		}catch(Exception e){
			stream.Close();
			Debug.Log(e.Message);
		}
		
		panelUISave.SetActive(false);
		actualMode = EDITMODE.CUBE;
		oldWidthSelected = -1;
		oldHeightSelected = -1;
	}
	
	public void loadLevel()
	{
		Stream stream = File.Open(Application.dataPath + "/Levels/" + panelUILoad.transform.FindChild("LabelFile").GetComponent<UILabel>().text, FileMode.Open);
		BinaryFormatter bformatter = new BinaryFormatter();
		bformatter.Binder = new VersionDeserializationBinder();
		Level l = (Level)bformatter.Deserialize(stream);
		stream.Close();
		
		
		//purge
		actualLevel.purge(maxWidth, maxHeight, maxVolume);
		for(int i=0; i<listUIEnemy.Count; i++)
		{
			for(int j=0; j<listUIEnemy.ElementAt(i).Value.Count; j++)
			{
				Destroy(listUIEnemy.ElementAt(i).Value.ElementAt(j));
			}
		}
		listUIEnemy.Clear();
		
		//load
		actualLevel.loadLevel(l);
		
		//Recreate the world
		var theLevelState = actualLevel.getEntireBlocState();
		for(int i=0; i<maxWidth; i++)
		{
			for(int j=0; j<maxHeight; j++)
			{
				for(int h=0; h<maxVolume; h++)
				{
					if(theLevelState[i,j,h] == BLOCSTATE.CUBE)
					{
						var go = (GameObject)Instantiate(h == 0 ? cubeBase : cubeBloc, new Vector3(j*2, h*2, i*2), cubeBase.transform.rotation);
						actualLevel.setCube(i, j, h, go);
						go.SetActive(true);
					}
				}
			}
		}
		actualLevel.setStartPlayerPosition((int)l.getPlayerSpawn().x, (int)l.getPlayerSpawn().y);
		actualLevel.setFinishPlayerPosition((int)l.getPlayerFinish().x, (int)l.getPlayerFinish().y);
		playerSpawnValid.SetActive(true);
		playerSpawn.transform.position = new Vector3(l.getPlayerSpawn().y*2f, 2f, l.getPlayerSpawn().x*2f);
		playerFinishValid.SetActive(true);
		playerFinish.transform.position = new Vector3(l.getPlayerFinish().y*2f, 2f, l.getPlayerFinish().x*2f);		
		
		//Recréation de la liste d'ennemy
		listUIEnemy = new Dictionary<Enemy, List<GameObject>>();
		for(int i=0; i<l.getEnemies().Count; i++)
		{
			var enemy = l.getEnemies().ElementAt(i);
			var go = (GameObject)Instantiate(enemy.isHelicopter ? enemyHelicoModel : enemyModel, new Vector3(enemy.startY*2, enemy.isHelicopter ? 6f : 2f, enemy.startX*2), enemyModel.transform.rotation);
			if(enemy.isHelicopter){
				actualLevel.setStartEnemyPosition(enemy.startX, enemy.startY, go);
			}else{
				actualLevel.setStartEnemyHelicoPosition(enemy.startX, enemy.startY, go);
			}
			go.SetActive(true);
			go.transform.GetComponent<Enemy>().loadEnemy(enemy);
			listUIEnemy.Add(go.transform.GetComponent<Enemy>(), new List<GameObject>());
			actualEnemyEdit = go.transform.GetComponent<Enemy>();
			for(int j=0; j<enemy.ea.Count; j++)
			{
				bool isDeplacement = ((int)actualEnemyEdit.getLastAction() <= 3);
				actualEnemyEdit.addAction(enemy.ea.ElementAt(j).state, enemy.ea.ElementAt(j).timeWait);
				if((int)enemy.ea.ElementAt(j).state <= 3)
				{
					actualNumberAction = actualEnemyEdit.getNumberOfAction();
					placeUIWay(enemy.ea.ElementAt(j).state, actualEnemyEdit.isHelico());
				}else{
					if(isDeplacement)
					{
						placeUIAction(actualEnemyEdit.getLastPosition(), actualEnemyEdit.isHelico());
					}
				}
				listUIEnemy[actualEnemyEdit].Last().SetActive(false);
			}
		}
		
		for(int i=0; i<l.getObjects().Count; i++)
		{
			var theObj = l.getObjects().ElementAt(i);
			if(theObj.isCanon)
			{
				var go = (GameObject)Instantiate(canonModel, new Vector3(theObj.positionX*2f, 2.2f, theObj.positionY*2f), canonModel.transform.rotation);
				actualLevel.setCanon((int)theObj.positionX, (int)theObj.positionY, go);
				go.SetActive(true);
				go.GetComponent<Canon>().setLook((LOOK)theObj.theLook);
				switch(theObj.theLook)
				{
				case LOOK.DOWN:
					go.transform.eulerAngles = new Vector3(0f, 0f, 0f);
					break;
				case LOOK.UP:
					go.transform.eulerAngles = new Vector3(0f, 180f, 0f);
					break;
				case LOOK.RIGHT:
					go.transform.eulerAngles = new Vector3(0f, -90f, 0f);
					break;
				case LOOK.LEFT:
					go.transform.eulerAngles = new Vector3(0f, 90f, 0f);
					break;
				}
			}else{
				var go = (GameObject)Instantiate(cameraEnModel, new Vector3(theObj.positionX*2f, 4f, theObj.positionY*2f), cameraEnModel.transform.rotation);
				actualLevel.setCameraEnemy((int)theObj.positionX, (int)theObj.positionY, go);
				go.GetComponent<CameraEnemy>().rightFirst = theObj.isRightFirst;
				go.SetActive(true);
				go.GetComponent<CameraEnemy>().setLook((LOOK)theObj.theLook);
				switch(theObj.theLook)
				{
				case LOOK.DOWN:
					go.transform.eulerAngles = new Vector3(0f, 0f, 0f);
					break;
				case LOOK.UP:
					go.transform.eulerAngles = new Vector3(0f, 180f, 0f);
					break;
				case LOOK.RIGHT:
					go.transform.eulerAngles = new Vector3(0f, -90f, 0f);
					break;
				case LOOK.LEFT:
					go.transform.eulerAngles = new Vector3(0f, 90f, 0f);
					break;
				}
			}
		}
		
		panelUILoad.SetActive(false);
		actualMode = EDITMODE.CUBE;
		oldWidthSelected = -1;
		oldHeightSelected = -1;
	}
	#endregion
}
				 