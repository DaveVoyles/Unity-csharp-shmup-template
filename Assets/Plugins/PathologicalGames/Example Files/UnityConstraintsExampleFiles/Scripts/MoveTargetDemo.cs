using UnityEngine;
using System.Collections;

public class MoveTargetDemo : MonoBehaviour {

    private Transform xform;
    private bool moveForward = true;
    private float speed = 20f;
    private float duration = 0.6f;
    private float delay = 3.0f;

	// Use this for initialization
	void Start () {
        this.xform = this.transform;

        this.StartCoroutine(this.MoveTarget());
	}

    private IEnumerator MoveTarget()
    {
        yield return new WaitForSeconds(delay);

        float savedTime = Time.time;

        while ((Time.time - savedTime) < duration)
        {
            if (moveForward)
            {
                this.xform.Translate(Vector3.forward * (Time.deltaTime * speed));
            }
            else
            {
                this.xform.Translate(Vector3.back * (Time.deltaTime * speed));
            }
            yield return null;
        }

        moveForward = moveForward ? false : true;

        this.StartCoroutine(MoveTarget());
    }
}
