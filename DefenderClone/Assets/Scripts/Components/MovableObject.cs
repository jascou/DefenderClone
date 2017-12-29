using UnityEngine;

public class MovableObject {
	public Vector2 position;
	public Vector2 velocity;
	public bool isTobeRemoved=false;
	public GameObject displayObject;
	protected SpriteRenderer spriteRenderer;
	protected bool isCloseToDefender=false;
    protected GameObject defenderRef;

	Vector2 topBottomLimits;
	Vector2 leftRightLimits;

	/*
	Base class for all moving objects in scene. it has all the necessary behaviour for general motion and ticking
	 */
	public MovableObject(GameObject gameObject, Vector2 initialPosition){
		topBottomLimits=GameManager.topBottomLimits;
		leftRightLimits=GameManager.leftRightLimits;
		position=initialPosition;
		displayObject=gameObject;
		displayObject.transform.localPosition=position;
		spriteRenderer=displayObject.GetComponent<SpriteRenderer>();
		if(spriteRenderer==null){
			spriteRenderer=displayObject.AddComponent<SpriteRenderer>();
		}

	}

	//moves the object based on values passed
	public virtual void Move(float deltaX, float deltaY){
		position.x+=deltaX;
		position.y+=deltaY;
		FixPosition();
		displayObject.transform.localPosition=position;
	}
	//moves to specific position, not used frequently or ever, just in case
	public virtual void MoveTo(float newX, float newY){
		position.x=newX;
		position.y=newY;
		FixPosition();
		displayObject.transform.localPosition=position;
	}
	//seemlessly scroll when they go out of level world bounds.
    protected virtual void FixPosition()
    {
        position.y=Mathf.Clamp(position.y,topBottomLimits.y,topBottomLimits.x);
		if(position.x<leftRightLimits.x){
			position.x=leftRightLimits.y;
		}
		if(position.x>leftRightLimits.y){
			position.x=leftRightLimits.x;
		}
    }
	//behaviour called when this object reaches hero proximity
	public virtual void Seek(GameObject defender){
        defenderRef=defender;
        isCloseToDefender=true;
    }
	//behaviour called when this object moved away from hero proximity
    public virtual void Roam(){
		isCloseToDefender=false;
    }
	//color the object
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
	//cleanup
    public void Remove()
    {
        Component.Destroy(displayObject);
    }
}
