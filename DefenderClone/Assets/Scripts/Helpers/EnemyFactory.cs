using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFactory {

	public Enemy GetEnemy(string whichEnemy, Vector2 initialPosition){
		string textureName=whichEnemy.ToLower();

		GameObject enemyGO=GameObject.Instantiate(GameManager.singleColliderPrefab,initialPosition,Quaternion.identity);
		SpriteRenderer sr = enemyGO.AddComponent<SpriteRenderer>();
		Sprite sprite= Sprite.Create(GameManager.Instance.textureManager.PackedTexture,GameManager.Instance.textureManager.GetTextureRectByName(textureName),new Vector2(0.5f,0.5f),1);
		sr.sprite=sprite; 
		enemyGO.transform.localScale=Vector2.one*6;
		enemyGO.name=whichEnemy;
		Enemy enemy =new Enemy(enemyGO,initialPosition);
		enemy.enemyType=whichEnemy;
		BoxCollider2D boxCollider2D=enemyGO.GetComponent<BoxCollider2D>();
		boxCollider2D.size= new Vector2(4,4);
		TriggerDispatcher dispatcher=enemyGO.GetComponent<TriggerDispatcher>();
		Component.Destroy(dispatcher);
		enemyGO.layer = LayerMask.NameToLayer("enemy");
		return enemy;
	}
}
