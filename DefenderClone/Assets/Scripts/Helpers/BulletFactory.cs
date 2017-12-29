using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Factory pattern for bullets
 */
public class BulletFactory {

	public Bullet GetBullet(string whichBullet, Vector2 initialPosition){
		GameObject bulletGO=GameObject.Instantiate(GameManager.singleColliderPrefab,initialPosition,Quaternion.identity);
		bulletGO.transform.localScale=Vector2.one*6;
		bulletGO.name=whichBullet;
		if(whichBullet.Equals(GameManager.DEFENDER_BULLET)){
			return new HeroBullet(bulletGO,initialPosition);
		}else if(whichBullet.Equals(GameManager.BULLET1)){
			return new Bullet1(bulletGO,initialPosition);
		}else if(whichBullet.Equals(GameManager.BULLET2)){
			return new Bullet2(bulletGO,initialPosition);
		}else if(whichBullet.Equals(GameManager.BULLET3)){
			return new Bullet3(bulletGO,initialPosition);
		}else if(whichBullet.Equals(GameManager.BULLET4)){
			return new Bullet4(bulletGO,initialPosition);
		}else if(whichBullet.Equals(GameManager.BULLET5)){
			return new Bullet5(bulletGO,initialPosition);
		}else{Debug.LogError("no such bullet "+whichBullet);}
		return null;
	}
}
