// Copyright (c) 2011 Matthew Miner.

using UnityEditor;
using UnityEngine;
using System.Reflection;

/// <summary>
/// Options for controlling which methods are shown / available for execution.
/// </summary>
public static class MethodicPrefs
{
	const string showStaticKey = "methodic_show_static";
	const string showPrivateKey = "methodic_show_private";
	
	static bool showStatic;
	static bool showPrivate;
	
	static readonly GUIContent optionsLabel = new GUIContent("Options");
	static readonly GUIContent showStaticLabel = new GUIContent("Show Static", "Show methods beyond those belonging to the instance.");
	static readonly GUIContent showPrivateLabel = new GUIContent("Show Private", "Show methods unavailable outside the class.");
	
	static bool showOptions;
	static bool prefsLoaded;
	
	const BindingFlags constantFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
	
	public static BindingFlags flags {
		get {
			if (!prefsLoaded) {
				LoadPrefs();
			}
			
			var _flags = constantFlags;
			if (showStatic) { _flags |= BindingFlags.Static; }
			if (showPrivate) { _flags |= BindingFlags.NonPublic; }
			return _flags;
		}
	}
	
	public static void OnGUI ()
	{
		if (!prefsLoaded) {
			LoadPrefs();
		}
		
		showOptions = EditorGUILayout.Foldout(showOptions, optionsLabel);
		
		if (showOptions) {
			// Ignore changes to previous GUI elements
			GUI.changed = false;
			
			EditorGUI.indentLevel = 2;
			showStatic = EditorGUILayout.Toggle(showStaticLabel, showStatic);
			showPrivate = EditorGUILayout.Toggle(showPrivateLabel, showPrivate);
			EditorGUI.indentLevel = 0;
			
			// Resave and reload methods shown if options are changed
			if (GUI.changed) {
				SavePrefs();
				Methodic.DiscoverMethods();
			}
		}
	}
	
	/// <summary>
	/// Loads options stored in EditorPrefs.
	/// </summary>
	static void LoadPrefs ()
	{
		showStatic = EditorPrefs.GetBool(showStaticKey, true);
		showPrivate = EditorPrefs.GetBool(showPrivateKey, true);
		prefsLoaded = true;
	}
	
	/// <summary>
	/// Saves options to EditorPrefs for later retrieval.
	/// </summary>
	static void SavePrefs ()
	{
		EditorPrefs.SetBool(showStaticKey, showStatic);
		EditorPrefs.SetBool(showPrivateKey, showPrivate);
	}
}