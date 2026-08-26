using Unity.Netcode;
using UnityEngine;

public class XROriginManager : NetworkBehaviour
{
    public GameObject xROrigin;

    public void ActivateXROriginForClient(ulong clientId)
    {
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientId }
            }
        };

        ActivateXROriginClientRpc(clientRpcParams);
    }

    [ClientRpc]
    private void ActivateXROriginClientRpc(ClientRpcParams clientRpcParams = default)
    {
        xROrigin.SetActive(true);
    }

    public void DeactivateXROriginForClient(ulong clientId)
    {
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientId }
            }
        };

        DeactivateXROriginClientRpc(clientRpcParams);
    }

    [ClientRpc]
    private void DeactivateXROriginClientRpc(ClientRpcParams clientRpcParams = default)
    {
        xROrigin.SetActive(false);
    }
}
