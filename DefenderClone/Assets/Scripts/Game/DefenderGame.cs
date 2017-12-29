using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
/*
This is the scene script which is attached to any gameobject on screen to drive the game
 */

public class DefenderGame : MonoBehaviour {
	public Material scrollingMaterial;
	public float scrollSpeedMax;
	public GameObject twinColliderPrefab;
	public GameObject singleColliderPrefab;
	public Camera miniMapCam;
	public GameObject menuPanel;
	public GameObject UIPanel;
	public Text scoreText;
	public Text lifeText;
	public Text levelText;
	public AudioClip[] audios;
	public AnimationCurve enemiesAtATimeCurve;
	public Color[] colorPallette;
	AnimationManager.GameState gameState;
	int minEnemiesInLevel;
	
	string[] availableEnemies={GameManager.ENEMY1,GameManager.ENEMY2,GameManager.ENEMY3,
		GameManager.ENEMY4,GameManager.ENEMY5,GameManager.ENEMY6,GameManager.ENEMY7,GameManager.ENEMY8,GameManager.ENEMY9,
		GameManager.ENEMY10,GameManager.ENEMY11,GameManager.ENEMY12};
	
	bool isMouseDown;
	AnimationManager animationManager;
	bool hasGameStarted=false;
	List<string> enemiesToSpawn;
	int enemiesAtATime;
	

	void Start () {
		if(!GameManager.isLevelingUp){//Game Launch, run one time in life time of the game.
			GameManager.Instance.Initialise(singleColliderPrefab,twinColliderPrefab);
			GameManager.Instance.InitLevel(-2560,(Screen.height/2)-120,2560,(Screen.height/-2)+50);
			GameManager.colorPallette=colorPallette;
			GameManager.audios=audios;
			menuPanel.SetActive(true);
			UIPanel.SetActive(false);
			miniMapCam.enabled=false;
			hasGameStarted=false;
		}
		//load level specific data
		GetLevelSettings();
		//animation manager handles all graphics and animation
		animationManager=new AnimationManager(scrollSpeedMax,enemiesToSpawn,enemiesAtATime);
		//pool the explosion effect
		animationManager.PreparePixelExplosion();
		//add defender and bg
		animationManager.AddDefender();
		animationManager.AddBackground(scrollingMaterial);

		isMouseDown=false;

		//listen to async events
		NotificationCenter.DefaultCenter.AddObserver (this, "BulletHitAlert");
		NotificationCenter.DefaultCenter.AddObserver (this, "HitAlert");
		NotificationCenter.DefaultCenter.AddObserver (this, "ProximityAlert");
		NotificationCenter.DefaultCenter.AddObserver (this, "NonProximityAlert");
		NotificationCenter.DefaultCenter.AddObserver (this, "LifeLoss");
		NotificationCenter.DefaultCenter.AddObserver (this, "EnemyKilled");

		if(GameManager.isLevelingUp){
			updateUI();
			//launch the initial number of enemies
			LaunchEnemies(enemiesAtATime);
			GameManager.isLevelingUp=false;
			hasGameStarted=true;
		}
	}
    void Update () {//game loop
		if(!hasGameStarted)return;
		Vector2 pos= Camera.main.ScreenToWorldPoint(Input.mousePosition);
		
		//this is where all action happens
		gameState=animationManager.Tick(pos);
		//..

		if(gameState.Equals(AnimationManager.GameState.LevelUp)){
			LevelUp();
		}else{
			if(Input.GetMouseButtonDown(0)){
				isMouseDown=true;
			}
			if(Input.GetMouseButtonUp(0)){
				isMouseDown=false;
			}
			//fire on mousedown
			if(isMouseDown)animationManager.FireDefender();
		}
	}

    void ProximityAlert(NotificationCenter.Notification note){//event fired when enemy or enemy bullet enters hero zone
		if(!hasGameStarted)return;
		Hashtable data = note.data;
		GameObject who=(GameObject)data["who"];
		animationManager.InformProximity(who, true);//sent seek alert
	}
	void NonProximityAlert(NotificationCenter.Notification note){//event fired when enemy or enemy bullet exits hero zone
		if(!hasGameStarted)return;
		Hashtable data = note.data;
		GameObject who=(GameObject)data["who"];
		animationManager.InformProximity(who, false);//sent roam alert
	}
	void HitAlert(NotificationCenter.Notification note){//event fired when hero collides with enemy or enemy bullet
		if(!hasGameStarted)return;
		Hashtable data = note.data;
		GameObject who=(GameObject)data["who"];
		animationManager.CheckAndRemoveEnemy(who);//check for defenders life status?
	}
	void BulletHitAlert(NotificationCenter.Notification note){//event fired when bullet hits enemy or enemy bullet
		if(!hasGameStarted)return;
		Hashtable data = note.data;
		GameObject who=(GameObject)data["who"];
		animationManager.CheckAndRemoveEnemyAndBullet(who,note.sender.gameObject);
	}
	void LifeLoss(NotificationCenter.Notification note){//event fired when life is lost
		if(GameManager.cheatEnabled)return;
		GameManager.Instance.life--;
		updateUI();
		if(GameManager.Instance.life<=0)GameOver();
	}
	void EnemyKilled(NotificationCenter.Notification note){//event fired when enemy is killed
		Hashtable data = note.data;
		int howMuch=(int)data["value"];//enemy specific score value
		GameManager.Instance.totalScore+=howMuch;
		updateUI();
	}

	//initial launch of enemies
    private void LaunchEnemies(int numCrafts)
    {
		for (int i=0;i<numCrafts;i++){
			animationManager.AddEnemy();
		}
    }
	//level specific settings
    private void GetLevelSettings()
    {
        //get current level or cheat here to load specific level
		int currentLevel=GameManager.Instance.currentLevel;//=14;
		//level will have enemies upto this enemy not higher enemies
		int worstEnemyInLevel=(int)Mathf.Clamp((currentLevel/1.25f)+3,0,12);
		//minimum/maximum number of enemies in the level
		minEnemiesInLevel=(currentLevel+1)*2;
		//a level difficulty value which needs to be satisfied with the sum of difficulties of all enemies in level
		int levelDifficulty=(currentLevel+1)*2+(currentLevel+1)*3;
		float curveTime=currentLevel/10.0f;
		//enemies that can be in scene at a specific time is loaded from a curve
		enemiesAtATime=(int)(10*enemiesAtATimeCurve.Evaluate(curveTime));
		//add 6 life per level to preexisting life as game is HARD :P
		GameManager.Instance.life+=6;
		
		Debug.Log("Level "+currentLevel.ToString()+" difficulty "+levelDifficulty.ToString()+" enemiesmax "+minEnemiesInLevel+" worst one "+worstEnemyInLevel.ToString()+" at a time "+enemiesAtATime.ToString());
		//load list of enemies based on above values
		List<int> enemyIdsToSpawn=PlaceMaximumEnemies(levelDifficulty,minEnemiesInLevel,worstEnemyInLevel);
		//randomise low difficulty enemies
		if(GameManager.Instance.currentLevel>4){
			for(int i=0;i<enemyIdsToSpawn.Count;i++){
				if(enemyIdsToSpawn[i]==1){//get rid of that too many 1 values
					enemyIdsToSpawn[i]=Random.Range(1,4);
				}
			}
		}
		enemiesToSpawn=new List<string>();
		
		for(int i=0;i<enemyIdsToSpawn.Count;i++){
			enemiesToSpawn.Add(availableEnemies[(enemyIdsToSpawn[i]-1)]);
		}
    }
	//logic to load enemies based on 3 limits. fill with lowest enemy and then replace to hit the level difficulty
    private List<int> PlaceMaximumEnemies(int levelDifficulty, int minEnemiesInLevel, int worstEnemyInLevel)
    {
        int originalworstEnemyInLevel=worstEnemyInLevel;
		List<int> spawn=new List<int>();
		for(int i=0;i<minEnemiesInLevel;i++){
			spawn.Add(1);
		}
		bool goingDown=true;
		while(GetTotalDifficulty(spawn)< levelDifficulty ){
			spawn.Sort();
			spawn[0]=worstEnemyInLevel;
			if(goingDown){
				worstEnemyInLevel--;
			}else{
				worstEnemyInLevel++;
			}
			if(worstEnemyInLevel<=0){
				worstEnemyInLevel=2;
				goingDown=false;
			}else if(worstEnemyInLevel>originalworstEnemyInLevel){
				worstEnemyInLevel=originalworstEnemyInLevel-1;
				goingDown=true;
			}
		}
		return spawn;
	}
	private int GetTotalDifficulty(List<int> spawn)
    {
        int total=0;
		foreach(int i in spawn){
			total+=i;
		}
		return total;
    }

    void updateUI(){
		lifeText.text="Life "+GameManager.Instance.life.ToString();
		scoreText.text="Score "+GameManager.Instance.totalScore.ToString();
		levelText.text="Level "+GameManager.Instance.currentLevel.ToString();
	}

    private void GameOver()
    {
        hasGameStarted=false;
		GameManager.isLevelingUp=false;
		StartCoroutine(Reload());
    }

    private void LevelUp()
    {
        hasGameStarted=false;
		GameManager.isLevelingUp=true;
		GameManager.Instance.currentLevel++;
		StartCoroutine(Reload());
    }

    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(0.5f);
		//Application.LoadLevel(0);
		SceneManager.LoadScene("Blocky Defender");
    }

    public void StartGame(){
		GameManager.cheatEnabled=false;
		miniMapCam.enabled=true;
		menuPanel.SetActive(false);
		UIPanel.SetActive(true);
		hasGameStarted=true;

		updateUI();
		LaunchEnemies(enemiesAtATime);
	}
	public void StartCheatGame(){
		StartGame();
		GameManager.cheatEnabled=true;
	}
}
