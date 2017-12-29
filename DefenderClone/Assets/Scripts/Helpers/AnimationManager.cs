using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
The brain of the game and hence the biggest class. Could be optimised further :)
Handles all graphics addition, removal and motion.
 */
public class AnimationManager  {
	//tacks game state
	public enum GameState
    {
        Active,
        LevelUp
    };

	Dictionary<GameObject,Enemy> craftsInScene;
	Dictionary<GameObject,Bullet> bulletsInScene;
	Defender defender;
	BulletFactory bulletFactory;
	EnemyFactory enemyFactory;
	float scrollSpeed=0;
	float textureScrollSpeed=0;
	float scrollSpeedMax;
	TextureManager textureManager;
	Vector2 offset=new Vector2();
	Material scrollingMaterial;
	float halfScreenWidth;
	List<Bullet> bulletsToRemove;
	List<Enemy> craftsToRemove;
	List<Enemy> craftsNeedingBullets;
	List<PixelExplosion> explosionsToRemove;
	List<PixelExplosion> explosionsInScene;

	Camera mainCamera;
	Vector3 camPos;
	float scrollVelocity;
	GameObject pixelGO;
	List<string> enemiesToSpawn;
	int enemiesAtATime;
	GameState currentState;
	Hashtable data=new Hashtable(1);

	public AnimationManager(float maxScrollSpeed, List<string> newEnemiesList, int enemyPresence){
		enemiesAtATime=enemyPresence;
		enemiesToSpawn=newEnemiesList;
		scrollSpeedMax=maxScrollSpeed;
		craftsInScene=new Dictionary<GameObject, Enemy>();
		bulletsInScene=new Dictionary<GameObject, Bullet>();
		bulletsToRemove=new List<Bullet>();
		craftsToRemove=new List<Enemy>();
		craftsNeedingBullets=new List<Enemy>();
		explosionsInScene=new List<PixelExplosion>();
		explosionsToRemove=new List<PixelExplosion>();
		bulletFactory=new BulletFactory();
		enemyFactory=new EnemyFactory();
		textureManager=GameManager.Instance.textureManager;
		halfScreenWidth=Screen.width/2;
		mainCamera=Camera.main;
		currentState=GameState.Active;
	}
	/*** Graphics addition ***/
	//add bullet to scene
	public Bullet AddBullet(string bulletName, Vector2 initialPosition){
		AudioManager.Main.PlayNewSound(GameManager.audios[0]);
		Bullet bullet=bulletFactory.GetBullet(bulletName, initialPosition);
		bulletsInScene.Add(bullet.displayObject,bullet);
		return bullet;
	}
	//variant with color
	public Bullet AddBullet(string bulletName, Vector2 initialPosition, Color color){
		AudioManager.Main.PlayNewSound(GameManager.audios[1]);
		Bullet bullet=AddBullet(bulletName,initialPosition);
		bullet.paint=color;
		return bullet;
	}
	//add enemy to scene
	public Enemy AddEnemy(){
		if(enemiesToSpawn.Count==0)return null;
		float height=GameManager.topBottomLimits.y;
		int width=2100;
		Vector2 pos=new Vector2();
		int enemyIndex;
		//always out of screen as we dont want hero to collide on spawn
		pos.x=((2*Random.Range(0,2))-1)*(450+Random.Range(0,width));
		pos.y=((2*Random.Range(0,2))-1)*Random.Range(0,height);
		enemyIndex=Random.Range(0,enemiesToSpawn.Count);
		
		Enemy enemy=enemyFactory.GetEnemy(enemiesToSpawn[enemyIndex], pos);
		craftsInScene.Add(enemy.displayObject,enemy);
		string intString=enemiesToSpawn[enemyIndex].Substring(6);
		enemy.paint=GameManager.colorPallette[int.Parse(intString)-1];
		//Debug.Log("Launch "+enemiesToSpawn[enemyIndex]);
		
		enemiesToSpawn.RemoveAt(enemyIndex);
		return enemy;
	}
	//provide bullet to all enemies who is firing this frame
	public void AddEnemyBullets(){
		foreach(Enemy enemy in craftsNeedingBullets){
			Bullet bullet= AddBullet(enemy.bulletType,enemy.position,enemy.paint);
			enemy.needsShooting=false;
			if(!enemy.bulletType.Equals(GameManager.BULLET4)&&!enemy.bulletType.Equals(GameManager.BULLET5)){
				if(enemy.position.x>defender.position.x){
					bullet.SetInitialVelocity(new Vector2(-1,0));
				}else{
					bullet.SetInitialVelocity(new Vector2(1,0));
				}
			}
		}
	}
	//add defender
    public void AddDefender()
    {
        GameObject craft=GameObject.Instantiate(GameManager.twinColliderPrefab,Vector2.zero,Quaternion.identity);
		SpriteRenderer sr = craft.AddComponent<SpriteRenderer>();
		Sprite sprite= Sprite.Create(textureManager.PackedTexture,textureManager.GetTextureRectByName("Defender"),new Vector2(0.5f,0.5f),1);
		sr.sprite=sprite; 
		sr.sortingLayerName="hero";
		craft.transform.localScale=Vector2.one*10;
		defender=new Defender(craft,Vector2.zero);
		defender.paint=Color.white;
		craft.name="defender";
    }
	//add BG. the lower half is just a single graphic scaled to fit camera, 
	//top portion is terrain graphic dynamically created
    public void AddBackground(Material newScrollingMaterial)
    {
        scrollingMaterial=newScrollingMaterial;
		GameObject gameObject=new GameObject();
		SpriteRenderer terrainRenderer = gameObject.AddComponent<SpriteRenderer>();
		terrainRenderer.material=scrollingMaterial;
		Sprite sprite= Sprite.Create(textureManager.TerrainTex,textureManager.GetTextureRectByName("terrain"),new Vector2(0.5f,0.5f),1);
		terrainRenderer.sprite=sprite; 
		gameObject.name="background";
		gameObject.transform.localScale=Vector2.one*20;

		gameObject=new GameObject();
		SpriteRenderer sr = gameObject.AddComponent<SpriteRenderer>();
		sprite= Sprite.Create(textureManager.PackedTexture,textureManager.GetTextureRectByName("bullets5"),new Vector2(0.5f,0.5f),1);
		sr.sprite=sprite;
		Color color;
		ColorUtility.TryParseHtmlString("#2A2D33", out color);
		sr.color=color; 
		gameObject.name="ground";
		gameObject.transform.localScale=new Vector2(5500,200);
		gameObject.transform.localPosition=new Vector2(0,-236);
    }
	//add a pixel explosion
	private PixelExplosion AddExplosion(Vector2 initialPosition, Color color)
    {
        AudioManager.Main.PlayNewSound(GameManager.audios[2]);
		PixelExplosion explosion=new PixelExplosion(pixelGO,initialPosition,color);
		explosionsInScene.Add(explosion);
		return explosion;
    }

	/*** Engine ***/

    public GameState Tick(Vector2 pos)
    {
        camPos=mainCamera.transform.localPosition;//scrolling speed in relation to mouse position
		scrollSpeed=scrollSpeedMax*((pos.x-camPos.x)/halfScreenWidth);
		//move camera first, then start scrolling
		if(scrollSpeed>0 && camPos.x<GameManager.cameraLeftRightLimits.y){
			camPos.x+=scrollSpeed*Time.deltaTime;
			scrollSpeed=0;
		}
		if(scrollSpeed<0 && camPos.x>GameManager.cameraLeftRightLimits.x){
			camPos.x+=scrollSpeed*Time.deltaTime;
			scrollSpeed=0;
		}
		camPos.x=Mathf.Clamp(camPos.x,GameManager.cameraLeftRightLimits.x,GameManager.cameraLeftRightLimits.y);
		mainCamera.transform.localPosition=camPos;
		//move defender to mouse y
		defender.MoveTo(new Vector2(0,pos.y));
		//switch defender facing based on mouse x
		if(pos.x<defender.position.x){
			defender.isFacingRight=false;
		}else defender.isFacingRight=true;
		//loop through crafts in scene to determine which needs removal and which needs bullets
		craftsToRemove.Clear();
		craftsNeedingBullets.Clear();
		foreach(Enemy enemy in craftsInScene.Values){
			if(enemy.Tick()){
				enemy.Move((-scrollSpeed*Time.deltaTime),0);
				if(enemy.needsShooting)craftsNeedingBullets.Add(enemy);
			}else{
				craftsToRemove.Add(enemy);
			}	
		}
		//assign bullets to those in need
		AddEnemyBullets();
		//do removal search fo all bullets in scene
		bulletsToRemove.Clear();
		foreach(Bullet bullet in bulletsInScene.Values){
			if(bullet.Tick()){
				bullet.Move((-scrollSpeed*Time.deltaTime),0);
			}else{
				bulletsToRemove.Add(bullet);
			}
		}
		//removal search for explosions in scene
		explosionsToRemove.Clear();
		foreach(PixelExplosion explosion in explosionsInScene){
			if(explosion.Tick()){
				explosion.Move((-scrollSpeed*Time.deltaTime),0);
			}else{
				explosionsToRemove.Add(explosion);
			}
		}
		//remove those needs removal this frame
		CleanUp();
		//if there are less crafts in scene than the level mandates then launch new enemy
		if(craftsInScene.Count<enemiesAtATime){
			if(enemiesToSpawn.Count>0){
				AddEnemy();
			}else if(craftsInScene.Count==0){
				//if we have run out of enemy crafts, then level up
				currentState=GameState.LevelUp;
			}
		}
		//convert scroll speed based on texture width to find texture scroll speed 
		textureScrollSpeed=(scrollSpeed/5120);
		offset.x+=textureScrollSpeed*Time.deltaTime;
		//scroll terrain texture to match enemy scroll speed.
		scrollingMaterial.mainTextureOffset = offset;
		return currentState;
    }

	/*** Graphics cleanup (section has minor code duplication) ***/

	//Defender bullet has hit enemy or enemy bullet
    public void CheckAndRemoveEnemyAndBullet(GameObject enemyObject, GameObject bulletObject)
    {
        Enemy enemy;
		Bullet bullet;
		bool foundItem=false;
		int value=0;
		if(bulletsInScene.TryGetValue(bulletObject, out bullet)){
			if(!bullet.isTobeRemoved){
				if(craftsInScene.TryGetValue(enemyObject, out enemy)){
					if(!enemy.isTobeRemoved){
						bullet.isTobeRemoved=true;
						enemy.isTobeRemoved=true;
						//Debug.Log("Bullet hit "+enemyObject.name);
						AddExplosion(enemyObject.transform.localPosition,enemy.paint);
						string intString=enemy.displayObject.name.Substring(6);//not very elegant or performant, surely could be saved as a enemy variable
						value=int.Parse(intString);
					}
					foundItem=true;
				}
				if(!foundItem){
					Bullet enemyBullet;
					if(bulletsInScene.TryGetValue(enemyObject, out enemyBullet)){
						if(!enemyBullet.isTobeRemoved){
							bullet.isTobeRemoved=true;
							enemyBullet.isTobeRemoved=true;
							//Debug.Log("Bullet hit "+enemyObject.name);
							value=1;
						}
					}
				}
			}
		}
		if(foundItem){//dispatch score event
			data=new Hashtable(1);
			data.Add("value",2*value);
			NotificationCenter.Notification note = new NotificationCenter.Notification (Camera.main, "EnemyKilled", data);
			NotificationCenter.DefaultCenter.PostNotification(note);
		}	
    }
	//enemy or enemy bullet has hit defender
    public void CheckAndRemoveEnemy(GameObject enemyObject)
    {
        Enemy enemy;
		bool foundItem=false;
		if(craftsInScene.TryGetValue(enemyObject, out enemy)){
			if(!enemy.isTobeRemoved){
				//Debug.Log("Defender hit "+enemyObject.name);
				enemy.isTobeRemoved=true;
				AddExplosion(defender.displayObject.transform.localPosition,defender.paint);
				//dispatch life loss event
				NotificationCenter.DefaultCenter.PostNotification(Camera.main, "LifeLoss");
			}
			foundItem=true;
		}
		if(foundItem)return;
		Bullet bullet;
		if(bulletsInScene.TryGetValue(enemyObject, out bullet)){
			if(!bullet.isTobeRemoved){
				//Debug.Log("Defender hit "+enemyObject.name);
				bullet.isTobeRemoved=true;	
				AddExplosion(defender.displayObject.transform.localPosition,defender.paint);
				//dispatch life loss event
				NotificationCenter.DefaultCenter.PostNotification(Camera.main, "LifeLoss");
			}
		}
    }
	//remove all graphics which needs removal this frame
    private void CleanUp()
    {
        foreach(Bullet bullet in bulletsToRemove){
			bulletsInScene.Remove(bullet.displayObject);
			bullet.Remove();
		}
		foreach(Enemy enemy in craftsToRemove){
			craftsInScene.Remove(enemy.displayObject);
			enemy.Remove();
		}
		foreach(PixelExplosion explosion in explosionsToRemove){
			explosionsInScene.Remove(explosion);
			explosion.Remove();
		}
    }

	/*** Support functionality ***/
	//fire defender
	public void FireDefender()
    {
        if(!defender.isReadyToFireAgain())return;
		defender.SetFireTime();
        Bullet bullet= AddBullet(GameManager.DEFENDER_BULLET,defender.position);
		
		if(!defender.isFacingRight){
			bullet.SetInitialVelocity(new Vector2(-1,0));
		}else{
			bullet.SetInitialVelocity(new Vector2(1,0));
		}
    }
	//inform a specific enemy or enemy bullet that it has come near or has gone away from hero proximity
	//this makes sure the corresponding seek and roam methods are fired
	public void InformProximity(GameObject gameObject, bool doSeek)
    {
        Enemy enemy;
		Bullet bullet;
		bool foundItem=false;
		if(craftsInScene.TryGetValue(gameObject, out enemy)){
			if(!enemy.isTobeRemoved){
				if(doSeek){
					enemy.Seek(defender.displayObject);
				}	
			}
			if(!doSeek)enemy.Roam();
			foundItem=true;
		}
		if(foundItem)return;
		if(bulletsInScene.TryGetValue(gameObject, out bullet)){
			if(!bullet.isTobeRemoved){
				if(doSeek){
					bullet.Seek(defender.displayObject);
				}	
			}
			if(!doSeek)bullet.Roam();
		}
    }
	//prepare and pool the game objects for pixel explosion
	public void PreparePixelExplosion()
    {
        pixelGO=new GameObject();
		SpriteRenderer sr = pixelGO.AddComponent<SpriteRenderer>();
		Sprite sprite= Sprite.Create(textureManager.PackedTexture,textureManager.GetTextureRectByName("bullets5"),new Vector2(0.5f,0.5f),1);
		sr.sprite=sprite;
		sr.sortingLayerName="hero";
		pixelGO.name="pixel";
		pixelGO.transform.localScale=new Vector2(10,10);
		SimplePool.Preload(pixelGO,20);
		//Component.Destroy(pixelGO);
		pixelGO.SetActive(false);
    }
}
