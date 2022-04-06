using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridCamera : MonoBehaviour
{
	public Button ZoomIn;
	public Button ZoomOut;

	public float ZoomAmount = 10f;

	Camera cam;

	void Start()
	{
		cam = Camera.main;
	}

	void OnEnable() 
	{
		ZoomIn.onClick.AddListener(delegate { Zoom(-ZoomAmount);  });
		ZoomOut.onClick.AddListener(delegate { Zoom(ZoomAmount); });
	}

	void Zoom(float value) 
	{
		cam.fieldOfView += value;
	}


	public void RotateLeft() 
	{
		transform.Rotate(Vector3.up, 30, Space.Self);
	}

	public void RotateRight() 
	{
		transform.Rotate(Vector3.up, -30, Space.Self);
	}
}
