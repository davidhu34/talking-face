using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CrazyMinnow.SALSA;

namespace CrazyMinnow.SALSA.MCS
{
	/// <summary>
	/// This script acts as a proxy between SALSA with RandomEyes and MCS characters,
	/// and allows users to link SALSA with RandomEyes to MCS characters without any model
	/// modifications.
	/// 
	/// Good default inspector values
	/// Salsa3D
	/// 	Trigger values will depend on your recordings
	/// 	Blend Speed: 10
	/// 	Range of Motion: 75
	/// RandomEyes3D
	/// 	Range of Motion: 60
	/// </summary>
	/// 
	/// Crazy Minnow Studio, LLC
	/// CrazyMinnowStudio.com
	/// 
	/// NOTE:While every attempt has been made to ensure the safe content and operation of 
	/// these files, they are provided as-is, without warranty or guarantee of any kind. 
	/// By downloading and using these files you are accepting any and all risks associated 
	/// and release Crazy Minnow Studio, LLC of any and all liability.
	[AddComponentMenu("Crazy Minnow Studio/MCS/CM_MCSSync")]
	public class CM_MCSSync : MonoBehaviour 
	{
		public Salsa3D salsa3D; // Salsa3D mouth component
		public RandomEyes3D randomEyes3D; // RandomEyes3D eye componet
		public SkinnedMeshRenderer skinnedMeshRenderer; // MCS character SkinnedMeshRenderer
		public string leftEyeName = "lEye"; // Used in search for left eye bone
		public GameObject leftEyeBone; // Left eye bone
		public string rightEyeName = "rEye"; // Used in search for right eye bone
		public GameObject rightEyeBone; // Right eye bone
        public string leftBlinkShapes = "EyesClosedL,BlinkLeft,Blink_l"; // Left blink shape search keywords
        public string rightBlinkShapes = "EyesClosedR,BlinkRight,Blink_r"; // Right blink shape search keywords
        public int leftBlinkIndex = -1; // Left blink shape index
        public int rightBlinkIndex = -1; // Right blink shape index
        public string leftBlinkShape = ""; // Left blink shape name
        public string rightBlinkShape = ""; // Right blink shape name
		public List<CM_ShapeGroup> saySmall = new List<CM_ShapeGroup>(); // saySmall shape group
		public List<CM_ShapeGroup> sayMedium = new List<CM_ShapeGroup>(); // sayMedium shape group
		public List<CM_ShapeGroup> sayLarge = new List<CM_ShapeGroup>(); // sayLarge shape group
		public string[] shapeNames; // Shape name string array for name picker popups
		public bool initialize = true; // Initialize once
        //public enum DazType { Dragon, Emotiguy, Genesis_Genesis2, Genesis3 }; // Supported Daz base character types
        //public DazType dazType = DazType.Genesis_Genesis2; // Default base type
        //public DazType prevType; // Tracks previous base type to detect changes

		private Transform[] children; // For searching through child objects during initialization
		private float eyeSensativity = 500f; // Eye movement reduction from shape value to bone transform value
        private float blinkWeight; // Blink weight is applied to the body Blink_Left and Blink_Right BlendShapes
		private float vertical; // Vertical eye bone movement amount
		private float horizontal; // Horizontal eye bone movement amount
		private bool lockShapes; // Used to allow access to shape group shapes when SALSA is not talking

		/// <summary>
		/// Reset the component to default values
		/// </summary>
		void Reset()
		{
			initialize = true;            
			GetSalsa3D();
			GetRandomEyes3D();
			GetSmr();
			GetEyeBones();
            GetBlinkIndexes();
			if (saySmall == null) saySmall = new List<CM_ShapeGroup>();
			if (sayMedium == null) sayMedium = new List<CM_ShapeGroup>();
			if (sayLarge == null) sayLarge = new List<CM_ShapeGroup>();
			GetShapeNames();

			SetSmall();
			SetMedium();
			SetLarge();
		}
        void ResetSalsa(Salsa3D s3d)
        {
            salsa3D = s3d;
            GetSmr();
        }
        /// <summary>
        /// Initial setup
        /// </summary>
		void Start()
		{
			// Initialize            
			GetSalsa3D();
			GetRandomEyes3D();
			GetSmr();
			GetEyeBones();
            GetBlinkIndexes();
			if (saySmall == null) saySmall = new List<CM_ShapeGroup>();
			if (sayMedium == null) sayMedium = new List<CM_ShapeGroup>();
			if (sayLarge == null) sayLarge = new List<CM_ShapeGroup>();
			GetShapeNames();
		}

        /// <summary>
        /// Perform the blendshape changes in LateUpdate for mechanim compatibility
        /// </summary>
		void LateUpdate() 
		{
			// Toggle shape lock to provide access to shape group shapes when SALSA is not talking
			if (salsa3D)
			{
				if (salsa3D.sayAmount.saySmall == 0f && salsa3D.sayAmount.sayMedium == 0f && salsa3D.sayAmount.sayLarge == 0f)
				{
					lockShapes = false;
				}
				else
				{
					lockShapes = true;
				}
			}

			if (salsa3D && skinnedMeshRenderer && lockShapes)
			{
				// Sync SALSA shapes
				for (int i=0; i<saySmall.Count; i++)
				{
					skinnedMeshRenderer.SetBlendShapeWeight(
						saySmall[i].shapeIndex, ((saySmall[i].percentage/100)*salsa3D.sayAmount.saySmall));
				}
				for (int i=0; i<sayMedium.Count; i++)
				{
					skinnedMeshRenderer.SetBlendShapeWeight(
						sayMedium[i].shapeIndex, ((sayMedium[i].percentage/100)*salsa3D.sayAmount.sayMedium));
				}			
				for (int i=0; i<sayLarge.Count; i++)
				{
					skinnedMeshRenderer.SetBlendShapeWeight(
						sayLarge[i].shapeIndex, ((sayLarge[i].percentage/100)*salsa3D.sayAmount.sayLarge));
				}
			}

			// Sync Blink
			if (randomEyes3D)
			{
				blinkWeight = randomEyes3D.lookAmount.blink;

				// Apply blink action
				if (skinnedMeshRenderer)
				{
                    if (leftBlinkIndex != -1) skinnedMeshRenderer.SetBlendShapeWeight(leftBlinkIndex, blinkWeight);
                    if (rightBlinkIndex != -1) skinnedMeshRenderer.SetBlendShapeWeight(rightBlinkIndex, blinkWeight);
				}

				// Apply look amount to bone rotation
				if (leftEyeBone || rightEyeBone)
				{
					// Apply eye movement weight direction variables
					if (randomEyes3D.lookAmount.lookUp > 0) 
						vertical = -(randomEyes3D.lookAmount.lookUp / eyeSensativity) * randomEyes3D.rangeOfMotion;
					if (randomEyes3D.lookAmount.lookDown > 0) 
						vertical = (randomEyes3D.lookAmount.lookDown / eyeSensativity) * randomEyes3D.rangeOfMotion;
					if (randomEyes3D.lookAmount.lookLeft > 0) 
						horizontal = -(randomEyes3D.lookAmount.lookLeft / eyeSensativity) * randomEyes3D.rangeOfMotion;
					if (randomEyes3D.lookAmount.lookRight > 0) 
						horizontal = (randomEyes3D.lookAmount.lookRight / eyeSensativity) * randomEyes3D.rangeOfMotion;

					// Set eye bone rotations
					if (leftEyeBone) leftEyeBone.transform.localRotation = Quaternion.Euler(vertical, horizontal, 0);
					if (rightEyeBone) rightEyeBone.transform.localRotation = Quaternion.Euler(vertical, horizontal, 0);
				}
			}
		}

		/// <summary>
		/// Call this when initializing characters at runtime
		/// </summary>
		public void Initialize()
		{
			Reset();
		}

		/// <summary>
		/// Get the Salsa3D component
		/// </summary>
		public void GetSalsa3D()
		{
			if (!salsa3D) salsa3D = GetComponent<Salsa3D>();
		}

		/// <summary>
		/// Get the RandomEyes3D component
		/// </summary>
		public void GetRandomEyes3D()
		{
			//if (!randomEyes3D) randomEyes3D = GetComponent<RandomEyes3D>();

            RandomEyes3D[] randomEyes = GetComponents<RandomEyes3D>();
            if (randomEyes.Length > 1)
            {
                for (int i = 0; i < randomEyes.Length; i++)
                {
                    // Verify this instance ID does not match the reEyes instance ID
                    if (!randomEyes[i].useCustomShapesOnly)
                    {
                        // Set the reShapes instance
                        randomEyes3D = randomEyes[i];
                    }
                }
            }
		}

		/// <summary>
		/// Find the Body child object SkinnedMeshRenderer
		/// </summary>
		public void GetSmr()
		{
			if (!skinnedMeshRenderer) 
			{
				SkinnedMeshRenderer[] smr = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
				if (smr.Length > 0)
				{
					for (int i=0; i<smr.Length; i++)
					{
						if (smr[i].sharedMesh.blendShapeCount > 0 && smr[i].enabled)
						{
							if (smr[i].gameObject.name.Contains("Genesis") && smr[i].gameObject.name.Contains("LOD0"))
							{
								skinnedMeshRenderer = smr[i];
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Find left and right eye bones
		/// </summary>
		public void GetEyeBones()
		{
			Transform leftEyeTrans = ChildSearch(leftEyeName);
			if(leftEyeTrans) 
			{
				if (!leftEyeBone) leftEyeBone = leftEyeTrans.gameObject;
			}
			Transform rightEyeTrans = ChildSearch(rightEyeName);
			if (rightEyeTrans) 
			{
				if (!rightEyeBone) rightEyeBone = rightEyeTrans.gameObject;
			}
		}

        /// <summary>
        /// Get blink indexes for multiple MCS BlendShape name variations
        /// </summary>
        public void GetBlinkIndexes()
        {
            if (skinnedMeshRenderer)
            {
                int leftIndex = -1;
                int rightIndex = -1;

                leftBlinkShapes = leftBlinkShapes.Replace(" ", "");
                rightBlinkShapes = rightBlinkShapes.Replace(" ", "");
                string[] leftNames = leftBlinkShapes.Split(',');
                string[] rightNames = rightBlinkShapes.Split(',');

                // Loop through left blink BlendShape names
                for (int i = 0; i < leftNames.Length; i++)
                {
                    if (leftIndex == -1) leftIndex = ShapeSearch(skinnedMeshRenderer, leftNames[i]);
                    leftBlinkIndex = leftIndex;
                }

                // Loop through right blink BlendShape names
                for (int i = 0; i < rightNames.Length; i++)
                {
                    if (rightIndex == -1) rightIndex = ShapeSearch(skinnedMeshRenderer, rightNames[i]);
                    if (rightIndex != -1)
                    {
                        rightBlinkIndex = rightIndex;
                        break;
                    }
                }
            }
        }

		/// <summary>
        /// Find a child by name that ends with the search string. 
        /// This should compensates for BlendShape name prefixes variations.
		/// </summary>
		/// <param name="endsWith"></param>
		/// <returns></returns>
		public Transform ChildSearch(string endsWith)
		{
			Transform trans = null;

			children = transform.gameObject.GetComponentsInChildren<Transform>();

			for (int i=0; i<children.Length; i++)
			{
                if (children[i].name.EndsWith(endsWith)) trans = children[i];
			}

			return trans;
		}	

		/// <summary>
        /// Find a shape by name, that ends with the search string.
		/// </summary>
		/// <param name="skndMshRndr"></param>
		/// <param name="endsWith"></param>
		/// <returns></returns>
        public int ShapeSearch(SkinnedMeshRenderer skndMshRndr, string endsWith)
		{
			int index = -1;
			if (skndMshRndr)
			{
				for (int i=0; i<skndMshRndr.sharedMesh.blendShapeCount; i++)
				{
                    if (skndMshRndr.sharedMesh.GetBlendShapeName(i).EndsWith(endsWith))
					{
						index = i;
						break;
					}
				}
			}
			return index;
		}

		/// <summary>
		/// Populate the shapeName popup list
		/// </summary>
		public int GetShapeNames()
		{
			int nameCount = 0;

			if (skinnedMeshRenderer)
			{
				shapeNames = new string[skinnedMeshRenderer.sharedMesh.blendShapeCount];
				for (int i=0; i<skinnedMeshRenderer.sharedMesh.blendShapeCount; i++)
				{
					shapeNames[i] = skinnedMeshRenderer.sharedMesh.GetBlendShapeName(i);
					if (shapeNames[i] != "") nameCount++;
				}
			}

			return nameCount;
		}

		/// <summary>
		/// Set the MCS saySmall shape group
		/// </summary>
		public void SetSmall()
		{
			int index = -1;
			string name = "";

			saySmall = new List<CM_ShapeGroup>();

			index = ShapeSearch(skinnedMeshRenderer, "VSMER");
			if (index != -1)
			{
				name = skinnedMeshRenderer.sharedMesh.GetBlendShapeName(index);
				saySmall.Add(new CM_ShapeGroup(index, name, 10f));
			}

			index = ShapeSearch(skinnedMeshRenderer, "VSMUW");
			if (index != -1)
			{
				name = skinnedMeshRenderer.sharedMesh.GetBlendShapeName(index);
				saySmall.Add(new CM_ShapeGroup(index, name, 30f));
			}

			index = ShapeSearch(skinnedMeshRenderer, "VSMMK");
			if (index != -1)
			{
				name = skinnedMeshRenderer.sharedMesh.GetBlendShapeName(index);
				saySmall.Add(new CM_ShapeGroup(index, name, 50f));
			}
		}
		/// <summary>
        /// Set the MCS sayMedium shape group
		/// </summary>
        public void SetMedium()
		{
			int index = -1;;
			string name = "";

			sayMedium = new List<CM_ShapeGroup>();

			index = ShapeSearch(skinnedMeshRenderer, "VSMAA");
			if (index != -1)
			{
				name = skinnedMeshRenderer.sharedMesh.GetBlendShapeName(index);
				sayMedium.Add(new CM_ShapeGroup(index, name, 100f));
			}

			index = ShapeSearch(skinnedMeshRenderer, "PHMMouthSmileOpen");
			if (index != -1)
			{
				name = skinnedMeshRenderer.sharedMesh.GetBlendShapeName(index);
				sayMedium.Add(new CM_ShapeGroup(index, name, 20f));
			}

			index = ShapeSearch(skinnedMeshRenderer, "PHMMouthSmile");
			if (index != -1)
			{
				name = skinnedMeshRenderer.sharedMesh.GetBlendShapeName(index);
				sayMedium.Add(new CM_ShapeGroup(index, name, 40f));
			}

			index = ShapeSearch(skinnedMeshRenderer, "PHMLipTopUpR");
			if (index != -1)
			{
				name = skinnedMeshRenderer.sharedMesh.GetBlendShapeName(index);
				sayMedium.Add(new CM_ShapeGroup(index, name, 40f));
			}

			index = ShapeSearch(skinnedMeshRenderer, "PHMLipTopUpL");
			if (index != -1)
			{
				name = skinnedMeshRenderer.sharedMesh.GetBlendShapeName(index);
				sayMedium.Add(new CM_ShapeGroup(index, name, 40f));
			}
		}
		/// <summary>
        /// Set the MCS sayLarge shape group
		/// </summary>
        public void SetLarge()
		{
			int index = -1;;
			string name = "";

			sayLarge = new List<CM_ShapeGroup>();

			index = ShapeSearch(skinnedMeshRenderer, "VSMTH");
			if (index != -1)
			{
				name = skinnedMeshRenderer.sharedMesh.GetBlendShapeName(index);
				sayLarge.Add(new CM_ShapeGroup(index, name, 60f));
			}

            index = ShapeSearch(skinnedMeshRenderer, "PHMMouthOpenWide");
            if (index != -1)
            {
                name = skinnedMeshRenderer.sharedMesh.GetBlendShapeName(index);
                sayLarge.Add(new CM_ShapeGroup(index, name, 40f));
            }

            index = ShapeSearch(skinnedMeshRenderer, "PHMMouthOpen");
            if (index != -1)
            {
                name = skinnedMeshRenderer.sharedMesh.GetBlendShapeName(index);
                sayLarge.Add(new CM_ShapeGroup(index, name, 60f));
            }
		}
	}

	/// <summary>
	/// Shape index and percentage class for SALSA/MCS shape groups
	/// </summary>
	[System.Serializable]
	public class CM_ShapeGroup
	{
		public int shapeIndex;
		public string shapeName;
		public float percentage;

		public CM_ShapeGroup()
		{
			this.shapeIndex = 0;
			this.shapeName = "";
			this.percentage = 100f;
		}

		public CM_ShapeGroup(int shapeIndex, string shapeName, float percentage)
		{
			this.shapeIndex = shapeIndex;
			this.shapeName = shapeName;
			this.percentage = percentage;
		}
	}
}
