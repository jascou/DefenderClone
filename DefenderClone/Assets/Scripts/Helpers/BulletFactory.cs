using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFactory {

	public Bullet GetBullet(string whichBullet, Vector2 initialPosition){
		bool isDefenderBullet=false;
		if(whichBullet.Equals(GameManager.DEFENDER_BULLET)){
			whichBullet=GameManager.BULLET1;
			isDefenderBullet=true;
		}
		string textureName=whichBullet.ToLower();

		GameObject bulletGO=GameObject.Instantiate(GameManager.singleColliderPrefab,initialPosition,Quaternion.identity);
		SpriteRenderer sr = bulletGO.AddComponent<SpriteRenderer>();
		Sprite sprite= Sprite.Create(GameManager.Instance.textureManager.PackedTexture,GameManager.Instance.textureManager.GetTextureRectByName(textureName),new Vector2(0.5f,0.5f),1);
		sr.sprite=sprite; 
		bulletGO.transform.localScale=Vector2.one*6;
		bulletGO.name=whichBullet;
		Bullet bullet=new Bullet(bulletGO,initialPosition);
		bullet.bulletType=whichBullet;
		BoxCollider2D boxCollider2D=bulletGO.GetComponent<BoxCollider2D>();
		boxCollider2D.size= new Vector2(1,1);
		if(!isDefenderBullet){
			TriggerDispatcher dispatcher=bulletGO.GetComponent<TriggerDispatcher>();
			Component.Destroy(dispatcher);
			bulletGO.layer = LayerMask.NameToLayer("enemybullet");
		}else{
			
		}
		return bullet;
	}
}
