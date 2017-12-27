using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MovableObject
{
    public string bulletType;
    public float life=1.5f;
    
    public Bullet(GameObject gameObject, Vector2 initialPosition) : base(gameObject, initialPosition)
    {
    }

	public override void Move(float deltaX, float deltaY){
		base.Move(deltaX+(velocity.x*Time.deltaTime),deltaY+(velocity.y*Time.deltaTime));
	}
    public bool Tick(){
        life-=Time.deltaTime;
        if(life<=0)isTobeRemoved=true;
        if(position.x<GameManager.screenLeftRightLimits.x)isTobeRemoved=true;
		if(position.x>GameManager.screenLeftRightLimits.y)isTobeRemoved=true;
        if(isTobeRemoved)return false;
        return true;
    }
}
