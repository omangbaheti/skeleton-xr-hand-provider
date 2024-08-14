using System.Collections.Generic;
using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.ProviderImplementation;

namespace ubco.ovilab.SkeletonXRHandProvider
{
    public class SkeletonXRHandProvider : XRHandSubsystemProvider
    {
        private IEnumerable<SkeletonKeyPair> rightHandTransforms;
        private IEnumerable<SkeletonKeyPair> leftHandTransforms;
        private bool[] handJointsInLayout;

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
            handJointsInLayout.CopyFrom(this.handJointsInLayout);
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
        /// Set the transforms of the right hand.
        /// </summary>
        internal void SetRightHandTransforms(IEnumerable<SkeletonKeyPair> transforms)
        {
            rightHandTransforms = transforms;
        }

        /// <summary>
        /// Set the transforms of the left hand.
        /// </summary>
        internal void SetLeftHandTransforms(IEnumerable<SkeletonKeyPair> transforms)
        {
            leftHandTransforms = transforms;
        }

        /// <summary>
        /// Set the joints in the hand layout
        /// </summary>
        internal void SetJointsInLayout(IEnumerable<XRHandJointID> jointsInLayout)
        {
            handJointsInLayout = new bool[XRHandJointID.EndMarker.ToIndex() + 1];
            foreach(XRHandJointID jointIDInLayout in jointsInLayout)
            {
                handJointsInLayout[jointIDInLayout.ToIndex()] = true;
            }
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
                    Pose pose = new Pose(pair.transform.position, pair.transform.rotation);
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
