using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CrazyMinnow.SALSA; // Import SALSA from the CrazyMinnow namespace

public class argAudio : MonoBehaviour
{
    public Salsa3D salsa; // Public reference to Salsa3D

    void Start()
    {
        string[] arguments = Environment.GetCommandLineArgs();
        Debug.Log(arguments);
        var www = new WWW("file://" + Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")) + "/result.ogg");
        Debug.Log("file://" + Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")) + "/result.wav");
        var www2 = new WWW("C:/Users/Warren/Documents/Salsa/Assets/Crazy Minnow Studio/SALSA with RandomEyes/Examples/Audio/DemoScenes/MilitaryMan/mil.2thinkFunnyHuh.ogg");
        AudioClip myAudioClip = www.audioClip;
       // myAudioClip.Play();

        // Get reference to the Salsa3D component
        salsa = (Salsa3D)FindObjectOfType(typeof(Salsa3D));
        Debug.Log(myAudioClip);
        salsa.SetAudioClip(myAudioClip); // Set the SALSA audio clip	

        salsa.Play(); // Play the audio clip
        //salsa.Pause(); // Pause the audio clip
        //salsa.Stop(); // Stop the audio clip
        
        salsa.saySmallTrigger = 0.001f; // Adjust the small trigger value
        salsa.sayMediumTrigger = 0.002f; // Adjust the medium trigger value
        salsa.sayLargeTrigger = 0.004f; // Adjust the large trigger value

        salsa.broadcast = true; // Set broadcasting to true
        salsa.broadcastReceiversCount = 1; // Setup one receiver slot
        //salsa.broadcastReceivers[0] = SOMEGAMEOBJECT; // Bind some game obejct to the receiver
        salsa.propagateToChildren = true; // Propagate broadcasts to SOMEGAMEOBJECT's children

        salsa.audioUpdateDelay = 0.08f; // The duration between audio sample updates

        salsa.blendSpeed = 10; // Shape key transition duration
        salsa.SetRangeOfMotion(85); // The percentage of total range of motion allowed
        
    }
}