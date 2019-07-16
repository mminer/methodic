//
// Author: Matthew Miner
//         matthew@matthewminer.com
//         https://matthewminer.com
//
// Copyright (c) 2016
//

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Methodic
{
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

        [SettingsProvider]
        static SettingsProvider CreateSettingsProvider()
        {
            return new SettingsProvider("Project/Methodic", SettingsScope.User, new[] { "Methodic", "Method" })
            {
                activateHandler = (searchContext, rootElement) => Load(),
                guiHandler = searchContext => DisplayGUI()
            };
        }

        static void DisplayGUI()
        {
            showPrivate = EditorGUILayout.Toggle(showPrivateLabel, showPrivate);
            showStatic = EditorGUILayout.Toggle(showStaticLabel, showStatic);

            if (GUI.changed) {
                Save();

                if (OnPreferencesChange != null) {
                    OnPreferencesChange();
                }
            }
        }

        static void Load()
        {
            showPrivate = EditorPrefs.GetBool(showPrivateKey, true);
            showStatic = EditorPrefs.GetBool(showStaticKey, true);
        }

        static void Save()
        {
            EditorPrefs.SetBool(showPrivateKey, showPrivate);
            EditorPrefs.SetBool(showStaticKey, showStatic);
        }
    }
}
