using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class ActivatorScript : MonoBehaviour
{
    public KeyCode Key;
    private SpriteRenderer sr;
    private bool active = false;
    private GameObject note;
    private Color old;
    private GameManager gm;
    void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    // Use this for initialization
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
       
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
            ThrowBack(note);
             active = false;
        }
    }

    public UnityAction HasiHit = delegate {  };
    public float HurlTime;
    private void ThrowBack(GameObject obj)
    {
        obj.GetComponentInChildren<EggNote>().Speed = 0;
        obj.layer =10;
        obj.transform.DOMove(new Vector3(0, 3.58f, 0), HurlTime);
        obj.transform.DOScale(new Vector3(0.05f, 0.05f, 0.05f), HurlTime);
        StartCoroutine(TriggerHasiHit(obj));
      
    }

    private IEnumerator TriggerHasiHit(GameObject obj)
    {
        yield return new WaitForSeconds(HurlTime);
        gm.RequestHasiHit();
        Destroy(obj);
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