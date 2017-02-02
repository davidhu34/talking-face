﻿using System;
using System.IO;
using System.Security.Permissions;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Windows;

using CrazyMinnow.SALSA; // Import SALSA from the CrazyMinnow namespace

public class runSalsa : MonoBehaviour {

    private string clipName = "dialog.wav";
    private string clipDir;
    public DateTime oldTime;
    public AudioClip myAudioClip;
    public AudioClip myAudioClip2;
    public WWW www;
    public WWW www2;
    public bool playing = false;
    public bool started = false;
    public bool waiting = false;
    public bool destroyed = false;
    private Salsa3D salsa3D;

    void makeSalsa() {
        Debug.Log("make");
        www = new WWW("file://" + clipDir);
        myAudioClip = www.audioClip;
        Debug.Log(myAudioClip.isReadyToPlay);
        while (!www.isDone) ;

        Debug.Log(myAudioClip.isReadyToPlay);
        // Salsa3D
        gameObject.AddComponent<Salsa3D>(); // Add a Salsa3D component
        salsa3D = GetComponent<Salsa3D>(); // Get reference to the Salsa3D component
        salsa3D.skinnedMeshRenderer = GameObject.Find("Ethan/char_ethan_body").GetComponent<SkinnedMeshRenderer>(); // Link the SkinnedMeshRenderer
        salsa3D.saySmallIndex = 0; // Set saySmall BlendShape index
        salsa3D.sayMediumIndex = 1; // Set sayMedium BlendShape index
        salsa3D.sayLargeIndex = 2; // Set sayLarge BlendShape index
        salsa3D.SetAudioClip(myAudioClip); // Set AudioClip
                                           // Or set the AudioClip from a clip in any Resources folder
                                           //salsa3D.SetAudioClip((Resources.Load("EthanEcho0", typeof(AudioClip)) as AudioClip));
        salsa3D.saySmallTrigger = 0.002f; // Set the saySmall amplitude trigger
        salsa3D.sayMediumTrigger = 0.004f; // Set the sayMedium amplitude trigger
        salsa3D.sayLargeTrigger = 0.005f; // Set the sayLarge amplitude trigger
        salsa3D.audioUpdateDelay = 0.05f; // Set the amplitutde sample update delay
        salsa3D.blendSpeed = 10f; // Set the blend speed
        salsa3D.rangeOfMotion = 100f; // Set the range of motion
        waiting = true;
    }
    // Use this for initialization
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    void Start () {
        clipDir = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")) + "/" + clipName;
        oldTime = File.GetLastWriteTimeUtc(clipDir);

        makeSalsa();
    }
    // Update is called once per frame
    void Update()
    {
        if (!playing && waiting && !destroyed)
        {
            Debug.Log("Play");
            salsa3D.Play();
            playing = true;

        }
        if (destroyed && !salsa3D) 
        {
            Debug.Log("waiting for new");
            DateTime newTime = File.GetLastWriteTimeUtc(clipDir);
            if (oldTime != newTime)
            {
                Debug.Log("oldyime"+oldTime);
                oldTime = newTime;
                Debug.Log("newtime"+oldTime);
                destroyed = false;
                makeSalsa();
            }
        }
    }
    void LateUpdate()
    {
        if (playing && !salsa3D.isTalking)
        {
            playing = false;
            waiting = false;
            Destroy(salsa3D);
            destroyed = true;
        }
    }
}