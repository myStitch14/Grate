using Grate.Extensions;
using Grate.Gestures;
using Grate.GUI;
using Grate.Modules.Movement;
using GorillaLocomotion;
using Grate.Networking;
using Grate.Tools;
using System;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using NetworkPlayer = NetPlayer;

namespace Grate.Modules.Misc
{
    public class BagHammer : GrateModule
    {
        public static readonly string DisplayName = "Bag Hammer";
        static GameObject Sword;

        protected override void Start()
        {
            base.Start();
            Sword = Instantiate(Plugin.assetBundle.LoadAsset<GameObject>("bagHammer"));
            NetworkPropertyHandler.Instance.OnPlayerModStatusChanged += OnPlayerModStatusChanged;
            Patches.VRRigCachePatches.OnRigCached += OnRigCached;
            Sword.transform.SetParent(GestureTracker.Instance.rightHand.transform);
            Sword.transform.localPosition = new Vector3(0.05f, 0.15f, 0.11f);
            Sword.transform.localRotation = Quaternion.Euler(78.4409f, 0, 0);
            Sword.transform.localScale = new Vector3(10, 10, 10);
            Sword.SetActive(false);
        }
        protected override void OnEnable()
        {
            if (!MenuController.Instance.Built) return;
            base.OnEnable();

            try
            {
                GestureTracker.Instance.rightGrip.OnPressed += ToggleHammerOn;
                GestureTracker.Instance.rightGrip.OnReleased += ToggleHammerOff;
            }
            catch (Exception e) { Logging.Exception(e); }
        }
        void OnPlayerModStatusChanged(NetworkPlayer player, string mod, bool enabled)
        {
            if (mod == DisplayName && player != NetworkSystem.Instance.LocalPlayer && player.UserId == "9ABD0C174289F58E")
            {
                if (enabled)
                {
                    player.Rig().gameObject.GetOrAddComponent<NetHammer>();
                }
                else
                {
                    Destroy(player.Rig().gameObject.GetComponent<NetHammer>());
                }
            }
        }

        void ToggleHammerOn(InputTracker tracker)
        {
            Sword?.SetActive(true);
        }

        void ToggleHammerOff(InputTracker tracker)
        {
            Sword?.SetActive(false);
        }
        protected override void Cleanup()
        {
            Sword?.SetActive(false);
            GestureTracker.Instance.rightGrip.OnPressed -= ToggleHammerOn;
            GestureTracker.Instance.rightGrip.OnReleased -= ToggleHammerOff;
            NetworkPropertyHandler.Instance.OnPlayerModStatusChanged -= OnPlayerModStatusChanged;
        }

        private void OnRigCached(NetPlayer player, VRRig rig)
        {
            rig?.gameObject?.GetComponent<NetHammer>()?.Obliterate();
        }

        public override string GetDisplayName()
        {
            return DisplayName;
        }

        public override string Tutorial()
        {
            return "I will cut you apart limb by limb";
        }

        class NetHammer : MonoBehaviour
        {
            NetworkedPlayer networkedPlayer;
            GameObject swordR;

            void Start()
            {
                networkedPlayer = gameObject.GetComponent<NetworkedPlayer>();
                var rightHand = networkedPlayer.rig.rightHandTransform;

                swordR = Instantiate(Sword);

                swordR.transform.SetParent(rightHand);

                Sword.transform.localPosition = new Vector3(0, 0.1f, 0);
                Sword.transform.localRotation = Quaternion.Euler(0, 180, 0);
                Sword.transform.localScale = new Vector3(10, 10, 10);

                networkedPlayer.OnGripPressed += OnGripPressed;
                networkedPlayer.OnGripReleased += OnGripReleased;
            }

            void OnGripPressed(NetworkedPlayer player, bool isLeft)
            {
                if (!isLeft)
                {
                    swordR.SetActive(true);
                }
            }
            void OnGripReleased(NetworkedPlayer player, bool isLeft)
            {
                if (!isLeft)
                {
                    swordR.SetActive(false);
                }
            }

            void OnDestroy()
            {
                networkedPlayer.OnGripPressed -= OnGripPressed;
                networkedPlayer.OnGripReleased -= OnGripReleased;
                swordR.Obliterate();
            }
            void OnDisable()
            {
                networkedPlayer.OnGripPressed -= OnGripPressed;
                networkedPlayer.OnGripReleased -= OnGripReleased;
                swordR.Obliterate();
            }
        }
    }
}
