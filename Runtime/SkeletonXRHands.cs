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
    }
}
