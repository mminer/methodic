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


## Contact

Questions or suggestions? Please contact me at matthew@matthewminer.com.
