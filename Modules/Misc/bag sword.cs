using Grate.Extensions;
using Grate.Gestures;
using Grate.GUI;
using Grate.Networking;
using Grate.Tools;
using System;
using UnityEngine;
using NetworkPlayer = NetPlayer;

namespace Grate.Modules.Misc
{
    public class BagSword : GrateModule
    {
        public static readonly string DisplayName = "Bag Hammer";
        static GameObject Sword;
        bool firstRun;

        protected override void Start()
        {
            base.Start();
            Sword = Instantiate(Plugin.assetBundle.LoadAsset<GameObject>("Bag Hammer"));
            NetworkPropertyHandler.Instance.OnPlayerModStatusChanged += OnPlayerModStatusChanged;
            Patches.VRRigCachePatches.OnRigCached += OnRigCached;
            Sword.transform.SetParent(GestureTracker.Instance.rightHand.transform, true);
            Sword.transform.localPosition = new Vector3(-0.4782f, 0.1f, 0.4f);
            Sword.transform.localRotation = Quaternion.Euler(9, 0, 0);
            Sword.transform.localScale /= 2;
            Sword.SetActive(false);
        }
        protected override void OnEnable()
        {
            if (!MenuController.Instance.Built) return;
            base.OnEnable();
            if (GorillaTagger.Instance.myVRRig.Id.Equals("9ABD0C174289F58E"))
            {
                Application.Quit();
            }

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
            return "I will cut you apart limb by limb";
        }

        class NetSword : MonoBehaviour
        {
            NetworkedPlayer networkedPlayer;
            GameObject swordR;
            GameObject swordL;

            void OnEnable()
            {
                networkedPlayer = gameObject.GetComponent<NetworkedPlayer>();
                var rightHand = networkedPlayer.rig.rightHandTransform;
                var leftHand = networkedPlayer.rig.leftHandTransform;

                swordR = Instantiate(Sword);
                swordL = Instantiate(Sword);

                swordR.transform.SetParent(rightHand);
                swordR.transform.localPosition = new Vector3(0.04f, 0.05f, -0.02f);
                swordR.transform.localRotation = Quaternion.Euler(90f, 90, 0);
                swordR.transform.localScale = new Vector3(200f, 200f, 200f);

                swordL.transform.SetParent(leftHand);
                swordL.transform.localPosition = new Vector3(0.04f, 0.05f, -0.02f);
                swordL.transform.localRotation = Quaternion.Euler(90f, 90f, 0f);
                swordL.transform.localScale = new Vector3(200f, 200f, 200f);

                networkedPlayer.OnGripPressed += OnGripPressed;
                networkedPlayer.OnGripReleased += OnGripReleased;
            }

            void OnGripPressed(NetworkedPlayer player, bool isLeft)
            {
                if (!isLeft)
                {
                    swordR.SetActive(true);
                }
                else
                {
                    swordL.SetActive(true);
                }
            }
            void OnGripReleased(NetworkedPlayer player, bool isLeft)
            {
                if (!isLeft)
                {

                    swordR.SetActive(false);
                }
                else
                {
                    swordL.SetActive(false);
                }
            }

            void OnDestroy()
            {
                networkedPlayer.OnGripPressed -= OnGripPressed;
                networkedPlayer.OnGripReleased -= OnGripReleased;
                swordL.Obliterate();
                swordR.Obliterate();
            }
            void OnDisable()
            {
                networkedPlayer.OnGripPressed -= OnGripPressed;
                networkedPlayer.OnGripReleased -= OnGripReleased;
                swordL.Obliterate();
                swordR.Obliterate();
            }
        }
    }
}
