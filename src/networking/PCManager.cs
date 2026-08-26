using UnityEngine;
using UnityEngine.UI;

public class PCManager : MonoBehaviour
{
    public RawImage vrCameraDisplay;
    private Texture2D receivedTexture;

    public static PCManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        receivedTexture = new Texture2D(2, 2);
    }

    public void UpdateRenderTexture(byte[] imageData)
    {
        receivedTexture.LoadImage(imageData);
        receivedTexture.Apply();

        vrCameraDisplay.texture = receivedTexture;
    }
}
