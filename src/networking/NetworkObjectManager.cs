using Unity.Netcode;
using UnityEngine;

public class NetworkObjectManager : NetworkBehaviour
{
    public static NetworkObjectManager Instance { get; private set; }
    private NetworkObject spawnedNetworkObject;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [ServerRpc]
    public void RegisterNetworkObjectServerRpc(ulong networkObjectId)
    {
        spawnedNetworkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
        NotifyClientsOfNewObjectClientRpc(networkObjectId);
    }

    [ClientRpc]
    private void NotifyClientsOfNewObjectClientRpc(ulong networkObjectId)
    {
        spawnedNetworkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
    }

    public NetworkObject GetNetworkObject()
    {
        return spawnedNetworkObject;
    }
}
