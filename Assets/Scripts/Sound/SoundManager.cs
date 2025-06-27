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
    
}

[RequireComponent(typeof(AudioSource)), ExecuteInEditMode]
public class SoundManager : MonoBehaviour
{
    // Note the comma in the bracket makes a 2D array
    [SerializeField] private SoundList[] soundList;
    private static SoundManager instance;
    private AudioSource audioSource;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public static void PlaySound(SoundType sound, float volume = 1.0f)
    {
        AudioClip[] clips = instance.soundList[(int)sound].Sounds;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];
        instance.audioSource.PlayOneShot(randomClip, volume);
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

