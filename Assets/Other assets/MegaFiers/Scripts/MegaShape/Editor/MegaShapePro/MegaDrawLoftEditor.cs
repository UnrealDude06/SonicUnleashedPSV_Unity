using UnityEngine;
using UnityEditor;

namespace MegaFiers
{
	[CustomEditor(typeof(MegaDrawLoft))]
	public class MegaDrawLoftEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			MegaDrawLoft mod = (MegaDrawLoft)target;
	#if !UNITY_5 && !UNITY_2017 && !UNITY_2018 && !UNITY_2019 && !UNITY_2020 && !UNITY_2021 && !UNITY_2022 && !UNITY_2023
			EditorGUIUtility.LookLikeControls();
	#endif

			mod.updatedist = Mathf.Clamp(EditorGUILayout.FloatField("Update Dist", mod.updatedist), 0.02f, 100.0f);
			mod.smooth = EditorGUILayout.Slider("Smooth", mod.smooth, 0.0f, 1.5f);
			mod.offset = EditorGUILayout.FloatField("Offset", mod.offset);
			mod.radius = EditorGUILayout.FloatField("Gizmo Radius", mod.radius);
			mod.closed = EditorGUILayout.Toggle("Build Closed", mod.closed);
			mod.closevalue = EditorGUILayout.Slider("Close Value", mod.closevalue, 0.0f, 1.0f);
			mod.constantspd = EditorGUILayout.Toggle("Constant Speed", mod.constantspd);
			mod.loft = (MegaShapeLoft)EditorGUILayout.ObjectField("Loft", mod.loft, typeof(MegaShapeLoft), true);

			if ( GUI.changed )
				EditorUtility.SetDirty(mod);
		}
	}
}