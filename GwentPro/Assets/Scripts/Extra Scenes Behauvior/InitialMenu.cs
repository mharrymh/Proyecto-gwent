using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class InitialMenu : MonoBehaviour
{

    [SerializeField] public AudioMixer audioMixer;
    [SerializeField] public AudioMixer audioMixerFX;
    public GameObject audio;
    public GameObject audioFX;



    public SoundManager soundM; 


    public void Start()
    {
        soundM = GameObject.Find("AudioSourceEffects").GetComponent<SoundManager>();

        DontDestroyOnLoad(audio);
        DontDestroyOnLoad(audioFX);
    }
    public void Play()
    {
        soundM.PlayButtonSound();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Exit()
    {
        soundM.PlayButtonSound();
        Debug.Log("Salir...");
        Application.Quit();
    }
    
    //Options MENU
    public void ChangeVolume(float volumen)
    {
        audioMixer.SetFloat("Volume", volumen);
    }

    public void ChangeVolumeFX(float volumen)
    {
        audioMixerFX.SetFloat("VolumeFX", volumen);
    }
}
