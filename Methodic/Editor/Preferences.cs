//
// Preferences.cs
//
// Author: Matthew Miner (matthew@matthewminer.com)
// Copyright (c) 2012
//

using UnityEditor;
using UnityEngine;
using System.Reflection;

namespace Methodic
{
	/// <summary>
	/// Options for controlling which methods are shown / available for execution.
	/// </summary>
	public static class Preferences
	{
		static readonly GUIContent showStaticLabel = new GUIContent("Show Static", "Show methods beyond those belonging to the instance.");
		static readonly GUIContent showPrivateLabel = new GUIContent("Show Private", "Show methods unavailable outside the class.");
		static readonly GUIContent displayClassLabel = new GUIContent("Display Class", "Show the class name beside the method name.");

		const string showStaticKey = "methodic_include_static";
		const string showPrivateKey = "methodic_include_private";
		const string displayClassKey = "methodic_display_class";

		public static bool showStatic { get; private set; }
		public static bool showPrivate { get; private set; }
		public static bool displayClass { get; private set; }

		const BindingFlags constantFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

		public static BindingFlags flags {
			get {
				var _flags = constantFlags;
				if (showStatic) { _flags |= BindingFlags.Static; }
				if (showPrivate) { _flags |= BindingFlags.NonPublic; }
				return _flags;
			}
		}

		/// <summary>
		/// Loads options stored in EditorPrefs.
		/// </summary>
		static Preferences ()
		{
			showStatic = EditorPrefs.GetBool(showStaticKey, true);
			showPrivate = EditorPrefs.GetBool(showPrivateKey, true);
			displayClass = EditorPrefs.GetBool(displayClassKey, false);
		}

		public static void OnGUI ()
		{
			// Ignore changes to previous GUI elements
			GUI.changed = false;

			showStatic = EditorGUILayout.Toggle(showStaticLabel, Preferences.showStatic);
			showPrivate = EditorGUILayout.Toggle(showPrivateLabel, Preferences.showPrivate);
			displayClass = EditorGUILayout.Toggle(displayClassLabel, Preferences.displayClass);

			if (GUI.changed) {
				Save();
				EditorWindow.GetWindow<MethodicWindow>().DiscoverMethods();
			}
		}

		/// <summary>
		/// Saves options to EditorPrefs for later retrieval.
		/// </summary>
		static void Save ()
		{
			EditorPrefs.SetBool(showStaticKey, showStatic);
			EditorPrefs.SetBool(showPrivateKey, showPrivate);
			EditorPrefs.SetBool(displayClassKey, displayClass);
		}
	}
}

