using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using CrazyMinnow.SALSA;

namespace CrazyMinnow.SALSA.MCS
{
	/// <summary>
	/// This is the custom inspector for CM_DazSync, a script that acts as a proxy between 
	/// SALSA with RandomEyes and Daz3D characters, and allows users to link SALSA with 
	/// RandomEyes to Daz3D characters without any model modifications.
	/// 
	/// Crazy Minnow Studio, LLC
	/// CrazyMinnowStudio.com
	/// 
	/// NOTE:While every attempt has been made to ensure the safe content and operation of 
	/// these files, they are provided as-is, without warranty or guarantee of any kind. 
	/// By downloading and using these files you are accepting any and all risks associated 
	/// and release Crazy Minnow Studio, LLC of any and all liability.
	[CustomEditor(typeof(CM_MCSSync)), CanEditMultipleObjects]
	public class CM_MCSSyncEditor : Editor 
	{
		private CM_MCSSync mcsSync; // CM_MCSSync reference
		private bool dirtySmall; // SaySmall dirty inspector status
		private bool dirtyMedium; // SayMedum dirty inspector status
		private bool dirtyLarge; // SayLarge dirty inspector status

		private float width = 0f; // Inspector width
		private float addWidth = 10f; // Percentage
		private float deleteWidth = 10f; // Percentage
		private float shapeNameWidth = 60f; // Percentage
		private float percentageWidth = 30f; // Percentage

		public void OnEnable()
		{
			// Get reference
			mcsSync = target as CM_MCSSync;

			// Initialize
			if (mcsSync.initialize)
			{                
				mcsSync.GetSalsa3D();
				mcsSync.GetRandomEyes3D();
				mcsSync.GetSmr();
				mcsSync.GetEyeBones();
                mcsSync.GetBlinkIndexes();
				if (mcsSync.saySmall == null) mcsSync.saySmall = new List<CM_ShapeGroup>();
				if (mcsSync.sayMedium == null) mcsSync.sayMedium = new List<CM_ShapeGroup>();
				if (mcsSync.sayLarge == null) mcsSync.sayLarge = new List<CM_ShapeGroup>();
				mcsSync.GetShapeNames();
                mcsSync.SetSmall();
                mcsSync.SetMedium();
                mcsSync.SetLarge();
				mcsSync.initialize = false;
			}
		}

		public override void OnInspectorGUI()
        {
            // Minus 45 width for the scroll bar
            width = Screen.width - 50f;

            // Set dirty flags
            dirtySmall = false;
            dirtyMedium = false;
            dirtyLarge = false;

            // Keep trying to get the shapeNames until I've got them
            if (mcsSync.GetShapeNames() == 0) mcsSync.GetShapeNames();

            // Make sure the CM_ShapeGroups are initialized
            if (mcsSync.saySmall == null) mcsSync.saySmall = new System.Collections.Generic.List<CM_ShapeGroup>();
            if (mcsSync.sayMedium == null) mcsSync.sayMedium = new System.Collections.Generic.List<CM_ShapeGroup>();
            if (mcsSync.sayLarge == null) mcsSync.sayLarge = new System.Collections.Generic.List<CM_ShapeGroup>();

            GUILayout.Space(10);
            EditorGUILayout.BeginVertical(new GUILayoutOption[] { GUILayout.Width(width) });
            {
                GUILayout.Space(10);
                GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) }); // Horizontal rule
                GUILayout.Space(10);

                mcsSync.salsa3D = EditorGUILayout.ObjectField(
                    "Salsa3D", mcsSync.salsa3D, typeof(Salsa3D), true) as Salsa3D;
                mcsSync.randomEyes3D = EditorGUILayout.ObjectField(
                    new GUIContent("RandomEyes3D", "The RandomEyes3D instance for controlling eye movement."),
                    mcsSync.randomEyes3D, typeof(RandomEyes3D), true) as RandomEyes3D;
                mcsSync.skinnedMeshRenderer = EditorGUILayout.ObjectField(
                    new GUIContent("SkinnedMeshRenderer", "The SkinnedMeshRenderer child object that contains all the BlendShapes"),
                    mcsSync.skinnedMeshRenderer, typeof(SkinnedMeshRenderer), true) as SkinnedMeshRenderer;
                mcsSync.leftEyeBone = EditorGUILayout.ObjectField(
                    "Left Eye Bone", mcsSync.leftEyeBone, typeof(GameObject), true) as GameObject;
                mcsSync.rightEyeBone = EditorGUILayout.ObjectField(
                    "Right Eye Bone", mcsSync.rightEyeBone, typeof(GameObject), true) as GameObject;
                GUILayout.Space(10);
                mcsSync.leftBlinkShapes = EditorGUILayout.TextField(
                    new GUIContent("Left Blink Shape Names", "Daz3D BlendShape names are not always " +
                        "consistant, this is a CSV list of common variations contained in blink shape names."),
                        mcsSync.leftBlinkShapes);
                mcsSync.rightBlinkShapes = EditorGUILayout.TextField(
                    new GUIContent("Right Blink Shape Names", "Daz3D BlendShape names are not always " +
                        "consistant, this is a CSV list of common variations contained in blink shape names."),
                        mcsSync.rightBlinkShapes);
                GUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(new GUIContent("Search",
                        "This will search the SkinnedMeshRenderer for BlendShapes that contain these names."));
                    if (GUILayout.Button("Search"))
                    {
                        mcsSync.GetBlinkIndexes();
                    }
                }
                GUILayout.EndHorizontal();


                mcsSync.leftBlinkShape = "";
                if (mcsSync.leftBlinkIndex != -1) mcsSync.leftBlinkShape =
                    mcsSync.skinnedMeshRenderer.sharedMesh.GetBlendShapeName(mcsSync.leftBlinkIndex);
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(new GUIContent("Left Blink", "-1 means no blink shape " +
                        "index was found, check the SkinnedMeshRenderer to find the left eye blink shape name"));
					GUILayout.Label(" (" + mcsSync.leftBlinkIndex.ToString() + ") " + mcsSync.leftBlinkShape);
                }
                GUILayout.EndHorizontal();

                mcsSync.rightBlinkShape = "";
                if (mcsSync.rightBlinkIndex != -1) mcsSync.rightBlinkShape =
                    mcsSync.skinnedMeshRenderer.sharedMesh.GetBlendShapeName(mcsSync.rightBlinkIndex);
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(new GUIContent("Right Blink", "-1 means no blink shape " +
                        "index was found, check the SkinnedMeshRenderer to find the right eye blink shape name"));
					GUILayout.Label("(" + mcsSync.rightBlinkIndex.ToString() + ") " + mcsSync.rightBlinkShape);
                }
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) }); // Horizontal rule
            GUILayout.Space(10);

            if (mcsSync.skinnedMeshRenderer)
            {
                EditorGUILayout.LabelField("SALSA shape groups");
                GUILayout.Space(10);

                EditorGUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Width(width) });
                EditorGUILayout.LabelField("SaySmall Shapes");
                if (GUILayout.Button("+", new GUILayoutOption[] { GUILayout.Width((addWidth / 100) * width) }))
                {
                    mcsSync.saySmall.Add(new CM_ShapeGroup());
                    mcsSync.initialize = false;
                }
                EditorGUILayout.EndHorizontal();
                if (mcsSync.saySmall.Count > 0)
                {
                    GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
                    EditorGUILayout.LabelField(
                        new GUIContent("Delete", "Remove shape"),
                        GUILayout.Width((deleteWidth / 100) * width));
                    EditorGUILayout.LabelField(
                        new GUIContent("ShapeName", "BlendShape - (shapeIndex)"),
                        GUILayout.Width((shapeNameWidth / 100) * width));
                    EditorGUILayout.LabelField(
                        new GUIContent("Percentage", "The percentage of total range of motion for this shape"),
                        GUILayout.Width((percentageWidth / 100) * width));
                    GUILayout.EndHorizontal();

                    for (int i = 0; i < mcsSync.saySmall.Count; i++)
                    {
                        GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.ExpandWidth(false), GUILayout.Width(width) });
                        if (GUILayout.Button(
                            new GUIContent("X", "Remove this shape from the list (index:" + mcsSync.saySmall[i].shapeIndex + ")"),
                            GUILayout.Width((deleteWidth / 100) * width)))
                        {
                            mcsSync.saySmall.RemoveAt(i);
                            dirtySmall = true;
                            break;
                        }
                        if (!dirtySmall)
                        {
                            mcsSync.saySmall[i].shapeIndex = EditorGUILayout.Popup(
                                mcsSync.saySmall[i].shapeIndex, mcsSync.shapeNames,
                                GUILayout.Width((shapeNameWidth / 100) * width));
                            mcsSync.saySmall[i].shapeName =
                                mcsSync.skinnedMeshRenderer.sharedMesh.GetBlendShapeName(mcsSync.saySmall[i].shapeIndex);
                            mcsSync.saySmall[i].percentage = EditorGUILayout.Slider(
                                mcsSync.saySmall[i].percentage, 0f, 100f,
                                GUILayout.Width((percentageWidth / 100) * width));
                            mcsSync.initialize = false;
                        }
                        GUILayout.EndHorizontal();
                    }
                }

                GUILayout.Space(10);

                EditorGUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Width(width) });
                EditorGUILayout.LabelField("SayMedium Shapes");
                if (GUILayout.Button("+", new GUILayoutOption[] { GUILayout.Width((addWidth / 100) * width) }))
                {
                    mcsSync.sayMedium.Add(new CM_ShapeGroup());
                    mcsSync.initialize = false;
                }
                EditorGUILayout.EndHorizontal();
                if (mcsSync.sayMedium.Count > 0)
                {
                    GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
                    EditorGUILayout.LabelField(
                        new GUIContent("Delete", "Remove shape"),
                        GUILayout.Width((deleteWidth / 100) * width));
                    EditorGUILayout.LabelField(
                        new GUIContent("ShapeName", "BlendShape - (shapeIndex)"),
                        GUILayout.Width((shapeNameWidth / 100) * width));
                    EditorGUILayout.LabelField(
                        new GUIContent("Percentage", "The percentage of total range of motion for this shape"),
                        GUILayout.Width((percentageWidth / 100) * width));
                    GUILayout.EndHorizontal();

                    for (int i = 0; i < mcsSync.sayMedium.Count; i++)
                    {
                        GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.ExpandWidth(false), GUILayout.Width(width) });
                        if (GUILayout.Button(
                            new GUIContent("X", "Remove this shape from the list (index:" + mcsSync.sayMedium[i].shapeIndex + ")"),
                            GUILayout.Width((deleteWidth / 100) * width)))
                        {
                            mcsSync.sayMedium.RemoveAt(i);
                            dirtyMedium = true;
                            break;
                        }
                        if (!dirtyMedium)
                        {
                            mcsSync.sayMedium[i].shapeIndex = EditorGUILayout.Popup(
                                mcsSync.sayMedium[i].shapeIndex, mcsSync.shapeNames,
                                GUILayout.Width((shapeNameWidth / 100) * width));
                            mcsSync.sayMedium[i].shapeName =
                                mcsSync.skinnedMeshRenderer.sharedMesh.GetBlendShapeName(mcsSync.sayMedium[i].shapeIndex);
                            mcsSync.sayMedium[i].percentage = EditorGUILayout.Slider(
                                mcsSync.sayMedium[i].percentage, 0f, 100f,
                                GUILayout.Width((percentageWidth / 100) * width));
                            mcsSync.initialize = false;
                        }
                        GUILayout.EndHorizontal();
                    }
                }

                GUILayout.Space(10);

                EditorGUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.Width(width) });
                EditorGUILayout.LabelField("SayLarge Shapes");
                if (GUILayout.Button("+", new GUILayoutOption[] { GUILayout.Width((addWidth / 100) * width) }))
                {
                    mcsSync.sayLarge.Add(new CM_ShapeGroup());
                    mcsSync.initialize = false;
                }
                EditorGUILayout.EndHorizontal();
                if (mcsSync.sayLarge.Count > 0)
                {
                    GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
                    EditorGUILayout.LabelField(
                        new GUIContent("Delete", "Remove shape"),
                        GUILayout.Width((deleteWidth / 100) * width));
                    EditorGUILayout.LabelField(
                        new GUIContent("ShapeName", "BlendShape - (shapeIndex)"),
                        GUILayout.Width((shapeNameWidth / 100) * width));
                    EditorGUILayout.LabelField(
                        new GUIContent("Percentage", "The percentage of total range of motion for this shape"),
                        GUILayout.Width((percentageWidth / 100) * width));
                    GUILayout.EndHorizontal();

                    for (int i = 0; i < mcsSync.sayLarge.Count; i++)
                    {
                        GUILayout.BeginHorizontal(new GUILayoutOption[] { GUILayout.ExpandWidth(false), GUILayout.Width(width) });
                        if (GUILayout.Button(
                            new GUIContent("X", "Remove this shape from the list (index:" + mcsSync.sayLarge[i].shapeIndex + ")"),
                            GUILayout.Width((deleteWidth / 100) * width)))
                        {
                            mcsSync.sayLarge.RemoveAt(i);
                            dirtyLarge = true;
                            break;
                        }
                        if (!dirtyLarge)
                        {
                            mcsSync.sayLarge[i].shapeIndex = EditorGUILayout.Popup(
                                mcsSync.sayLarge[i].shapeIndex, mcsSync.shapeNames,
                                GUILayout.Width((shapeNameWidth / 100) * width));
                            mcsSync.sayLarge[i].shapeName = mcsSync.skinnedMeshRenderer.sharedMesh.GetBlendShapeName(mcsSync.sayLarge[i].shapeIndex);
                            mcsSync.sayLarge[i].percentage = EditorGUILayout.Slider(
                                mcsSync.sayLarge[i].percentage, 0f, 100f,
                                GUILayout.Width((percentageWidth / 100) * width));
                            mcsSync.initialize = false;
                        }
                        GUILayout.EndHorizontal();
                    }
                }
            }
        }
	}
}
