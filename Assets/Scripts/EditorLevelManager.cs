using UnityEngine;
using System.Collections;

public enum EDITMODE{
	CUBE,
	STARTPOSITION,
	ENEMY
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
	
	//PlayerSpawn
	private GameObject playerSpawn;
	//Element du playerSpawn
	private GameObject playerSpawnValid;
	private GameObject playerSpawnInvalid;
	
	//Taille du terrain par défaut (sera changé pour être rentré par l'utilisateur)
	public int widthLevelDefault;
	public int heightLevelDefault;
	
	//Taille max du terrain (à choisir selon les performances)
	public int maxWidthLevel;
	public int maxHeightLevel;
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
	private int oldWidthSelelected;
	private int oldHeightSelected;
	
	//Bloc déjà en place selectionné
	private GameObject previousBlocSelected;
	
	//Mode de construction
	private EDITMODE actualMode;
	
	private bool isInHidingMode;
	
	// Use this for initialization
	void Start () {
		
		isInHidingMode = false;
		actualMode = EDITMODE.CUBE;
		placementCube = cursor.transform.FindChild("PlacementCube").gameObject;
		deleteCube = cursor.transform.FindChild("DeleteCube").gameObject;
		playerSpawnValid = playerSpawn.transform.FindChild("Valid").gameObject;
		playerSpawnInvalid = playerSpawn.transform.FindChild("Invalid").gameObject;
		
		actualSizeSelected = 0;
		
		actualLevel = new Level(maxWidthLevel, maxHeightLevel, maxVolume);
		
		
		for(int i = 0; i<widthLevelDefault; i++)
		{
			for(int j=0; j<heightLevelDefault; j++)
			{
				var go = (GameObject)Instantiate(cubeBase, new Vector3(j*2, 0f, i*2), cubeBase.transform.rotation);
				actualLevel.setCube(i, j, 0);
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
			case EDITMODE.CUBE:
				UpdateStartPosition();
				break;
		}
	}
	
	
	
	void UpdateCube () {
	
		#region Raycast
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		
		if(Physics.Raycast(ray, out hit)){
			//Detection de la position curseur
			UpdatePositionMouse();
			
			//Verification de la nouvelle position
			if(actualWidthSelected != oldWidthSelelected || actualHeightSelected != oldHeightSelected)
			{
				oldWidthSelelected = actualWidthSelected;
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
						previousBlocSelected.renderer.enabled = true;
						previousBlocSelected = null;
					}
					deleteCube.SetActive(false);
					placementCube.SetActive(true);
					break;
					
				case BLOCSTATE.CUBE: //Case pleine
					if(previousBlocSelected != null)
					{
						previousBlocSelected.renderer.enabled = true;
					}
					previousBlocSelected = actualLevel.getGameObjectAt(gridWidthSelected, gridHeightSelected, actualSizeSelected);
					previousBlocSelected.renderer.enabled = false;
					deleteCube.SetActive(true);
					placementCube.SetActive(false);
					break;
					
				case BLOCSTATE.PLAYERSTART: //Case du respawn
					if(previousBlocSelected != null)
					{
						previousBlocSelected.renderer.enabled = true;
						previousBlocSelected = null;
					}
					deleteCube.SetActive(true);
					placementCube.SetActive(false);
					break;
				}
			}
			
		}else{
			//Desactivation du curseur
			if(placementCube.activeSelf)
			{
				placementCube.SetActive(false);
			}else
			if(deleteCube.activeSelf)
			{
				placementCube.SetActive(false);	
			}
			if(previousBlocSelected != null)
			{
				previousBlocSelected.renderer.enabled = true;
				previousBlocSelected = null;
			}
		}
		#endregion
		
		#region Input 
		
		//Clic souris
		if(Input.GetMouseKey(0) && placementCube.activeSelf)
		{
			var go = (GameObject)Instantiate(actualSizeSelected == 0 ? cubeBase : cubeBloc, new Vector3(actualHeightSelected, actualSizeSelected, actualWidthSelected), cubeBase.transform.rotation);
			actualLevel.setCube(gridWidthSelected, gridHeightSelected, actualSizeSelected);
			go.SetActive(true);
			oldWidthSelected = -1;
			oldHeightSelected = -1;
			
		}else if(Input.GetMouseKey(1) && deleteCube.activeSelf){
			var blocState = actualLevel.getBlocState(gridWidthSelected, gridHeightSelected, actualSizeSelected);
			switch(blocState)
			{
				case BLOCSTATE.CUBE:
					actualLevel.removeCube(gridWidthSelected, gridHeightSelected, actualSizeSelected);
					break;
				case BLOCSTATE.PLAYERSTART:
					actualLevel.removePlayerPosition();
					playerSpawn.SetActive(false);
					break;
			}
			oldWidthSelected = -1;
			oldHeightSelected = -1;
		}
		
		//Wheel
		if(Input.GetAxis("Mouse ScrollWheel") > 0 && actualSizeLevel < maxVolume){
			actualSizeLevel++;
			if(isInHidingMode)
			{
				actualLevel.setDisplayThisLevel(true, maxWidth, maxHeight, actualSizeLevel);
			}
			rayCastCollider.transform.Translate(0f, 2f, 0f);
			oldWidthSelected = -1;
			oldHeightSelected = -1;
		}else if(Input.GetAxis("Mouse ScrollWheel") < 0 && actualSizeLevel > 0){
			if(isInHidingMode)
			{
				actualLevel.setDisplayThisLevel(false, maxWidth, maxHeight, actualSizeLevel);
			}
			actualSizeLevel--;
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
			UpdatePositionMouse();
			
			//Verification de la nouvelle position
			if(actualWidthSelected != oldWidthSelelected || actualHeightSelected != oldHeightSelected)
			{
				oldWidthSelelected = actualWidthSelected;
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
		if(Input.GetMouseKey(0) && playerSpawnValid.activeSelf)
		{
			actualLevel.setStartPlayerPosition(gridWidthSelected, gridHeightSelected);
			actualMode = EDITMODE.CUBE;
			oldWidthSelected = -1;
			oldHeightSelected = -1;
			
		}
		
		
		UpdateCamera();
		
		#endregion
	}
	
	void UpdatePositionMouse()
	{
		if(hit.point.z%2 > 1)
		{
			actualWidthSelected = (hit.point.z + (2 - (hit.point.z%2)));
		}else{
			actualWidthSelected = (hit.point.z - (hit.point.z%2));	
		}
		
		if(hit.point.x%2 > 1)
		{
			actualHeightSelected = (hit.point.x + (2 - (hit.point.x%2)));	
		}else{
			actualHeightSelected = (hit.point.x + (hit.point.x%2));
		}
		
	}
	
	void UpdateCamera()
	{
		if(Input.GetKey(KeyCode.Up))
		{
			Camera.main.transform.Translate(Input.GetKey(KeyCode.Left) ? speedCameraMove*Time.deltaTime : 0f, 0f, Input.GetKey(KeyCode.Right) ?speedCameraMove*Time.deltaTime : 0f);
		}else if(Input.GetKey(KeyCode.Down))
		{
			Camera.main.transform.Translate(Input.GetKey(KeyCode.Right) ? -speedCameraMove*Time.deltaTime : 0f, 0f, Input.GetKey(KeyCode.Left) ? -speedCameraMove*Time.deltaTime : 0f);
		}else if(Input.GetKey(KeyCode.Left))
		{
			Camera.main.transform.Translate(Input.GetKey(KeyCode.Up) ? -speedCameraMove*Time.deltaTime : 0f, 0f, Input.GetKey(KeyCode.Down) ?speedCameraMove*Time.deltaTime : 0f);
		}else if(Input.GetKey(KeyCode.Right))
		{
			Camera.main.transform.Translate(Input.GetKey(KeyCode.Down) ? speedCameraMove*Time.deltaTime : 0f, 0f, Input.GetKey(KeyCode.Up) ? -speedCameraMove*Time.deltaTime : 0f);
		}
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
	
	public void goOnSpawnPlayerMode()
	{
		actualMode = EDITMODE.STARTPOSITION;
		rayCastCollider.transform.position = new Vector3(0f, 2f, 0f);
		actualSizeLevel = 1;
		actualLevel.setDisplayThisLevel(true, maxWidth, maxHeight, actualSizeLevel);
		if(isInHidingMode)
		{
			actualLevel.setDisplayUpperBlocs(false, maxWidth, maxHeight, maxVolume, actualSizeLevel);
		}
		oldWidthSelected = -1;
		oldHeightSelected = -1;
	}
}
				 