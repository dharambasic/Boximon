using UnityEngine;

public class Music : MonoBehaviour
{
    private AudioSource _audioSource;
    public AudioClip sound;
    

   //funkcija za sviranje iste glazbe na više različitim scenama
     void Awake ()
    { 
        _audioSource = GetComponent<AudioSource>();

        GameObject[] obj = GameObject.FindGameObjectsWithTag("Music");
        if (obj.Length > 1)
            Destroy(this.gameObject);

        if (!_audioSource.isPlaying)
        {   
            Play();
            DontDestroyOnLoad(transform.gameObject);
       
        }    
    }
    public void Play()
    {
       
            _audioSource.PlayOneShot(sound);
    }
    public void StopMusic()
    {
        if(_audioSource)
        _audioSource.Stop();
    }
}