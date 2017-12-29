using UnityEngine;
/*
File holds Enemy base class and all 12 enemy variants
 */
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
  protected float firingDelay=0.2f;
  protected float lastFiredTime;
  protected float timeToRetarget;
  protected float timeToRetargetDefault=0.8f;
  protected float velocityLerpVal=0.2f;
  //seek and roam becomes more important for enemy classes
  public Enemy(GameObject gameObject, Vector2 initialPosition) : base(gameObject, initialPosition)
  {	
    boxCollider2D=gameObject.GetComponent<BoxCollider2D>();
		boxCollider2D.size= new Vector2(4,4);
		TriggerDispatcher dispatcher=gameObject.GetComponent<TriggerDispatcher>();
		Component.Destroy(dispatcher);
		gameObject.layer = LayerMask.NameToLayer("enemy");
    enemyType=gameObject.name;
    spriteRenderer.sortingLayerName="enemy";
  }
  //move pushes enemy to a destination, when reached sets time for retargetting as the hero will move away
  public override void Move(float deltaX, float deltaY){
    if(isCloseToDefender){
      Vector2 desiredVelocity=(destination-position).normalized*speed;
      velocity=Vector2.Lerp(velocity,desiredVelocity,velocityLerpVal);
      if(Vector2.Distance(destination,position)<20 &&!destinationReached){
        timeToRetarget=timeToRetargetDefault;
        destinationReached=true;
      }
    }
    base.Move(deltaX+(velocity.x*Time.deltaTime),deltaY+(velocity.y*Time.deltaTime));
	}
  //if within hero space, randomly fires, also handles retargetting after some time
  public virtual bool Tick(){
    if(!isTobeRemoved && isCloseToDefender){
      if(Random.Range(0,(agressionMax-agression))<1 || destinationReached){
        Shoot();
      }
      if(destinationReached){
        timeToRetarget-=Time.deltaTime;
        if(timeToRetarget<=0){//Debug.Log("retargetting");
          destination.y=defenderRef.transform.localPosition.y;
          destination.x=(defenderRef.transform.localPosition.x+position.x)/2;
          destinationReached=false;needsShooting=false;
        }
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
  //firing delay
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
      timeToRetargetDefault=0.8f;
      velocityLerpVal=0.2f;firingDelay=0.3f;
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
{//1 variant bullet 2
    public Enemy2(GameObject gameObject, Vector2 initialPosition) : base(gameObject, initialPosition)
    {
      string textureName=GameManager.ENEMY2.ToLower();
      Sprite sprite= Sprite.Create(GameManager.Instance.textureManager.PackedTexture,GameManager.Instance.textureManager.GetTextureRectByName(textureName),new Vector2(0.5f,0.5f),1);
      spriteRenderer.sprite=sprite; 
      speed=400;
      velocity=new Vector2(speed,0);
      if(Random.Range(0,2)==0)velocity.x*=-1;
      bulletType=GameManager.BULLET2;
      agression=2;
      timeToRetargetDefault=0.6f;
      velocityLerpVal=0.3f;
      firingDelay=0.4f;
    }
    //different destination than 1
    public override void Seek(GameObject defender){
      destination.y=defender.transform.localPosition.y;
      destination.x=position.x;
      base.Seek(defender);
    }
    public override void Roam(){
      velocity=new Vector2(speed,0);
      if(Random.Range(0,2)==0)velocity.x*=-1;
      base.Roam();
    }
}
public class Enemy3 : Enemy
{//uses bullet 3
    public Enemy3(GameObject gameObject, Vector2 initialPosition) : base(gameObject, initialPosition)
    {
      string textureName=GameManager.ENEMY3.ToLower();
      Sprite sprite= Sprite.Create(GameManager.Instance.textureManager.PackedTexture,GameManager.Instance.textureManager.GetTextureRectByName(textureName),new Vector2(0.5f,0.5f),1);
      spriteRenderer.sprite=sprite; 
      speed=100;
      velocity=new Vector2(speed,0);
      if(Random.Range(0,2)==0)velocity.x*=-1;
      bulletType=GameManager.BULLET3;
      agression=3;
      timeToRetargetDefault=0.5f;
      velocityLerpVal=0.3f;
      firingDelay=0.4f;
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
public class Enemy4 : Enemy
{//won't shoot, will flee, annoyance enemy
    public Enemy4(GameObject gameObject, Vector2 initialPosition) : base(gameObject, initialPosition)
    {
      string textureName=GameManager.ENEMY4.ToLower();
      Sprite sprite= Sprite.Create(GameManager.Instance.textureManager.PackedTexture,GameManager.Instance.textureManager.GetTextureRectByName(textureName),new Vector2(0.5f,0.5f),1);
      spriteRenderer.sprite=sprite; 

      speed=450;
      velocity=new Vector2(speed,0);
      if(Random.Range(0,2)==0)velocity.x*=-1;
    }
    public override void Seek(GameObject defender){
      velocity=new Vector2(speed,0);
      if(position.x<defender.transform.localPosition.x){
        velocity.x*=-1;
      }
      base.Roam();
    }
}
public class Enemy5 : Enemy
{//another fleeing annoying enemy, more fast
    public Enemy5(GameObject gameObject, Vector2 initialPosition) : base(gameObject, initialPosition)
    {
      string textureName=GameManager.ENEMY5.ToLower();
      Sprite sprite= Sprite.Create(GameManager.Instance.textureManager.PackedTexture,GameManager.Instance.textureManager.GetTextureRectByName(textureName),new Vector2(0.5f,0.5f),1);
      spriteRenderer.sprite=sprite; 
      
      speed=550;
      velocity=new Vector2(speed,0);
      if(Random.Range(0,2)==0)velocity.x*=-1;
    }
    public override void Seek(GameObject defender){
      velocity=new Vector2(speed,0);
      if(position.x<defender.transform.localPosition.x){
        velocity.x*=-1;
      }
      base.Roam();
    }
}
public class Enemy6 : Enemy
{//stays at place, sin bullet, moves across
    public Enemy6(GameObject gameObject, Vector2 initialPosition) : base(gameObject, initialPosition)
    {
      string textureName=GameManager.ENEMY12.ToLower();
      Sprite sprite= Sprite.Create(GameManager.Instance.textureManager.PackedTexture,GameManager.Instance.textureManager.GetTextureRectByName(textureName),new Vector2(0.5f,0.5f),1);
      spriteRenderer.sprite=sprite; 
      speed=300;
      bulletType=GameManager.BULLET3;
      agression=5;
      timeToRetargetDefault=0.05f;
      velocityLerpVal=0.3f;
      firingDelay=0.1f;
    }
    //moves across, hence harder to dodge
    public override void Seek(GameObject defender){
      destination.y=(position.y*-1)+(Random.Range(5,20)*Mathf.Sign(destination.y)*-1);
      destination.x=position.x*-1;
      base.Seek(defender);
    }
    public override void Roam(){
      velocity=Vector2.zero;
      base.Roam();
    }
}
public class Enemy7 : Enemy
{//same as 6, but flare bullet
    public Enemy7(GameObject gameObject, Vector2 initialPosition) : base(gameObject, initialPosition)
    {
      string textureName=GameManager.ENEMY7.ToLower();
      Sprite sprite= Sprite.Create(GameManager.Instance.textureManager.PackedTexture,GameManager.Instance.textureManager.GetTextureRectByName(textureName),new Vector2(0.5f,0.5f),1);
      spriteRenderer.sprite=sprite; 
      speed=200;
      bulletType=GameManager.BULLET4;
      agression=4;
      timeToRetargetDefault=0.1f;
      velocityLerpVal=0.3f;
      firingDelay=0.1f;
    }
    public override void Seek(GameObject defender){
      destination.y=(position.y*-1)+(Random.Range(5,20)*Mathf.Sign(destination.y)*-1);
      destination.x=position.x*-1;
      base.Seek(defender);
    }
    public override void Roam(){
      velocity=Vector2.zero;
      base.Roam();
    }
}
public class Enemy8 : Enemy
{//simple suicider, may shoot
    public Enemy8(GameObject gameObject, Vector2 initialPosition) : base(gameObject, initialPosition)
    {
      string textureName=GameManager.ENEMY8.ToLower();
      Sprite sprite= Sprite.Create(GameManager.Instance.textureManager.PackedTexture,GameManager.Instance.textureManager.GetTextureRectByName(textureName),new Vector2(0.5f,0.5f),1);
      spriteRenderer.sprite=sprite; 
      speed=200;
      bulletType=GameManager.BULLET1;
      agression=3;
      timeToRetargetDefault=0.1f;
      velocityLerpVal=0.5f;
      firingDelay=0.8f;
    }
    //homes on defender
    public override void Seek(GameObject defender){
      destination=defender.transform.localPosition;
      base.Seek(defender);
    }
    public override void Roam(){
      velocity=Vector2.zero;
      base.Roam();
    }
}
public class Enemy9 : Enemy
{//like 6 but homing bullets, low aggression
    public Enemy9(GameObject gameObject, Vector2 initialPosition) : base(gameObject, initialPosition)
    {
      string textureName=GameManager.ENEMY9.ToLower();
      Sprite sprite= Sprite.Create(GameManager.Instance.textureManager.PackedTexture,GameManager.Instance.textureManager.GetTextureRectByName(textureName),new Vector2(0.5f,0.5f),1);
      spriteRenderer.sprite=sprite; 
      speed=200;
      bulletType=GameManager.BULLET5;
      agression=4;
      timeToRetargetDefault=0.15f;
      velocityLerpVal=0.35f;
      firingDelay=0.5f;
    }
    public override void Seek(GameObject defender){
      destination.y=(position.y*-1)+(Random.Range(5,20)*Mathf.Sign(destination.y)*-1);
      destination.x=position.x*-1;
      base.Seek(defender);
    }
    public override void Roam(){
      velocity=Vector2.zero;
      base.Roam();
    }
}
public class Enemy10 : Enemy
{//aggressive suicider
    public Enemy10(GameObject gameObject, Vector2 initialPosition) : base(gameObject, initialPosition)
    {
      string textureName=GameManager.ENEMY10.ToLower();
      Sprite sprite= Sprite.Create(GameManager.Instance.textureManager.PackedTexture,GameManager.Instance.textureManager.GetTextureRectByName(textureName),new Vector2(0.5f,0.5f),1);
      spriteRenderer.sprite=sprite; 
      speed=200;
      bulletType=GameManager.BULLET2;
      agression=6;
      timeToRetargetDefault=0.1f;
      velocityLerpVal=0.5f;
      firingDelay=0.2f;
    }
    public override void Seek(GameObject defender){
      destination=defender.transform.localPosition;
      base.Seek(defender);
    }
    public override void Roam(){
      velocity=Vector2.zero;
      base.Roam();
    }
}
public class Enemy11 : Enemy
{//like enemy 6 but with homing bullet 5 high aggression, most interesting enemy
    public Enemy11(GameObject gameObject, Vector2 initialPosition) : base(gameObject, initialPosition)
    {
      string textureName=GameManager.ENEMY6.ToLower();
      Sprite sprite= Sprite.Create(GameManager.Instance.textureManager.PackedTexture,GameManager.Instance.textureManager.GetTextureRectByName(textureName),new Vector2(0.5f,0.5f),1);
      spriteRenderer.sprite=sprite; 
      speed=200;
      bulletType=GameManager.BULLET5;
      agression=7;
      timeToRetargetDefault=0.1f;
      velocityLerpVal=0.3f;
      firingDelay=0.3f;
    }
    public override void Seek(GameObject defender){
      destination.y=(position.y*-1)+(Random.Range(5,20)*Mathf.Sign(destination.y)*-1);
      destination.x=position.x*-1;
      base.Seek(defender);
    }
    public override void Roam(){
      velocity=Vector2.zero;
      base.Roam();
    }
}
public class Enemy12 : Enemy
{//suicide craft, rushes at you.
    Color savedColor;
    float amplitude=400;
    public Enemy12(GameObject gameObject, Vector2 initialPosition) : base(gameObject, initialPosition)
    {
      string textureName=GameManager.ENEMY11.ToLower();
      Sprite sprite= Sprite.Create(GameManager.Instance.textureManager.PackedTexture,GameManager.Instance.textureManager.GetTextureRectByName(textureName),new Vector2(0.5f,0.5f),1);
      spriteRenderer.sprite=sprite; 

      speed=500;
      velocity=new Vector2(speed,0);
      if(Random.Range(0,2)==0)velocity.x*=-1;
    }

  public override void Move(float deltaX, float deltaY){
    if(isCloseToDefender){
      Vector2 defenderPos=defenderRef.transform.localPosition;
      Vector2 desiredVelocity=(defenderPos-position).normalized*(speed);
      velocity=Vector2.Lerp(velocity,desiredVelocity,0.08f);
    }else{
      velocity.y=(Mathf.Sin(10*elapsedTime)*amplitude);
    }
    position.x+=deltaX+(velocity.x*Time.deltaTime);
		position.y+=deltaY+(velocity.y*Time.deltaTime);
		FixPosition();
		displayObject.transform.localPosition=position;
	}
  public override bool Tick(){
    elapsedTime+=Time.deltaTime;
    if(isTobeRemoved)return false;
    return true;
  }
//all suiciders get red paint when they target defender and home in.
    public override void Seek(GameObject defender){
      savedColor=paint;
      paint=Color.red;
      base.Seek(defender);
    }
    public override void Roam(){
      paint=savedColor;
      velocity=new Vector2(speed,0);
      if(Random.Range(0,2)==0)velocity.x*=-1;
      base.Roam();
    }
}