using UnityEngine;
using System.Collections;

public class RhythmNote : MonoBehaviour 
{
	public Vector3 Destination;

	private Transform _Transform;
	private SpriteRenderer _Sprite;

	private Vector3 StartingPosition;
	private Vector3 StartingScale;

	private float Speed;
	private float Height;
	
	// Use this for initialization
	void Start () 
	{
		Initialize();
	}

	public void Initialize()
	{
		Debug.Log("InitNote");
		_Transform = transform;
		_Sprite = GetComponent<SpriteRenderer>();

		StartingPosition = _Transform.position;
		//StartingScale = _Transform.localScale; Lazy hack
		StartingScale = new Vector3(1.5f, 1.5f, 1.5f);

		CalculateHeight();
	}
	
	void CalculateHeight()
	{
		// (center - bottom) * 2
		Height = (_Sprite.bounds.center.y - _Transform.position.y) * 2f;
	}

	public void Scale(float scale)
	{
		Vector3 tempScale = StartingScale;
		tempScale.y *= scale;
		_Transform.localScale = tempScale;

		CalculateHeight();
	}

	public void Move(float time)
	{
		Move(Destination, time);
	}

	public void Move(Vector3 destination, float time)
	{
		// Set if haven't
		Destination = destination;

		Initialize();

		// Calculate speed
		float distance = destination.y - StartingPosition.y;
		Speed = distance / time;

		//Debug.Log(distance + " " + destination.y + " " + StartingPosition.y);
	}
	
	// Update is called once per frame
	void Update () 
	{
		// Move down
		Vector3 tempPos = _Transform.position;
		tempPos.y += Speed * Time.deltaTime;
		_Transform.position = tempPos;

		// Stop when too down
		if (_Transform.position.y + Height < -7f)
		{
			_Transform.position = StartingPosition;
			_Transform.localScale = StartingScale;

			ObjectPool.instance.PoolObject(this.gameObject);
		}
	}
}
