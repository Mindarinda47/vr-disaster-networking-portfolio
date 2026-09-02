using UnityEngine;
using UnityEngine.UI;

public class VoiceChatController : MonoBehaviour
{
    public Button MikeButton;
    public Sprite[] sprites;

    private bool isMicOn = false;

    void Start()
    {
        MikeButton.onClick.AddListener(ToggleMicrophone);
        GameObject.Find("MikeButton").GetComponent<Image>().sprite = sprites[0];
    }

    public void ToggleMicrophone()
    {  
        if (isMicOn)
        {
            isMicOn = false;
            GameObject.Find("MikeButton").GetComponent<Image>().sprite = sprites[0];
        }
        else
        {
            isMicOn = true;
            GameObject.Find("MikeButton").GetComponent<Image>().sprite = sprites[1];
        }
    }
}
