using UnityEngine;
using UnityEditor;
using System.Collections;

namespace CrazyMinnow.SALSA.MCS
{
    [CustomEditor(typeof(CM_MCSSetup))]
    public class CM_MCSSetupEditor : Editor
    {
        private CM_MCSSetup mcsSetup; // CM_MCSSetup reference

        public void OnEnable()
        {
			// Get reference
			mcsSetup = target as CM_MCSSetup;

			// Run Setup
			mcsSetup.Setup();

            // Remove setup component
            DestroyImmediate(mcsSetup);
        }
    }
}