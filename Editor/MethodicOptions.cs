// Copyright (c) 2011 Matthew Miner.

using UnityEditor;
using UnityEngine;
using System.Reflection;

/// <summary>
/// Options for controlling which methods are shown / available for execution.
/// </summary>
public static class MethodicOptions
{
	const string showStaticKey = "methodic_include_static";
	const string showPrivateKey = "methodic_include_private";
	const string displayClassKey = "methodic_display_class";
	
	public static bool showStatic { get; set; }
	public static bool showPrivate { get; set; }
	public static bool displayClass { get; set; }
	
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
	static MethodicOptions ()
	{
		showStatic = EditorPrefs.GetBool(showStaticKey, true);
		showPrivate = EditorPrefs.GetBool(showPrivateKey, true);
		displayClass = EditorPrefs.GetBool(displayClassKey, false);
	}
	
	/// <summary>
	/// Saves options to EditorPrefs for later retrieval.
	/// </summary>
	public static void Save ()
	{
		EditorPrefs.SetBool(showStaticKey, showStatic);
		EditorPrefs.SetBool(showPrivateKey, showPrivate);
		EditorPrefs.SetBool(displayClassKey, displayClass);
	}
}