using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum EDITMODE{
	CUBE,
	STARTPOSITION,
	ENEMY,
	EDITENEMY,
	TESTENEMY
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
	public GameObject panelEnemy;
	private GameObject enemyValid;
	private GameObject enemyHover;
	private GameObject enemyInvalid;
	
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
	
	public GameObject panelUIEdit;
	
	private Dictionary<Enemy, List<GameObject>> listUIEnemy;
	
	//PlayerSpawn
	public GameObject playerSpawn;
	//Element du playerSpawn
	private GameObject playerSpawnValid;
	private GameObject playerSpawnInvalid;
	
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
	private Level actualLevel;
	
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
	
	//Mode de construction
	private EDITMODE actualMode;
	
	private bool isInHidingMode;
	
	//Camera Contener en mode game
	private GameObject cameraContener;
	
	// Use this for initialization
	void Start () {
		
		isInHidingMode = false;
		inEnemyTest = false;
		cameraContener = Camera.main.transform.parent.gameObject;
		actualMode = EDITMODE.CUBE;
		placementCube = cursor.transform.FindChild("PlacementCube").gameObject;
		deleteCube = cursor.transform.FindChild("DeleteCube").gameObject;
		playerSpawnValid = playerSpawn.transform.FindChild("SpawnValid").gameObject;
		playerSpawnInvalid = playerSpawn.transform.FindChild("SpawnInvalid").gameObject;
		enemyValid = enemyCursor.transform.FindChild("EnemyValid").gameObject;
		enemyHover = enemyCursor.transform.FindChild("EnemyHover").gameObject;
		enemyInvalid = enemyCursor.transform.FindChild("EnemyInvalid").gameObject;
		cursorWayValid = cursorEdit.transform.FindChild("WayEditValid").gameObject;
		cursorWayAction = cursorEdit.transform.FindChild("ActionEdit").gameObject;
		listUIEnemy = new Dictionary<Enemy, List<GameObject>>();
		
		
		actualSizeSelected = 0;
		actualActionSelected = (int)MOVE.WAIT;
		
		actualLevel = new Level(maxWidth, maxHeight, maxVolume);
		
		
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
			case EDITMODE.ENEMY:
				UpdateEnemy();
				break;
			case EDITMODE.EDITENEMY:
				UpdateEnemyEdit();
				break;
			case EDITMODE.TESTENEMY:
				UpdateCamera();
				break;
		}
		
		//Debug inputs
		if(Input.GetKeyDown(KeyCode.O) && EDITMODE == EDITMODE.CUBE)
		{
			goOnSpawnPlayerMode();	
		}
		
		if(Input.GetKeyDown(KeyCode.U) && EDITMODE == EDITMODE.CUBE)
		{
			goOnSpawnEnemy();
		}
		
		if(Input.GetKeyDown(KeyCode.I))
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
		
		if(Input.GetKeyDown(KeyCode.T) && (EDITMODE == EDITMODE.EDITENEMY || EDITMODE == EDITMODE.TESTENEMY) && actualEnemyEdit.isValidEnemy(actualLevel))
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
				actualMode = EDITMODE.CUBE;
				oldWidthSelected = -1;
				oldHeightSelected = -1;
			}else{
				inEnemyTest = false;
				actualEnemyEdit.endTestEnemy();
				enemyHover.SetActive(false);
				refreshPossiblePositionAction();
				panelUIEdit.SetActive(true);
				actualMode = EDITMODE.EDITENEMY;
				enableEnemyWay(actualEnemyEdit);
				oldWidthSelected = -1;
				oldHeightSelected = -1;
			}
		}
		
	}
	
	
	
	void UpdateCube () {
	
		#region Raycast
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
				case BLOCSTATE.ENEMYSTART:
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
		#endregion
		
		#region Input 
		
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
					case BLOCSTATE.ENEMYSTART:
						removeEnemyObject(actualLevel.getGameObjectAt(gridWidthSelected, gridHeightSelected, 1).GetComponent<Enemy>());
						actualLevel.removeEnemyPosition(gridWidthSelected, gridHeightSelected);
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
		
		#endregion
	}
	
	
	void UpdateStartPosition () {
	
		#region Raycast
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
		#endregion
		
		#region Input 
		
		//Clic souris
		if(Input.GetMouseButton(0) && playerSpawnValid.activeSelf)
		{
			actualLevel.setStartPlayerPosition(gridWidthSelected, gridHeightSelected);
			actualMode = EDITMODE.CUBE;
			oldWidthSelected = -1;
			oldHeightSelected = -1;
			
		}
		
		
		UpdateCamera();
		
		#endregion
	}
	
	
	void UpdateEnemy()
	{
		#region Raycast
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
		#endregion
		
		
		if(Input.GetMouseButton(0))
		{
			if(enemyValid.activeSelf){
				var go = (GameObject)Instantiate(enemyModel, new Vector3(actualHeightSelected, actualSizeSelected*2 + 0.225f, actualWidthSelected), enemyModel.transform.rotation);
				actualLevel.setStartEnemyPosition(gridWidthSelected, gridHeightSelected, go);
				go.transform.parent = panelEnemy.transform;
				go.SetActive(true);
				go.GetComponent<Enemy>().init (enemyLook, new Vector2((float)gridWidthSelected, (float)gridHeightSelected));
				listUIEnemy.Add(go.GetComponent<Enemy>(), new List<GameObject>());
				enemyValid.SetActive(false);
				actualMode = EDITMODE.CUBE;
				oldWidthSelected = -1;
				oldHeightSelected = -1;
			}else if(enemyHover.activeSelf)
			{
				enemyHover.SetActive(false);
				actualEnemyEdit = actualLevel.getGameObjectAt(gridWidthSelected, gridHeightSelected, 1).GetComponent<Enemy>();
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
				enemyValid.GetComponent<UISprite>().spriteName = AnimationSprite.staticConvertLookToString(enemyLook) + "IDLE";
				enemyInvalid.GetComponent<UISprite>().spriteName = AnimationSprite.staticConvertLookToString(enemyLook) + "IDLE";
				enemyHover.GetComponent<UISprite>().spriteName = AnimationSprite.staticConvertLookToString(enemyLook) + "IDLE";
				oldWidthSelected = -1;
				oldHeightSelected = -1;
			
		}
		
		UpdateCamera();
	}
	
	
	void UpdateEnemyEdit()
	{
		#region Raycast
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
		#endregion
		
		
		#region Input
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
		
		UpdateCamera();
	}
	
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
		UpdateCameraInEditor();
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
	
	void UpdateCameraInGame()
	{
		if(Input.GetKey(KeyCode.UpArrow)){
			cameraContener.transform.Translate(speedCameraMove*Time.deltaTime, 0f , 0f);
		}else if(Input.GetKey(KeyCode.DownArrow)){
			cameraContener.transform.Translate(-speedCameraMove*Time.deltaTime, 0f, 0f);
		}else if(Input.GetKey(KeyCode.RightArrow)){
			cameraContener.transform.Translate(0f, 0f, -speedCameraMove*Time.deltaTime);
		}else if(Input.GetKey(KeyCode.LeftArrow)){
			cameraContener.transform.Translate(0f, 0f, speedCameraMove*Time.deltaTime);
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
	#endregion
	
	
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
		if(y + 1 < maxHeight && (actualLevel.getBlocState(x,y-1,1) == BLOCSTATE.CUBE || actualLevel.getBlocState(x, y-1, 0) == BLOCSTATE.EMPTY))
		{
			down.SetActive(false);	
		}
		if(y - 1 > 0 && (actualLevel.getBlocState(x,y+1,1) == BLOCSTATE.CUBE || actualLevel.getBlocState(x, y+1, 0) == BLOCSTATE.EMPTY))
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
	
	public void placeUIAction()
	{
		addGameObjectToList(actionObject, 0f);	
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
	
	public void insertGameObjectToList(GameObject model, float rotation, int positionToInsert, Vector3 oldPosition)
	{
		var go = (GameObject)Instantiate(model, oldPosition, cursorEdit.transform.rotation);
		go.transform.Rotate(0f, rotation, 0f);
		go.SetActive(true);
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
		case MOVE.POWER1:
			return "Laser Look";
		case MOVE.POWER2:
			return "Zone Look";
		case MOVE.POWER3:
			return "Grenade Look";
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
		if(actualActionSelected > (int)MOVE.POWER3)
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
			actualActionSelected = (int)MOVE.POWER3;	
		}
		refreshText(new EnemyAction((MOVE)actualActionSelected, 3f));
	}
}
				 