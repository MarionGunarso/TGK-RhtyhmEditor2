using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

[ExecuteInEditMode]
public class RhythmDesigner : MonoBehaviour 
{
	[HideInInspector]
	public int BPM = 100;
	[HideInInspector]
	public int NotePerBar = 16;

	public GameObject prefab; //prefab of board per beat

	public string SongTitle;
	public AudioSource _Source;

	public NoteBoardScript noteBoardScript;
	public GameObject[] noteSample;

	public RhythmNote[] RhythmNotes;

	public GameObject objectNote;

	private SongData Song;

	private float yPerBar; //the height of 1/4 beat in board

	private float NoteMoveTime = 1f;


	private float TimePerNote;

	private float noteHeight; //height each note

	private SongNote[] songNote;

	List<GameObject> listNote = new List<GameObject>();


	// Use this for initialization
	void Start () 
	{


		//height of each note
		noteHeight = noteSample[0].renderer.bounds.size.y;

		//the height of 1/4 beat in board
		yPerBar = noteBoardScript.yPerBar;

		Debug.Log(yPerBar);


	}

	//clear list of note and destroy object
	public void Clear()
	{
		GameObject [] temp = listNote.ToArray();
		for(int i=0; i<temp.Length; i++)
		{
			DestroyImmediate(temp[i]);

		}
		listNote.Clear();

	}

	//load file and get notes in songNote
	public void Load(string name)
	{
		if(File.Exists(Application.persistentDataPath + "/" + SongTitle + ".dat"))
		{	
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/" + SongTitle + ".dat",FileMode.Open);
			
			Song = new SongData();
			Song = (SongData)bf.Deserialize(file);
			file.Close();
			
			//Debug.Log("File Loaded...");

			//note in the song
			songNote = Song.Notes;
		}
		else
		{
			//Debug.Log("File Not Exists...");
		}

		//take the bpm of the song
		BPM = Song.BPM;

		// Calculate how long it takes per note
		TimePerNote = 60.0f / (float)BPM / (float)Song.NotePerBar;

	}

	//generate notes from the file
	public void GenerateNotes()
	{

		//used in scale magic hack
		int offsetBeat = (int) (NoteMoveTime * Song.BPM);



		for(int i=0 ; i<songNote.Length ; i++)
		{
			if(songNote[i].Type==0)
			{
				//get beat position
				float position  = songNote[i].BeatPosition;

				float scenePosition = (position*yPerBar) - (noteHeight/2); //define note position in board
				int index = songNote[i].Button;

				//instantiate object at scenePosition in board
				GameObject a = (GameObject)Instantiate(noteSample[index], new Vector3(noteSample[index].transform.position.x, scenePosition, noteSample[index].transform.position.z ),noteSample[index].transform.rotation);

				//a.transform.parent = GameObject.Find("ObjectNote").transform;
				a.transform.parent = objectNote.transform;


				//add the object to list
				listNote.Add(a);


				
			}
			else if(songNote[i].Type==1)
			{
				HeldNote heldNote = (HeldNote) songNote[i];
				


				float position  = songNote[i].BeatPosition;
				float scenePosition = (position*yPerBar) - (noteHeight/2); //define note position in board
				int index = songNote[i].Button;
				
				GameObject a = (GameObject)Instantiate(noteSample[index], new Vector3(noteSample[index].transform.position.x, scenePosition, noteSample[index].transform.position.z ),noteSample[index].transform.rotation);
				RhythmNote note = a.GetComponent<RhythmNote>();
				note.Initialize();

				// Magic hack to scale
				float scale = ((float)(offsetBeat +  heldNote.EndBeatPosition - heldNote.BeatPosition - ( 2 * Song.NotePerBar)) / (float)Song.NotePerBar);
				note.Scale(scale);

				a.transform.parent = objectNote.transform;

				listNote.Add(a);
			}

		}
	}

	public void Save(string name)
	{
		listNote.Clear();


		foreach(Transform child in objectNote.transform)
		{
			Debug.Log("Object: "+child.gameObject.name);
			listNote.Add(child.gameObject);

		}


		GameObject [] temp = listNote.ToArray();

		List<SongNote> newSongNote = new List<SongNote>();

		// for each note in listNote
		for(int i=0; i<temp.Length; i++)
		{
			SongNote newNote = new SongNote();
			float position;

			if(temp[i].transform.localScale == new Vector3(1.5f,1.5f,1.5f))
			{
				newNote.Type=0;
			}
			else
			{
				newNote.Type=1;
			}

			if(temp[i].GetComponent<SpriteRenderer>().sprite.name=="Blue Note")
			{
				newNote.Button=0;
			}
			else if(temp[i].GetComponent<SpriteRenderer>().sprite.name=="Green Note")
			{
				newNote.Button=1;
			}
			else if(temp[i].GetComponent<SpriteRenderer>().sprite.name=="Red Note")
			{
				newNote.Button=2;
			}
			else if(temp[i].GetComponent<SpriteRenderer>().sprite.name=="Yellow Note")
			{
				newNote.Button=3;
			}

			//get the position
			float scenePosition = temp[i].transform.position.y;

			//reverth the position back
			float realPosition = scenePosition + noteHeight/2;

			//if the position isn't in the correct position(a little bit highter than the beat), correct it
			if(realPosition % yPerBar !=0)
			{
				float leftover = realPosition % yPerBar;
				position = (realPosition-leftover) / yPerBar;
			}
			//get the beatPosition
			else
			{
				position = realPosition / yPerBar;
			}

			newNote.BeatPosition = (int)position;
			//songNote[i].BeatPosition = (int)position;

			//calculate timeprecise
			newNote.TimePrecise = newNote.BeatPosition * TimePerNote;
			newNote.TimeTapped = newNote.TimePrecise;
			//songNote[i].TimePrecise = songNote[i].BeatPosition * TimePerNote;

			//calculate timeSamplePrecise
			float x = newNote.TimePrecise * _Source.clip.frequency;
			//float x = songNote[i].TimePrecise * _Source.clip.frequency;

			newNote.TimeSamplePrecise = (int)x;
			//songNote[i].TimeSamplePrecise = (int)x;

			if(newNote.Type==0)
			{
				newSongNote.Add(newNote);
			}
			//if heldNote
			else if(newNote.Type==1)
			{
				//get endPosition
				float endScenePosition = temp[i].transform.position.y + temp[i].renderer.bounds.size.y;
				//revertPosition
				float endRealPosition = endScenePosition + noteHeight/2;

				//if the position isn't in the correct position(a little bit highter than the beat), correct it
				if(endRealPosition % yPerBar !=0)
				{
					float leftover = endRealPosition % yPerBar;
					position = (endRealPosition-leftover) / yPerBar;
				}
				//get the beatPosition
				else
				{
					position = endRealPosition / yPerBar;
				}

				//convert into heldnote
				HeldNote heldNote = new HeldNote();
				//HeldNote heldNote = (HeldNote) newNote;

				heldNote.Button = newNote.Button;
				heldNote.BeatPosition = newNote.BeatPosition;
				heldNote.TimeTapped = newNote.TimeTapped;
				heldNote.TimePrecise = newNote.TimePrecise;
				heldNote.TimeSamplePrecise = newNote.TimeSamplePrecise;
				
				// Held note
				heldNote.Type = 1;
				//get the endbeatposition
				heldNote.EndBeatPosition = (int)position;
				//calculate endtimeprecise using endbeatposition
				heldNote.EndTimePrecise = heldNote.EndBeatPosition * TimePerNote;
				heldNote.EndTimeTapped = heldNote.EndTimePrecise;
				//calculate EndTimeSamplePrecise
				float y = heldNote.EndTimePrecise * _Source.clip.frequency;
				heldNote.EndTimeSamplePrecise = (int)y;

				//newNote = heldNote;
				newSongNote.Add(heldNote);
			}




		}

		/*List<SongNote> tes = new List<SongNote>();
		for(int i=0 ; i<songNote.Length ; i++)
		{
			tes.Add(songNote[i]);
		}*/

		//Saving into file
		SongData song = new SongData();
		song.Title = _Source.clip.name;
		song.BPM = BPM;
		song.NotePerBar = NotePerBar;
		song.Notes = newSongNote.ToArray();

		//sort
		song.Sort();

		if(File.Exists(Application.persistentDataPath + "/" + song.Title + ".dat"))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/" + song.Title + ".dat", FileMode.Open);
			
			bf.Serialize(file, song);
			file.Close();
			
			//Debug.Log("File Saved...");
		}
		else
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Create(Application.persistentDataPath + "/" + song.Title +".dat");
			
			bf.Serialize(file, song);
			file.Close();
			
			//Debug.Log("File Created...");
		}

	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
