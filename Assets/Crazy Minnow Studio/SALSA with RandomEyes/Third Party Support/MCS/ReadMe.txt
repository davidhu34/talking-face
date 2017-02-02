-----------------
Version 1.0
-----------------
CM_MCSSync is designed to be used in congunction with SALSA with RandomEyes, Morph 3D's MCS Character models, as outlined in the workflow created by:

Crazy Minnow Studio, LLC
CrazyMinnowStudio.com

This workflow is documented at the following URL, along with a downloadable zip file that contains the supporting files.

http://crazyminnowstudio.com/posts/using-daz3d-characters-with-salsa-with-randomeyes/


Package Contents
----------------
Crazy Minnow Studio/SALSA with RandomEyes/Third Party Support/
	MCS
		Editor
			CM_MCSSyncEditor.cs
				Custom inspector for CM_MCSSync.cs
			CM_MCSSetupEditor.cs
				Custom inspector for CM_MCSSetup.cs
		CM_MCSSync.cs
			Helper script to apply Salsa and RandomEyes BlendShape data to MCS character BlendShapes.
		CM_MCSSetup.cs
			SALSA 1-click MCS setup script for new MCS characters.
		ReadMe.txt
			This readme file.
	Shared
		CM_RandomMovement.CS
			Random movement script for simple precedural idle animations.


Installation Instructions
-------------------------
1. Install your MCS character(s) into your project.

2. Install SALSA with RandomEyes into your project.
	Select [Window] -> [Asset Store]
	Once the Asset Store window opens, select the download icon, and download and import [SALSA with RandomEyes].

3. Import the SALSA with RandomEyes MCS Character support package.
	Select [Assets] -> [Import Package] -> [Custom Package...]
	Browse to the [SALSA_3rdPartySupport_MCS.unitypackage] file and [Open].


Usage Instructions
------------------
1. Add a MCS character to your scene.

2. Select the character root, then select:
	[Component] -> [Crazy Minnow Studio] -> [MCS] -> [SALSA 1-Click MCS Setup]
	This will add and configure all necessary component for a complete SALSA with RandomEyes setup.

3. Add your dialogue audio file to the Salsa3D [Audio Clip] field.

** The 1-Click setup links to LOD0, which means the remaining three LOD's will not perform lipsync. If you need the lower detail LOD's to perform lipsync, re-map the following SkinnedMeshRenderer's at runtime when switching the LOD:
	RandomEyes.skinnedMeshRenderer (for custom shapes)
	CM_MCSSync.skinnedMeshRenderer


What [SALSA 1-Click MCS Setup] does
-------------------------------------
1. It adds the following components:
	[Component] -> [Crazy Minnow Studio] -> [Salsa3D] (for lip sync)
	[Component] -> [Crazy Minnow Studio] -> [RandomEyes3D] (for eyes)
	[Component] -> [Crazy Minnow Studio] -> [RandomEyes3D] (for custom shapes)
	[Component] -> [Crazy Minnow Studio] -> [MCS] -> [CM_MCSSync] (for syncing SALSA with RandomEyes to your MCS character)

2. On the Salsa3D component, it leaves the SkinnedMeshRenderer empty, and sets the SALSA [Range of Motion] to 100. (Set this to your preference)

3. On the RandomEyes3D componet for eyes, it leaves the SkinnedMeshRenderer empty, and sets the [Range of Motion] to 60. (Set this to your preference)

4. On the RandomEyes3D component for custom shapes, it attempts to find and link the main SkinnedMeshRenderer with BlendShapes.

5. On the RandomEyes3D component for custom shapes, it checks [Use Custom Shapes Only], Auto-Link's to the non-body morph facial expressions, and removes all but brow movement them from random selection. 
	You should selectively include small shapes, like eyebrow and facial twitches, in random selection to add natural random facial movement.

6. On the CM_MCSSync.cs component it attempts to link the following:
	Salsa3D
	RandomEyes3D (for eyes)
	The main SkinnedMeshRenderer with BlendShapes.
	The Left and Right eye bones.
	Find and link the left and right blink shape indexes.
		This search process uses the CSV keywork lists in the [Left Blink Shape Names] and [Right Blink Shape Names] fields.