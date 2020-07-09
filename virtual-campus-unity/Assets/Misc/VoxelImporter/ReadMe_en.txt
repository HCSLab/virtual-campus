-----------------------------------------------------
	Voxel Importer
	Copyright (c) 2016 AloneSoft
	http://alonesoft.sakura.ne.jp/
	mail: support@alonesoft.sakura.ne.jp
	twitter: @AlSoSupport
-----------------------------------------------------

Thank you for purchasing "Voxel Importer".


[How to Update] 
Remove old VoxelImporter folder
Import VoxelImporter


[Documentation]
Assets/VoxelImporter/Documents


[Tutorial]
Humanoid Setup Tutorial
https://youtu.be/hkudVsxtxn4
Frame Animation Tutorial
https://youtu.be/rg6KhqDq-bU
Voxel Chunks Object
https://youtu.be/9Fh5WRbrIGE


[Examples]
Assets/VoxelImporter/Examples


[Support]
mail: support@alonesoft.sakura.ne.jp
twitter: @AlSoSupport


[Update History]
Version 1.1.13p1
- FIX : Change the minimum operation version to Unity2018.4
- FIX : Undo : Fixed the problem that Undo may not work in Unity 2018 LTS or later
- FIX : VoxelSkinnedAnimationObject : Edit Bone Weight : Error correction when there are no voxels on the Mirror side
- FIX : Fixed an issue where an error was displayed on the console in Save processing

Version 1.1.13
- ADD : Voxel Explosion : Supports URP and HDRP (Unity2019.3 or later, Shader Graph 7.3.1 or later)
- FIX : URP : Corrected material import errors
- FIX : VoxelSkinnedAnimationObjectExplosion : Fixed an issue where Mesh could become the initial Pose in Bake.
- FIX : Voxel Explosion : Fixed the problem that Prefab selected in Project Window repeats updating
- FIX : Voxel Explosion : Fixed not to be able to select Preview of Prefab selected in Project Window
- FIX : VoxelFrameAnimationObject : Fixed an error in adding a frame to a vox with a material or invalid face set.
- FIX : VoxelSkinnedAnimationObject : Fixed an issue where the existing Avatar reference was lost and an error occurred when updating PrefabMode
- FIX : Material : Fixed Reset behavior

Version 1.1.12p1
- FIX : ScriptedImporter : Remapped Materials can now be changed with multiple selections.
- FIX : ScriptedImporter : Fixed loss of references when renaming (if the meta data already exists, you will need to change some settings to update the meta data)

Version 1.1.12
- ADD : Unity2020.1 : support
- ADD : Optimize : 'Remove unused palettes' flag added
- FIX : DaeExporter : Update to latest version

Version 1.1.11p3
- FIX : Bug fix when png size and texture size are different

Version 1.1.11p2
- ADD : Unity2019.3 : support
- FIX : Fixed the problem that Gizmos is not displayed in the scene in the hidden state in Unity2019.3
- FIX : Fixed an issue where importing vox with material failed in Russian and other environments

Version 1.1.11p1
- FIX : DaeExporter : Fixed an issue where output fails in Russian and other environments
- FIX : Change the minimum operation version to Unity2017.4

Version 1.1.11
- Add : ScriptedImporter : Added Export flag
- Add : VoxelObject : Added display of ObjectField when empty

Version 1.1.10p1
- FIX : Unity2019.3 : Fixed display issue

Version 1.1.10
- ADD : Unity2019.2 : support
- FIX : Unity2019.3 : Fixed display issue

Version 1.1.9
- Add : ScriptedImporter : Added Mesh mode to support importing Mesh separately
- FIX : ScriptedImporter : Fixed the problem that DropDown etc. do not work properly on Unity 2019.2 or later
- FIX : DaeExporter : Update to latest version

Version 1.1.8p3
- FIX : DaeExporter : Change settings to Importer as much as possible

Version 1.1.8p2
- FIX : MagicaVoxel 0.99.4 support
- FIX : DaeExporter : Update to latest version

Version 1.1.8p1
- FIX : Configure Disable Face : Fix the problem that the character is hard to see with Dark Skin

Version 1.1.8
- ADD : Unity2019.1 : support

Version 1.1.7p5
- ADD : VoxelFrameAnimationObject : Add "Export Animation Curve (Clip)" to output animation information of all frames
- FIX : VoxelFrameAnimationObject : Speeding up import
- FIX : VoxelFrameAnimationObject : Add Repaint after changing Frame
- FIX : VoxelFrameAnimationObjectExplosion : Fixed Explosion Mesh to be created in the current Frame

Version 1.1.7p4
- ADD : Optimize : Addition of "Share same face" flag related to Mesh and texture generation
- ADD : Optimize : Add text to tooltip

Version 1.1.7p3
- ADD : Voxel Chunks Object : Added "Reset All Chunks Transform" button to reset all Chunk to imported Transform
- FIX : DaeExporter : Update to latest version

Version 1.1.7p2
- FIX : Unity2019.1 : Error
- FIX : DaeExporter : Correction of path related

Version 1.1.7p1
- FIX : VoxelObject : Correct problem that can not be read by selecting from the Open button
- FIX : Unity2017.3,4 : Compile error

Version 1.1.7
- ADD : LWRP, HDRP support (Explosion is not yet supported)
- ADD : Material : Fixed to combine when loading material from vox file and same setting
- ADD : ScriptedImporter : Add Set button to Scale and Offset
- FIX : Material : Fix materials reflection from vox file to material
- FIX : Material : Fixed a problem that waste material was created when there was no default material Voxel
- FIX : ScriptedImporter : Action correction when there is no default material Voxel

Version 1.1.6
- ADD : ScriptedImporter : Corresponds to material change and replacement corresponding to Extract Materials and Remap
- FIX : Nested Prefab : Fixing Nest and Variant
- FIX : Correction of icon loading
- FIX : DaeExpolter : Active judgment correction
- FIX : Prefab creation process : Fixed problems that can not be created normally when not selected
- FIX : FrameAnimation : Fix default Mesh name

Version 1.1.5p1
- FIX : Correct import errors occurring in vox

Version 1.1.5
- ADD : Nested Prefab support (Beta) (Unity2018.3.0b7 or later)
- ADD : Import Scale setting Simplification function addition, default value setting supported
- FIX : Fix Prefab creation process
- FIX : VoxelSkinnedAnimationObjectExplosion : Fix error
- FIX : VoxelChunksObjectExplosion : Fix error
- FIX : VoxelObjectExplosion : Fix preview

Version 1.1.4p3
- FIX : VoxelSkinnedAnimationObject : Fix to prevent Refresh when "Edit Bone Animation" edit mode is started

Version 1.1.4p2
- FIX : VoxelSkinnedAnimationObject : When AnimationType is None, change so that Bone can be added and deleted
- FIX : Obsolete API

Version 1.1.4p1
- ADD : Disable Face Editor : Add buttons to manipulate each face collectively
- ADD : Exsamples : ShareFacialAnimation
- FIX : DaeExporter : Export from prefab will fail, so it will be invalidated
- FIX : VoxelFrameAnimationObject : Preview Icon Render
- FIX : VoxelFrameAnimationObject : FrameListWindow : Fixed an issue that information not necessary for Frame change was added to Animation during Animation recording
- FIX : VoxelFrameAnimationObject : Fixed not to be cleared when specified frame name can not be found by change from script

Version 1.1.4
- ADD : Unity2018.2 support
- ADD : Change the minimum version to Unity 5.6.2
- ADD : Configure Disable Face : Discontinue former Enable Face and correspond to more advanced Configure Disable Face
- ADD : VoxelFrameAnimationObject : Added Name to Frame data so that script can change Frame
- ADD : VoxelFrameAnimationObject : Add button to set Mesh to None
- ADD : Generate Tangents : Add flag to generate Tangents
- ADD : Generate Lightmap UVs : Add Advanced setting
- FIX : Voxel Importer 's Script Nested There was a problem with Prefab. We responded so that it can be created normally.
- FIX : VoxelFrameAnimationObject : Refresh immediately after rearrangement causes update failure
- FIX : VoxelSkinnedAnimationObject : Configure Avatar : UI
- FIX : Undo

Version 1.1.3p2
- FIX : Unity2018.1 support
- FIX : Problems hard to see with Dark Skin

Version 1.1.3p1
- FIX : Unity2018.1 : Fixed 'Configure Avatar' error

Version 1.1.3
- ADD : MagicaVoxel 0.99.1 World Editor (multi object) support
- ADD : VoxelChunksObject : Added "Refresh all chunks" button to update completely
- FIX : ColladaExporter : Error handling correction
- FIX : VoxelChunksObject : Disable Refresh operation with Prefab selection
- FIX : Import error for non-lower case extension

Version 1.1.2p1
- FIX : Explosion : Supports not to split Mesh when number of vertices is 65k or more (Only Unity 2017.3 or later)
- FIX : DaeExporter : Correcting errors in the material without the '_Color' property

Version 1.1.2
- ADD : ScriptedImporter
- ADD : BonePosition : Scaling All
- ADD : BonePosition : Change rotation setting possible
- ADD : VoxelStructure
- ADD : Optimize : Combine faces
- ADD : Examples : EditMesh, CreateCubes
- FIX : BonePosition : The position returns when changing the position after setting the Rig type to Humanoid

Version 1.1.1p3
- FIX : Fixed an issue that can not be imported when using .Net 4.6

Version 1.1.1p2
- FIX : Explosion : Fixed an issue where Settings contents were not reflected at run time

Version 1.1.1p1
- FIX : Fixed bug that palette and material can not be read correctly by reading vox saved on MagicaVoxel 0.99 alpha
		Supports only single object data, not reading multiple object data

Version 1.1.1
- ADD : Supports more than 65k vertices (Only Unity 2017.3 or later)
- FIX : Simple mode

Version 1.1.0
- ADD : Simple mode
- ADD : Voxel Skinned Animation Object : Support for adding Animator and Animation components automatically by changing Animation Type
- FIX : HiDPI / Macbook Retina screen resolution support - Problem of misalignment of brush

Version 1.0.9p6
- FIX : ColladaExporter : Animation and Specular color
- FIX : Edit Bone Animation : Mirror side is not recorded in animation

Version 1.0.9p5
- FIX : ColladaExporter : Animation and Material Color
- FIX : Gizmos : Fixed problem that Selection Wire disappeared

Version 1.0.9p4
- FIX : ColladaExporter

Version 1.0.9p3
- FIX : ColladaExporter
- FIX : Skeleton : Preview

Version 1.0.9p2
- FIX : WeightPaint : Undo
- FIX : ColladaExporter : Animation Export

Version 1.0.9p1
- FIX : ColladaExporter : Change FootIK valid at the time of the warning correspondence, FootIK effective to default.
- FIX : Skeleton : Change Preview

Version 1.0.9
- ADD : ColladaExporter : Animation support
- ADD : Unity5.6 or later : Mecanim Humanoid Upper Chest support
- FIX : ColladaExporter : Fixed problem that Hierarchy of file output from 'VoxelSkinnedObject' changed from before output
- FIX : Change operating environment to Unity 5.4.0 or later

Version 1.0.8p3
- FIX : FrameAnimation : Fixed bug in reading additional files outside the project

Version 1.0.8p2
- ADD : Reflect part of MagicaVoxel's material settings

Version 1.0.8p1
- FIX : Added countermeasure for problem that polygon crack is visible at low resolution

Version 1.0.8
- ADD : Collada Exporter
- FIX : Change Voxel Data load timing
- FIX : Import Offset scaling

Version 1.0.7p1
- FIX : Generate Low-Poly Texture
- FIX : Edit Bone Animation - Move
- FIX : Unity 5.6.0 support
- FIX : Explosion : Shader : UNITY_MATRIX_MVP -> UnityObjectToClipPos
 
Version 1.0.7
- ADD : Editor : Window
- ADD : Prefab creation simplification
- FIX : Import Flag
- FIX : Explosion Material
- FIX : Unity5.5 support

Version 1.0.6.p3
- FIX : Manual

Version 1.0.6.p2
- ADD : Bone - "Remove This Bone"
- FIX : HiDPI / Macbook Retina screen resolution support - Problem of position shift

Version 1.0.6.p1
- ADD : ContextMenu - "Remove All Voxel Importer Compornent"
- FIX : Materials - "Load From Voxel File"
- FIX : Voxel Skinned Animation Object - Erase MeshFilter
- FIX : Problem in the case of the Bone of the children of Voxel Skinned Animation Object there is a Voxel Skinned Animation Object
- FIX : Import Mode - Low Poly - Texture generation

Version 1.0.6
- ADD : MagicaVoxel 0.98 support - vox animes and materials
- ADD : Optimize - "Ignore Cavity"
- ADD : Import Offset - Set
- FIX : Frame Animation List Window

Version 1.0.5
- ADD : Voxel Frame Animation
- ADD : Bone Position Editor - "Snap to half-voxel"
- ADD : Editor help
- FIX : Bone weight mirroring

Version 1.0.4.p2
- ADD : Weight preview of the root bone
- ADD : Weight preview transparent display
- ADD : File open in the Drag & Drop
- ADD : Update component flags
- ADD : Context Menu "Save All Unsaved Assets" & "Reset All Assets"
- FIX : Undo

Version 1.0.4.p1
- ADD : Add and remove the root bone
- FIX : Error check

Version 1.0.4
- ADD : Voxel Explosion

Version 1.0.3.p2
- FIX : Prefab Edit
- FIX : Undo
- FIX : Create Avatar

Version 1.0.3.p1
- FIX : Prefab Edit
- FIX : Undo

Version 1.0.3
- ADD : Multi Material
- FIX : Mirroring

Version 1.021
- ADD : Configure Humanoid Avatar
- ADD : Folding corresponding menu

Version 1.02
- ADD : Mecanim Humanoid support
- ADD : Edit Weight Paint : Auto Normalize
- FIX : "Disable Animation" and "Mirror Animation"
- FIX : Modify so that there are no unintended change of "Bone Position"

Version 1.011
- FIX : Weight mirroring process
- FIX : Undo

Version 1.01
- ADD : "Voxel Chunks Object"
- FIX : Weight mirroring process
- FIX : Undo
- FIX : "BoneTemplate" Search
- FIX : Delete shortcut key competing

Version 1.00
- first release

