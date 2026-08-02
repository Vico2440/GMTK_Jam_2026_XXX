using UnityEngine;

[RequireComponent(typeof(AudioSource))] 
public class PlayerFootsteps : MonoBehaviour
{
    [Header("Configuration des Pas")]
    [Tooltip("La vitesse minimale pour considérer que le joueur marche")]
    public float velocityThreshold = 0.1f;

    private Rigidbody2D rb;
    private AudioSource footstepSource;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        footstepSource = GetComponent<AudioSource>();
        
        footstepSource.loop = true; 
        footstepSource.playOnAwake = false; 
    }

    private void Update()
    {
        if (Time.timeScale == 0f)
        {
            if (footstepSource.isPlaying) footstepSource.Pause();
            return;
        }

        if (rb != null && rb.linearVelocity.magnitude > velocityThreshold)
        {
            if (!footstepSource.isPlaying)
            {
                footstepSource.Play();
            }
        }
        else
        {
            if (footstepSource.isPlaying)
            {
                footstepSource.Pause();
            }
        }
    }
}