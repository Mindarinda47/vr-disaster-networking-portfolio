using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;


public class PlayerSpawnPlace : NetworkBehaviour
{
    public Dropdown floorDropdown;
    public Vector3 firstFloor = new Vector3(0, 2, 0);
    public Vector3 secondFloor = new Vector3(0, 6, 0);
    public Vector3 thirdFloor = new Vector3(0, 10, 0) ;
    public GameObject XROrigin;
    private NetworkObject netObj;

    private IEnumerator WaitForNetworkObject()
    {
        while (NetworkObjectManager.Instance.GetNetworkObject() == null)
        {
            yield return new WaitForSeconds(0.1f); // 네트워크 오브젝트가 등록될 때까지 대기
        }

        netObj = NetworkObjectManager.Instance.GetNetworkObject();
        // NetworkObject가 준비된 뒤에만 층 이동 입력을 연결
        floorDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    private void Start()
    {
        StartCoroutine(WaitForNetworkObject());
    }

    // 층 선택이 변경되면 해당 위치로 이동 요청
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
                break;
            case 1:
                TeleportToPosition(secondFloor);
                break;
            case 2:
                TeleportToPosition(thirdFloor);
                break;
        }
    }

    private void TeleportToPosition(Vector3 newPosition)
    {
        if (IsServer)
        {
            netObj.transform.position = newPosition;
            RequestPositionChangeClientRpc(newPosition);
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
        }
    }
}
