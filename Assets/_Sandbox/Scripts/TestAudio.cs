using UnityEngine;
using MyLibrary.Modules.Audio;

public class TestAudio : MonoBehaviour
{
    public AudioClip musiqueTest;
    public AudioClip bruitageTest;

    void Update()
    {
        // Touche M : Lance la musique à 30% du volume (0.3f)
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("Lancement Musique (Volume 10%)...");
            // Arguments : (Clip, DuréeFondu, Volume)
            AudioManager.Instance.PlayMusic(musiqueTest, 2.0f, 0.1f);
        }

        // Touche B : Lance le bruitage à 50% du volume (0.5f)
        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("PAN ! (Bruitage 5%)");
            // Arguments : (Clip, Volume)
            AudioManager.Instance.PlaySFX(bruitageTest, 0.05f);
        }
    }
}