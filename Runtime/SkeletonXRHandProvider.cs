using System.Collections.Generic;
using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.ProviderImplementation;

namespace ubco.ovilab.SkeletonXRHandProvider
{
    public enum Axis
    {
        XPlus,
        XMinus,
        YPlus,
        YMinus,
        ZPlus,
        ZMinus
    };

    public class SkeletonXRHandProvider : XRHandSubsystemProvider
    {
        internal static IEnumerable<SkeletonKeyPair> rightHandTransforms;
        internal static IEnumerable<SkeletonKeyPair> leftHandTransforms;
        internal static Axis forwardAxis;
        internal static Axis upAxis;
        internal static bool[] handJointsInLayout;

        /// <inheritdoc />
        public override void Destroy()
        {}

        /// <inheritdoc />
        public override void Start()
        {}

        /// <inheritdoc />
        public override void Stop()
        {}

        /// <inheritdoc />
        public override void GetHandLayout(NativeArray<bool> handJointsInLayout)
        {
            handJointsInLayout.CopyFrom(SkeletonXRHandProvider.handJointsInLayout);
        }

        /// <inheritdoc />
        public override XRHandSubsystem.UpdateSuccessFlags TryUpdateHands(XRHandSubsystem.UpdateType updateType,
                                                                          ref Pose leftHandRootPose,
                                                                          NativeArray<XRHandJoint> leftHandJoints,
                                                                          ref Pose rightHandRootPose,
                                                                          NativeArray<XRHandJoint> rightHandJoints)
        {
            XRHandSubsystem.UpdateSuccessFlags successFlags = XRHandSubsystem.UpdateSuccessFlags.None;
            if (UpdateJointData(Handedness.Left, leftHandJoints, ref leftHandRootPose))
            {
                successFlags |= XRHandSubsystem.UpdateSuccessFlags.LeftHandRootPose | XRHandSubsystem.UpdateSuccessFlags.LeftHandJoints;
            }

            if (UpdateJointData(Handedness.Right, rightHandJoints, ref rightHandRootPose))
            {
                successFlags |= XRHandSubsystem.UpdateSuccessFlags.RightHandRootPose | XRHandSubsystem.UpdateSuccessFlags.RightHandJoints;
            }

            return successFlags;
        }

        /// <summary>
        /// Populate the handJoints array.
        /// </summary>
        protected bool UpdateJointData(Handedness handedness, NativeArray<XRHandJoint> handJoints, ref Pose handRootPose)
        {
            try
            {
                IEnumerable<SkeletonKeyPair> handTransformCache = handedness switch
                {
                    Handedness.Right => rightHandTransforms,
                    Handedness.Left => rightHandTransforms,
                    _ => throw new System.InvalidOperationException()
                };

                if (handTransformCache == null)
                {
                    return false;
                }

                foreach (SkeletonKeyPair pair in handTransformCache)
                {
                    Quaternion rotation = Quaternion.LookRotation(
                        forwardAxis switch
                        {
                            Axis.XPlus => pair.transform.right,
                            Axis.XMinus => -pair.transform.right,
                            Axis.YPlus => pair.transform.up,
                            Axis.YMinus => -pair.transform.up,
                            Axis.ZPlus => pair.transform.forward,
                            Axis.ZMinus => -pair.transform.forward,
                            _ => pair.transform.forward
                                },
                        upAxis switch
                        {
                            Axis.XPlus => pair.transform.right,
                            Axis.XMinus => -pair.transform.right,
                            Axis.YPlus => pair.transform.up,
                            Axis.YMinus => -pair.transform.up,
                            Axis.ZPlus => pair.transform.forward,
                            Axis.ZMinus => -pair.transform.forward,
                            _ => pair.transform.forward
                        }
                    );
                    Pose pose = new Pose(pair.transform.position, rotation);
                    XRHandJoint joint = XRHandProviderUtility.CreateJoint(handedness, XRHandJointTrackingState.Pose, pair.jointID, pose);
                    int jointIndex = XRHandJointIDUtility.ToIndex(pair.jointID);
                    if (pair.jointID == XRHandJointID.Wrist)
                    {
                        handRootPose = pose;
                    }
                    handJoints[jointIndex] = joint;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }
            return true;
        }
    }
}
