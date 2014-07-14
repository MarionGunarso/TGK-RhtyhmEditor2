using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(RhythmDesigner))]
public class RhythmDesignerEditor : Editor 
{
	void DrawLoadLayout(RhythmDesigner designer)
	{
		EditorGUILayout.BeginHorizontal();

		if (GUILayout.Button("Clear"))
			designer.Clear();

		if (GUILayout.Button("Load"))
			designer.Load(designer.SongTitle);

		if (GUILayout.Button("Generate"))
			designer.GenerateNotes();

		EditorGUILayout.EndHorizontal();
	}
	
	void DrawSaveLayout(RhythmDesigner designer)
	{
		if (GUILayout.Button("Save"))
			designer.Save(designer.SongTitle);
	}
	
	public override void OnInspectorGUI ()
	{
		RhythmDesigner script = (RhythmDesigner) target;

		DrawDefaultInspector();

		DrawLoadLayout(script);
		
		DrawSaveLayout(script);
	}
}
