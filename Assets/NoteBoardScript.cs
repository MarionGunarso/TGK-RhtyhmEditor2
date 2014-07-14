using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class NoteBoardScript : MonoBehaviour {

	// Use this for initialization

	//how many Board has summoned

	public GameObject prefab;
	public AudioSource audioSource;


	private AudioClip audioClip;

	[HideInInspector]
	public float boardHeight;
	public int bpm;

	[HideInInspector]
	public float yPerBar; //height every 1/4 beat

	//audio length in second
	private float lengthSecond;

	private int countBoard;
	private int beatPerSong;
	private List<GameObject> listBoard = new List<GameObject>();

	public GameObject objectBoard;
	void Awake () {
		Debug.Log("start");

		countBoard = 0;
		audioClip = audioSource.clip;

		lengthSecond = audioClip.length;
		float temp = (float)bpm / 60 * lengthSecond;
		beatPerSong = (int)temp;
		boardHeight = prefab.renderer.bounds.size.y;
		yPerBar = boardHeight/4;
		//Debug.Log(beatPerSong);

		//SpawnBoard();

	}

	public void SpawnBoard()
	{
		//if no board spawned
		if(countBoard==0)
		{

			float posY = this.transform.position.y;
			while(countBoard<beatPerSong)
			{
				
				GameObject a = (GameObject)Instantiate(prefab,new Vector3(this.transform.position.x,posY, this.transform.position.z), transform.rotation);
				posY+=boardHeight;
				a.transform.parent = objectBoard.transform;
				listBoard.Add(a);
				countBoard++;
			}
		}

	}

	public void Clear()
	{
		GameObject [] temp = listBoard.ToArray();
		for(int i=0; i<temp.Length; i++)
		{
			DestroyImmediate(temp[i]);
			
		}
		listBoard.Clear();
		countBoard = 0;
	}
	// Update is called once per frame
	void Update () {

	
	}
}
