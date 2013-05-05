//
// Author: Matthew Miner
//         matthew@matthewminer.com
//         http://matthewminer.com
//
// Copyright (c) 2013
//

using UnityEditor;

namespace Methodic
{
	/// <summary>
	/// Menu items to open Methodic.
	/// </summary>
	static class Menus
	{
		[MenuItem("Window/Methodic %#m")]
		static void OpenMethodic ()
		{
			// Get existing open window, or make new one if there isn't one.
			EditorWindow.GetWindow<Window>("Methodic");
		}
	}
}
