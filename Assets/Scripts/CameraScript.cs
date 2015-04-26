using UnityEngine;
using System.Collections;


public class CameraScript : MonoBehaviour {

    public GameObject follow;
    public float followDist;
    public float followHeight;

    private Vector3 prevPosition;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        Vector3 pos = follow.transform.position - follow.transform.forward * followDist + follow.transform.up * followHeight;
        if (Vector3.Distance(prevPosition, pos) >= 0.01f)
        {
            transform.position = pos;
        }
             
            
        Quaternion rot = Quaternion.LookRotation(follow.transform.position - transform.position);
        //Quaternion newRot = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 7.5f);
        transform.rotation = rot;

        prevPosition = transform.position;
	}
}
