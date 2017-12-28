using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MovableObject
{
    protected string bulletType;
    protected float life=1.5f;
    protected float elapsedTime=0;
    protected BoxCollider2D boxCollider2D;

    protected float speed;
    
    public Bullet(GameObject gameObject, Vector2 initialPosition) : base(gameObject, initialPosition){
        gameObject.layer = LayerMask.NameToLayer("enemybullet");
        bulletType=gameObject.name;
        boxCollider2D=gameObject.GetComponent<BoxCollider2D>();
		boxCollider2D.size= new Vector2(2,2);
        spriteRenderer.sortingLayerName="enemybullet";
    }
	public override void Move(float deltaX, float deltaY){
		base.Move(deltaX+(velocity.x*Time.deltaTime),deltaY+(velocity.y*Time.deltaTime));
	}
    protected override void FixPosition()
    {
        if(position.y>GameManager.screenTopBottomLimits.x||position.y<GameManager.screenTopBottomLimits.y){
            isTobeRemoved=true;
        }
		if(position.x<GameManager.leftRightLimits.x){
			position.x=GameManager.leftRightLimits.y;
		}
		if(position.x>GameManager.leftRightLimits.y){
			position.x=GameManager.leftRightLimits.x;
		}
    }
    public virtual bool Tick(){
        elapsedTime+=Time.deltaTime;
        if(elapsedTime>=life)isTobeRemoved=true;
        if(isTobeRemoved)return false;
        return true;
    }
    public void SetInitialVelocity(Vector2 direction){
        velocity=direction*speed;
    }
}
public class HeroBullet : Bullet
{
    public HeroBullet(GameObject gameObject, Vector2 initialPosition) : base(gameObject, initialPosition)
    {
        life=1.5f;
        boxCollider2D.size= new Vector2(1,1);
        gameObject.layer = LayerMask.NameToLayer("herobullet");
		string textureName=GameManager.BULLET1.ToLower();
		Sprite sprite= Sprite.Create(GameManager.Instance.textureManager.PackedTexture,GameManager.Instance.textureManager.GetTextureRectByName(textureName),new Vector2(0.5f,0.5f),1);
		spriteRenderer.sprite=sprite; 
        spriteRenderer.sortingLayerName="herobullet";
        speed=1000;
    }
    public override bool Tick(){
       if(position.x<GameManager.screenLeftRightLimits.x)isTobeRemoved=true;
		if(position.x>GameManager.screenLeftRightLimits.y)isTobeRemoved=true;
        return base.Tick();
    }
} 

public class Bullet1 : Bullet
{
    public Bullet1(GameObject gameObject, Vector2 initialPosition) : base(gameObject, initialPosition)
    {//straight motion
        life=1.2f;
        boxCollider2D.size= new Vector2(1,1);
        TriggerDispatcher dispatcher=gameObject.GetComponent<TriggerDispatcher>();
		Component.Destroy(dispatcher);
        string textureName=GameManager.BULLET1.ToLower();
		Sprite sprite= Sprite.Create(GameManager.Instance.textureManager.PackedTexture,GameManager.Instance.textureManager.GetTextureRectByName(textureName),new Vector2(0.5f,0.5f),1);
		spriteRenderer.sprite=sprite; 
        speed=600;
    }
} 

public class Bullet2 : Bullet
{//straight motion graphic variant
    public Bullet2(GameObject gameObject, Vector2 initialPosition) : base(gameObject, initialPosition)
    {
        life=1.2f;
        TriggerDispatcher dispatcher=gameObject.GetComponent<TriggerDispatcher>();
		Component.Destroy(dispatcher);
        string textureName=GameManager.BULLET2.ToLower();
		Sprite sprite= Sprite.Create(GameManager.Instance.textureManager.PackedTexture,GameManager.Instance.textureManager.GetTextureRectByName(textureName),new Vector2(0.5f,0.5f),1);
		spriteRenderer.sprite=sprite; 
        speed=600;
    }
} 

public class Bullet3 : Bullet
{//sin motion

    bool isHorizontal=true;
    float amplitude=400;
    public Bullet3(GameObject gameObject, Vector2 initialPosition) : base(gameObject, initialPosition)
    {
        life=1.5f;
        TriggerDispatcher dispatcher=gameObject.GetComponent<TriggerDispatcher>();
		Component.Destroy(dispatcher);
        string textureName=GameManager.BULLET3.ToLower();
		Sprite sprite= Sprite.Create(GameManager.Instance.textureManager.PackedTexture,GameManager.Instance.textureManager.GetTextureRectByName(textureName),new Vector2(0.5f,0.5f),1);
		spriteRenderer.sprite=sprite; 
        speed=750;
    }
    public override void Move(float deltaX, float deltaY){
        if(isHorizontal){
            velocity.y=(Mathf.Sin(50*elapsedTime)*amplitude);
        }else{
            velocity.x=(Mathf.Sin(50*elapsedTime)*amplitude);
        }
		base.Move(deltaX,deltaY);
	}
} 

public class Bullet4 : Bullet
{//random disperse in circle stays longer. should not assign velocity to this bullet externally
    float damping=0.96f;
    public Bullet4(GameObject gameObject, Vector2 initialPosition) : base(gameObject, initialPosition)
    {
        life=3;
        TriggerDispatcher dispatcher=gameObject.GetComponent<TriggerDispatcher>();
		Component.Destroy(dispatcher);
        string textureName=GameManager.BULLET4.ToLower();
		Sprite sprite= Sprite.Create(GameManager.Instance.textureManager.PackedTexture,GameManager.Instance.textureManager.GetTextureRectByName(textureName),new Vector2(0.5f,0.5f),1);
		spriteRenderer.sprite=sprite; 
        speed=250;
        velocity=Random.insideUnitCircle*speed;
    }
    public override void Move(float deltaX, float deltaY){//slows to halt
        velocity*=damping;
		base.Move(deltaX,deltaY);
	}
} 

public class Bullet5 : Bullet
{//homes on defender, need to receive proximity alerts from animation manager, do not assign velocity
    float damping=0.96f;

    public Bullet5(GameObject gameObject, Vector2 initialPosition) : base(gameObject, initialPosition)
    {
        life=5f;
        TriggerDispatcher dispatcher=gameObject.GetComponent<TriggerDispatcher>();
		Component.Destroy(dispatcher);
        string textureName=GameManager.BULLET5.ToLower();
		Sprite sprite= Sprite.Create(GameManager.Instance.textureManager.PackedTexture,GameManager.Instance.textureManager.GetTextureRectByName(textureName),new Vector2(0.5f,0.5f),1);
		spriteRenderer.sprite=sprite; 
        speed=200;
        paint=Color.white;
    }
    public override void Move(float deltaX, float deltaY){
        if(!isCloseToDefender){
            if(velocity.sqrMagnitude<100){//moves randomly
                velocity=Random.insideUnitCircle*speed;
            }else{//slows
                velocity*=damping;
            }
        }else{//seeks defender
            Vector2 defenderPos=defenderRef.transform.localPosition;
            Vector2 desiredVelocity=(defenderPos-position).normalized*(2.5f*speed);
            velocity=Vector2.Lerp(velocity,desiredVelocity,0.05f);
        }
		base.Move(deltaX,deltaY);
	}
    public override void Seek(GameObject defender){
        velocity=Random.insideUnitCircle*speed;
        paint=Color.red;
        base.Seek(defender);
    }
    public override void Roam(){
        paint=Color.white;
		base.Roam();
    }
} 