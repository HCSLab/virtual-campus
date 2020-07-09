-----------------------------------------------------
	Voxel Importer
	Copyright (c) 2016 AloneSoft
	http://alonesoft.sakura.ne.jp/
	mail: support@alonesoft.sakura.ne.jp
	twitter: @AlSoSupport
-----------------------------------------------------

"Voxel Importer"をご購入いただきありがとうございます。


【更新方法】
・Assets/VoxelImporterを削除
・再度VoxelImporterをインポート


【ドキュメント】
Assets/VoxelImporter/Documents


【チュートリアル】
Humanoid Setup Tutorial
https://youtu.be/hkudVsxtxn4
Frame Animation Tutorial
https://youtu.be/rg6KhqDq-bU
Voxel Chunks Object
https://youtu.be/9Fh5WRbrIGE


【サンプル】
Assets/VoxelImporter/Examples


【サポート】
mail: support@alonesoft.sakura.ne.jp
twitter: @AlSoSupport


【更新履歴】
Version 1.1.13p1
- FIX : 最低動作バージョンをUnity2018.4に変更
- FIX : Undo : Unity 2018 LTS以降でUndoが動作しない事がある問題の修正
- FIX : VoxelSkinnedAnimationObject : Edit Bone Weight : Mirror側にボクセルが存在しない場合のエラー修正
- FIX : Save処理でコンソールにエラーが表示される問題の修正

Version 1.1.13
- ADD : Voxel Explosion : URPとHDRPに対応 (Unity2019.3以降、Shader Graph 7.3.1以降)
- FIX : URP : マテリアルのインポートミス修正
- FIX : VoxelSkinnedAnimationObjectExplosion : BakeでMeshが初期ポーズになることがある問題の修正
- FIX : Voxel Explosion : ProjectWindowで選択したPrefabが更新を繰り返す問題の修正
- FIX : Voxel Explosion : ProjectWindowで選択したPrefabのPreviewを選択できないように修正
- FIX : VoxelFrameAnimationObject : マテリアルや無効の面が設定されているvoxへのフレーム追加のエラー修正
- FIX : VoxelSkinnedAnimationObject : 既にあるAvatarの参照が失われる問題とPrefabModeで更新時にエラーが発生する問題の修正
- FIX : Material : Resetの処理修正

Version 1.1.12p1
- FIX : ScriptedImporter : Remapped Materialsを複数選択して行えるように修正
- FIX : ScriptedImporter : 名前の変更で参照が失われる問題の修正 (既にmetaデータが存在する場合、何らかの設定を変更してmetaデータが更新される必要があります)

Version 1.1.12
- ADD : Unity2020.1 : 対応
- ADD : Optimize : 'Remove unused palettes'フラグ追加
- FIX : DaeExporter : 最新版へ更新

Version 1.1.11p3
- FIX : pngのサイズとtextureサイズが異なる場合の不具合修正

Version 1.1.11p2
- ADD : Unity2019.3 : 対応
- FIX : Unity2019.3でGizmos非表示状態でSceneに表示されない問題の修正
- FIX : ロシア語などの環境でマテリアルを持つvoxのインポートに失敗する問題の修正

Version 1.1.11p1
- FIX : DaeExporter : ロシア語などの環境で出力が失敗する問題の修正
- FIX : 最低動作バージョンをUnity2017.4に変更

Version 1.1.11
- Add : ScriptedImporter : Exportフラグを追加
- Add : VoxelObject : 空の状態時にObjectFieldを表示追加

Version 1.1.10p1
- FIX : Unity2019.3 : 表示の問題を修正

Version 1.1.10
- ADD : Unity2019.2 : 対応
- FIX : Unity2019.3 : 表示の問題を修正

Version 1.1.9
- Add : ScriptedImporter : MeshModeを追加しMesh個別でのインポートに対応
- FIX : ScriptedImporter : Unity2019.2以降でDropDownなどが正常に動作しない問題の修正
- FIX : DaeExporter : 最新版へ更新

Version 1.1.8p3
- FIX : DaeExporter : Importerにできるだけ設定をそのまま引き継ぐように変更

Version 1.1.8p2
- FIX : MagicaVoxel 0.99.4 support
- FIX : DaeExporter : 最新版へ更新

Version 1.1.8p1
- FIX : Configure Disable Face : Dark Skinで見えにくい問題の修正

Version 1.1.8
- ADD : Unity2019.1 : 対応

Version 1.1.7p5
- ADD : VoxelFrameAnimationObject : すべてのFrameのアニメーション情報を出力する"Export Animation Curve (Clip)"を追加
- FIX : VoxelFrameAnimationObject : インポートの高速化
- FIX : VoxelFrameAnimationObject : Frame変更後のRepaint追加
- FIX : VoxelFrameAnimationObjectExplosion : 現在のFrameで爆発Meshが作成されるように修正

Version 1.1.7p4
- ADD : Optimize : Meshとテクスチャ生成に関係する"Share same face"フラグの追加
- ADD : Optimize : Tooltipにテキストを追加

Version 1.1.7p3
- ADD : Voxel Chunks Object : すべてのChunkをインポートしたTransformへリセットする"Reset All Chunks Transform"ボタンの追加
- FIX : DaeExporter : 最新版へ更新

Version 1.1.7p2
- FIX : Unity2019.1 : エラー修正
- FIX : DaeExporter : パス関連の修正

Version 1.1.7p1
- FIX : VoxelObject : Openボタンから選択しての読み出しができない問題の修正
- FIX : Unity2017.3,4 : コンパイルエラー修正

Version 1.1.7
- ADD : LWRP, HDRPの対応 (Explosionはまだ未対応)
- ADD : Material : voxファイルからのマテリアル読み込みで同一設定の場合は纏めるよう修正
- ADD : ScriptedImporter : Scale, OffsetにSetボタン追加
- FIX : Material : voxファイルからのマテリアル読み込みのマテリアルへの反映を修正
- FIX : Material : デフォルトマテリアルのVoxelが一つもない場合に無駄なマテリアルが作成される問題の修正
- FIX : ScriptedImporter : デフォルトマテリアルのVoxelが一つもない場合の動作修正

Version 1.1.6
- ADD : ScriptedImporter : Extract MaterialsとRemapに対応してマテリアルの変更と置き換えに対応
- FIX : Nested Prefab : NestとVariantの修正
- FIX : iconのロード修正
- FIX : DaeExpolter : active判定修正
- FIX : Prefab作成処理 : 選択していない状態で正常に作成できない問題など修正
- FIX : FrameAnimation : Mesh名デフォルト修正

Version 1.1.5p1
- FIX : voxで発生するインポートエラーの修正

Version 1.1.5
- ADD : Nested Prefab対応 (ベータ版) (Unity2018.3.0b7以降)
- ADD : Import Scaleの設定簡易化機能追加、デフォルト値設定対応
- FIX : Prefab作成処理修正
- FIX : VoxelSkinnedAnimationObjectExplosion : エラーの修正
- FIX : VoxelChunksObjectExplosion : エラーの修正
- FIX : VoxelObjectExplosion : Previewの修正

Version 1.1.4p3
- FIX : VoxelSkinnedAnimationObject : "Edit Bone Animation"の編集モード開始時にRefreshが行われないよう修正

Version 1.1.4p2
- FIX : VoxelSkinnedAnimationObject : AnimationTypeがNoneの場合にはBoneの追加と削除が可能なように変更
- FIX : Obsolete API

Version 1.1.4p1
- ADD : Disable Face Editor : 面ごとまとめて操作するボタンを追加
- ADD : Exsamples : ShareFacialAnimation
- FIX : DaeExporter : prefabからの出力は失敗するため無効化
- FIX : VoxelFrameAnimationObject : PreviewIconレンダリング修正
- FIX : VoxelFrameAnimationObject : FrameListWindow : Animation記録中にFrame変更に必要のない情報がAnimationに追加される問題の修正
- FIX : VoxelFrameAnimationObject : スクリプトからの変更で指定フレーム名が見つからない場合にクリアされないように修正

Version 1.1.4
- ADD : Unity2018.2サポート
- ADD : 最低バージョンをUnity5.6.2に変更
- ADD : Configure Disable Face : 旧Enable Faceを廃止しより高度なConfigure Disable Faceに対応
- ADD : VoxelFrameAnimationObject : FrameデータにNameを追加しスクリプトからFrameを変更できるように対応
- ADD : VoxelFrameAnimationObject : MeshをNoneにできるボタンを追加
- ADD : Generate Tangents : Tangentを生成するフラグを追加
- ADD : Generate Lightmap UVs : Advanced設定の追加
- FIX : Voxel ImporterのScriptが入れ子状態のPrefab化に対応
- FIX : VoxelFrameAnimationObject : 順番の並び替え直後のRefreshで更新不具合
- FIX : VoxelSkinnedAnimationObject : Configure Avatar : UI修正
- FIX : Undo

Version 1.1.3p2
- FIX : Unity2018.1サポート
- FIX : DarkSkinで見えづらい問題の修正

Version 1.1.3p1
- FIX : Unity2018.1 : 'Configure Avatar'のエラーを修正

Version 1.1.3
- ADD : MagicaVoxel 0.99.1 World Editor(マルチオブジェクト)対応
- ADD : VoxelChunksObject : 完全に更新する"Refresh all chunks"ボタンを追加
- FIX : ColladaExporter : エラー処理修正
- FIX : VoxelChunksObject : Prefab選択でのRefresh操作を無効化
- FIX : 小文字ではない拡張子の場合のインポートエラー

Version 1.1.2p1
- FIX : Explosion : 65000以上の頂点数になる場合にMeshを分割しないよう対応 (Unity 2017.3以降のみ)
- FIX : DaeExporter : '_Color'プロパティがないマテリアルでのエラー修正

Version 1.1.2
- ADD : ScriptedImporter対応
- ADD : BonePosition : Scaling All
- ADD : BonePosition : 回転の設定を可能に変更
- ADD : VoxelStructure
- ADD : Optimize : Combine faces
- ADD : Examples : EditMesh, CreateCubes
- FIX : BonePosition : Humanoid設定後の位置変更が戻る不具合

Version 1.1.1p3
- FIX : .Net 4.6使用時にインポートできないことがある問題の修正

Version 1.1.1p2
- FIX : Explosion : Settingsの内容が実行時に反映されていなかった問題の修正

Version 1.1.1p1
- FIX : MagicaVoxel 0.99 alphaで保存されたvox読み出しでパレットとマテリアルが正しく読めない不具合修正 (単体オブジェクトデータのみ対応、複数オブジェクトデータの読み出し未対応)

Version 1.1.1
- ADD : 65000以上の頂点数になる場合に正常に表示されるよう対応 (Unity 2017.3以降のみ)
- FIX : Simple mode

Version 1.1.0
- ADD : Simple mode
- ADD : Voxel Skinned Animation Object : Animation Typeの変更でAnimatorやAnimationコンポーネントを自動的に追加するように対応
- FIX : HiDPI / Macbook Retina screen resolution support - ブラシ位置ずれ問題

Version 1.0.9p6
- FIX : ColladaExporter : Animation and Specular color
- FIX : Edit Bone Animation : Mirror側がアニメーションに記録されない不具合

Version 1.0.9p5
- FIX : ColladaExporter : Animation and Material Color
- FIX : Gizmos : Selection Wireが表示されなくなる問題の修正

Version 1.0.9p4
- FIX : ColladaExporter

Version 1.0.9p3
- FIX : ColladaExporter
- FIX : Skeleton : 表示を修正

Version 1.0.9p2
- FIX : WeightPaint : Undo修正
- FIX : ColladaExporter : Animation Export

Version 1.0.9p1
- FIX : ColladaExporter : FootIK有効時の警告対応、FootIK有効をデフォルトに変更
- FIX : Skeleton : 表示を変更

Version 1.0.9
- ADD : ColladaExporter : Animation出力対応
- ADD : Unity5.6 or later : Mecanim Humanoid Upper Chest対応
- FIX : ColladaExporter : VoxelSkinnedObjectから出力したファイルのHierarchyが出力前と変わる問題の修正
- FIX : 動作環境をUnity5.4.0以上に変更

Version 1.0.8p3
- FIX : FrameAnimation : プロジェクト外のファイルの追加読み出し不具合

Version 1.0.8p2
- ADD : MagicaVoxelのマテリアル設定の一部を反映

Version 1.0.8p1
- FIX : 低解像度でポリゴンの隙間が見える問題の対策追加

Version 1.0.8
- ADD : Collada(dae)ファイル出力
- FIX : VoxelDataロードタイミングを変更
- FIX : ImportOffsetのScale反映バグ

Version 1.0.7p1
- FIX : Generate Low-Poly Texture
- FIX : Edit Bone Animation - Move
- FIX : Unity 5.6.0 support
- FIX : Explosion : Shader : UNITY_MATRIX_MVP -> UnityObjectToClipPos
 
Version 1.0.7
- ADD : EditorのWindow化
- ADD : Prefab作成簡易化
- FIX : Import Flag
- FIX : Explosion Material
- FIX : Unity5.5 support

Version 1.0.6.p3
- FIX : マニュアル

Version 1.0.6.p2
- ADD : Bone - "Remove This Bone"
- FIX : HiDPI / Macbook Retina screen resolution support - 位置がずれる問題

Version 1.0.6.p1
- ADD : ContextMenu - "Remove All Voxel Importer Compornent"
- FIX : Materials - "Load From Voxel File"
- FIX : Voxel Skinned Animation Object - Erase MeshFilter
- FIX : Voxel Skinned Animation Object のboneにVoxel Skinned Animation Objectがある場合の不具合
- FIX : Import Mode - Low Poly テクスチャ生成

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
- ADD : ルートボーンのウェイトプレビュー
- ADD : ウェイトプレビュー半透明表示
- ADD : Drag&Dropでのファイルオープン
- ADD : Component反映フラグ
- ADD : Contextメニュー "Save All Unsaved Assets" & "Reset All Assets"
- FIX : Undo

Version 1.0.4.p1
- ADD : ルートボーンの追加削除機能
- FIX : エラーチェック強化

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
- ADD : マルチマテリアル対応
- FIX : ミラーリング処理

Version 1.021
- ADD : Humanoid Avatarの設定画面
- ADD : メニュー折りたたみ関係

Version 1.02
- ADD : Mecanim Humanoid サポート
- ADD : Edit Weight Paint : Auto Normalize
- FIX : DisableAnimation & MirrorAnimationの高速化
- FIX : Bone Positionに意図しない変更が起きないよう対応

Version 1.011
- FIX : Weightのミラーリング処理
- FIX : Undo全般

Version 1.01
- ADD : "Voxel Chunks Object"
- FIX : Weightのミラーリング処理
- FIX : Undo全般
- FIX : BoneTemplate検索
- FIX : 競合するショートカットキーの削除

Version 1.00
- ファーストリリース
