using UnityEngine;
using System.Collections;

public class RhythmEditorSceneController : MonoBehaviour 
{
	public enum Mode {
		Editor, Play
	};
	public Mode _Mode = Mode.Editor;
	public TextMesh ModeText;
	public Button2D ChangeModeButton;

	public AudioSource Song;
	public Button2D PlayButton;

	public float StartDelay = 1.0f;

	public Button2D[] RhythmButtons;

	private TextMesh PlayButtonText;

	// Record
	private bool IsRecording = false;
	private RhythmRecorder _Recorder;

	// Play
	private bool IsPlaying = false;
	private RhythmPlayer _Player;

	private bool IsSliding = false;

	// Use this for initialization
	void Start () 
	{
		Application.targetFrameRate = 60;

		PlayButtonText = PlayButton.GetComponent<TextMesh>();

		ChangeMode(_Mode);
		ChangeModeButton.OnClick += FlipMode;

		_Recorder = GetComponent<RhythmRecorder>();
		_Player = GetComponent<RhythmPlayer>();
	}

	void FlipMode(Button2D button)
	{
		if (_Mode == Mode.Editor) ChangeMode(Mode.Play);
		else if (_Mode == Mode.Play) ChangeMode(Mode.Editor);
	}

	void ChangeMode(Mode mode)
	{
		_Mode = mode;

		if (_Mode == Mode.Editor)
		{
			if (IsPlaying) StopPlaying(PlayButton);
			PlayButton.OnClick -= StartPlaying;

			ModeText.text = "Editor Mode";

			PlayButtonText.text = "RECORD";
			PlayButton.OnClick += StartRecording;

			foreach(Button2D button in RhythmButtons)
			{
				button.OnDown += StartNote;
				button.OnClick += RecordNote;
			}
		}
		else if (_Mode == Mode.Play)
		{
			if (IsRecording) StopRecording(PlayButton);
			PlayButton.OnClick -= StartRecording;

			ModeText.text = "Play Mode";

			PlayButtonText.text = "PLAY";
			PlayButton.OnClick += StartPlaying;

			foreach(Button2D button in RhythmButtons)
			{
				button.OnDown -= StartNote;
				button.OnClick -= RecordNote;
			}
		}
	}

	void StartRecording(Button2D button)
	{
		PlayButton.OnClick -= StartRecording;

		double initTime = AudioSettings.dspTime;
		Song.PlayScheduled(initTime + StartDelay);

		IsRecording = true;
		_Recorder.StartRecording(Song);

		PlayButtonText.text = "STOP RECORDING";
		PlayButton.OnClick += StopRecording;
	}

	void StartNote(Button2D button)
	{
		// Get note index
		int index = -1;
		foreach(Button2D rhythmButton in RhythmButtons)
		{
			index++;
			if (button == rhythmButton)
				break;
		}

		// Record the start note
		_Recorder.RecordNote(index);
	}

	void RecordNote(Button2D button)
	{
		// Get note index
		int index = -1;
		foreach(Button2D rhythmButton in RhythmButtons)
		{
			index++;
			if (button == rhythmButton)
				break;
		}
		
		if (!button.IsHeldDown)
		{
			// Note is single when not held
			_Recorder.AddSingleNote(index);
		}
		else
		{
			// Obviously record held note
			_Recorder.AddHeldNote(index);
		}
	}

	void StopRecording(Button2D button)
	{
		PlayButton.OnClick -= StopRecording;

		Song.Stop();

		IsRecording = false;
		_Recorder.StopRecording();

		PlayButtonText.text = "RECORD";
		PlayButton.OnClick += StartRecording;
	}

	void StartPlaying(Button2D button)
	{
		PlayButton.OnClick -= StartPlaying;
		
		double initTime = AudioSettings.dspTime;
		Song.PlayScheduled(initTime + StartDelay);

		IsPlaying = true;
		_Player.StartPlaying(Song);
		
		PlayButtonText.text = "STOP PLAYING";
		PlayButton.OnClick += StopPlaying;
	}

	void StopPlaying(Button2D button)
	{
		PlayButton.OnClick -= StopPlaying;
		
		Song.Stop();

		IsPlaying = false;
		_Player.StopPlaying();
		
		PlayButtonText.text = "PLAY";
		PlayButton.OnClick += StartPlaying;
	}

	// Update is called once per frame
	void Update () 
	{
		
	}
}
