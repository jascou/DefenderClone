using System;
using UnityEngine;

public class MovableObject {
	public Vector2 position;
	public Vector2 velocity;
	public bool isTobeRemoved=false;
	public GameObject displayObject;
	protected SpriteRenderer spriteRenderer;

	
	Vector2 topBottomLimits;
	Vector2 leftRightLimits;


	public MovableObject(GameObject gameObject, Vector2 initialPosition){
		topBottomLimits=GameManager.topBottomLimits;
		leftRightLimits=GameManager.leftRightLimits;
		position=initialPosition;
		displayObject=gameObject;
		displayObject.transform.localPosition=position;
		spriteRenderer=displayObject.GetComponent<SpriteRenderer>();
	}

	public virtual void Move(float deltaX, float deltaY){
		position.x+=deltaX;
		position.y+=deltaY;
		FixPosition();
		displayObject.transform.localPosition=position;
	}
	public virtual void MoveTo(float newX, float newY){
		position.x=newX;
		position.y=newY;
		FixPosition();
		displayObject.transform.localPosition=position;
	}

    private void FixPosition()
    {
        position.y=Mathf.Clamp(position.y,topBottomLimits.y,topBottomLimits.x);
		if(position.x<leftRightLimits.x){
			position.x=leftRightLimits.y;
		}
		if(position.x>leftRightLimits.y){
			position.x=leftRightLimits.x;
		}
    }

    public Color paint
    {
        get
        {
            return spriteRenderer.color;
        }
        set
        {
            spriteRenderer.color=value;
        }
    }


    public void Remove()
    {
        Component.Destroy(displayObject);
    }
}
