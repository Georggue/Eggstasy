using UnityEngine;
using System.Collections;
using DG.Tweening;


public class SoundManager : MonoBehaviour
{
    public AudioSource platzhalter;

    public enum Sound
    {
     HitNote,
     MissNote,

     WinSound,
     LoseSound,
     BunnySoundNormal,
     BunnySoundRage,
     BunnySoundHit

}
   
public void PlaySound(Sound sound)
    {
        AudioSource source = null;
        switch (sound)
        {
            case Sound.HitNote:
                break;
            case Sound.MissNote:
                break;
            case Sound.WinSound:
                break;
            case Sound.LoseSound:
                break;
            case Sound.BunnySoundNormal:
                break;
            case Sound.BunnySoundRage:
                break;
            case Sound.BunnySoundHit:
                break;
            default:
                source = null;
                break;
        }
        if (source != null)
         PlaySound(source);
    }
    private void PlaySound(AudioSource sound)
    {
        if (!sound.isPlaying)
        {
            sound.Play();
            sound.DORewind();
        }
    }
    // Use this for initialization
    public static SoundManager Instance;

    void Start()
    {
        Instance = this;
    }

    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
    }




}