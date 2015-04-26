using UnityEngine;
using System.Collections;

public class DrawSpline : MonoBehaviour {

    public GameObject waypoints;
    public int splineColor;
    private Transform[] wpPositions;
    

    private static GameObject start;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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

    void OnDrawGizmos()
    {
        if(splineColor == 1)
        {
            Gizmos.color = Color.blue;
        }
        else if(splineColor == 2)
        {
            Gizmos.color = Color.green;
        }
        else if(splineColor == 3)
        {
            Gizmos.color = Color.red;
        }
        else if(splineColor == 4)
        {
            Gizmos.color = Color.cyan;
        }
        else
        {
            Gizmos.color = Color.white;
        }
        
        wpPositions = waypoints.GetComponentsInChildren<Transform>();
        Transform[] temp = new Transform[wpPositions.Length - 1];
        int j = 0;
        for (int i = 1; i < wpPositions.Length; i++ )
        {
            temp[j] = wpPositions[i];
            j++;
        }
        wpPositions = new Transform[temp.Length];
        temp.CopyTo(wpPositions,0);

            for (int i = 0; i < wpPositions.Length; i++)
            {
                Gizmos.DrawWireSphere(wpPositions[i].position, 0.3f);
            }

        for (int i = 0; i < wpPositions.Length; i++)
        {
            //Cant draw between the endpoints
            //Neither do we need to draw from the second to the last endpoint
            //...if we are not making a looping line
            if ((i == wpPositions.Length))
            {
                continue;
            }
            DisplayCatmullRomSpline(i);
        }

    }

    void DisplayCatmullRomSpline(int pos)
    {
        Vector3 p0 = Vector3.zero;

        if (pos == 0)
        {
            p0 = wpPositions[wpPositions.Length - 1].position;
        }
        else
        {
            p0 = wpPositions[pos - 1].position;
        }
        Vector3 p1 = wpPositions[(pos) % wpPositions.Length].position;
        Vector3 p2 = wpPositions[(pos + 1) % wpPositions.Length].position;
        Vector3 p3 = wpPositions[(pos + 2) % wpPositions.Length].position;


        //Just assign a tmp value to this
        Vector3 lastPos = Vector3.zero;

        //t is always between 0 and 1 and determines the resolution of the spline
        //0 is always at p1
        for (float t = 0; t < 1; t += 0.1f)
        {
            //Find the coordinates between the control points with a Catmull-Rom spline
            Vector3 newPos = GetCatmullRom(t, p0, p1, p2, p3);

            //Cant display anything the first iteration
            if (t == 0)
            {
                lastPos = newPos;
                continue;
            }
            
            Gizmos.DrawLine(lastPos, newPos);
            lastPos = newPos;
        }

        //Also draw the last line since it is always less than 1, so we will always miss it
        Gizmos.DrawLine(lastPos, p2);
    }


    //Clamp the list positions to allow looping
    //start over again when reaching the end or beginning
    int ClampListPos(int pos)
    {
        if (pos < 0)
        {
            pos = wpPositions.Length - 1;
        }

        if (pos > wpPositions.Length)
        {
            pos = 1;
        }
        else if (pos > wpPositions.Length - 1)
        {
            pos = 0;
        }

        return pos;
    }
}