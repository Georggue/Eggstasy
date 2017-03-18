using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBurnScript : MonoBehaviour
{
    private GameManager gm;
    private void OnTriggerEnter2D(Collider2D other)
    {
        gm.RequestCityHit();
        Destroy(other.gameObject);
    }

    // Use this for initialization
	void Start ()
	{
	    gm = GameObject.FindObjectOfType<GameManager>();

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
