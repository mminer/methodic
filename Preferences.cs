//
// Author: Matthew Miner
//         matthew@matthewminer.com
//         http://matthewminer.com
//
// Copyright (c) 2016
//

using System;
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
        internal static event Action OnPreferencesChange;

        internal static BindingFlags reflectionOptions
        {
            get {
                var flags = standardRelectionOptions;

                if (showPrivate) {
                    flags |= BindingFlags.NonPublic;
                }

                if (showStatic) {
                    flags |= BindingFlags.Static;
                }

                return flags;
            }
        }

        static bool showPrivate;
        static bool showStatic;

        const BindingFlags standardRelectionOptions =
            BindingFlags.DeclaredOnly |
            BindingFlags.Instance |
            BindingFlags.Public;

        // EditorPrefs keys.
        const string showPrivateKey = "methodic_include_private";
        const string showStaticKey = "methodic_include_static";

        // Toggle labels:

        static readonly GUIContent showPrivateLabel = new GUIContent(
            "Show Private",
            "Show methods unavailable outside the class."
        );

        static readonly GUIContent showStaticLabel = new GUIContent(
            "Show Static",
            "Show methods beyond those belonging to the instance."
        );

        static Preferences ()
        {
            LoadPreferences();
        }

        /// <summary>
        /// Displays preferences GUI.
        /// </summary>
        [PreferenceItem("Methodic")]
        static void OnGUI ()
        {
            showPrivate = EditorGUILayout.Toggle(showPrivateLabel, showPrivate);
            showStatic = EditorGUILayout.Toggle(showStaticLabel, showStatic);

            if (GUI.changed) {
                SavePreferences();

                if (OnPreferencesChange != null) {
                    OnPreferencesChange();
                }
            }
        }

        /// <summary>
        /// Reads the preferences from disk.
        /// </summary>
        static void LoadPreferences ()
        {
            showPrivate = EditorPrefs.GetBool(showPrivateKey, true);
            showStatic = EditorPrefs.GetBool(showStaticKey, true);
        }

        /// <summary>
        /// Saves the preferences to disk.
        /// </summary>
        static void SavePreferences ()
        {
            EditorPrefs.SetBool(showPrivateKey, showPrivate);
            EditorPrefs.SetBool(showStaticKey, showStatic);
        }
    }
}
