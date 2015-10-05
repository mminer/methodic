using UnityEngine;

/// <summary>
/// Another demo script showing other functionality of Methodic.
/// </summary>
public class CustomComponent : MonoBehaviour
{
	/// <summary>
	/// Demonstrates printing a return value.
	/// </summary>
	static float PythagoreanTheorem (float a, float b)
	{
		var c = Mathf.Sqrt(Mathf.Pow(a, 2) + Mathf.Pow(b, 2));
		return c;
	}

	/// <summary>
	/// Demonstrates modifying a property of the target game object.
	/// </summary>
	void SetBoxName (string name)
	{
		gameObject.name = name;
		Debug.Log("Changing box name to " + name + ".");
	}

	/// <summary>
	/// Demonstrates parameters of several types.
	/// This doesn't actually do anything particularly special with them.
	/// </summary>
	void ModifyProperties (Vector3 position, Color color, KeyCode key, Rect rectangle, bool toggle)
	{
		Debug.Log("Method invoked (doing nothing particularly special with parameters.");
	}
}
