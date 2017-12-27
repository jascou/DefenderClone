using System;
using UnityEngine;

public class Enemy : MovableObject{
  public string enemyType;
  public Enemy(GameObject gameObject, Vector2 initialPosition) : base(gameObject, initialPosition)
  {	
  }
  public override void Move(float deltaX, float deltaY){
    base.Move(deltaX+(velocity.x*Time.deltaTime),deltaY+(velocity.y*Time.deltaTime));
	}
  public bool Tick(){
    if(isTobeRemoved)return false;
    return true;
  }
}
