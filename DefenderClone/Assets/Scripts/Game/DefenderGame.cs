using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DefenderGame : MonoBehaviour {
	public Material scrollingMaterial;
	public float scrollSpeedMax;
	public GameObject twinColliderPrefab;
	public GameObject singleColliderPrefab;
	public Camera miniMapCam;
	public GameObject menuPanel;
	public GameObject UIPanel;
	public Text scoreText;
	public Text muxText;

	public Color[] colorPallette;
	
	bool isMouseDown;
	AnimationManager animationManager;
	bool hasGameStarted=false;
	int score;
	int mux;
	void Awake(){
		menuPanel.SetActive(true);
		UIPanel.SetActive(false);
		miniMapCam.enabled=false;
	}

	void Start () {
		GameManager.Instance.Initialise(singleColliderPrefab,twinColliderPrefab);
		GameManager.Instance.InitLevel(-2560,(Screen.height/2)-120,2560,(Screen.height/-2)+50);
		animationManager=new AnimationManager(scrollSpeedMax);
		GameManager.colorPallette=colorPallette;
		animationManager.PreparePixelExplosion();
		CreateSprites();
		isMouseDown=false;
		hasGameStarted=false;
		NotificationCenter.DefaultCenter.AddObserver (this, "BulletHitAlert");
		NotificationCenter.DefaultCenter.AddObserver (this, "HitAlert");
		NotificationCenter.DefaultCenter.AddObserver (this, "ProximityAlert");
		NotificationCenter.DefaultCenter.AddObserver (this, "NonProximityAlert");
	}
	void Update () {
		if(!hasGameStarted)return;
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
		if(!hasGameStarted)return;
		Hashtable data = note.data;
		GameObject who=(GameObject)data["who"];
		animationManager.InformProximity(who, true);//sent seek alert
	}
	void NonProximityAlert(NotificationCenter.Notification note){//enemy or enemy bullet exits hero zone
		if(!hasGameStarted)return;
		Hashtable data = note.data;
		GameObject who=(GameObject)data["who"];
		animationManager.InformProximity(who, false);//sent roam alert
	}
	void HitAlert(NotificationCenter.Notification note){//hero collides with enemy or enemy bullet
		if(!hasGameStarted)return;
		Hashtable data = note.data;
		GameObject who=(GameObject)data["who"];
		animationManager.CheckAndRemoveEnemy(who);//check for defenders life status
	}
	void BulletHitAlert(NotificationCenter.Notification note){//bullet hits enemy or enemy bullet
		if(!hasGameStarted)return;
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
		}

		animationManager.AddDefender();

		animationManager.AddBackground(scrollingMaterial);
    }
	void updateUI(){
		muxText.text=mux.ToString()+" x";
		scoreText.text=score.ToString()+" x";
	}
	public void StartGame(){
		GameManager.cheatEnabled=false;
		miniMapCam.enabled=true;
		hasGameStarted=true;
		menuPanel.SetActive(false);
		UIPanel.SetActive(true);
		score=0;
		mux=1;
		updateUI();
	}
	public void StartCheatGame(){
		StartGame();
		GameManager.cheatEnabled=true;
	}
}
