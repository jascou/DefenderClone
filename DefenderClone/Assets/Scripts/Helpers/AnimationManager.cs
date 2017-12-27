using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager  {

	//List<Enemy> craftsInScene;
	//List<Bullet> bulletsInScene;
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

	public AnimationManager(float maxScrollSpeed){
		scrollSpeedMax=maxScrollSpeed;
		craftsInScene=new Dictionary<GameObject, Enemy>();
		bulletsInScene=new Dictionary<GameObject, Bullet>();
		bulletsToRemove=new List<Bullet>();
		craftsToRemove=new List<Enemy>();
		craftsNeedingBullets=new List<Enemy>();
		bulletFactory=new BulletFactory();
		enemyFactory=new EnemyFactory();
		textureManager=GameManager.Instance.textureManager;
		halfScreenWidth=Screen.width/2;
	}
	public Bullet AddBullet(string bulletName, Vector2 initialPosition){
		Bullet bullet=bulletFactory.GetBullet(bulletName, initialPosition);
		bulletsInScene.Add(bullet.displayObject,bullet);
		return bullet;
	}
	public Enemy AddEnemy(string enemyName, Vector2 initialPosition){
		Enemy enemy=enemyFactory.GetEnemy(enemyName, initialPosition);
		craftsInScene.Add(enemy.displayObject,enemy);
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
			Bullet bullet= AddBullet(enemy.bulletType,enemy.position);
			enemy.needsShooting=false;
			if(enemy.position.x>defender.position.x){
				bullet.SetInitialVelocity(new Vector2(-1,0));
			}else{
				bullet.SetInitialVelocity(new Vector2(1,0));
			}
		}
	}

    internal void Tick(Vector2 pos)
    {
        defender.MoveTo(0,pos.y);
		if(pos.x<0){
			defender.isFacingRight=false;
		}else defender.isFacingRight=true;

		scrollSpeed=scrollSpeedMax*(pos.x/halfScreenWidth);
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
		CleanUp();
		textureScrollSpeed=(scrollSpeed/5120);
		offset.x+=textureScrollSpeed*Time.deltaTime;
		scrollingMaterial.mainTextureOffset = offset;
    }

    public void CheckAndRemoveEnemyAndBullet(GameObject enemyObject, GameObject bulletObject)
    {//bullet has hit enemy or enemy bullet
        Enemy enemy;
		Bullet bullet;
		bool foundItem=false;
		if(bulletsInScene.TryGetValue(bulletObject, out bullet)){
			if(!bullet.isTobeRemoved){
				if(craftsInScene.TryGetValue(enemyObject, out enemy)){
					if(!enemy.isTobeRemoved){
						bullet.isTobeRemoved=true;
						enemy.isTobeRemoved=true;
						Debug.Log("Bullet hit "+enemyObject.name);
					}
					foundItem=true;
				}
				if(!foundItem){
					Bullet enemyBullet;
					if(bulletsInScene.TryGetValue(enemyObject, out enemyBullet)){
						if(!enemyBullet.isTobeRemoved){
							bullet.isTobeRemoved=true;
							enemyBullet.isTobeRemoved=true;
							Debug.Log("Bullet hit "+enemyObject.name);
						}
					}
				}
			}
		}
		
    }

    public void CheckAndRemoveEnemy(GameObject enemyObject)
    {//enemy or enemy bullet has hit defender
        Enemy enemy;
		bool foundItem=false;
		if(craftsInScene.TryGetValue(enemyObject, out enemy)){
			if(!enemy.isTobeRemoved){
				Debug.Log("Defender hit "+enemyObject.name);
				enemy.isTobeRemoved=true;
			}
			foundItem=true;
		}
		if(foundItem)return;
		Bullet bullet;
		if(bulletsInScene.TryGetValue(enemyObject, out bullet)){
			if(!bullet.isTobeRemoved){
				Debug.Log("Defender hit "+enemyObject.name);
				bullet.isTobeRemoved=true;	
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
		craft.transform.localScale=Vector2.one*10;
		defender=new Defender(craft,Vector2.zero);
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
    }
}
