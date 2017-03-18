using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public int SelectedLevel;
    public GameObject CityHealthText;
    public int CityMaxHealth;
    private Text CityText;
    private int currentCityHealth;
    private int currentHasiHealth;
    private bool endOfGame;
    private Text EndText;
    public int Level1BPM;
    public int Level2BPM;
    public float Frequency;
    public GameObject GameEndText;
    public GameObject LevelUpText;

    private SpriteRenderer hasi;
    private Image hasiDamageImg;
    private Image houseLeftImg;
    private Image houseRightImg;
    private SpriteRenderer city;
    public GameObject HasiGameObject;
    public GameObject CityGameObject;
    public GameObject HasiHealthText;
    public int HasiMaxHealth;
    //private Text HasiText;
    public float SpawnOffset;
    private NoteList list = new NoteList();
    [NotNull] public GameObject NotePrefab;
    private List<GameObject> Notes = new List<GameObject>();
    public UnityAction RequestCityHit = delegate { };
    public UnityAction RequestHasiHit = delegate { };
    private readonly Stopwatch timer = new Stopwatch();
    private UnityAction TriggerNote = delegate { };
    public List<GameObject> Fires;
    //private UnityAction TriggerUpdateText = delegate { };
    private int maxAmount = 1;
    private Text lvlUp;
    private void SpawnNote()
    {
        //list.Notes.Add(new Note{timePos = timer.Elapsed.TotalSeconds.ToString(CultureInfo.InvariantCulture
        List<int> existing = new List<int>();
        var amount = Random.Range(1, maxAmount+1);
        for (int i = 0; i < amount; i++)
        {
            int column = 0;
            bool newItem = false;
            while (!newItem)
            {
                column = Random.Range(0, 7);
                if (!existing.Contains(column) && column != 3)
                {
                    newItem = true;
                }
            }
            existing.Add(column);
            Vector3 pos = Vector3.zero;
            if (column < 3f)
            {
                pos = new Vector3(-2.8f + column * 1f, 5.5f, 0f);
            }
            else if (column > 3f)
            {
                pos = new Vector3(2.8f - ((column-4) * 1f), 5.5f, 0f);
            }
            var note = Instantiate(NotePrefab, pos, Quaternion.identity);
            if(SelectedLevel == 2)note.GetComponent<EggNote>().Speed *= 2;
        }

        
    }

    private void Serialize()
    {
        var serializer = new XmlSerializer(typeof(NoteList));
        using (var stream = new FileStream(@"D:\song1.xml", FileMode.Create))
        {
            serializer.Serialize(stream, list);
        }
    }

    private void Deserialize()
    {
        var serializer = new XmlSerializer(typeof(NoteList));
        using (var stream = new FileStream(@"D:\song1.xml", FileMode.Open))
        {
            list = serializer.Deserialize(stream) as NoteList;
        }
    }

    private void GenerateNoteList()
    {
        foreach (var note in list.Notes)
        {
            var column = Random.Range(0, 7);
            Debug.Log(column);
            var yPos = Convert.ToDouble(note.timePos);
            var pos = new Vector3(-4.5f + column * 1.5f, 6 + (float) yPos, 0f);
            Instantiate(NotePrefab, pos, Quaternion.identity);
        }
    }

    private void DecrementHasiHealth()
    {
        if (currentHasiHealth > 0)
            currentHasiHealth--;
        //TriggerUpdateText();
        StartCoroutine(HasiHit());
       
    }

    private IEnumerator HasiHit()
    {
        Color col = new Color(1f, 99f / 255f, 99f / 255f);
        hasi.color = col;
       // HasiText.color = col;
        yield return new WaitForSeconds(0.05f);
        hasi.color = Color.white;
        //HasiText.color = Color.black;
        hasiDamageImg.fillAmount = 1f-(float)currentHasiHealth/HasiMaxHealth ;
    }

    private int fireCounter = 0;
    private void DecrementCityHealth()
    {
        if(currentCityHealth>0)
            currentCityHealth--;
        //TriggerUpdateText();
        houseLeftImg.fillAmount = 1f - (float) currentCityHealth / CityMaxHealth;
        houseRightImg.fillAmount = 1f - (float) currentCityHealth / CityMaxHealth;
        StartCoroutine(CityHit());
        if (currentCityHealth % 5 == 0)
        {
            if (fireCounter < Fires.Count)
            {
                Fires[fireCounter].SetActive(true);
                fireCounter++;
            }
           
        }
    }

    private IEnumerator CityHit()
    {
        Color col = new Color(1f, 99f / 255f, 99f / 255f);
        city.color = col;
        //CityText.color = Color.white;
        yield return new WaitForSeconds(0.05f);
       // CityText.color = Color.black;
        city.color = Color.white;
    }

    private void Start()
    {
        lvlUp = LevelUpText.GetComponentInChildren<Text>();
        city = CityGameObject.GetComponentInChildren<SpriteRenderer>();
        hasi = HasiGameObject.GetComponentInChildren<SpriteRenderer>();
        hasiDamageImg = GameObject.FindGameObjectWithTag("HasiDamage").GetComponent<Image>();

        houseLeftImg = GameObject.FindGameObjectsWithTag("HousesDamage")[0].GetComponent<Image>();
        houseRightImg = GameObject.FindGameObjectsWithTag("HousesDamage")[1].GetComponent<Image>();

         RequestHasiHit += DecrementHasiHealth;
         RequestCityHit += DecrementCityHealth;
        //TriggerUpdateText += UpdateTexts;
        //HasiText = HasiHealthText.GetComponentInChildren<Text>();
      //  CityText = CityHealthText.GetComponentInChildren<Text>();
        EndText = GameEndText.GetComponentInChildren<Text>();
        currentHasiHealth = HasiMaxHealth;
        currentCityHealth = CityMaxHealth;
        //HasiText.text = "Hasi: " + HasiMaxHealth;
        //CityText.text = "City: " + CityMaxHealth;
        timer.Reset();
        timer.Start();
        //Deserialize();
        //GenerateNoteList();
        //GameObject.FindObjectOfType<CameraSound>().BeatFired += SpawnNote;
        //GameObject.FindObjectOfType<AudioProcessor>().PlaybackFinished += Serialize;
        if (SelectedLevel == 1)
        {
            Frequency = Level1BPM / 120f;
        }else if (SelectedLevel == 2)
        {
            Frequency = Level2BPM / 240f;
        }
        TriggerNote += SpawnNote;
        GameObject.FindObjectOfType<CameraSound>().StartMusic(SelectedLevel);
        StartCoroutine(StartTimer());
        StartCoroutine(Difficulty());
    }

    private void UpdateTexts()
    {
      //  HasiText.text = "Hasi: " + currentHasiHealth;
        //CityText.text = "City: " + currentCityHealth;
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
        if (endOfGame) StartCoroutine(Reset());
    }

    private IEnumerator Reset()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(0);
    }

    private bool difficultySwitch = false;
    private IEnumerator Difficulty()
    {
        for (var j = 0; j < 3; j++)
        {
            float waitForSeconds = 25f;
            if (SelectedLevel == 2) waitForSeconds /= 2;
             yield return new WaitForSeconds(waitForSeconds);
            if (difficultySwitch)
            {
                Frequency /= 1.5f;
            }
            else
            {
                maxAmount++;
            }
            StartCoroutine(ShowLvlUp(difficultySwitch));
            difficultySwitch = !difficultySwitch;
            
        }
    }

    private IEnumerator ShowLvlUp(bool difficulty)
    {
        if (difficulty)
            lvlUp.text = "Speed up";
        else
        {
            lvlUp.text = "Difficulty up";
        }
        yield return new WaitForSeconds(1.5f);
        lvlUp.text = "";
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

    public class Note
    {
        [XmlAttribute("timePos")] public string timePos;
    }

    [XmlRoot("NoteList")]
    public class NoteList
    {
        [XmlArray("Notes")] [XmlArrayItem("Note")] public List<Note> Notes = new List<Note>();
    }
}