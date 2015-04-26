using UnityEngine;
using System.Collections;

public class Powerup : MonoBehaviour {

    private bool hasPowerup;
    private int powerupType;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public bool HasPowerup() { return hasPowerup; }

    public int GetPowerupType() { return powerupType; }

    public void UsePowerup()
    {
        switch(powerupType)
        {
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
        }

    }
}
