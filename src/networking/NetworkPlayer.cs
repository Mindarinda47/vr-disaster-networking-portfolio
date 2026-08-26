using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkPlayer : NetworkBehaviour
{
    public Transform root;
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    public Renderer[] meshToDisable;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        { 
            foreach (var item in meshToDisable)
            {
                item.enabled = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(IsOwner)
        {
            root.position = VRRigRefrences.Singleton.root.position;
            root.rotation = VRRigRefrences.Singleton.root.rotation;

            head.position = VRRigRefrences.Singleton.head.position;
            head.rotation = VRRigRefrences.Singleton.head.rotation;

            leftHand.position = VRRigRefrences.Singleton.leftHand.position;
            leftHand.rotation = VRRigRefrences.Singleton.leftHand.rotation;

            rightHand.position = VRRigRefrences.Singleton.rightHand.position;
            rightHand.rotation = VRRigRefrences.Singleton.rightHand.rotation;
        }
    }
}
