using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class RhythmPlayer : MonoBehaviour 
{
	public GameObject[] RhythmNotes;

	public string Title;
	public float NoteMoveTime = 1f;

	private AudioSource _Source;

	private SongData Song;

	void Awake ()
	{
		Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
	}

	// Use this for initialization
	void Start () 
	{

	}

	public void Load(string name)
	{
		if(File.Exists(Application.persistentDataPath + "/" + Title + ".dat"))
		{	
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/" + Title + ".dat",FileMode.Open);

			Song = new SongData();
			Song = (SongData)bf.Deserialize(file);
			file.Close();

			//Debug.Log("File Loaded...");
		}
		else
		{
			//Debug.Log("File Not Exists...");
		}
	}

	public void StartPlaying(AudioSource source)
	{
		Load(Title);

		_Source = source;

		if (Song.Notes.Length > 0)
			StartCoroutine(PlaySong());
	}

	IEnumerator PlaySong()
	{
		int currentSample = 0;

		SongNote[] notes = Song.Notes;

		for(int i=0 ; i<notes.Length ; i++)
		{
			Debug.Log("button: "+notes[i].Button);
		}

	

		float tempOffset = NoteMoveTime * _Source.clip.frequency;
		int offsetSample = (int) tempOffset;
		int offsetBeat = (int) (NoteMoveTime * Song.BPM);

		int noteIndex = 0;
		int nextNoteSample = notes[noteIndex].TimeSamplePrecise - offsetSample;

		while(_Source.isPlaying && noteIndex < notes.Length)
		{
			//currentSample = (float)AudioSettings.dspTime * _Source.clip.frequency;
			currentSample = _Source.timeSamples;

			if (currentSample >= nextNoteSample)
			{
				Debug.Log("a");
				SongNote currentNote = notes[noteIndex];

				GameObject noteObject = ObjectPool.instance.GetObjectForType(RhythmNotes[currentNote.Button].gameObject.name, false);
				if (noteObject != null)
				{
					RhythmNote note = noteObject.GetComponent<RhythmNote>();

					note.Move(NoteMoveTime);

 					if (currentNote.Type == 1) 
					{
						HeldNote heldNote = (HeldNote) currentNote;

						// Magic hack to scale
						float scale = ((float)(offsetBeat +  heldNote.EndBeatPosition - heldNote.BeatPosition - ( 2 * Song.NotePerBar)) / (float)Song.NotePerBar);
						note.Scale(scale);
					}
					
					noteIndex++;
					if (noteIndex < notes.Length)
						nextNoteSample = notes[noteIndex].TimeSamplePrecise - offsetSample;
				}
			}

			yield return new WaitForSeconds(0);
		}
	}

	public void StopPlaying()
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
