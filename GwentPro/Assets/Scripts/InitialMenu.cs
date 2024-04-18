using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class InitialMenu : MonoBehaviour
{

    [SerializeField] private AudioMixer audioMixer;
    public GameObject audio;

    public void Start()
    {
        DontDestroyOnLoad(audio);
    }
    public void Play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Exit()
    {
        Debug.Log("Salir...");
        Application.Quit();
    }

    
    //Menu opciones 
    public void ChangeVolume(float volumen)
    {
        audioMixer.SetFloat("Volume", volumen);
    }
}
