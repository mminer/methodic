# Methodic

An editor extension for Unity to test and interact with functions through a GUI.
An editor window provides a simple interface to select your script's functions
and run them with parameters of your choice. Ideal for manually triggering
events during gameplay and for modifying game objects in the editor.

The package can be purchased and downloaded from the
[Unity Asset Store](http://u3d.as/content/matthew-miner/methodic/1Xw).


## Getting Started

Open Methodic via Window > General > Methodic. I like to keep this window docked
below or beside the Inspector panel.

Click on a game object with your custom scripts attached. Methodic displays
the script's functions in a snazzy dropdown. If the selected function accepts
parameters, they are displayed in editable form below the dropdown.

Once satisfied with the parameter values to send, click the Invoke button to
run the function. If function returns a value, it is displayed in the Console.

Display options are available in Unity's preferences pane for the adventurous.


## Limitations

Methodic currently lacks support for array parameters. In such cases, a null
value is sent when the function in invoked. This also happens when the
parameter type is unrecognized; say, if it's a class of your own creation.


## Compatibility

Requires Unity 2020.3 or later.


## Version History

1.3.1 - May 15, 2022
- Fix lock option
- Show lock button in window toolbar
- Move delay field to settings popup

1.3.0 - January 6, 2022
- Add option to invoke method after delay
- Move preferences to window's custom menu
- Move Methodic menu to Window > General

1.2.2 - July 15, 2019
- Migrate from `PreferenceItem` to `SettingsProvider` to avoid deprecation
  warning

1.2.1 - April 24, 2016
- Add icon to editor window title

1.2 - January 27, 2014
- Split components into separate dropdown
- Add option to lock onto a selected game object

1.1.2 - December 24, 2013
- Replace deprecated UnityEngine.Undo function use

1.1.1 - May 9, 2012
- Added demo scene
- Fixed failed repaint on selection change
- Tweaked editor window layout

1.1 - May 5, 2012
- Moved preferences to Unity's preferences window
- Updated EditorGUILayout.ObjectField() call to remove deprecation warning
- Namespaced editor window; this change drops support for Unity 3

1.0.1 - June 23, 2011
- Initial release


## Contact

Questions or suggestions? Please contact me at matthew@matthewminer.com.
