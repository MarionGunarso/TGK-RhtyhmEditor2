using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(NoteBoardScript))]
public class NoteBoardEditor : Editor {

	void DrawLoadLayout(NoteBoardScript noteBoard)
	{
		EditorGUILayout.BeginHorizontal();
		
		if (GUILayout.Button("Clear"))
			noteBoard.Clear();
		

		
		if (GUILayout.Button("Generate"))
			noteBoard.SpawnBoard();
		
		EditorGUILayout.EndHorizontal();
	}
	
	/*void DrawSpawnLayout(NoteBoardScript noteBoard)
	{
		if (GUILayout.Button("Generate"))
			noteBoard.SpawnBoard();
	}*/
	
	public override void OnInspectorGUI ()
	{
		NoteBoardScript script = (NoteBoardScript) target;
		
		DrawDefaultInspector();
		
		DrawLoadLayout(script);
		
		//DrawSpawnLayout(script);
	}
}
