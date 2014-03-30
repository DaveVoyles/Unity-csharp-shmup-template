// Copyright (c) 20013 Rocket Alien
// Please direct any bugs, comments or suggestions to http://rocketalien.com
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using UnityEngine;

public class iTweenMultiPath : MonoBehaviour
{
    public Transform actor;
    public iTweenEvent actorTween;
    public iTweenPath[] actorPaths;
    public float actorSpeed = 1000.0f;
    public float nextPathDistance = 10.0f;

	public enum NextPathMethod { StartNextPath, StartClosestPath };
	public NextPathMethod nextPathMethod = NextPathMethod.StartNextPath;

    private int m_nextPathIndex = 0;

    private void Awake ()
    {
        if (actor == null)
		{
			actor = transform;
		}

        actorTween = actor.gameObject.GetComponent<iTweenEvent> ();
        if (actorTween == null)
		{
			actorTween = actor.gameObject.AddComponent<iTweenEvent> ();
		}

        //these can be set in Unity editor
        actorTween.type = iTweenEvent.TweenType.MoveTo;
        actorTween.Values["position"]     = actor;
        actorTween.Values["speed"]        = actorSpeed;
        actorTween.Values["movetopath"]   = false;
        actorTween.Values["orienttopath"] = true;
        actorTween.Values["easetype"]     = iTween.EaseType.linear;
		actorTween.Values["oncomplete"]   = nextPathMethod.ToString ();
        actorTween.Values["oncompletetarget"] = gameObject;

        //square distance for faster calculations
        nextPathDistance *= nextPathDistance;
    }

    private void Start ()
    {
		StartPath ();
    }
	
	private void StartPath ()
	{
		switch (nextPathMethod)
		{
		case NextPathMethod.StartNextPath:
			StartNextPath ();
			break;
		case NextPathMethod.StartClosestPath:
			StartClosestPath ();
			break;
		}
	}

    private void StartNextPath ()
    {
        actorTween.Values["path"] = actorPaths[m_nextPathIndex].pathName;
        actorTween.Play ();
        m_nextPathIndex = (m_nextPathIndex + 1) % actorPaths.Length;
    }

    private void StartClosestPath ()
    {
        iTweenPath closestPath = null;
        float closestDistance = Mathf.Infinity;
        foreach (iTweenPath path in actorPaths)
        {
            if (path.nodeCount > 0)
            {
                Vector3 start = path.nodes[0];
                Vector3 diff = start - actor.position;
                float distance = diff.sqrMagnitude;
                if (distance < closestDistance)
                {
                    //found closer path
                    closestPath = path;
                    closestDistance = distance;
                }
            }
        }
        if (closestPath != null)
        {
            //found closest path
            actorTween.Values["path"] = closestPath.pathName;
            actorTween.Play ();
        }
    }
}
