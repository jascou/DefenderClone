using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager  {
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

    public GameState Tick(Vector2 pos)
    {
        camPos=mainCamera.transform.localPosition;
		scrollSpeed=scrollSpeedMax*((pos.x-camPos.x)/halfScreenWidth);
		
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

		defender.MoveTo(new Vector2(0,pos.y));
		if(pos.x<defender.position.x){
			defender.isFacingRight=false;
		}else defender.isFacingRight=true;

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
		AddEnemyBullets();
		bulletsToRemove.Clear();
		foreach(Bullet bullet in bulletsInScene.Values){
			if(bullet.Tick()){
				bullet.Move((-scrollSpeed*Time.deltaTime),0);
			}else{
				bulletsToRemove.Add(bullet);
			}
		}
		explosionsToRemove.Clear();
		foreach(PixelExplosion explosion in explosionsInScene){
			if(explosion.Tick()){
				explosion.Move((-scrollSpeed*Time.deltaTime),0);
			}else{
				explosionsToRemove.Add(explosion);
			}
		}
		CleanUp();
		if(craftsInScene.Count<enemiesAtATime){
			if(enemiesToSpawn.Count>0){
				AddEnemy();
			}else if(craftsInScene.Count==0){
				currentState=GameState.LevelUp;
			}
		}
		textureScrollSpeed=(scrollSpeed/5120);
		offset.x+=textureScrollSpeed*Time.deltaTime;
		scrollingMaterial.mainTextureOffset = offset;
		return currentState;
    }

	public Bullet AddBullet(string bulletName, Vector2 initialPosition){
		AudioManager.Main.PlayNewSound(GameManager.audios[0]);
		Bullet bullet=bulletFactory.GetBullet(bulletName, initialPosition);
		bulletsInScene.Add(bullet.displayObject,bullet);
		return bullet;
	}
	public Bullet AddBullet(string bulletName, Vector2 initialPosition, Color color){
		AudioManager.Main.PlayNewSound(GameManager.audios[1]);
		Bullet bullet=AddBullet(bulletName,initialPosition);
		bullet.paint=color;
		return bullet;
	}
	public Enemy AddEnemy(){
		if(enemiesToSpawn.Count==0)return null;
		float height=GameManager.topBottomLimits.y;
		int width=2100;
		Vector2 pos=new Vector2();
		int enemyIndex;
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

    public void CheckAndRemoveEnemyAndBullet(GameObject enemyObject, GameObject bulletObject)
    {//bullet has hit enemy or enemy bullet
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
						string intString=enemy.displayObject.name.Substring(6);
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
		if(foundItem){
			data=new Hashtable(1);
			data.Add("value",2*value);
			NotificationCenter.Notification note = new NotificationCenter.Notification (Camera.main, "EnemyKilled", data);
			NotificationCenter.DefaultCenter.PostNotification(note);
		}	
    }

    public void CheckAndRemoveEnemy(GameObject enemyObject)
    {//enemy or enemy bullet has hit defender
        Enemy enemy;
		bool foundItem=false;
		if(craftsInScene.TryGetValue(enemyObject, out enemy)){
			if(!enemy.isTobeRemoved){
				//Debug.Log("Defender hit "+enemyObject.name);
				enemy.isTobeRemoved=true;
				AddExplosion(defender.displayObject.transform.localPosition,defender.paint);
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
				NotificationCenter.DefaultCenter.PostNotification(Camera.main, "LifeLoss");
			}
		}
    }

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
	private PixelExplosion AddExplosion(Vector2 initialPosition, Color color)
    {
        AudioManager.Main.PlayNewSound(GameManager.audios[2]);
		PixelExplosion explosion=new PixelExplosion(pixelGO,initialPosition,color);
		explosionsInScene.Add(explosion);
		return explosion;
    }
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
