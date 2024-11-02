using Grate.Extensions;
using Grate.Gestures;
using Grate.GUI;
using Grate.Tools;
using System;
using UnityEngine;

namespace Grate.Modules.Misc
{
    public class RatSword : GrateModule
    {
        public static readonly string DisplayName = "Rat Sword";
        private GameObject sword;
        protected override void OnEnable()
        {
            if (!MenuController.Instance.Built) return;
            base.OnEnable();

            try
            {
                sword = Instantiate(Plugin.assetBundle.LoadAsset<GameObject>("Rat Sword"));
                sword.GetComponent<MeshRenderer>().materials[1].shader = sword.GetComponent<MeshRenderer>().materials[0].shader;
                sword.transform.SetParent(GestureTracker.Instance.rightHand.transform, true);
                sword.transform.localPosition = new Vector3(-0.4782f, 0.1f, 0.4f);
                sword.transform.localRotation = Quaternion.Euler(9, 0, 0);
                sword.transform.localScale /= 2;
                sword.SetActive(false);
                GestureTracker.Instance.rightGrip.OnPressed += ToggleRatSwordOn;
                GestureTracker.Instance.rightGrip.OnReleased += ToggleRatSwordOff;
            }
            catch (Exception e) { Logging.Exception(e); }
        }

        void ToggleRatSwordOn(InputTracker tracker)
        {
            sword?.SetActive(true);
        }

        void ToggleRatSwordOff(InputTracker tracker)
        {
            sword?.SetActive(false);
        }

        protected override void Cleanup()
        {
            GestureTracker.Instance.rightGrip.OnPressed -= ToggleRatSwordOn;
            GestureTracker.Instance.rightGrip.OnReleased -= ToggleRatSwordOff;
            sword?.Obliterate();
        }

        public override string GetDisplayName()
        {
            return DisplayName;
        }

        public override string Tutorial()
        {
            return "I met a lil' kid in canyons who wanted me to make him a sword.\n" +
                "[Grip] to wield your weapon, rat kid.";
        }
    }
}
