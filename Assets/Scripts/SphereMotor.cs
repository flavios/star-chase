using UnityEngine;

public class SphereMotor : MonoBehaviour {
	public Rigidbody rb;
	public float turnSpeed = 5.0f;
	public float thrust = 25.0f;

	void Start() {
		rb = GetComponent<Rigidbody>();

		rb.velocity = transform.forward * thrust;
		rb.angularVelocity = new Vector3 (0.0f, 0.0f, 0.0f);
	}


	void FixedUpdate() {

		var angle = new Vector3 (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"), 0.0f);

		rb.velocity = ((transform.forward + angle) * thrust);
		rb.freezeRotation = false;
		

		if (Input.GetKey(KeyCode.LeftArrow)) {
			// Unity measures angular velocity in radians
			rb.angularVelocity = new Vector3(0.0f, -turnSpeed*Mathf.Deg2Rad, 0.0f);        
		} else if (Input.GetKey(KeyCode.RightArrow)) {
			rb.angularVelocity = new Vector3 (0.0f, turnSpeed*Mathf.Deg2Rad, 0.0f);        
		} else if (Input.GetKey(KeyCode.UpArrow)) {
			rb.angularVelocity = new Vector3 (-turnSpeed*Mathf.Deg2Rad, 0.0f, 0.0f);        
		} else if (Input.GetKey(KeyCode.DownArrow)) {
			rb.angularVelocity = new Vector3 (turnSpeed*Mathf.Deg2Rad, 0.0f, 0.0f);        
		} else {
			rb.freezeRotation = true; // No key pressed - stop
		}
	}
}