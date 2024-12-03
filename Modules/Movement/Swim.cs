using Grate.Extensions;
using Grate.GUI;
using GorillaLocomotion;
using GorillaLocomotion.Swimming;
using HarmonyLib;
using UnityEngine;

namespace Grate.Modules
{
    public class Swim : GrateModule
    {
        public static readonly string DisplayName = "Swim";
        public WaterVolume waterVolume;

        void LateUpdate()
        {
            waterVolume.transform.position = GorillaTagger.Instance.headCollider.transform.position + new Vector3(0f, 200f, 0f);
            Player.Instance.audioManager.UnsetMixerSnapshot(0.1f);
        }

        protected override void OnEnable()
        {
            if (!MenuController.Instance.Built) return;
            base.OnEnable();
            waterVolume = GameObject.CreatePrimitive(PrimitiveType.Cube).AddComponent<WaterVolume>();
            waterVolume.GetComponent<Collider>().isTrigger = true;
            waterVolume.gameObject.layer = LayerMask.NameToLayer("Water");
            waterVolume.transform.localScale = new Vector3(5f, 1000f, 5f);
            waterVolume.GetComponent<Renderer>().enabled = false;
        }

        protected override void Cleanup()
        {
            if (!MenuController.Instance.Built) return;
            waterVolume.gameObject.Obliterate();
            Player.Instance.audioManager.UnsetMixerSnapshot(0.1f);
        }

        public override string GetDisplayName()
        {
            return DisplayName;
        }
        public override string Tutorial()
        {
            return "Effect: Surrounds you with invisible water.";
        }

    }
}
