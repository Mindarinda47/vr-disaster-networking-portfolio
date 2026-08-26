using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ChangeCanvas : MonoBehaviour
{   
    public GameObject SettingCanvas;
    public GameObject ManagerCanvas;
    public GameObject WaitCanvas;
    public GameObject XROrigin;
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

        //GameObject xrOriginPrefab = Instantiate(XROrigin);
        //NetworkObject netObj = xrOriginPrefab.GetComponent<NetworkObject>();
        //xrOriginPrefab.SetActive(true);
        //netObj.SpawnAsPlayerObject(1);

        //NetworkObjectManager.Instance.RegisterNetworkObjectServerRpc(netObj.NetworkObjectId);


    }

    public void VRStart()
    {
        WaitCanvas.SetActive(false);
        NetworkObject netObj = NetworkObjectManager.Instance.GetNetworkObject();
        if (netObj != null)
        {
            VoiceChatCanvas = GameObject.Find("VoiceChatCanvas");
            VoiceChatCanvas.SetActive(true);
            Camera playerCamera = netObj.GetComponentInChildren<Camera>();
            playerCamera.targetDisplay = 0;
        }
        else
        {
            Debug.LogError("Network object not found!");
        }

    }
}
