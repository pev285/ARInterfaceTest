using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneTextureScaler : MonoBehaviour {

    private Material material;

    private Transform transf;

    private float tilesPerMeter = 10.0f;

	void Start () 
	{
        material = GetComponent<Renderer>().material;
        transf = gameObject.transform.parent;
	}

    private void FixedUpdate()
    {
        material.SetTextureScale("_MainTex", new Vector2(transf.localScale.x * tilesPerMeter, transf.localScale.z * tilesPerMeter));
    }

} // End Of Class //


