# Skeletong XR Hands

This provides a component that feeds the locations of transforms to XR Hands. 
This is useful if when you would like transforms poses, generally in the form of skeletons that may be controlled by other systems, to be exposed through XR Hands API so that any system that uses the XR Hands API can be used with  the custom skeleton.

To use simply add the `SkeletonXRHands` component anywhere and configure the JointIDs and the corresponding transform whose pose is to be exposed through the XR Hands API.
