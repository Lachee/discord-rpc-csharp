using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpinner : MonoBehaviour {

	public Vector3 rotation;
	public new Renderer renderer;
	public float w;

	void Update ()
	{
		transform.Rotate(rotation * Time.deltaTime);
		UpdateRenderer();
	}

	void UpdateRenderer()
	{
		if (!renderer) return;

		w = (Mathf.Sin(Time.time * 0.25f) + 1) * 2f;
		renderer.material.SetColor("_EmissionColor", new Color(w, w, w, 1f));
	}
}
