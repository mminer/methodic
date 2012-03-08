//
// Util.cs
//
// Author: Matthew Miner (matthew@matthewminer.com)
// Copyright (c) 2012
//

using UnityEditor;
using UnityEngine;

namespace Methodic
{
	/// <summary>
	/// Utility functions.
	/// </summary>
	public static class Util
	{
		/// <summary>
		/// Displays a visibile dividing line between GUI components.
		/// </summary>
		public static void DrawDivider ()
		{
			EditorGUILayout.Space();

			var rect = GUILayoutUtility.GetLastRect();
			var top = rect.yMax - (rect.height / 2) - 1;
			var start = new Vector3(0, top);
			var end = new Vector3(rect.width, top);

			Handles.color = Color.grey;
			Handles.DrawLine(start, end);
		}
	}
}

