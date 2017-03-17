using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggNote : MonoBehaviour
{
    private Rigidbody2D rb;
    public float Speed;
    private void Awake()
    {
        rb = GetComponentInChildren<Rigidbody2D>();
    }

    // Use this for initialization
	void Start () {
        rb.velocity = new Vector2(0, -Speed);
        
    }
	
	// Update is called once per frame
	void Update () {
        
    }
}
