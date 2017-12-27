using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenderGame : MonoBehaviour {
	public Material scrollingMaterial;
	public float scrollSpeedMax;
	public GameObject twinColliderPrefab;
	public GameObject singleColliderPrefab;
	
	bool isMouseDown;
	AnimationManager animationManager;

	void Start () {
		GameManager.Instance.Initialise(singleColliderPrefab,twinColliderPrefab);
		GameManager.Instance.InitLevel(-2560,(Screen.height/2)-120,2560,(Screen.height/-2)+50);
		animationManager=new AnimationManager(scrollSpeedMax);
		CreateSprites();
		isMouseDown=false;
		NotificationCenter.DefaultCenter.AddObserver (this, "BulletHitAlert");
		NotificationCenter.DefaultCenter.AddObserver (this, "HitAlert");
		NotificationCenter.DefaultCenter.AddObserver (this, "ProximityAlert");
		NotificationCenter.DefaultCenter.AddObserver (this, "NonProximityAlert");
	}
	void Update () {
		Vector2 pos= Camera.main.ScreenToWorldPoint(Input.mousePosition);
		animationManager.Tick(pos);
		
		if(Input.GetMouseButtonDown(0)){
			isMouseDown=true;
		}
		if(Input.GetMouseButtonUp(0)){
			isMouseDown=false;
		}
		
		if(isMouseDown)animationManager.FireDefender();
	}
	void ProximityAlert(NotificationCenter.Notification note){//enemy or enemy bullet enters hero zone
		Hashtable data = note.data;
		GameObject who=(GameObject)data["who"];
		Debug.Log("Enter "+who.name);
	}
	void NonProximityAlert(NotificationCenter.Notification note){//enemy or enemy bullet exits hero zone
		Hashtable data = note.data;
		GameObject who=(GameObject)data["who"];
		Debug.Log("Exit "+who.name);
	}
	void HitAlert(NotificationCenter.Notification note){//hero collides with enemy or enemy bullet
		Hashtable data = note.data;
		GameObject who=(GameObject)data["who"];
		animationManager.CheckAndRemoveEnemy(who);//check for defenders life status
	}
	void BulletHitAlert(NotificationCenter.Notification note){//bullet hits enemy or enemy bullet
		Hashtable data = note.data;
		GameObject who=(GameObject)data["who"];
		animationManager.CheckAndRemoveEnemyAndBullet(who,note.sender.gameObject);
	}

    private void CreateSprites()
    {
        int numCrafts=1;
		int height=Screen.height/2;
		int width=2500;
		Vector2 pos=new Vector2();
		for (int i=0;i<numCrafts;i++){
			pos.x=((2*Random.Range(0,2))-1)*Random.Range(0,width);
			pos.y=((2*Random.Range(0,2))-1)*Random.Range(0,height);
			Enemy enemy= animationManager.AddEnemy(GameManager.ENEMY1,pos);
			enemy.paint=Color.red;
		}

		animationManager.AddDefender();

		animationManager.AddBackground(scrollingMaterial);
    }
}
