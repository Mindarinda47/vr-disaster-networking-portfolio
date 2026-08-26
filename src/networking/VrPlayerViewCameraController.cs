using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class VrPlayerViewCameraController : MonoBehaviour
{
    public Transform target;
    public Vector3 positionOffset = new Vector3(-0.5f, 0.5f, -1f);
    private IEnumerator WaitForNetworkObject()
    {
        while (NetworkObjectManager.Instance.GetNetworkObject() == null)
        {
            Debug.Log("Waiting for network object...");
            yield return new WaitForSeconds(0.1f); // 네트워크 오브젝트가 등록될 때까지 대기
        }

        NetworkObject netObj = NetworkObjectManager.Instance.GetNetworkObject();
        //playerCamera = netObj.GetComponentInChildren<Camera>();
        if (netObj != null)
        {
            target = netObj.transform.Find("Head");
        }
        else
        {
            Debug.LogError("NetworkObject is null.");
        }
    }

 
    private void Start()
    {
        StartCoroutine(WaitForNetworkObject());
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + target.rotation * positionOffset;
            transform.rotation = target.rotation;
        }
    }
}
