using UnityEngine;
using System.Collections;

public class Car : MonoBehaviour {

    private Transform[] wpPositions;
    private ArrayList transforms;
    private WayPoint[] waypointVars;
    private int currentWaypoint;
    private float time = 0;
    private float currentSpeed = 0;
    private Quaternion prevRot;
    private bool changedTrack;
    private int prevTrack;
    private int currTrack;
    private GameObject waypoints;
    private Random randomInt;
    

    public GameObject[] wpArr;
    public float maxSpeed;
    public int initOff;
    public float acceleration;
    public string name;
    public int seed;

	// Use this for initialization
	void Start () {
        //System.Random rand = new System.Random();
        randomInt = new Random();
        transforms = new ArrayList();
        for (int i = 0; i < wpArr.Length; i++)
        {
            transforms.Add(setupWaypoints(wpArr, i));
        }

        waypointVars =  wpArr[0].GetComponentsInChildren<WayPoint>();
        changedTrack = false;
        prevTrack = initOff;
        currTrack = initOff;
        //check for ai and player drivers in vicinity of this car
        CheckForDrivers();
        wpPositions = (Transform[]) transforms[initOff];
	}

    Transform[] setupWaypoints(GameObject[] wp, int x)
    {
        Transform[] arr = wpArr[x].GetComponentsInChildren<Transform>();
        int j = 0;
        Transform[] temp = new Transform[arr.Length - 1];
        for (int i = 1; i < arr.Length; i++)
        {
            temp[j] = arr[i];
            j++;
        }

        arr = temp;
        return arr;
    }
	
    void FixedUpdate()
    {
        //update position of object (catmull-rom spline)
        UpdatePosition();      
        //update rotation of object to face waypoint
        UpdateRotation();
        //check for ai and player drivers in vicinity of this car
        CheckForDrivers();
    }

	// Update is called once per frame
    void Update()
    {
        //have camera follow AI objects !-- For Testing Purposes ONLY --!
        if (name == "Cube")
        {
            //Vector3 camPos = transform.TransformDirection(Vector3.back) / 3;
            //Debug.Log(camPos);
            //Quaternion rot = Quaternion.LookRotation(transform.position + transform.TransformDirection(Vector3.back));
            ////camPos.x += 5.0f;
            //Camera.main.transform.position = camPos;
            //Camera.main.transform.rotation = rot;
        }
        //Debug.Log("positions = " + wpPositions.Length + " vars = " + waypointVars.Length);
       //-----------------end test----------------------------------------
    }

    //method to get position of object in accordance to catmull rom spline
    Vector3 GetCatmullRom(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Vector3 a = 0.5f * (2f * p1);
        Vector3 b = 0.5f * (p2 - p0);
        Vector3 c = 0.5f * (2f * p0 - 5f * p1 + 4f * p2 - p3);
        Vector3 d = 0.5f * (-p0 + 3f * p1 - 3f * p2 + p3);

        Vector3 pos = a + (b * t) + (c * t * t) + (d * t * t * t);

        return pos;
    }

    //Method to change position of object in accordance to catmull-rom spline
    void UpdatePosition()
    {
        //init p0 pre if statement
        //Debug.Log(wpPositions[0]);
        Vector3 p0 = Vector3.zero;
        Vector3 p1 = Vector3.zero;

        //check current position to set first point
        if (changedTrack)
        {
            Transform[] temp = (Transform[])transforms[prevTrack];
            if (currentWaypoint == 0) //if current waypoint is 0 first point is the last
            {
                p0 = temp[wpPositions.Length - 1].position;
            }
            else //else first point is the previous point of current waypoint
            {
                p0 = temp[currentWaypoint - 1].position;
            }
        }
        else
        {
            if (currentWaypoint == 0) //if current waypoint is 0 first point is the last
            {
                p0 = wpPositions[wpPositions.Length - 1].position;
            }
            else //else first point is the previous point of current waypoint
            {
                p0 = wpPositions[currentWaypoint - 1].position;
            }
        }

        //get rest of positions actual path is between points p1 and p2 
        //but 4 points are required for catmull rom spline
        if (changedTrack)
        {
            Transform[] temp = (Transform[])transforms[prevTrack];
            p1 = temp[currentWaypoint].position;
               
        }
        else
        {
            p1 = wpPositions[(currentWaypoint)].position;
        }
        Vector3 p2 = wpPositions[(currentWaypoint + 1) % wpPositions.Length].position;
        Vector3 p3 = wpPositions[(currentWaypoint + 2) % wpPositions.Length].position;

        float distance = Vector3.Distance(p1, p2);

        
        //update time to move along catmull-rom spline in accordance to speed
        if(maxSpeed >= currentSpeed)
        {
            currentSpeed += acceleration;
        }

        if (waypointVars[(currentWaypoint + 1) % waypointVars.Length].isTurn)// && currentSpeed >= maxSpeed)
        {
            currentSpeed -= currentSpeed / 80;
        }

        time += (Time.deltaTime * currentSpeed) / distance; //scale movement by time, speed and distance

        //update position based on catmull rom
        Vector3 newPos = GetCatmullRom(time, p0, p1, p2, p3);
        transform.position = newPos;

        //if time >= 1 then the object has reached p2 and it is time to calculate the next waypoint
        if (time >= 1f)
        {
            prevTrack = currTrack;
            currTrack = Random.Range(0, wpArr.Length);
            wpPositions = (Transform[]) transforms[currTrack];

            if(currTrack != prevTrack)
            {
                changedTrack = true;
            }
            else
            {
                changedTrack = false;
            }

            currentWaypoint = (currentWaypoint + 1) % wpPositions.Length;
            
            time = time - 1;

        }
    }

    //Method to update rotation in reference to waypoints
    void UpdateRotation()
    {
        prevRot = transform.rotation;
        //get rotation that driver should rotate to look at waypoint
        Quaternion lookRot = Quaternion.LookRotation(wpPositions[(currentWaypoint + 1) % wpPositions.Length].transform.position - transform.position);

        //smooth rotation to look at waypoint
        Quaternion newRot = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 4);
        newRot.x = 0;
        newRot.z = 0;
        transform.rotation = newRot;
    }

    void CheckForDrivers()
    {
        Vector3 fwd = transform.TransformDirection(Vector3.forward) * 2f;
        Vector3 right = transform.TransformDirection(Vector3.right) * 2f;
        Vector3 left = transform.TransformDirection(Vector3.left) * 2f;
        Vector3 back = transform.TransformDirection(Vector3.back) * 2f;

        RaycastHit fwdHit;
        RaycastHit rightHit;
        RaycastHit leftHit;
        RaycastHit backHit;

        if (Physics.Raycast(transform.position, fwd, out fwdHit))
        {
            if (fwdHit.collider.gameObject.tag == "Car")
            {
               // Debug.Log("Car is in Front  " + name + "Fwd = " + fwd);
            }
        }
        else if (Physics.Raycast(transform.position, right, out rightHit))
        {
            if (rightHit.collider.gameObject.tag == "Car")
            {
                //Debug.Log("Car is Right of vehicle " + name);
            }
        }
        else if(Physics.Raycast(transform.position,left,out leftHit))
        {
            if(leftHit.collider.gameObject.tag == "Car")
            {
                //Debug.Log("Car is Left of vehile " + name);
            }

        }
        else if (Physics.Raycast(transform.position, back, out backHit))
        {
            if(backHit.collider.gameObject.tag == "Car")
            {
               // Debug.Log("Car is bakc of vehicle " + name);
            }
            
        }
        else
        {
        }
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Car")
        {
            currentSpeed -= currentSpeed / 15;
            Debug.Log("Collision! " + name);
        }
    }

}

//LEGACY  METHODS

/*	bool NavigateTowardWaypoint()
    {
        Vector3 relativeWaypointPosition = wpPositions[currentWaypoint].position - transform.position;
        float dist = Vector3.Distance(wpPositions[currentWaypoint].position, transform.position);
        

        if (relativeWaypointPosition.magnitude < 0.25f)
        {
            currentWaypoint = (currentWaypoint + 1) % wpPositions.Length;
            print("Current waypoint: " + currentWaypoint);
            time = 0;
            return true;
        }
        return false;
    }*/


//Vector3 ComputePointOnCatmullRomCurve(float u)
//{
//    // TODO - compute and return a point as a Vector3       
//    // Points on segment number 0 start at controlPoints[0] and end at controlPoints[1]
//    // Points on segment number 1 start at controlPoints[1] and end at controlPoints[2]
//    //       etc...

//    Vector3 point = new Vector3();
//    Vector3 p0 = Vector3.zero;

//    if (currentWaypoint == 0)
//    {
//        p0 = wpPositions[wpPositions.Length - 1].position;
//    }
//    else
//    {
//        p0 = wpPositions[currentWaypoint - 1].position;
//    }
//    Vector3 p1 = wpPositions[(currentWaypoint) % wpPositions.Length].position;
//    Vector3 p2 = wpPositions[(currentWaypoint + 1) % wpPositions.Length].position;
//    Vector3 p3 = wpPositions[(currentWaypoint + 2) % wpPositions.Length].position;

//    //u = transform.position.magnitude;

//    float c0 = ((-u + 2f) * u - 1f) * u * 0.5f;
//    float c1 = (((3f * u - 5f) * u) * u + 2f) * 0.5f;
//    float c2 = ((-3f * u + 4f) * u + 1f) * u * 0.5f;
//    float c3 = ((u - 1f) * u * u) * 0.5f;


//    point.x = (p0.x * c0) + (p1.x * c1) + (p2.x * c2) + (p3.x * c3);
//    point.y = (p0.y * c0) + (p1.y * c1) + (p2.y * c2) + (p3.y * c3);
//    point.z = (p0.z * c0) + (p1.z * c1) + (p2.z * c2) + (p3.z * c3);

//    //Debug.Log(point);

//    return point;
//}
