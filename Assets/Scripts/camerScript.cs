using UnityEngine;
using System.Collections;

public class camerScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.UpArrow)) {
			Vector3 temp = transform.position;
			temp += new Vector3 (0.0f, 0.0f, 10.0f);
			transform.position = temp;
		}

		if (Input.GetKey (KeyCode.DownArrow)) {
			Vector3 temp = transform.position;
			temp += new Vector3 (0.0f, 0.0f, -10.0f);
			transform.position = temp;
		}

		if (Input.GetKey (KeyCode.LeftArrow)) {
			Vector3 temp = transform.position;
			temp += new Vector3 (-10.0f, 0.0f, 0.0f);
			transform.position = temp;
		}

		if (Input.GetKey (KeyCode.RightArrow)) {
			Vector3 temp = transform.position;
			temp += new Vector3 (10.0f, 0.0f, 0.0f);
			transform.position = temp;
		}
		/*
		if (Input.GetKey (KeyCode.A)) {
			transform.Rotate (Vector3.down, Space.Self);
		}

		if (Input.GetKey (KeyCode.D)) {
			transform.Rotate (Vector3.up, Space.Self);
		}
*/
/*
		if (Input.GetKey (KeyCode.W)) {
			transform.Rotate (Vector3.left, Space.Self);
		}

		if (Input.GetKey (KeyCode.S)) {
			transform.Rotate (Vector3.right, Space.Self);
		}
        */
	}
}
