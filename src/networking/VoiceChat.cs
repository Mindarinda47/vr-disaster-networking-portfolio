using UnityEngine;
using UnityEngine.UI;

public class VoiceChatController : MonoBehaviour
{
    //public AudioSource pcAudioSource;
    //public AudioSource vrAudioSource;
    public Button MikeButton;
    public Sprite[] sprites;

    private bool isMicOn = false;
    private string microphoneName;

    void Start()
    {
        MikeButton.onClick.AddListener(ToggleMicrophone);
        GameObject.Find("MikeButton").GetComponent<Image>().sprite = sprites[0];
        /*if (Microphone.devices.Length > 0)
        {
            microphoneName = Microphone.devices[0];
        }*/
    }

    public void ToggleMicrophone()
    {  
        if (isMicOn)
        {   //pcAudioSource.Stop();
            //Microphone.End(microphoneName);
            isMicOn = false;
            GameObject.Find("MikeButton").GetComponent<Image>().sprite = sprites[0];
        }
        else
        {   //pcAudioSource.clip = Microphone.Start(microphoneName, true, 10, 44100);
            //pcAudioSource.loop = true;

            //while (!(Microphone.GetPosition(microphoneName) > 0)) { }
            //pcAudioSource.Play();
            isMicOn = true;
            GameObject.Find("MikeButton").GetComponent<Image>().sprite = sprites[1];
        }
    }

    void Update()
    {
        /*if (isMicOn && Microphone.IsRecording(microphoneName))
        {   vrAudioSource.clip = pcAudioSource.clip;
            vrAudioSource.Play();
        }*/
    }
}
