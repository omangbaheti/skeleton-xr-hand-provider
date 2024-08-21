using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.XR.Hands;

namespace ubco.ovilab.SkeletonXRHandProvider
{
    [Serializable]
    /// <summary>
    /// The container struct that contains the <see cref="XRHandJointID">jointID</see> and <see cref="Transform"/>
    /// </summary>
    public struct SkeletonKeyPair
    {
        public XRHandJointID jointID;
        public Transform transform;
    }

    /// <summary>
    /// The component that is used to setup the skeleton hands.
    /// Only one component is expected to be active at a time.
    /// </summary>
    public class SkeletonXRHands: MonoBehaviour
    {
        private static bool instanceExists = false;
        
        [SerializeField, Tooltip("The jointID to transform mapping for the right hand. Changing in runtime has no effect.")]
        private List<SkeletonKeyPair> rightHandTransforms;
        
        [SerializeField, Tooltip("The jointID to transform mapping for the left hand. Changing in runtime has no effect.")]
        private List<SkeletonKeyPair> leftHandTransforms;

        [SerializeField, Tooltip("This axis will be used as the forward vector of the pose provided to the subsystem.")]
        private Axis forwardAxis = Axis.ZPlus;

        [SerializeField, Tooltip("This axis will be used as the up vector of the pose provided to the subsystem.")]
        private Axis upAxis = Axis.YPlus;

        [SerializeField, Tooltip("Should other subsystems be enabled/disabled when this is activated. Changing in runtime has no effect.")]
        private bool disableOtherSubsystems;
        
        [SerializeField] public GameObject rightHand;
        private Dictionary<XRHandJointID, string> skeletonTransformNamePair = new()
        {
            {XRHandJointID.ThumbProximal,      "R1D2"},
            {XRHandJointID.ThumbDistal,        "R1D3"},
            {XRHandJointID.ThumbTip,           "R1D4"},
            {XRHandJointID.IndexProximal,      "R2D1"},
            {XRHandJointID.IndexIntermediate,  "R2D2"},
            {XRHandJointID.IndexDistal,        "R2D3"},
            {XRHandJointID.IndexTip,           "R2D3.001"},
            {XRHandJointID.MiddleProximal,     "R3D1"},
            {XRHandJointID.MiddleIntermediate, "R3D2"},
            {XRHandJointID.MiddleDistal,       "R3D3"},
            {XRHandJointID.MiddleTip,          "R3D3.001"},
            {XRHandJointID.RingProximal,       "R4D1"},
            {XRHandJointID.RingIntermediate,   "R4D2"},
            {XRHandJointID.RingDistal,         "R4D3"},
            {XRHandJointID.RingTip,            "R4D3.001"},
            {XRHandJointID.LittleProximal,     "R5D1"},
            {XRHandJointID.LittleIntermediate, "R5D2"},
            {XRHandJointID.LittleDistal,       "R5D3"},
            {XRHandJointID.LittleTip,          "R5D3.001"}
        };
        /// <summary>
        /// This axis will be used as the forward vector of the pose provided to the subsystem.
        /// </summary>
        public Axis ForwardAxis {
            get => forwardAxis;
            set
            {
                forwardAxis = value;
                SkeletonXRHandProvider.forwardAxis = forwardAxis;
            }
        }

        /// <summary>
        /// This axis will be used as the up vector of the pose provided to the subsystem.
        /// </summary>
        public Axis UpAxis
        {
            get => upAxis;
            set
            {
                upAxis = value;
                SkeletonXRHandProvider.upAxis = upAxis;
            }
        }

        protected void Awake()
        {
            if (instanceExists)
            {
                Debug.LogWarning($"There are more than one SkeletonXRHands. Only the first one that awakes will be used.");
                return;
            }
            instanceExists = true;
            SkeletonHandSubsystem.MaybeInitializeHandSubsystem(disableOtherSubsystems, rightHandTransforms, leftHandTransforms, ForwardAxis, UpAxis);
        }

        protected void OnEnable()
        {
            SkeletonHandSubsystem.subsystem?.Start();
        }

        protected void OnDisable()
        {
            SkeletonHandSubsystem.subsystem?.Stop();
        }

        protected void OnValidate()
        {
            if (Application.isPlaying)
            {
                SkeletonXRHandProvider.forwardAxis = ForwardAxis;
                SkeletonXRHandProvider.upAxis = UpAxis;
            }
        }

        public void AutoPopulate()
        {
            foreach (KeyValuePair<XRHandJointID,string> pair in skeletonTransformNamePair)
            {
                SkeletonKeyPair skeletonKeyPair = new SkeletonKeyPair();
                skeletonKeyPair.jointID = pair.Key;
                skeletonKeyPair.transform = FindChildByName(rightHand, pair.Value).transform;
                rightHandTransforms.Add(skeletonKeyPair);
            }
        }
        
        public GameObject FindChildByName(GameObject parent, string childName)
        {
            // If the parent GameObject matches the name, return it
            if (parent.name == childName)
            {
                return parent;
            }

            // Recursively search through all children
            foreach (Transform child in parent.transform)
            {
                GameObject result = FindChildByName(child.gameObject, childName);
                if (result != null)
                {
                    return result;
                }
            }

            // Return null if no matching GameObject is found
            return null;
        }
    }
}
