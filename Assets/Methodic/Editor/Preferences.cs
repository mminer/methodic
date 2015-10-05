//
// Author: Matthew Miner
//         matthew@matthewminer.com
//         http://matthewminer.com
//
// Copyright (c) 2015
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
		static bool showStatic;
		static bool showPrivate;

		internal static BindingFlags reflectionOptions
		{
			get {
				var flags = BindingFlags.Public |
				            BindingFlags.Instance |
				            BindingFlags.DeclaredOnly;

				if (showStatic) {
					flags |= BindingFlags.Static;
				}

				if (showPrivate) {
					flags |= BindingFlags.NonPublic;
				}

				return flags;
			}
		}

		// EditorPrefs keys.
		const string showStaticKey = "methodic_include_static";
		const string showPrivateKey = "methodic_include_private";

		// Toggle labels.
		static readonly GUIContent showStaticLabel = new GUIContent("Show Static", "Show methods beyond those belonging to the instance.");
		static readonly GUIContent showPrivateLabel = new GUIContent("Show Private", "Show methods unavailable outside the class.");

		/// <summary>
		/// Loads options stored in EditorPrefs.
		/// </summary>
		static Preferences ()
		{
			showStatic = EditorPrefs.GetBool(showStaticKey, true);
			showPrivate = EditorPrefs.GetBool(showPrivateKey, true);
		}

		/// <summary>
		/// Displays preferences GUI.
		/// </summary>
		[PreferenceItem("Methodic")]
		static void OnGUI ()
		{
			showStatic = EditorGUILayout.Toggle(showStaticLabel, showStatic);
			showPrivate = EditorGUILayout.Toggle(showPrivateLabel, showPrivate);

			if (GUI.changed) {
				SavePreferences();
				RefreshEditorWindow();
			}
		}

		/// <summary>
		/// Saves the preferences to disk.
		/// </summary>
		static void SavePreferences ()
		{
			EditorPrefs.SetBool(showStaticKey, showStatic);
			EditorPrefs.SetBool(showPrivateKey, showPrivate);
		}

		/// <summary>
		/// Tells the editor window to update.
		/// </summary>
		static void RefreshEditorWindow ()
		{
			if (!Window.isOpen) {
				return;
			}

			var window = EditorWindow.GetWindow<Window>("Methodic", false);
			window.Refresh();
		}
	}
}
