//
// MethodicWindow.cs
//
// Author: Matthew Miner
//         matthew@matthewminer.com
//         http://www.matthewminer.com/
//
// Copyright (c) 2012
//

using Methodic;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// The Methodic editor window.
/// </summary>
public class MethodicWindow : EditorWindow
{
	enum Panel { Main, Preferences }

	static readonly GUIContent optionsLabel = new GUIContent("Preferences", "Customize which methods are shown.");

	Panel selectedPanel;

	void OnGUI ()
	{
		// Toolbar
		EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

			GUILayout.FlexibleSpace();

			var optionsToggle = GUILayout.Toggle(selectedPanel == Panel.Preferences, optionsLabel, EditorStyles.toolbarButton);
			selectedPanel = optionsToggle ? Panel.Preferences : Panel.Main;

		EditorGUILayout.EndHorizontal();

		// Panel
		switch (selectedPanel) {
			case Panel.Main:
				Manager.OnGUI();
				break;

			case Panel.Preferences:
				Preferences.OnGUI();
				break;
		}
	}

	public void OnSelectionChange ()
	{
		Manager.DiscoverMethods(Selection.activeGameObject);
		Repaint();
	}
}

