using UnityEngine;
using System.Collections.Generic;
using CrazyMinnow.SALSA;

namespace CrazyMinnow.SALSA.MCS
{
    [AddComponentMenu("Crazy Minnow Studio/MCS/SALSA 1-Click MCS Setup")]
    public class CM_MCSSetup : MonoBehaviour
    {
		/// <summary>
		/// This initializes Setup when setting up characters at runtime
		/// </summary>
		void Awake()
		{
			Setup();
			Destroy(this);
		}

		/// <summary>
		/// Configures a complete SALSA with RandomEyes enabled MCS character
		/// </summary>
		public void Setup()
        {
            GameObject activeObj; // Selected hierarchy object
            Salsa3D salsa3D; // Salsa3D
            RandomEyes3D reEyes; // RandomEyes3D for eye
            RandomEyes3D reShapes; // RandomEyes3D for custom shapes
            RandomEyes3D[] randomEyes; // All RandomEyes3D compoents
            CM_MCSSync mcsSync; // CM_MCSSync
			List<int> shapeIndexes = new List<int>();
			bool foundVSM = false;
			bool pastVSM = false;
			Transform lEye = null;
			Transform rEye = null;
			Transform[] children;

			activeObj = this.gameObject;

            #region Add and get components
            salsa3D = activeObj.AddComponent<Salsa3D>().GetComponent<Salsa3D>(); // Add/get Salsa3D
            reEyes = activeObj.AddComponent<RandomEyes3D>().GetComponent<RandomEyes3D>(); // Add/get reEyes
			reEyes.FindOrCreateEyePositionGizmo();
			children = activeObj.GetComponentsInChildren<Transform>();
			for (int i = 0; i < children.Length; i++)
			{
				if (children[i].name == "lEye") lEye = children[i];
				if (children[i].name == "rEye") rEye = children[i];
			}
			if (lEye && rEye) // Position the RandomEyes_Eye_Position gizmo between the eyes
			{
				reEyes.eyePosition.transform.position = ((lEye.position - rEye.position) * 0.5f) + rEye.position;
			}
			children = null;
			reShapes = reEyes; // Temporarily set the reShapes instance to reEyes so it's not null
            activeObj.AddComponent<RandomEyes3D>(); // Add reShapes
            // Get all RandomEyes compoents so we can distinguish the second reShapes instance
            randomEyes = activeObj.GetComponents<RandomEyes3D>();
            if (randomEyes.Length > 1)
            {
                for (int i = 0; i < randomEyes.Length; i++)
                {
                    // Verify this instance ID does not match the reEyes instance ID
                    if (randomEyes[i].GetInstanceID() != reEyes.GetInstanceID())
                    {
                        // Set the reShapes instance
                        reShapes = randomEyes[i];
                    }
                }
            }
			mcsSync = activeObj.AddComponent<CM_MCSSync>().GetComponent<CM_MCSSync>(); // Add/get CM_MCSSync
			mcsSync.Initialize();
			#endregion

			#region Set Salsa3D and RandomEyes3D component parameters
			salsa3D.saySmallTrigger = 0.0005f;
            salsa3D.sayMediumTrigger = 0.0035f;
            salsa3D.sayLargeTrigger = 0.007f;
            salsa3D.SetRangeOfMotion(100f); // Set mouth range of motion
            salsa3D.blendSpeed = 10f; // Set blend speed

            salsa3D.audioSrc = activeObj.GetComponent<AudioSource>(); // Set the salsa3D.audioSrc
            if (salsa3D.audioSrc) salsa3D.audioSrc.playOnAwake = false; // Disable play on wake

            reEyes.SetRangeOfMotion(60f); // Set eye range of motion
			reEyes.SetBlinkDuration(0.03f); // Set blink duration
			reEyes.SetBlinkSpeed(30f); // Set blink speed
            reShapes.useCustomShapesOnly = true; // Set reShapes to custom shapes only
            reShapes.skinnedMeshRenderer = mcsSync.skinnedMeshRenderer; // Set the SkinnedMeshRenderer

			// Get PHM indexes after the VSM's
			for (int i = 0; i < mcsSync.skinnedMeshRenderer.sharedMesh.blendShapeCount; i++)
			{
				if (mcsSync.skinnedMeshRenderer.sharedMesh.GetBlendShapeName(i).Contains("VSM")) foundVSM = true;
				if (foundVSM && !mcsSync.skinnedMeshRenderer.sharedMesh.GetBlendShapeName(i).Contains("VSM")) pastVSM = true;
				if (foundVSM && pastVSM)
				{
					if (mcsSync.skinnedMeshRenderer.sharedMesh.GetBlendShapeName(i).Contains("PHM"))
					{
						shapeIndexes.Add(i);
					}
				}
			}
			// Link PHM indexes to RandomEyes custom shapes
			reShapes.customShapeCount = shapeIndexes.Count;
			reShapes.customShapes = new RandomEyesCustomShape[reShapes.customShapeCount];
			for (int i = 0; i < shapeIndexes.Count; i++)
			{
				reShapes.customShapes[i] = new RandomEyesCustomShape();
				reShapes.customShapes[i].shapeIndex = shapeIndexes[i];
				reShapes.customShapes[i].shapeName = mcsSync.skinnedMeshRenderer.sharedMesh.GetBlendShapeName(shapeIndexes[i]);
				reShapes.customShapes[i].blendSpeed = 5f;
				reShapes.customShapes[i].rangeOfMotion = 100f;
				if (reShapes.customShapes[i].shapeName.Contains("Brow"))
					reShapes.customShapes[i].notRandom = false;
				else
					reShapes.customShapes[i].notRandom = true;
			}
			reShapes.noneShapeIndex = reShapes.RebuildCurrentCustomShapeList();
			reShapes.selectedCustomShape = reShapes.noneShapeIndex;
			#endregion

			#region CM_MCSSync settings
			mcsSync.salsa3D = salsa3D;
			mcsSync.randomEyes3D = reEyes;
            #endregion
        }
    }
}