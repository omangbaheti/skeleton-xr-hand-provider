using System.Collections.Generic;
using UnityEngine;
using System;
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
        [SerializeField, Tooltip("The jointID to transform mapping for the right hand. Changing in runtime has no effect.")]
        private List<SkeletonKeyPair> rightHandTransforms;

        [SerializeField, Tooltip("The jointID to transform mapping for the left hand. Changing in runtime has no effect.")]
        private List<SkeletonKeyPair> leftHandTransforms;

        [SerializeField, Tooltip("Should other subsystems be enabled/disabled when this is activated. Changing in runtime has no effect.")]
        private bool disableOtherSubsystems;

        protected void Awake()
        {
            SkeletonHandSubsystem.MaybeInitializeHandSubsystem(disableOtherSubsystems, rightHandTransforms, leftHandTransforms);
        }

        protected void OnEnable()
        {
            SkeletonHandSubsystem.subsystem?.Start();
        }

        protected void OnDisable()
        {
            SkeletonHandSubsystem.subsystem?.Stop();
        }
    }
}
