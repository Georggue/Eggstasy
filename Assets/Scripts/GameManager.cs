using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public float Frequency;
    public GameObject HasiHealthText;
    public GameObject CityHealthText;
    public GameObject GameEndText;
    public int HasiMaxHealth;
    public int CityMaxHealth;
    private int currentHasiHealth;
    private int currentCityHealth;
    private Text HasiText;
    private Text CityText;
    private Text EndText;
    [NotNull] public GameObject NotePrefab;
    private UnityAction TriggerNote = delegate {  };

    public class Note
    {
        [XmlAttribute("timePos")] public string timePos;
    }

    [XmlRoot("NoteList")]
    public class NoteList
    {
        [XmlArray("Notes")]
        [XmlArrayItem("Note")]
        public List<Note> Notes = new List<Note>();
    }

    private NoteList list = new NoteList();
    private Stopwatch timer = new Stopwatch();
    private List<GameObject> Notes = new List<GameObject>();
    private void SpawnNote()
    {
        //list.Notes.Add(new Note{timePos = timer.Elapsed.TotalSeconds.ToString(CultureInfo.InvariantCulture)});
        int column = Random.Range(0, 6);
        Vector3 pos = new Vector3(-3.75f + column * 1.5f, 5.5f, 0f);
        Instantiate(NotePrefab, pos, Quaternion.identity);
    }

    private void Serialize()
    {
          XmlSerializer serializer = new XmlSerializer(typeof (NoteList));
        using (var stream = new FileStream(@"D:\song1.xml", FileMode.Create))
        {
            serializer.Serialize(stream, list);
        }
    }

    private void Deserialize()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(NoteList));
        using (var stream = new FileStream(@"D:\song1.xml", FileMode.Open))
        {
           list =  serializer.Deserialize(stream) as NoteList;
        }
    }

    private void GenerateNoteList()
    {
        foreach (var note in list.Notes)
        {
            int column = Random.Range(0, 6);
            UnityEngine.Debug.Log(column);
            double yPos = Convert.ToDouble(note.timePos);
            Vector3 pos = new Vector3(-3.75f+column*1.5f,6+ (float)yPos, 0f);
            Instantiate(NotePrefab, pos, Quaternion.identity);
        }
    }
    
    private void DecrementHasiHealth()
    {
        currentHasiHealth--;
        TriggerUpdateText();
    }
    private void DecrementCityHealth()
    {
        currentCityHealth--;
        TriggerUpdateText();
    }
    UnityAction TriggerUpdateText = delegate {  };
    public UnityAction RequestHasiHit = delegate { };
    public UnityAction RequestCityHit = delegate { };
    private bool endOfGame = false;
    private void Start()
    {
        RequestHasiHit += DecrementHasiHealth;
        RequestCityHit += DecrementCityHealth;
        TriggerUpdateText += UpdateTexts;
        HasiText = HasiHealthText.GetComponentInChildren<Text>();
        CityText = CityHealthText.GetComponentInChildren<Text>();
        EndText = GameEndText.GetComponentInChildren<Text>();
        currentHasiHealth = HasiMaxHealth;
        currentCityHealth = CityMaxHealth;
        HasiText.text = "Hasi: " + HasiMaxHealth;
        CityText.text = "City: " + CityMaxHealth;
        timer.Reset();
        timer.Start();
        //Deserialize();
        //GenerateNoteList();
        //GameObject.FindObjectOfType<CameraSound>().BeatFired += SpawnNote;
        //GameObject.FindObjectOfType<AudioProcessor>().PlaybackFinished += Serialize;
        TriggerNote += SpawnNote;
        StartCoroutine(StartTimer());
        StartCoroutine(Difficulty());
    }

    private void UpdateTexts()
    {
        HasiText.text = "Hasi: " + currentHasiHealth;
        CityText.text = "City: " + currentCityHealth;
        if (currentCityHealth == 0)
        {
            EndText.text = "You Lose!";
            endOfGame = true;
        }
        if (currentHasiHealth == 0)
        {
            EndText.text = "You Win!";
            endOfGame = true;
        }
       if(endOfGame) StartCoroutine(Reset());
    }

    private IEnumerator Reset()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(0);
    }

    private IEnumerator Difficulty()
    {
        for (int j = 0; j < 3; j++)
        {
           yield return new WaitForSeconds(20);
            Frequency /= 2f;
        }
    }

    private IEnumerator StartTimer()
    {
        while (true)
        {
            TriggerNote();
            yield return new WaitForSeconds(Frequency);
            
        }
    }

    private void Update()
    {
       
    }
}