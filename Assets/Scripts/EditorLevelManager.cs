using UnityEngine;
using System.Collections;

public class EditorLevelManager : MonoBehaviour {
	
	
	public GameObject cubeBase;
	public GameObject cursor;
	private GameObject placementCube;
	private GameObject deleteCube;
	
	public int widthLevelDefault;
	public int heightLevelDefault;
	
	public int maxWidthLevel;
	public int maxHeightLevel;
	public int maxVolume;
	
	
	private int actualSizeSelected;
	
	private Level actualLevel;
	
	private int actualWidthSelected;
	private int actualHeightSelected;
	private int oldWidthSelelected;
	private int oldHeightSelected;
	
	private GameObject previousBlocSelected;
	
	// Use this for initialization
	void Start () {
		
		placementCube = cursor.transform.FindChild("PlacementCube").gameObject;
		deleteCube = cursor.transform.FindChild("DeleteCube").gameObject;
		
		actualSizeSelected = 0;
		
		actualLevel = new Level(maxWidthLevel, maxHeightLevel, maxVolume);
		
		
		for(int i = 0; i<widthLevelDefault; i++)
		{
			for(int j=0; j<heightLevelDefault; j++)
			{
				var go = (GameObject)Instantiate(cubeBase, new Vector3(j*2, 0f, i*2), cubeBase.transform.rotation);
				actualLevel.setCube(j, i, 0);
				go.SetActive(true);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		
		if(Physics.Raycast(ray, out hit)){
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
			
			if(actualWidthSelected != oldWidthSelelected || actualHeightSelected != oldHeightSelected)
			{
				oldWidthSelelected = actualWidthSelected;
				oldHeightSelected = actualHeightSelected;
				
				cursor.transform.position = new Vector3(actualHeightSelected, actualSizeSelected, actualWidthSelected);
				
				if(actualLevel.isEmpty(actualWidthSelected/2, actualHeightSelected/2, actualSizeSelected))
				{
					deleteCube.SetActive(false);
					placementCube.SetActive(true);
				}else{
					
					deleteCube.SetActive(true);
					placementCube.SetActive(false);
				}
			}
			
		}else{
			if(placementCube.activeSelf)
			{
				placementCube.SetActive(false);
			}else
			if(deleteCube.activeSelf)
			{
				placementCube.SetActive(false);	
			}
		}
	}
}
				 