using UnityEngine;
using System.Collections;

public class AnimationSprite {
	
	private LOOK actualLook;
	
	private int animationState;
	
	private int numberFrameAnimation = 8;
	
	private float frameRate = 0.0625f;
	
	private float time;
	
	private UISprite sprite;
	
	private bool isIDLE;
	
	private string prefix;
	
	public AnimationSprite(UISprite spritePlayer, string tprefix, LOOK defaultLook = LOOK.UP)
	{
		actualLook = defaultLook;	
		sprite = spritePlayer;
		prefix = tprefix;
		sprite.spriteName = prefix + convertLookToString() + "IDLE";
		sprite.MakePixelPerfect();
		time = 0f;
		isIDLE = true;
	}
	
	public void anim(){
		time += Time.deltaTime;	
		if(time >= frameRate)
		{
			sprite.spriteName = prefix + convertLookToString() + (animationState + 1);
			sprite.MakePixelPerfect();
			time = 0f;
			
			animationState++;
			if(animationState >= numberFrameAnimation)
			{
				animationState = 0;	
			}
			isIDLE = false;
		}
	}
	
	public void idle(bool forceRefresh = false)
	{
		if(!isIDLE || forceRefresh)
		{
			time = frameRate;
			animationState = 0;
			sprite.spriteName = prefix + convertLookToString() + "IDLE";
			sprite.MakePixelPerfect();
			isIDLE = true;
		}
	}
	
	public void refreshPosition(LOOK pl)
	{
		if(actualLook != pl)
		{
			actualLook = pl;	
		}	
	}
	
	
	string convertLookToString()
	{
		switch(actualLook)
		{
		case LOOK.LEFT:
			return "Left";
		case LOOK.RIGHT:
			return "Right";
		case LOOK.UP:
			return "Up";
		case LOOK.DOWN:
			return "Down";
		default:
			return "";
		}
	}
	
	public static string staticConvertLookToString(LOOK actualLook)
	{
		switch(actualLook)
		{
		case LOOK.LEFT:
			return "Left";
		case LOOK.RIGHT:
			return "Right";
		case LOOK.UP:
			return "Up";
		case LOOK.DOWN:
			return "Down";
		default:
			return "";
		}
	}
	
	public LOOK getActualLook()
	{
		return actualLook;	
	}
	
}
