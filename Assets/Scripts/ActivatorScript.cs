using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatorScript : MonoBehaviour
{
    public KeyCode Key;
    private SpriteRenderer sr;
    private bool active = false;
    private GameObject note;
    private Color old;

    void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    // Use this for initialization
    void Start()
    {
        old = sr.color;
     
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(Key))
        {
            StartCoroutine(Pressed());
        }
        if (Input.GetKeyDown(Key) && active)
        {
            Debug.Log("Destroy");
            Destroy(note);
            active = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        active = true;
        Debug.Log("active");
        if (other.gameObject.CompareTag("Note"))
        {
            note = other.gameObject;
            
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        active = false;
    }

    private IEnumerator Pressed()
    {
       sr.color = new Color(0,0,0);
       yield return new WaitForSeconds(0.05f);
       sr.color = old;
    }
}