using UnityEngine;

public class ChangeCanvas : MonoBehaviour
{   
    public GameObject SettingCanvas;
    public GameObject ManagerCanvas;
    public GameObject VrPlayerViewCamera;
    public GameObject InformPlayerLocation;
    public GameObject VoiceChatCanvas;

    private void Start()
    {
        GameObject oldObject = GameObject.Find("StartScene");
        if (oldObject != null)
        {
            Destroy(oldObject);
        }
    }
    public void ShowManagerCanvas()
    {
        VoiceChatCanvas = GameObject.Find("VoiceChatCanvas");
        VoiceChatCanvas.SetActive(true);
        SettingCanvas.SetActive(false);
        VrPlayerViewCamera.SetActive(true);
        InformPlayerLocation.SetActive(true);
        ManagerCanvas.SetActive(true);
    }

}
