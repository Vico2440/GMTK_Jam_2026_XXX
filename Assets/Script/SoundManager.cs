using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [System.Serializable]
    public struct SoundEntry
    {
        public string soundName; 
        public AudioClip clip;
        [Range(0f, 1f)] public float volume;
    }

    [Header("Configuration")]
    [Tooltip("L'AudioSource qui va lire les effets sonores")]
    [SerializeField] private AudioSource sfxSource;
    
    [Header("Liste des Sons")]
    [SerializeField] private List<SoundEntry> sounds;

    private Dictionary<string, SoundEntry> soundDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        soundDictionary = new Dictionary<string, SoundEntry>();
        foreach (var s in sounds)
        {
            if (s.volume == 0f && s.clip != null) 
            {
                var correctedEntry = s;
                correctedEntry.volume = 1f;
                soundDictionary.Add(s.soundName, correctedEntry);
            }
            else if (!soundDictionary.ContainsKey(s.soundName))
            {
                soundDictionary.Add(s.soundName, s);
            }
        }
    }

    /// <summary>
    /// Joue un son à partir de son nom.
    /// </summary>
    public void PlaySound(string soundName)
    {
        if (soundDictionary.TryGetValue(soundName, out SoundEntry entry))
        {
            if (sfxSource != null && entry.clip != null)
            {
                sfxSource.PlayOneShot(entry.clip, entry.volume);
            }
        }
        else
        {
            Debug.LogWarning($"[SoundManager] Impossible de trouver le son : {soundName}");
        }
    }

    public void StopAllSounds()
    {
        sfxSource.Stop();
    }
}