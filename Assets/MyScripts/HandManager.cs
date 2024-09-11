using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;


public class HandManager : MonoBehaviour
{

    private XRHand hand1 = new XRHand();



    void Start()
    {
        XRHandSubsystem m_Subsystem =
        XRGeneralSettings.Instance?
        .Manager?
        .activeLoader?
        .GetLoadedSubsystem<XRHandSubsystem>();

        if (m_Subsystem != null)
            m_Subsystem.updatedHands += OnHandUpdate;



        for (var i = XRHandJointID.BeginMarker.ToIndex();
        i < XRHandJointID.EndMarker.ToIndex();
        i++)
        {
            var trackingData = hand1.GetJoint(XRHandJointIDUtility.FromIndex(i));

            if (trackingData.TryGetPose(out Pose pose))
            {
                // displayTransform is some GameObject's Transform component
               Debug.Log(pose.position);
               Debug.Log(pose.rotation);
                Debug.Log("SONO QUIIIII");
            }
        }


    }

    void OnHandUpdate(XRHandSubsystem subsystem,
                  XRHandSubsystem.UpdateSuccessFlags updateSuccessFlags,
                  XRHandSubsystem.UpdateType updateType)
    {
        switch (updateType)
        {
            case XRHandSubsystem.UpdateType.Dynamic:
                // Update game logic that uses hand data
                break;
            case XRHandSubsystem.UpdateType.BeforeRender:
                // Update visual objects that use hand data
                break;
        }
    }

}
