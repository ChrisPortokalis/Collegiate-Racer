using UnityEngine;
using System.Collections;


public class WayPoint : MonoBehaviour {

    public GameObject next;
    public bool isStart = false;
    public bool isTurn = false;
    public GameObject waypoints;

    private Transform[] wpPositions;

    private static GameObject start;

    void Awake()
    {
        if (!next)
        {
           // print("This waypoint is not connected; fix the problem" + this);
        }

        if (isStart)
        {
            start = gameObject;
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    Vector3 getTarget(Vector3 position)
    {
        if (Vector3.Distance(transform.position, position) < 5.0)
        {
            return next.transform.position;
        }
        else
        {
            return transform.position;
        }
    }

    Vector3 GetCatmullRom(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Vector3 a = 0.5f * (2f * p1);
        Vector3 b = 0.5f * (p2 - p0);
        Vector3 c = 0.5f * (2f * p0 - 5f * p1 + 4f * p2 - p3);
        Vector3 d = 0.5f * (-p0 + 3f * p1 - 3f * p2 + p3);

        Vector3 pos = a + (b * t) + (c * t * t) + (d * t * t * t);

        return pos;
    }
}
