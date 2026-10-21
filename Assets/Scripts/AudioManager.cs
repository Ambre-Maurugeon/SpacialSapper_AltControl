using System;
using System.Collections.Generic;
using UnityEngine;

// Add all your sound identifiers here
public enum SoundType
{
    AudioCardDeselect,
    AudioCardDraw,
    AudioCardSelect,
    CarteEmpilee,
    LevelWin,
    LiaisonCarte,
    MainTheme,
    MiniJeu,
    MonsterGrowl,
    PointsGagnes,
    PointsPerdus
}

[Serializable]
public struct SoundData
{
    public SoundType soundType;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private List<SoundData> sounds = new List<SoundData>();

    private readonly Dictionary<SoundType, SoundData> _soundDict = new Dictionary<SoundType, SoundData>();

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Ensure AudioSources exist
        if (sfxSource == null) sfxSource = gameObject.AddComponent<AudioSource>();
        if (musicSource == null) musicSource = gameObject.AddComponent<AudioSource>();

        // Cache sounds for fast lookup
        for (int i = 0; i < sounds.Count; i++)
        {
            if (!_soundDict.ContainsKey(sounds[i].soundType))
            {
                _soundDict.Add(sounds[i].soundType, sounds[i]);
            }
        }
    }

    // Play a single sound effect once without cutting other sounds
    public void PlaySFX(SoundType type)
    {
        if (_soundDict.TryGetValue(type, out SoundData data))
        {
            if (data.clip != null)
            {
                sfxSource.PlayOneShot(data.clip, data.volume > 0 ? data.volume : 1f);
            }
        }
        else
        {
            Debug.LogWarning($"Sound {type} not found in AudioManager!");
        }
    }

    // Play background music (loops continuously)
    public void PlayMusic(SoundType type)
    {
        if (_soundDict.TryGetValue(type, out SoundData data))
        {
            if (data.clip != null)
            {
                musicSource.clip = data.clip;
                musicSource.volume = data.volume > 0 ? data.volume : 1f;
                musicSource.loop = true;
                musicSource.Play();
            }
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
}