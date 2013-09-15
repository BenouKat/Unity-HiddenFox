using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class InputManager : MonoBehaviour {
	
	public static InputManager instance;
	void Awake()
	{
		if(instance == null)
		{
			instance = this;	
		}
	}
	
	private int positionCam = 0;

	private KeyCode[] direction = new KeyCode[4]{KeyCode.UpArrow, KeyCode.RightArrow, KeyCode.DownArrow, KeyCode.LeftArrow };
	private KeyCode[] keyCodeCase = new KeyCode[4]{KeyCode.Z, KeyCode.D, KeyCode.S, KeyCode.Q };
	private KeyCode[] queueKeyInput = new KeyCode[4]{KeyCode.None, KeyCode.None, KeyCode.None, KeyCode.None };
	
	private KeyCode keyResult;
	
	void Start()
	{
		if(Input.GetKey(keyCodeCase[0]) && isNotInputed(keyCodeCase[0]))
		{
			setInput(keyCodeCase[0]);
		}
		
		if(Input.GetKey(keyCodeCase[1]) && isNotInputed(keyCodeCase[1]))
		{
			setInput(keyCodeCase[1]);
		}
		
		if(Input.GetKey(keyCodeCase[2]) && isNotInputed(keyCodeCase[2]))
		{
			setInput(keyCodeCase[2]);
		}
		
		if(Input.GetKey(keyCodeCase[3]) && isNotInputed(keyCodeCase[3]))
		{
			setInput(keyCodeCase[3]);
		}
	}
	
	void Update()
	{
		if(Input.GetKeyDown(keyCodeCase[0]) && isNotInputed(keyCodeCase[0]))
		{
			setInput(keyCodeCase[0]);
		}
		
		if(Input.GetKeyDown(keyCodeCase[1]) && isNotInputed(keyCodeCase[1]))
		{
			setInput(keyCodeCase[1]);
		}
		
		if(Input.GetKeyDown(keyCodeCase[2]) && isNotInputed(keyCodeCase[2]))
		{
			setInput(keyCodeCase[2]);
		}
		
		if(Input.GetKeyDown(keyCodeCase[3]) && isNotInputed(keyCodeCase[3]))
		{
			setInput(keyCodeCase[3]);
		}
		
		
		if(Input.GetKeyUp(keyCodeCase[0]) && !isNotInputed(keyCodeCase[0]))
		{
			removeInput(keyCodeCase[0]);
		}
		
		if(Input.GetKeyUp(keyCodeCase[1]) && !isNotInputed(keyCodeCase[1]))
		{
			removeInput(keyCodeCase[1]);
		}
		
		if(Input.GetKeyUp(keyCodeCase[2]) && !isNotInputed(keyCodeCase[2]))
		{
			removeInput(keyCodeCase[2]);
		}
		
		if(Input.GetKeyUp(keyCodeCase[3]) && !isNotInputed(keyCodeCase[3]))
		{
			removeInput(keyCodeCase[3]);
		}
		
		
	}
	
	public void camGoRight()
	{
		positionCam--;
		if(positionCam < 0) positionCam = 3;
	}
	
	public void camGoLeft()
	{
		positionCam++;
		if(positionCam > 3) positionCam = 0;
	}
	
	public KeyCode getDirection(int index)
	{
		index += positionCam;
		if(index > 3)
		{
			index = index - 4;	
		}
		
		return direction[index];
	}
	
	
	public bool isNotInputed(KeyCode code)
	{
		return !queueKeyInput.Contains(code);
	}
	
	public void setInput(KeyCode code)
	{
		for(int i=0; i<4; i++)
		{
			if(queueKeyInput[i] == KeyCode.None)
			{
				queueKeyInput[i] = code;
				i = 4;
			}
		}
	}
	
	public void removeInput(KeyCode code)
	{
		int index=0;
		KeyCode[] tempQueue = queueKeyInput;
		
		for(int i=0; i<4; i++)
		{
			if(queueKeyInput[i] == code)
			{
				queueKeyInput[i] = KeyCode.None;
			}
			
			if(queueKeyInput[i] != KeyCode.None)
			{
				tempQueue[index] = queueKeyInput[i];
				index++;
			}
		}
		
		queueKeyInput = tempQueue;
	}
	
	
	public KeyCode getKeyInput()
	{
		int index = -1;
		for(int i=0; i<4; i++)
		{
			if(queueKeyInput[i] != KeyCode.None)
			{
				index = i;
			}
		}
		
		return index == -1 ? KeyCode.None : queueKeyInput[index];
	}
	
	public int indexOf(KeyCode code)
	{
		for(int i=0; i<4; i++)
		{
			if(keyCodeCase[i] == code)
			{
				return i;
			}
		}
		
		return -1;
	}
	
	public KeyCode getDirectionNow()
	{
		keyResult = getKeyInput();
		return keyResult == KeyCode.None ? KeyCode.None : getDirection(indexOf(getKeyInput()));
	}
	
	
}
