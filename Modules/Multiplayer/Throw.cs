using Grate.GUI;
using Grate.Gestures;
using Grate.Patches;
using Grate.Modules.Physics;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.XR;
using Grate.Extensions;
using Grate.Tools;
using System;
using Grate.Networking;
using System.Collections.Generic;
using GorillaLocomotion.Climbing;

namespace Grate.Modules.Multiplayer
{
    public class Throw : GrateModule
    {
        public static readonly string DisplayName = "Throw";
        public static bool mounted;
        private Transform mount;
        private VRRig mountedRig;
        private bool latchedWithLeft;
        private const float mountDistance = 1.5f;
        private Vector3 mountOffset = new Vector3(0, 1f, -1f);
        private Vector3 mountPosition;

        public static Throw Instance;
        void Awake() { Instance = this; }
        protected override void Start()
        {
            base.Start();
        }

        void Mount(Transform t, VRRig rig)
        {
            mountPosition = GTPlayer.Instance.bodyCollider.transform.position;
            mountedRig = rig;
            mounted = true;
            mount = t;
        }


        void Unmount(bool isLeft)
        {
            if (isLeft)
            {
                Vector3 VELOCITYMYCHILD = mountedRig.leftHandTransform.GetComponent<GorillaVelocityTracker>().GetAverageVelocity();
                GorillaTagger.Instance.offlineVRRig.GetComponent<Rigidbody>().velocity = VELOCITYMYCHILD;
            }
            else
            {
                Vector3 VELOCITYMYCHILD = mountedRig.rightHandTransform.GetComponent<GorillaVelocityTracker>().GetAverageVelocity();
                GorillaTagger.Instance.offlineVRRig.GetComponent<Rigidbody>().velocity = VELOCITYMYCHILD;
            }

            mount = null;
            mounted = false;
            mountedRig = null;
            mount = null;
        }
        void FixedUpdate()
        {
            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                if (Grabbed(rig))
                {
                    TryMount(rig.GetComponent<NetworkedPlayer>().RightGripPressed ? false : true);
                }
            }

            if (mounted)
            {
                if (!Grabbed(mountedRig))
                {
                    Unmount(latchedWithLeft);
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(98, false, 1f);
                }
                else
                {
                    Vector3 position = mount.TransformPoint(mountOffset);
                    GTPlayer.Instance.TeleportTo(mount, true);
                }
            }
        }

        public struct RigScanResult
        {
            public Transform transform;
            public VRRig rig;
            public float distance;
        }

        public RigScanResult ClosestRig(Transform hand)
        {
            VRRig closestRig = null;
            Transform closestTransform = null;
            float closestDistance = Mathf.Infinity;
            foreach (var rig in GorillaParent.instance.vrrigs)
            {
                try
                {
                    if (rig.OwningNetPlayer.IsLocal)
                    {
                        continue;
                    }
                    var rigTransform = rig.transform.FindChildRecursive("head");
                    float distanceToTarget = Vector3.Distance(hand.position, rigTransform.position);

                    if (distanceToTarget < closestDistance)
                    {
                        closestDistance = distanceToTarget;
                        closestTransform = rigTransform;
                        closestRig = rig;
                    }
                }
                catch (Exception e)
                {
                    Logging.Exception(e);
                }
            }
            return new RigScanResult()
            {
                transform = closestTransform,
                distance = closestDistance,
                rig = closestRig
            };
        }

        bool Grabbed(VRRig rig)
        {
            var np = rig.GetComponent<NetworkedPlayer>();
            return np.RightGripPressed || np.LeftGripPressed;
        }

        Transform Held(VRRig rig, bool isLeft) 
        {
            return isLeft ? rig.leftHandTransform : rig.rightHandTransform;
        }


        bool TryMount(bool isLeft)
        {
            var hand = isLeft ? GestureTracker.Instance.leftHand : GestureTracker.Instance.rightHand;
            RigScanResult closest = ClosestRig(hand.transform);
            if (closest.distance < mountDistance && enabled && !mounted)
            {
                if (Grabbed(closest.rig))
                {
                    if (!PositionValidator.Instance.isValidAndStable) return false;

                    Mount(Held(closest.rig, isLeft), closest.rig);
                    return true;
                }
            }
            return false;
        }

        void Latch(InputTracker input)
        {
            if (input.node == XRNode.LeftHand)
                latchedWithLeft = TryMount(true);
            else
                latchedWithLeft = !TryMount(false);
        }

        void Unlatch(InputTracker input)
        {
            if (!enabled || !mounted) return;
            if (input.node == XRNode.LeftHand && latchedWithLeft ||
                input.node == XRNode.RightHand && !latchedWithLeft)
                Unmount(latchedWithLeft);
        }

        protected override void OnEnable()
        {
            if (!MenuController.Instance.Built) return;
            base.OnEnable();
            GestureTracker.Instance.leftGrip.OnPressed += Latch;
            GestureTracker.Instance.leftGrip.OnReleased += Unlatch;
            GestureTracker.Instance.rightGrip.OnPressed += Latch;
            GestureTracker.Instance.rightGrip.OnReleased += Unlatch;
        }

        protected override void Cleanup()
        {
            if (!MenuController.Instance.Built) return;
            if (mounted)
                Unmount(latchedWithLeft);
            if (GestureTracker.Instance is null) return;
            GestureTracker.Instance.leftGrip.OnPressed -= Latch;
            GestureTracker.Instance.leftGrip.OnReleased -= Unlatch;
            GestureTracker.Instance.rightGrip.OnPressed -= Latch;
            GestureTracker.Instance.rightGrip.OnReleased -= Unlatch;

        }

        public override string GetDisplayName()
        {
            return DisplayName;
        }

        public override string Tutorial()
        {
            return "- People can grab you (without your consent) and throw you!";
        }
    }
}
