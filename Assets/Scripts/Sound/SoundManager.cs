using UnityEngine;
using System;
public enum SoundType
{
    LAND,
    COLLIDE,
    HURT,
    LAUNCH,
    JUMP,
    JEWELCOLLIDE,
    JEWELROLL,
    SLIMESTEPS,
    Level1
    
}

[RequireComponent(typeof(AudioSource)), ExecuteInEditMode]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private float BGMVolume = .25f;
    [SerializeField] private SoundList[] soundList;
    [SerializeField] private SoundType StartingTrack;
   
    private static SoundManager instance;
    private AudioSource[] AudioSources;
    private AudioSource SFXSource, BGMSource;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (Application.isPlaying)
        {
            AudioSources = GetComponents<AudioSource>();
            if (AudioSources.Length == 2 )
            {
                SFXSource = AudioSources[0];
                BGMSource = AudioSources[1];
            }

            else
            {
                Debug.Log("There is not 2 audiosources, there is " + AudioSources.Length);
            }


                PlayMusic(StartingTrack, BGMVolume);
        }
    }


    private void Update()
    {
        if (Application.isPlaying)
        {
            if (!BGMSource.isPlaying)
            {
                PlayMusic(StartingTrack, .25f);
            }
        }
    }

    public static void PlaySound(SoundType sound, float volume = 1.0f)
    {
        AudioClip[] clips = instance.soundList[(int)sound].Sounds;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];
        instance.SFXSource.PlayOneShot(randomClip, volume);
    }

    public static void PlayMusic(SoundType sound, float volume = 1.0f)
    {
        AudioClip[] clips = instance.soundList[(int)sound].Sounds;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];
        instance.BGMSource.PlayOneShot(randomClip, volume);
    }

#if UNITY_EDITOR
    private void OnEnable()
    {
        string[] names = Enum.GetNames(typeof(SoundType));
        Array.Resize(ref soundList, names.Length);
        for (int i = 0; i < soundList.Length; i++)
        {
            soundList[i].name = names[i];
        }
    }
#endif
}
    [Serializable]
    public struct SoundList
    {
        public AudioClip[] Sounds { get => sounds; }
        [HideInInspector] public string name;
        [SerializeField] private AudioClip[] sounds;
    }

