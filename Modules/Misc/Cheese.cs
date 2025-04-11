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
    public class Cheese : GrateModule
    {
        public static readonly string DisplayName = "Cheesination";
        static GameObject DaCheese;

        protected override void Start()
        {
            base.Start();
            DaCheese = Instantiate(Plugin.assetBundle.LoadAsset<GameObject>("cheese"));
            NetworkPropertyHandler.Instance.OnPlayerModStatusChanged += OnPlayerModStatusChanged;
            Patches.VRRigCachePatches.OnRigCached += OnRigCached;
            DaCheese.transform.SetParent(GestureTracker.Instance.rightHand.transform, true);
            DaCheese.transform.localPosition = new Vector3(-1.5f, 0.2f ,0.1f);
            DaCheese.transform.localRotation = Quaternion.Euler(2, 10, 0);
            DaCheese.transform.localScale /= 2;
            DaCheese.SetActive(false);
        }
        protected override void OnEnable()
        {
            if (!MenuController.Instance.Built) return;
            base.OnEnable();

            try
            {
                DaCheese.SetActive(true);
            }
            catch (Exception e) { Logging.Exception(e); }
        }
        void OnPlayerModStatusChanged(NetworkPlayer player, string mod, bool enabled)
        {
            if (mod == DisplayName && player != NetworkSystem.Instance.LocalPlayer && player.UserId == "B1B20DEEEDB71C63")
            {
                if (enabled)
                {
                    player.Rig().gameObject.GetOrAddComponent<NetCheese>();
                }
                else
                {
                    Destroy(player.Rig().gameObject.GetComponent<NetCheese>());
                }
            }
        }

        protected override void Cleanup()
        {
            DaCheese?.SetActive(false);
            NetworkPropertyHandler.Instance.OnPlayerModStatusChanged -= OnPlayerModStatusChanged;
        }

        private void OnRigCached(NetPlayer player, VRRig rig)
        {
            rig?.gameObject?.GetComponent<NetCheese>()?.Obliterate();
        }

        public override string GetDisplayName()
        {
            return DisplayName;
        }

        public override string Tutorial()
        {
            return "Cheese is cheese because I like cheese";
        }

        class NetCheese : MonoBehaviour
        {
            NetworkedPlayer networkedPlayer;
            GameObject cheese;

            void OnEnable()
            {
                networkedPlayer = gameObject.GetComponent<NetworkedPlayer>();
                var rightHand = networkedPlayer.rig.rightHandTransform;

                cheese = Instantiate(DaCheese);

                cheese.transform.SetParent(rightHand);
                cheese.transform.localPosition = new Vector3(-1.5f, 0.2f ,0.1f);
                cheese.transform.localRotation = Quaternion.Euler(2, 10, 0);
                cheese.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

                cheese.SetActive(true);
            }

            void OnDestroy()
            {
                Destroy(cheese);
            }
            void OnDisable()
            {
                Destroy(cheese);
            }
        }
    }
}