using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlaySoundOnUIButton : MonoBehaviour
{
    public AudioSource audioSource; // Assign from AudioManager

    public void PlaySound()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
}