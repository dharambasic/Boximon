using UnityEngine;

public class Music : MonoBehaviour
{
    private AudioSource _audioSource;
    
 
   //funkcija za sviranje iste glazbe na više različitim scenama
    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayMusic()
    {
        if (_audioSource.isPlaying) return;
        

        
            _audioSource.Play();
        
    }

    public void StopMusic()
    {
        _audioSource.Stop();
    }
}