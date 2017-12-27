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

	public AnimationManager(float maxScrollSpeed){
		scrollSpeedMax=maxScrollSpeed;
		craftsInScene=new Dictionary<GameObject, Enemy>();
		bulletsInScene=new Dictionary<GameObject, Bullet>();
		bulletsToRemove=new List<Bullet>();
		craftsToRemove=new List<Enemy>();
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

    internal void Tick(Vector2 pos)
    {
        defender.MoveTo(0,pos.y);
		if(pos.x<0){
			defender.isFacingRight=false;
		}else defender.isFacingRight=true;

		scrollSpeed=scrollSpeedMax*(pos.x/halfScreenWidth);
		craftsToRemove.Clear();
		foreach(Enemy enemy in craftsInScene.Values){
			if(enemy.Tick()){
				enemy.Move((-scrollSpeed*Time.deltaTime),0);
			}else{
				craftsToRemove.Add(enemy);
			}
			
		}
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
    {
        Enemy enemy;
		Bullet bullet;
		if(craftsInScene.TryGetValue(enemyObject, out enemy)){
			if(!enemy.isTobeRemoved){
				if(bulletsInScene.TryGetValue(bulletObject, out bullet)){
					if(!bullet.isTobeRemoved){
						Debug.Log("Bullet hit "+enemyObject.name);
						bullet.isTobeRemoved=true;
						enemy.isTobeRemoved=true;
					}
				}
			}
		}
    }

    public void CheckAndRemoveEnemy(GameObject enemyObject)
    {
        Enemy enemy;
		if(craftsInScene.TryGetValue(enemyObject, out enemy)){
			if(!enemy.isTobeRemoved){
				Debug.Log("Defender hit "+enemyObject.name);
				enemy.isTobeRemoved=true;
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

    public void FireDefender()
    {
        if(!defender.isReadyToFireAgain())return;
		defender.SetFireTime();
        Bullet bullet= AddBullet(GameManager.DEFENDER_BULLET,defender.position);
		bullet.velocity=new Vector2(1000,0);
		if(!defender.isFacingRight){
			bullet.velocity.x=-1000;
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
