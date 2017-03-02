using System;
using System.IO;
using System.Security.Permissions;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

using CrazyMinnow.SALSA; // Import SALSA from the CrazyMinnow namespace
using CrazyMinnow.SALSA.MCS;

public class runSalsa2 : MonoBehaviour {
    public CM_MCSSync mcs;
	private string clipDir;
	private string txtDir;
	public DateTime oldTime;
	public AudioClip myAudioClip;
	public WWW www;
	public WWW txtfile;
	public bool playing = false;
	public bool destroyed;
	public bool making = false;
	private Salsa3D salsa3D;
	public Animator anim;
	public RandomEyes3D eyes;
	public Text answer;

	void makeSalsa(string name) {
        www = new WWW("file://" + clipDir + name);
        myAudioClip = www.audioClip;
        Debug.Log(myAudioClip.isReadyToPlay);
        while (!www.isDone) ;

        Debug.Log(myAudioClip.isReadyToPlay);
        // Salsa3D
        gameObject.AddComponent<Salsa3D>(); // Add a Salsa3D component
        salsa3D = GetComponent<Salsa3D>(); // Get reference to the Salsa3D component
        mcs.salsa3D = salsa3D;


        salsa3D.saySmallIndex = 0; // Set saySmall BlendShape index
        salsa3D.sayMediumIndex = 1; // Set sayMedium BlendShape index
        salsa3D.sayLargeIndex = 2; // Set sayLarge BlendShape index
        salsa3D.SetAudioClip(myAudioClip); // Set AudioClip
                                           // Or set the AudioClip from a clip in any Resources folder
                                           //salsa3D.SetAudioClip((Resources.Load("EthanEcho0", typeof(AudioClip)) as AudioClip));
        salsa3D.saySmallTrigger = 0.0005f; // Set the saySmall amplitude trigger
        salsa3D.sayMediumTrigger = 0.0001f; // Set the sayMedium amplitude trigger
        salsa3D.sayLargeTrigger = 0.0012f; // Set the sayLarge amplitude trigger
        salsa3D.audioUpdateDelay = 0.05f; // Set the amplitutde sample update delay
        salsa3D.blendSpeed = 5f; // Set the blend speed
        salsa3D.rangeOfMotion = 100f; // Set the range of motion
        making = false;
        destroyed = false;
    }
    // Use this for initialization
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    void Start()
    {
		destroyed = true;
		clipDir = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")) + "/lib/wavs/";
		//clipDir = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")) + "/wavs/";
		txtDir = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")) + "/wavname.txt";
		oldTime = File.GetLastWriteTimeUtc(txtDir);
		txtfile = new WWW("file://" + txtDir);
		while (!txtfile.isDone) { };

		anim = GetComponent<Animator>();

		//makeSalsa(txtfile.text);
    }
    // Update is called once per frame
    void Update()
    {
        if (!playing && !destroyed)
        {
            Debug.Log("Play");
			Debug.Log(destroyed);
            playing = true;
			anim.SetBool("talking", true);
            salsa3D.Play();
        }
        if (destroyed && !salsa3D && !making)
        {
            Debug.Log("waiting for new");
            DateTime newTime = File.GetLastWriteTimeUtc(txtDir);
            if (oldTime != newTime)
            {
                Debug.Log("oldyime" + oldTime);
                oldTime = newTime;
                Debug.Log("newtime" + oldTime);
                making = true;
				txtfile = new WWW("file://" + txtDir);
				while (!txtfile.isDone) { };
				makeSalsa(txtfile.text);
            }
        }
    }
    void LateUpdate()
    {
        if (playing && !salsa3D.isTalking)
        {
            playing = false;
            Destroy(salsa3D);
            destroyed = true;
			anim.SetBool("talking", false);
        }
    }
}
