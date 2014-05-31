using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoidFlocking : MonoBehaviour
{
	internal BoidController controller;
    private Transform _playerXform;

	IEnumerator Start()
    {
        _playerXform = GameObject.Find("Player").transform;
		while (true)
		{
			if (controller)
			{
				rigidbody.velocity += steer() * Time.deltaTime;

				// enforce minimum and maximum speeds for the boids
				float speed = rigidbody.velocity.magnitude;
				if (speed > controller.maxVelocity)
				{
					rigidbody.velocity = rigidbody.velocity.normalized * controller.maxVelocity;
				}
				else if (speed < controller.minVelocity)
				{
					rigidbody.velocity = rigidbody.velocity.normalized * controller.minVelocity;
				}
			}
			float waitTime = Random.Range(0.3f, 0.5f);
			yield return new WaitForSeconds(waitTime);
		}
	}

	Vector3 steer()
	{
		Vector3 randomize = new Vector3((Random.value * 2) - 1, (Random.value * 2) - 1, _playerXform.position.z);
		//randomize.Normalize();
		randomize *= controller.randomness;

	//	Vector3 center = controller.flockCenter - transform.localPosition;
		Vector3 velocity = controller.flockVelocity - rigidbody.velocity;
		Vector3 follow = controller.target.localPosition - transform.localPosition;

		return ( velocity + follow);
	}
}