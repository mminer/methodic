using UnityEngine;

/// <summary>
/// Demo script showing game object manipulation.
/// </summary>
public class BoxController : MonoBehaviour
{
	void MoveLeft ()
	{
		transform.Translate(Vector3.left);
		Debug.Log("Moving box left.");
	}

	void MoveRight ()
	{
		transform.Translate(Vector3.right);
		Debug.Log("Moving box right.");
	}

	void MoveCustomDirection (Vector2 direction)
	{
		transform.Translate(direction);
		Debug.Log("Moving box in a custom direction.");
	}
}
