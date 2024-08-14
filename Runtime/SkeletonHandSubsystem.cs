using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.ProviderImplementation;

namespace ubco.ovilab.SkeletonXRHandProvider
{
    public class SkeletonHandSubsystem: XRHandSubsystem
    {
        internal static string id = "skeleton-hand";

        public static SkeletonHandSubsystem subsystem;

        private SkeletonXRHandProvider handsProvider => provider as SkeletonXRHandProvider;
        private XRHandProviderUtility.SubsystemUpdater subsystemUpdater;

        // This method registers the subsystem descriptor with the SubsystemManager
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void RegisterDescriptor()
        {
            var handsSubsystemCinfo = new XRHandSubsystemDescriptor.Cinfo
            {
                id = id,
                providerType = typeof(SkeletonXRHandProvider),
                subsystemTypeOverride = typeof(SkeletonHandSubsystem)
            };
            XRHandSubsystemDescriptor.Register(handsSubsystemCinfo);
        }

        /// <summary>
        /// Initilize the hand subsystem
        /// </summary>
        public static void MaybeInitializeHandSubsystem(bool disableOtherSubsystems, IEnumerable<SkeletonKeyPair> rightTransforms, IEnumerable<SkeletonKeyPair> leftTransforms)
        {
            IEnumerable<XRHandJointID> jointsInLayout = rightTransforms.Select(el => el.jointID).Union(leftTransforms.Select(el => el.jointID)).Distinct();
            bool[] handJointsInLayout = new bool[XRHandJointID.EndMarker.ToIndex()];
            foreach(XRHandJointID jointIDInLayout in jointsInLayout)
            {
                handJointsInLayout[jointIDInLayout.ToIndex()] = true;
            }

            SkeletonXRHandProvider.rightHandTransforms = rightTransforms;
            SkeletonXRHandProvider.leftHandTransforms = leftTransforms;
            SkeletonXRHandProvider.handJointsInLayout = handJointsInLayout;



            if (subsystem == null)
            {
                if (disableOtherSubsystems)
                {
                    List<XRHandSubsystem> currentHandSubsystems = new List<XRHandSubsystem>();
                    SubsystemManager.GetSubsystems(currentHandSubsystems);
                    foreach (XRHandSubsystem handSubsystem in currentHandSubsystems)
                    {
                        if (handSubsystem.running)
                            handSubsystem.Stop();
                    }
                }

                List<XRHandSubsystemDescriptor> descriptors = new List<XRHandSubsystemDescriptor>();
                SubsystemManager.GetSubsystemDescriptors(descriptors);
                for (var i = 0; i < descriptors.Count; ++i)
                {
                    var descriptor = descriptors[i];
                    if (descriptor.id == id)
                    {
                        subsystem = descriptor.Create() as SkeletonHandSubsystem;
                        break;
                    }
                }
            }
        }

        /// <inheritdoc />
        protected override void OnStart()
        {
            base.OnStart();
            if (subsystemUpdater == null)
            {
                subsystemUpdater = new XRHandProviderUtility.SubsystemUpdater(SkeletonHandSubsystem.subsystem);
            }
            subsystemUpdater.Start();

        }

        /// <inheritdoc />
        protected override void OnStop()
        {
            base.OnStop();
            subsystemUpdater.Stop();
        }

        /// <inheritdoc />
        protected override void OnDestroy()
        {
            base.OnDestroy();
            subsystemUpdater.Destroy();
            subsystemUpdater = null;
        }
    }
}
