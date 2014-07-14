using UnityEngine;
using System.Collections;

public class Button2D : MonoBehaviour 
{
	// Default detection on down
	public bool ClickOnRelease = false;

	// Scale down for dummy effect
	public bool ScaleWhenDown = false;
	public Vector3 Scale = Vector3.zero;

	public delegate void _OnClick(Button2D button);
	public _OnClick OnClick;

	public delegate void _OnDown(Button2D button);
	public _OnDown OnDown;

	public delegate void _OnCancel(Button2D button);
	public _OnCancel OnCancel;

	public delegate void _OnUp(Button2D button);
	public _OnUp OnUp;

	public delegate void _OnHeldDown(Button2D button);
	public _OnHeldDown OnHeldDown;

	public delegate void _OnSlideOut();
	public _OnSlideOut OnSlideOut;

	public delegate void _OnSlideIn();
	public _OnSlideIn OnSlideIn;

	protected Camera _MainCamera;
	protected BoxCollider2D _Collider;

	protected bool IsButtonDown = false;

	protected Transform _Transform;
	protected Vector3 StartingScale;

	// How long the button must be held down before the delegate called
	protected bool _IsHeldDown = false;
	public bool IsHeldDown
	{
		get { return _IsHeldDown; }
	}

	protected float _HeldDownOffset = 0.4f; 
	public float HeldDownOffset
	{
		get { return _HeldDownOffset; }
	}

#if UNITY_IOS || UNITY_ANDROID
	protected int _CurrentTouchIndex = -1;
#endif

	// Use this for initialization
	void Start () 
	{
		_MainCamera = Camera.main;
		_Collider = GetComponent<BoxCollider2D>();

		_Transform = transform;
		StartingScale = _Transform.localScale;
	}

	bool IsPositionOnCollider(Vector3 position)
	{
		Vector3 screenPosition =  _MainCamera.ScreenToWorldPoint(position);
		Vector2 screenPosition2D = new Vector2(screenPosition.x, screenPosition.y);

		return _Collider == Physics2D.OverlapPoint(screenPosition2D);
	}

	void OnTriggerEnter2D(Collider2D otherCollider)
	{

	}

	void OnTriggerExit2D(Collider2D otherCollider)
	{

	}

	bool canSlide = true;
	// Update is called once per frame
	void Update () 
	{
#if UNITY_EDITOR
		if (IsButtonDown)
		{
			if (!IsPositionOnCollider(Input.mousePosition))
			{
				// Call cancel delegate when we're outside
				if (OnCancel != null) OnCancel(this);

				//if (OnSlideOut !=null) OnSlideOut(this);

				// Reset held down
				StopAllCoroutines();
				_IsHeldDown = false;


				// Reset state and scale
				//if(Input.GetMouseButtonUp(0))
				//{
					IsButtonDown = false;
					if (ScaleWhenDown)
						_Transform.localScale = new Vector3(StartingScale.x, StartingScale.y, StartingScale.z);
				//}

				

			}
			else
			{
				// Call held down delegate when needed
				if (_IsHeldDown && OnHeldDown != null) OnHeldDown(this);
			}
		}

		if (Input.GetMouseButtonDown(0))
		{
			if (IsPositionOnCollider(Input.mousePosition))
			{
				//if(canSlide==true)
				//{
				//	if(OnSlideIn != true) OnSlideIn(this);
				//}

				// Call down delegate
				if (OnDown != null) OnDown(this);

				// Call click delegates if needed
				if (!ClickOnRelease && OnClick != null) OnClick(this);

				// Lock state and change scale
				IsButtonDown = true;
				if (ScaleWhenDown)
					_Transform.localScale = new Vector3(StartingScale.x * Scale.x, StartingScale.y * Scale.y, StartingScale.z * Scale.z);

				// Start counting whether we're held down or not
				StartCoroutine(HeldDown());
			}

		}
		else if (IsButtonDown && Input.GetMouseButtonUp(0))
		{
			// Call up delegate
			if (OnUp != null) OnUp(this);

			// Call click delegate if still on that position when up
			if (IsPositionOnCollider(Input.mousePosition))
			{
				if (ClickOnRelease && OnClick != null) OnClick(this);
			}

			// Reset held down
			StopAllCoroutines();
			_IsHeldDown = false;

			// Reset state and scale
			IsButtonDown = false;
			if (ScaleWhenDown)
				_Transform.localScale = new Vector3(StartingScale.x, StartingScale.y, StartingScale.z);
		}
#elif UNITY_IOS || UNITY_ANDROID
		int totalCount = Input.touchCount;
		for(int i=0;i<totalCount;i++)
		{
			Touch t = Input.GetTouch(i);
			
			if (IsButtonDown)
			{
				if (_CurrentTouchIndex == t.fingerId)
				{
					if (!IsPositionOnCollider(t.position))
					{
						//Debug.Log("Touch cancel: " + _CurrentTouchIndex);
						
						// Call cancel delegate when we're outside
						if (OnCancel != null) OnCancel(this);
						
						// Reset held down
						StopAllCoroutines();
						_IsHeldDown = false;
						
						// Reset state and scale
						IsButtonDown = false;
						if (ScaleWhenDown)
							_Transform.localScale = new Vector3(StartingScale.x, StartingScale.y, StartingScale.z);
						
						// Reset touch index
						_CurrentTouchIndex = -1;
					}
					else
					{
						//Debug.Log("Touch held: " + _CurrentTouchIndex);
						
						// Call held down delegate when needed
						if (_IsHeldDown && OnHeldDown != null) OnHeldDown(this);
					}
				}
			}
			
			if (t.phase == TouchPhase.Began)
			{
				if (IsPositionOnCollider(t.position))
				{
					// Call down delegate
					if (OnDown != null) OnDown(this);
					
					// Call click delegates if needed
					if (!ClickOnRelease && OnClick != null)
					{
						//Debug.Log("Touch clicked: " + _CurrentTouchIndex);
						
						OnClick(this);
					}
					
					// Lock state and change scale
					IsButtonDown = true;
					if (ScaleWhenDown)
						_Transform.localScale = new Vector3(StartingScale.x * Scale.x, StartingScale.y * Scale.y, StartingScale.z * Scale.z);
					
					// Start counting whether we're held down or not
					StartCoroutine(HeldDown());
					
					// Save touch index
					_CurrentTouchIndex = t.fingerId;
					
					//Debug.Log("Touch down: " + _CurrentTouchIndex);
				}
			}
			else if (IsButtonDown && t.phase == TouchPhase.Ended)
			{
				if (_CurrentTouchIndex == t.fingerId)
				{
					//Debug.Log("Touch up: " + _CurrentTouchIndex);
					
					// Call up delegate
					if (OnUp != null) OnUp(this);
					
					// Call click delegate if still on that position when up
					if (IsPositionOnCollider(t.position))
					{
						if (ClickOnRelease && OnClick != null)
						{
							//Debug.Log("Touch clicked: " + _CurrentTouchIndex);
							
							OnClick(this);
						}
					}
					
					// Reset held down
					StopAllCoroutines();
					_IsHeldDown = false;
					
					// Reset state and scale
					IsButtonDown = false;
					if (ScaleWhenDown)
						_Transform.localScale = new Vector3(StartingScale.x, StartingScale.y, StartingScale.z);
					
					// Reset touch index
					_CurrentTouchIndex = -1;
				}
			}
		}
#endif
	}

	IEnumerator HeldDown()
	{
		yield return new WaitForSeconds(_HeldDownOffset);

		_IsHeldDown = true;

		yield return null;
	}
}
