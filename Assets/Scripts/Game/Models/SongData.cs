using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class SongData 
{
	[SerializeField]
	public string Title;

	[SerializeField]
	public int BPM;

	[SerializeField]
	public int NotePerBar;

	[SerializeField]
	public SongNote[] Notes;

	//sort
	public void Sort()
	{
		List <SongNote> temp = new List<SongNote>();
		for(int i=0 ; i<Notes.Length; i++)
		{
			temp.Add(Notes[i]);
		}
		temp.OrderBy(x=>x.TimeSamplePrecise).ToList();
		Notes = temp.ToArray();
	}
}

[System.Serializable]
public class SongNote
{
	[SerializeField]
	public int Type; // 0 = Single, 1 = Held, 2 = Sliding

	[SerializeField]
	public int Button;

	[SerializeField]
	public int BeatPosition;

	[SerializeField]
	public float TimeTapped;

	[SerializeField]
	public float TimePrecise;

	[SerializeField]
	public int TimeSamplePrecise;
}

[System.Serializable]
public class HeldNote : SongNote
{
	[SerializeField]
	public int EndBeatPosition;
	
	[SerializeField]
	public float EndTimeTapped;
	
	[SerializeField]
	public float EndTimePrecise;
	
	[SerializeField]
	public int EndTimeSamplePrecise;
}

[System.Serializable]
public class SlideNote : HeldNote
{
	[SerializeField]
	public int [] NextButton;

	[SerializeField]
	public int [] BeginSlideBeatPosition;
	
	[SerializeField]
	public float [] BeginSlideTimeTapped;
	
	[SerializeField]
	public float [] BeginSlideTimePrecise;
	
	[SerializeField]
	public int [] BeginSlideTimeSamplePrecise;

	[SerializeField]
	public int [] EndSlideBeatPosition;

	[SerializeField]
	public float [] EndSlideTimeTapped;
	
	[SerializeField]
	public float [] EndSlideTimePrecise;
	
	[SerializeField]
	public int [] EndSlideTimeSamplePrecise;


}