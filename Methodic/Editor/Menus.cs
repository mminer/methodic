//
// Menus.cs
//
// Author: Matthew Miner (matthew@matthewminer.com)
// Copyright (c) 2012
//

using UnityEditor;
using UnityEngine;

namespace Methodic
{
	/// <summary>
	/// Menu items to open Methodic.
	/// </summary>
	public class Menus
	{
		[MenuItem ("Window/Methodic %#m")]
		static void OpenMethodic ()
		{
			// Get existing open window, or make new one if none
			EditorWindow.GetWindow<MethodicWindow>("Methodic");
		}
	}
}

