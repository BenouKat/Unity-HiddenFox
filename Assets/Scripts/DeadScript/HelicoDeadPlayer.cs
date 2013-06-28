using UnityEngine;
using System.Collections;

public class HelicoDeadPlayer : MonoBehaviour {
	
	public float puissance;
	
	void OnEnable()
	{
		rigidbody.velocity = Vector3.zero;
		rigidbody.angularVelocity = Vector3.zero;
		rigidbody.AddForce(Vector3.up*puissance);
		rigidbody.AddTorque((new Vector3(1f, 1f, 1f))*puissance);	
	}
}
