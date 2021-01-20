using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMenager : MonoBehaviour
{
    public void LoadLevel(string name)
    {
        Debug.Log("Učitavanje razine:" + name);
        Application.LoadLevel(name);
    }

    
    public void QuitRequest()
    {
        Debug.Log("Gašenje!");
        Application.Quit();
    }
}
