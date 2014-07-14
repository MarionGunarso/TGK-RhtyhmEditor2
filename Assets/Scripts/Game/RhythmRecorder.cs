using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class RhythmRecorder : MonoBehaviour 
{
	public int BPM = 110;
	public int NotePerBar = 16;
	public int NoteTypes = 4;

	private AudioSource _Source;

	private bool IsRecording = false;
	private List<SongNote> Notes = new List<SongNote>();
	
	private float TimePerNote;

	private SongNote[] TemporaryNotes;

	void Awake ()
	{
		Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
	}

	// Use this for initialization
	void Start () 
	{
		// Calculate how long it takes per note
		TimePerNote = 60.0f / (float)BPM / (float)NotePerBar;

		// Temp array for notes, because there can't be two same notes at the same time
		TemporaryNotes = new SongNote[NoteTypes];
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void Save()
	{
		//Debug.Log("Saving File...");
		//Debug.Log(Application.persistentDataPath);

		SongData song = new SongData();
		song.Title = _Source.clip.name;
		song.BPM = BPM;
		song.NotePerBar = NotePerBar;
		song.Notes = Notes.ToArray();

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

	public void RecordNote(int button)
	{
		if (!IsRecording) return;

		// Save note first to be checked later
		SongNote note = new SongNote();
		note.Button = button;

		// Note tapped time
		float currentTime = _Source.time;
		note.TimeTapped = currentTime;

		// Note precise time in beat
		if(currentTime % TimePerNote != 0)
		{
			float leftover = currentTime % TimePerNote;
			note.TimePrecise = note.TimeTapped - leftover;
		}
		else
			note.TimePrecise = note.TimeTapped;

		// Note precise time in sample
		float tempSamplePrecise = note.TimePrecise * _Source.clip.frequency;
		note.TimeSamplePrecise = (int) tempSamplePrecise;

		// Note beat position
		note.BeatPosition = (int)(note.TimePrecise / TimePerNote);

		// Save to temporary
		TemporaryNotes[note.Button] = note;
		
		//Debug.Log("Start Recording Note");
	}

	public void AddSingleNote(int button)
	{
		if (!IsRecording) return;

		// Get note first
		SongNote note = TemporaryNotes[button];

		// Single note
		note.Type = 0;

		//Debug.Log(note.Button + " " + note.BeatPosition + " " + note.TimeTapped + " " + note.TimePrecise + " " + note.TimeSamplePrecise + " " + _Source.timeSamples);

		// Save note data
		Notes.Add(note);

		// Reset temporary
		TemporaryNotes[note.Button] = null;
		
		//Debug.Log("Recorded Single Note");
	}

	public void AddHeldNote(int button)
	{
		if (!IsRecording) return;

		// Get temporary note first
		SongNote note = TemporaryNotes[button];

		// Create held note
		HeldNote heldNote = new HeldNote();
		heldNote.Button = note.Button;
		heldNote.BeatPosition = note.BeatPosition;
		heldNote.TimeTapped = note.TimeTapped;
		heldNote.TimePrecise = note.TimePrecise;
		heldNote.TimeSamplePrecise = note.TimeSamplePrecise;

		// Held note
		heldNote.Type = 1;

		// Note end tapped time
		float currentTime = _Source.time;
		heldNote.EndTimeTapped = currentTime;
		
		// Note precise time in beat
		if(currentTime % TimePerNote != 0)
		{
			float leftover = currentTime % TimePerNote;
			heldNote.EndTimePrecise = heldNote.EndTimeTapped - leftover;
		}
		else
			heldNote.EndTimePrecise = heldNote.EndTimeTapped;
		
		// Note precise time in sample
		float tempSamplePrecise = heldNote.EndTimePrecise * _Source.clip.frequency;
		heldNote.EndTimeSamplePrecise = (int) tempSamplePrecise;

		//Debug.Log(heldNote.Button + " " + heldNote.EndBeatPosition + " " + heldNote.EndTimeTapped + " " + heldNote.EndTimePrecise + " " + heldNote.EndTimeSamplePrecise + " " + _Source.timeSamples);

		// Note beat position
		heldNote.EndBeatPosition = (int)(heldNote.EndTimePrecise / TimePerNote);

		// Save note data
		Notes.Add(heldNote);
		
		// Reset temporary
		TemporaryNotes[note.Button] = null;

		//Debug.Log("Recorded Held Note");
	}

	public void StartRecording(AudioSource source)
	{
		Notes.Clear();

		_Source = source;

		IsRecording = true;
	}

	public void StopRecording()
	{
		IsRecording = false;

		Save();
	}
}
