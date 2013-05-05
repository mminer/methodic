//
// Author: Matthew Miner
//         matthew@matthewminer.com
//         http://matthewminer.com
//
// Copyright (c) 2013
//

using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Methodic
{
	/// <summary>
	/// Options for controlling which methods are shown / executable.
	/// </summary>
	static class Preferences
	{
		internal static bool showStatic { get; private set; }
		internal static bool showPrivate { get; private set; }
		internal static bool displayClass { get; private set; }

		internal static BindingFlags reflectionFlags
		{
			get {
				var flags = BindingFlags.Public |
				            BindingFlags.Instance |
				            BindingFlags.DeclaredOnly;
				if (showStatic) { flags |= BindingFlags.Static; }
				if (showPrivate) { flags |= BindingFlags.NonPublic; }
				return flags;
			}
		}

		// EditorPrefs keys.
		const string showStaticKey = "methodic_include_static";
		const string showPrivateKey = "methodic_include_private";
		const string displayClassKey = "methodic_display_class";

		// Toggle labels.
		static readonly GUIContent showStaticLabel = new GUIContent(
			"Show Static",
			"Show methods beyond those belonging to the instance.");
		static readonly GUIContent showPrivateLabel = new GUIContent(
			"Show Private",
			"Show methods unavailable outside the class.");
		static readonly GUIContent displayClassLabel = new GUIContent(
			"Display Class",
			"Show the class name beside the method name.");

		/// <summary>
		/// Loads options stored in EditorPrefs.
		/// </summary>
		static Preferences ()
		{
			showStatic = EditorPrefs.GetBool(showStaticKey, true);
			showPrivate = EditorPrefs.GetBool(showPrivateKey, true);
			displayClass = EditorPrefs.GetBool(displayClassKey, false);
		}

		/// <summary>
		/// Displays preferences GUI.
		/// </summary>
		[PreferenceItem("Methodic")]
		public static void OnGUI ()
		{
			showStatic = EditorGUILayout.Toggle(showStaticLabel, showStatic);
			showPrivate = EditorGUILayout.Toggle(showPrivateLabel, showPrivate);
			displayClass = EditorGUILayout.Toggle(displayClassLabel, displayClass);

			if (GUI.changed) {
				// Save preferences.
				EditorPrefs.SetBool(showStaticKey, showStatic);
				EditorPrefs.SetBool(showPrivateKey, showPrivate);
				EditorPrefs.SetBool(displayClassKey, displayClass);

				// Update editor window.
				EditorWindow.GetWindow<Window>().OnSelectionChange();
			}
		}
	}
}
