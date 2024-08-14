using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.ProviderImplementation;

namespace ubco.ovilab.SkeletonXRHandProvider
{
    [Serializable]
    public struct SkeletonKeyPair
    {
        public XRHandJointID jointID;
        public Transform transform;
    }

    public class SkeletonXRHands: MonoBehaviour
    {
        [SerializeField, Tooltip("The jointID to transform mapping for the right hand.")]
        private List<SkeletonKeyPair> rightHandTransforms;

        /// <summary>
        /// The jointID to transform mapping for the right hand.
        /// </summary>
        public List<SkeletonKeyPair> RightHandTransforms => rightHandTransforms;

        [SerializeField, Tooltip("The jointID to transform mapping for the left hand.")]
        private List<SkeletonKeyPair> leftHandTransforms;

        /// <summary>
        /// The jointID to transform mapping for the left hand.
        /// </summary>
        public List<SkeletonKeyPair> LeftHandTransforms => leftHandTransforms;

        [SerializeField, Tooltip("Should other subsystems be enabled/disabled when this is activated. Changing in runtime has no effect.")]
        private bool disableOtherSubsystems;

        protected void Awake()
        {
            SkeletonHandSubsystem.MaybeInitializeHandSubsystem(disableOtherSubsystems);
        }

        protected void OnEnable()
        {
            SkeletonHandSubsystem.subsystem?.Start();
            SetValues();
        }

        protected void OnDisable()
        {
            SkeletonHandSubsystem.subsystem?.Stop();
        }

        protected void OnValidate()
        {
            if (Application.isPlaying && SkeletonHandSubsystem.subsystem != null)
            {
                SetValues();
            }
        }

        protected void SetValues()
        {
            SkeletonHandSubsystem.SetJointsInLayout(RightHandTransforms.Select(el => el.jointID).Union(LeftHandTransforms.Select(el => el.jointID)).Distinct());
            SkeletonHandSubsystem.SetHandTransforms(RightHandTransforms, LeftHandTransforms);
        }
    }
}
