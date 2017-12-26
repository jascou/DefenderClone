using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenderGame : MonoBehaviour {
	public Material scrollingMaterial;
	float scrollSpeed=0;
	float textureScrollSpeed=0;
	public float scrollSpeedMax;
	TextureManager textureManager;
	Vector2 offset=new Vector2();
	GameObject defender;
	
	float halfScreenWidth;

	GameObject[] craftsInScene;


	void Start () {
		GameManager.Instance.Initialise();
		textureManager=GameManager.Instance.textureManager;
		GameManager.Instance.InitLevel();
		CreateSprites();
		halfScreenWidth=Screen.width/2;
	}
	void Update () {
		Vector2 pos= Camera.main.ScreenToWorldPoint(Input.mousePosition);
		scrollSpeed=scrollSpeedMax*(pos.x/halfScreenWidth);
		foreach(GameObject craft in craftsInScene){
			pos=craft.transform.localPosition;
			pos.x-=scrollSpeed*Time.deltaTime;
			if(pos.x>2560){
				pos.x=-2560;
			}else if(pos.x<-2560){
				pos.x=2560;
			}
			craft.transform.localPosition=pos;
		}
		textureScrollSpeed=(scrollSpeed/5120);
		offset.x+=textureScrollSpeed*Time.deltaTime;
		scrollingMaterial.mainTextureOffset = offset;
	}
	
	private void CreateSprites()
    {
        int numCrafts=100;
		craftsInScene=new GameObject[numCrafts];
		int height=Screen.height/2;
		int width=2500;
		SpriteRenderer sr;
		Vector2 pos=new Vector2();
		Sprite sprite;
		GameObject craft;
		for (int i=0;i<numCrafts;i++){
			craft=new GameObject();
			sr = craft.AddComponent<SpriteRenderer>();
			sprite= Sprite.Create(textureManager.PackedTexture,textureManager.GetTextureRectByName("Crafts1"),new Vector2(0.5f,0.5f),1);
			sr.sprite=sprite; 
			pos.x=((2*Random.Range(0,2))-1)*Random.Range(0,width);
			pos.y=((2*Random.Range(0,2))-1)*Random.Range(0,height);
			craft.transform.localPosition=pos;
			craft.transform.localScale=Vector2.one*10;
			craftsInScene[i]=craft;
		}

		defender=new GameObject();
		sr = defender.AddComponent<SpriteRenderer>();
		sprite= Sprite.Create(textureManager.PackedTexture,textureManager.GetTextureRectByName("Defender"),new Vector2(0.5f,0.5f),1);
		sr.sprite=sprite; 
		defender.transform.localScale=Vector2.one*10;
		
		GameObject gameObject=new GameObject();
		SpriteRenderer terrainRenderer = gameObject.AddComponent<SpriteRenderer>();
		terrainRenderer.material=scrollingMaterial;
		sprite= Sprite.Create(textureManager.TerrainTex,textureManager.GetTextureRectByName("terrain"),new Vector2(0.5f,0.5f),1);
		terrainRenderer.sprite=sprite; 
		gameObject.transform.localScale=Vector2.one*20;
    }
}
