using UnityEngine;

public class Enemy : MovableObject{
  protected string enemyType;
  public string bulletType;
  public bool needsShooting;
  protected float elapsedTime=0;
  protected BoxCollider2D boxCollider2D;
  protected float speed;
  protected Vector2 destination;
  protected bool destinationReached=false;
  protected int agression;
  protected int agressionMax=10;
  protected float firingDelay=0.1f;
  protected float lastFiredTime;
  public Enemy(GameObject gameObject, Vector2 initialPosition) : base(gameObject, initialPosition)
  {	
    boxCollider2D=gameObject.GetComponent<BoxCollider2D>();
		boxCollider2D.size= new Vector2(4,4);
		TriggerDispatcher dispatcher=gameObject.GetComponent<TriggerDispatcher>();
		Component.Destroy(dispatcher);
		gameObject.layer = LayerMask.NameToLayer("enemy");
    enemyType=gameObject.name;
  }
  public override void Move(float deltaX, float deltaY){
    base.Move(deltaX+(velocity.x*Time.deltaTime),deltaY+(velocity.y*Time.deltaTime));
	}
  public virtual bool Tick(){
    if(!isTobeRemoved && isCloseToDefender){
      if(Random.Range(0,(agressionMax-agression))<1 || destinationReached){
        Shoot();
      }
    }
    if(isTobeRemoved)return false;
    return true;
  }
  public override void Roam(){
    needsShooting=false;
		destinationReached=false;
    base.Roam();
  }
  void Shoot(){
    if(isReadyToFireAgain()){
      needsShooting=true;
      SetFireTime();
    }
  }
  public bool isReadyToFireAgain(){
		if(Time.timeSinceLevelLoad-lastFiredTime>firingDelay){
			return true;
		}
		return false;
	}
	public void SetFireTime(){
		lastFiredTime=Time.timeSinceLevelLoad;
	}
}
public class Enemy1 : Enemy
{
    float timeToRetarget;
    public Enemy1(GameObject gameObject, Vector2 initialPosition) : base(gameObject, initialPosition)
    {//aligns vertically with defender, shoots bullet 1, roams straight
      string textureName=GameManager.ENEMY1.ToLower();
      Sprite sprite= Sprite.Create(GameManager.Instance.textureManager.PackedTexture,GameManager.Instance.textureManager.GetTextureRectByName(textureName),new Vector2(0.5f,0.5f),1);
      spriteRenderer.sprite=sprite; 
      speed=300;
      velocity=new Vector2(speed,0);
      if(Random.Range(0,2)==0)velocity.x*=-1;
      bulletType=GameManager.BULLET1;
      agression=1;
    }
    public override void Move(float deltaX, float deltaY){
      if(isCloseToDefender){
        Vector2 desiredVelocity=(destination-position).normalized*speed;
        velocity=Vector2.Lerp(velocity,desiredVelocity,0.25f);
        if(Vector2.Distance(destination,position)<20 &&!destinationReached){//Debug.Log("reached");
          timeToRetarget=0.8f;
          destinationReached=true;
        }
      }
        base.Move(deltaX,deltaY);
	  }
    public override bool Tick(){
      if(!isTobeRemoved && isCloseToDefender && destinationReached){
        timeToRetarget-=Time.deltaTime;
        if(timeToRetarget<=0){//Debug.Log("retargetting");
          destination.y=defenderRef.transform.localPosition.y;
          destination.x=(defenderRef.transform.localPosition.x+position.x)/2;
          destinationReached=false;needsShooting=false;
        }
      }
      
      return base.Tick();
  }
    public override void Seek(GameObject defender){
      destination.y=defender.transform.localPosition.y;
      destination.x=(defender.transform.localPosition.x+position.x)/2;
      base.Seek(defender);
    }
    public override void Roam(){
      velocity=new Vector2(speed,0);
      if(Random.Range(0,2)==0)velocity.x*=-1;
      base.Roam();
    }
}
public class Enemy2 : Enemy
{
    public Enemy2(GameObject gameObject, Vector2 initialPosition) : base(gameObject, initialPosition)
    {
      string textureName=GameManager.ENEMY2.ToLower();
      Sprite sprite= Sprite.Create(GameManager.Instance.textureManager.PackedTexture,GameManager.Instance.textureManager.GetTextureRectByName(textureName),new Vector2(0.5f,0.5f),1);
      spriteRenderer.sprite=sprite; 
    }
}
public class Enemy3 : Enemy
{
    public Enemy3(GameObject gameObject, Vector2 initialPosition) : base(gameObject, initialPosition)
    {
      string textureName=GameManager.ENEMY3.ToLower();
      Sprite sprite= Sprite.Create(GameManager.Instance.textureManager.PackedTexture,GameManager.Instance.textureManager.GetTextureRectByName(textureName),new Vector2(0.5f,0.5f),1);
      spriteRenderer.sprite=sprite; 
    }
}
public class Enemy4 : Enemy
{
    public Enemy4(GameObject gameObject, Vector2 initialPosition) : base(gameObject, initialPosition)
    {
      string textureName=GameManager.ENEMY4.ToLower();
      Sprite sprite= Sprite.Create(GameManager.Instance.textureManager.PackedTexture,GameManager.Instance.textureManager.GetTextureRectByName(textureName),new Vector2(0.5f,0.5f),1);
      spriteRenderer.sprite=sprite; 
    }
}
public class Enemy5 : Enemy
{
    public Enemy5(GameObject gameObject, Vector2 initialPosition) : base(gameObject, initialPosition)
    {
      string textureName=GameManager.ENEMY5.ToLower();
      Sprite sprite= Sprite.Create(GameManager.Instance.textureManager.PackedTexture,GameManager.Instance.textureManager.GetTextureRectByName(textureName),new Vector2(0.5f,0.5f),1);
      spriteRenderer.sprite=sprite; 
    }
}
public class Enemy6 : Enemy
{
    public Enemy6(GameObject gameObject, Vector2 initialPosition) : base(gameObject, initialPosition)
    {
      string textureName=GameManager.ENEMY6.ToLower();
      Sprite sprite= Sprite.Create(GameManager.Instance.textureManager.PackedTexture,GameManager.Instance.textureManager.GetTextureRectByName(textureName),new Vector2(0.5f,0.5f),1);
      spriteRenderer.sprite=sprite; 
    }
}
public class Enemy7 : Enemy
{
    public Enemy7(GameObject gameObject, Vector2 initialPosition) : base(gameObject, initialPosition)
    {
      string textureName=GameManager.ENEMY7.ToLower();
      Sprite sprite= Sprite.Create(GameManager.Instance.textureManager.PackedTexture,GameManager.Instance.textureManager.GetTextureRectByName(textureName),new Vector2(0.5f,0.5f),1);
      spriteRenderer.sprite=sprite; 
    }
}
public class Enemy8 : Enemy
{
    public Enemy8(GameObject gameObject, Vector2 initialPosition) : base(gameObject, initialPosition)
    {
      string textureName=GameManager.ENEMY8.ToLower();
      Sprite sprite= Sprite.Create(GameManager.Instance.textureManager.PackedTexture,GameManager.Instance.textureManager.GetTextureRectByName(textureName),new Vector2(0.5f,0.5f),1);
      spriteRenderer.sprite=sprite; 
    }
}
public class Enemy9 : Enemy
{
    public Enemy9(GameObject gameObject, Vector2 initialPosition) : base(gameObject, initialPosition)
    {
      string textureName=GameManager.ENEMY9.ToLower();
      Sprite sprite= Sprite.Create(GameManager.Instance.textureManager.PackedTexture,GameManager.Instance.textureManager.GetTextureRectByName(textureName),new Vector2(0.5f,0.5f),1);
      spriteRenderer.sprite=sprite; 
    }
}
public class Enemy10 : Enemy
{
    public Enemy10(GameObject gameObject, Vector2 initialPosition) : base(gameObject, initialPosition)
    {
      string textureName=GameManager.ENEMY10.ToLower();
      Sprite sprite= Sprite.Create(GameManager.Instance.textureManager.PackedTexture,GameManager.Instance.textureManager.GetTextureRectByName(textureName),new Vector2(0.5f,0.5f),1);
      spriteRenderer.sprite=sprite; 
    }
}
public class Enemy11 : Enemy
{
    public Enemy11(GameObject gameObject, Vector2 initialPosition) : base(gameObject, initialPosition)
    {
      string textureName=GameManager.ENEMY11.ToLower();
      Sprite sprite= Sprite.Create(GameManager.Instance.textureManager.PackedTexture,GameManager.Instance.textureManager.GetTextureRectByName(textureName),new Vector2(0.5f,0.5f),1);
      spriteRenderer.sprite=sprite; 
    }
}
public class Enemy12 : Enemy
{
    public Enemy12(GameObject gameObject, Vector2 initialPosition) : base(gameObject, initialPosition)
    {
      string textureName=GameManager.ENEMY12.ToLower();
      Sprite sprite= Sprite.Create(GameManager.Instance.textureManager.PackedTexture,GameManager.Instance.textureManager.GetTextureRectByName(textureName),new Vector2(0.5f,0.5f),1);
      spriteRenderer.sprite=sprite; 
    }
}