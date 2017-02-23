using System;
using System.IO;
using System.Security.Permissions;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class bgImg : MonoBehaviour {
	private string imgDir;
	private DateTime oldTime;
	private Material material;
	private Texture2D texture;
	private RawImage img;
	// Use this for initialization
	void Start () {
		img = gameObject.GetComponent<RawImage> ();
		imgDir = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")) + "/ibm.png";
		oldTime = File.GetLastWriteTimeUtc (imgDir);
		byte[] bytes = File.ReadAllBytes (imgDir);
		texture = new Texture2D (1,1);
		texture.LoadImage(bytes);
		img.texture = texture;
	}

	// Update is called once per frame
	void Update () {
		DateTime newTime = File.GetLastWriteTimeUtc(imgDir);
		if (oldTime != newTime)
		{
			oldTime = newTime;
		}
	}
}
