using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zoom : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetAxis("Mouse ScrollWheel") < transform.position.z - 0.5)
        {
            transform.Translate(Vector3.forward * Input.GetAxis("Mouse ScrollWheel"));
        }
    }
}
