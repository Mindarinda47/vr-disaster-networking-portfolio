using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class NetworkConnect : NetworkBehaviour
{
    public GameObject StartCanvas;
    public GameObject SettingCanvas;
    public GameObject WaitingCanvas;
    public GameObject EnterIPCanvas;
    //public GameObject PCManager;
    public GameObject VRPlayer;
    public GameObject NoVrPlayer;
    //public GameObject XRorigin;
    public Camera playerCamera;
    private GameObject NetworkPlayer;
    public GameObject VrPlayerViewCamera;
    public GameObject ManagerCanvas;
    public GameObject InformPlayerLocation;
    public XROriginManager XROriginManager;
    public List<NetworkObject> networkObjects = new List<NetworkObject>();

    // 호스트가 될 때 실행
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        //XROriginManager.DeactivateXROriginForClient(NetworkManager.Singleton.LocalClientId);
        LoadPCSceneServerRPC(NetworkManager.Singleton.LocalClientId); // Host는 PC 씬을 로드
    }

    public void EnterIP()
    {
        StartCanvas.SetActive(false);
        EnterIPCanvas.SetActive(true);
    }

    // 클라이언트가 될 때 실행
    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        StartCoroutine(WaitForClientConnection());
        
        EnterIPCanvas.SetActive(false);
        WaitingCanvas.SetActive(true);
        //playerCamera.targetDisplay = 0;
    }



    // 클라이언트가 연결될 때까지 대기
    private IEnumerator WaitForClientConnection()
    {
        while (!NetworkManager.Singleton.IsClient || !NetworkManager.Singleton.IsConnectedClient)
        {
            Debug.Log("Waiting for connection...");
            yield return new WaitForSeconds(1f);
        }

        // 연결 후 VR 씬 로드
        LoadVRSceneServerRPC(NetworkManager.Singleton.LocalClientId);
    }

    // VR 씬 로드 (서버에서 실행)
    [ServerRpc(RequireOwnership = false)]
    public void LoadVRSceneServerRPC(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer) return; // 서버에서만 실행

        // VR 플레이어를 생성하고 네트워크 객체로 스폰
        NetworkPlayer = Instantiate(VRPlayer);
        NetworkObject netObj = NetworkPlayer.GetComponent<NetworkObject>();
        netObj.SpawnAsPlayerObject(clientId, true); // 클라이언트 ID에 맞는 네트워크 객체 스폰
        NetworkObjectManager.Instance.RegisterNetworkObjectServerRpc(netObj.NetworkObjectId);

        foreach (var networkObject in networkObjects)
        {
            networkObject.ChangeOwnership(clientId); // 클라이언트에게 소유권 전환
        }
        //SetCameraForClient(clientId, netObj.NetworkObjectId);
        // 캔버스 비활성화 및 카메라 설정
    }

    public void ShowManagerCanvas()
    {
        if (NetworkObjectManager.Instance.GetNetworkObject() != null)
        {
            SettingCanvas.SetActive(false);
            VrPlayerViewCamera.SetActive(true);
            ManagerCanvas.SetActive(true);
            InformPlayerLocation.SetActive(true);
            CameraSettingClientRpc();
        }
        else
        {
            NoVrPlayer.SetActive(true);
        }
    }

    public void OKButton()
    {
        NoVrPlayer.SetActive(false);
    }

    [ClientRpc]
    private void CameraSettingClientRpc()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            // Host의 카메라는 따로 설정하지 않음
            return;
        }
        //NetworkObject spawnedNetworkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
        //Camera playerCamera = spawnedNetworkObject.GetComponentInChildren<Camera>();
        WaitingCanvas.SetActive(false);
        playerCamera.targetDisplay = 0;
    }

    /*public void SetCameraForClient(ulong clientId, ulong networkObjectId)
    {
        
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientId }
            }
        };

        CameraSettingClientRpc(networkObjectId, clientRpcParams);
    }*/

    // PC 씬 로드 (서버에서 실행)
    [ServerRpc(RequireOwnership = false)]
    public void LoadPCSceneServerRPC(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer) return; // 서버에서만 실행

        // PC 설정 캔버스 활성화 및 StartCanvas 비활성화
        StartCanvas.SetActive(false);
        SettingCanvas.SetActive(true);
        

        // PC 플레이어를 생성하고 네트워크 객체로 스폰
        // GameObject pcCamera = Instantiate(PCManager);
        // NetworkObject netObj = pcCamera.GetComponent<NetworkObject>();
        // netObj.SpawnAsPlayerObject(clientId, true); // 클라이언트 ID에 맞는 네트워크 객체 스폰
    }


    // 게임 종료 버튼
    public void OnClickExit()
    {
        Application.Quit();
    }
}
