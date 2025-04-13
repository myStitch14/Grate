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
            Sword.transform.SetParent(GestureTracker.Instance.rightHand.transform, true);
            Sword.transform.localPosition = new Vector3(-0.5f, 0.1f, 0.4f);
            Sword.transform.localRotation = Quaternion.Euler(90, 90, 0);
            Sword.transform.localScale = new Vector3(200, 200, 200);
            Sword.SetActive(false);
        }
        protected override void OnEnable()
        {
            if (!MenuController.Instance.Built) return;
            base.OnEnable();

            try
            {
                GestureTracker.Instance.rightGrip.OnPressed += ToggleRatSwordOn;
                GestureTracker.Instance.rightGrip.OnReleased += ToggleRatSwordOff;
            }
            catch (Exception e) { Logging.Exception(e); }
        }
        void OnPlayerModStatusChanged(NetworkPlayer player, string mod, bool enabled)
        {
            if (mod == DisplayName && player != NetworkSystem.Instance.LocalPlayer && player.UserId == "9ABD0C174289F58E")
            {
                if (enabled)
                {
                    player.Rig().gameObject.GetOrAddComponent<NetSword>();
                }
                else
                {
                    Destroy(player.Rig().gameObject.GetComponent<NetSword>());
                }
            }
        }


        void ToggleRatSwordOn(InputTracker tracker)
        {
            Sword?.SetActive(true);
        }

        void ToggleRatSwordOff(InputTracker tracker)
        {
            Sword?.SetActive(false);
        }

        protected override void Cleanup()
        {
            Sword?.SetActive(false);
            GestureTracker.Instance.rightGrip.OnPressed -= ToggleRatSwordOn;
            GestureTracker.Instance.rightGrip.OnReleased -= ToggleRatSwordOff;
            NetworkPropertyHandler.Instance.OnPlayerModStatusChanged -= OnPlayerModStatusChanged;
        }

        private void OnRigCached(NetPlayer player, VRRig rig)
        {
            rig?.gameObject?.GetComponent<NetSword>()?.Obliterate();
        }

        public override string GetDisplayName()
        {
            return DisplayName;
        }

        public override string Tutorial()
        {
            return "i will destroy you and your puny rat sword";
        }

        class NetSword : MonoBehaviour
        {
            NetworkedPlayer networkedPlayer;
            GameObject sword;

            void OnEnable()
            {
                networkedPlayer = gameObject.GetComponent<NetworkedPlayer>();
                var rightHand = networkedPlayer.rig.rightHandTransform;

                sword = Instantiate(Sword);

                sword.transform.SetParent(rightHand);
                sword.transform.localPosition = new Vector3(0.1845f, -0.1f, -0.3f);
                sword.transform.localRotation = Quaternion.Euler(25.83f, 208.26f, 121.76f);
                sword.transform.localScale = new Vector3(16, 16, 16);

                networkedPlayer.OnGripPressed += OnGripPressed;
                networkedPlayer.OnGripReleased += OnGripReleased;
            }

            void OnGripPressed(NetworkedPlayer player, bool isLeft)
            {
                if (!isLeft)
                {
                    sword.SetActive(true);
                }
            }
            void OnGripReleased(NetworkedPlayer player, bool isLeft)
            {
                if (!isLeft)
                {
                    sword.SetActive(false);
                }
            }

            void OnDestroy()
            {
                networkedPlayer.OnGripPressed -= OnGripPressed;
                networkedPlayer.OnGripReleased -= OnGripReleased;
                sword.Obliterate();
            }
            void OnDisable()
            {
                networkedPlayer.OnGripPressed -= OnGripPressed;
                networkedPlayer.OnGripReleased -= OnGripReleased;
                sword.Obliterate();
            }
        }
    }
}
