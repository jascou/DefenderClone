using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFactory {
	
	public Enemy GetEnemy(string whichEnemy, Vector2 initialPosition){
		string textureName=whichEnemy.ToLower();
		
		GameObject enemyGO=GameObject.Instantiate(GameManager.singleColliderPrefab,initialPosition,Quaternion.identity);
		enemyGO.transform.localScale=Vector2.one*6;
		enemyGO.name=whichEnemy;

		if(whichEnemy.Equals(GameManager.ENEMY1)){
			return new Enemy1(enemyGO,initialPosition);
		}else if(whichEnemy.Equals(GameManager.ENEMY2)){
			return new Enemy2(enemyGO,initialPosition);
		}else if(whichEnemy.Equals(GameManager.ENEMY3)){
			return new Enemy3(enemyGO,initialPosition);
		}else if(whichEnemy.Equals(GameManager.ENEMY4)){
			return new Enemy4(enemyGO,initialPosition);
		}else if(whichEnemy.Equals(GameManager.ENEMY5)){
			return new Enemy5(enemyGO,initialPosition);
		}else if(whichEnemy.Equals(GameManager.ENEMY6)){
			return new Enemy6(enemyGO,initialPosition);
		}else if(whichEnemy.Equals(GameManager.ENEMY7)){
			return new Enemy7(enemyGO,initialPosition);
		}else if(whichEnemy.Equals(GameManager.ENEMY8)){
			return new Enemy8(enemyGO,initialPosition);
		}else if(whichEnemy.Equals(GameManager.ENEMY9)){
			return new Enemy9(enemyGO,initialPosition);
		}else if(whichEnemy.Equals(GameManager.ENEMY10)){
			return new Enemy10(enemyGO,initialPosition);
		}else if(whichEnemy.Equals(GameManager.ENEMY11)){
			return new Enemy11(enemyGO,initialPosition);
		}else if(whichEnemy.Equals(GameManager.ENEMY12)){
			return new Enemy12(enemyGO,initialPosition);
		}else{Debug.LogError("no such enemy "+whichEnemy);}
		return null;
	}
}
