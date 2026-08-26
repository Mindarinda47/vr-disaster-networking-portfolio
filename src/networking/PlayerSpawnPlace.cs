using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.XR.Interaction.Toolkit;


public class PlayerSpawnPlace : NetworkBehaviour
{
    public Dropdown floorDropdown;
    public Transform firstFloorPosition;
    public Transform secondFloorPosition;
    public Transform thirdFloorPosition;
    public Vector3 firstFloor = new Vector3(0, 2, 0);
    public Vector3 secondFloor = new Vector3(0, 6, 0);
    public Vector3 thirdFloor = new Vector3(0, 10, 0) ;
    public GameObject XROrigin;
    private NetworkObject netObj;

    private IEnumerator WaitForNetworkObject()
    {
        while (NetworkObjectManager.Instance.GetNetworkObject() == null)
        {
            Debug.Log("Waiting for network object...");
            yield return new WaitForSeconds(0.1f); // 네트워크 오브젝트가 등록될 때까지 대기
        }

        netObj = NetworkObjectManager.Instance.GetNetworkObject();
        Debug.Log("Network object found, initializing...");

        // netObj가 할당된 후에야 Dropdown 리스너를 등록
        floorDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    private void Start()
    {
        // 코루틴을 실행하여 netObj가 할당될 때까지 기다림
        StartCoroutine(WaitForNetworkObject());
    }

    // Dropdown에서 층을 선택했을 때 호출될 함수
    private void OnDropdownValueChanged(int index)
    {
        if (netObj == null)
        {
            Debug.LogError("Network object is not assigned yet.");
            return;
        }

        switch (index)
        {
            case 0:
                TeleportToPosition(firstFloor);
                Debug.Log("Player Location Change(1st)");
                break;
            case 1:
                TeleportToPosition(secondFloor);
                Debug.Log("Player Location Change(2nd)");
                break;
            case 2:
                TeleportToPosition(thirdFloor);
                Debug.Log("Player Location Change(3rd)");
                break;
        }
    }

    private void TeleportToPosition(Vector3 newPosition)
    {
        if (IsServer)
        {
            netObj.transform.position = newPosition;
            RequestPositionChangeClientRpc(newPosition);

            Debug.Log("Request Teleport");
        }
        else
        {
            // 클라이언트는 서버에 위치 변경 요청
            TeleportServerRpc(newPosition);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void TeleportServerRpc(Vector3 newPosition)
    {
        XROrigin.transform.position = newPosition;
        netObj.transform.position = newPosition;        

        RequestPositionChangeClientRpc(newPosition);
    }

    [ClientRpc]
    private void RequestPositionChangeClientRpc(Vector3 newPosition)
    {
        if (!IsOwner)
        {
            XROrigin.transform.position = newPosition;
            netObj.transform.position = newPosition;

            Debug.Log("Successfully teleported on client");
        }
    }
}
