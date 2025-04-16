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
        public GameObject waterVolume;

        void LateUpdate()
        {
            if(waterVolume != null)
                waterVolume.transform.position = GorillaTagger.Instance.headCollider.transform.position + new Vector3(0f, 200f, 0f);
                GTPlayer.Instance.audioManager.UnsetMixerSnapshot(0.1f);
        }

        protected override void Start()
        {
            base.Start();
            waterVolume = Instantiate(Plugin.water);
            waterVolume.transform.localScale = new Vector3(5f, 1000f, 5f);
            waterVolume.SetActive(false);
            if (waterVolume.GetComponent<Renderer>())
            {
                waterVolume.GetComponent<Renderer>().enabled = false;
            }
        }

        protected override void OnEnable()
        {
            if (!MenuController.Instance.Built) return;
            base.OnEnable();
            waterVolume.SetActive(true);
            ReloadConfiguration();
        }

        protected override void Cleanup()
        {
            if (!MenuController.Instance.Built) return;
            waterVolume.SetActive(false);
            GTPlayer.Instance.audioManager.UnsetMixerSnapshot(0.1f);
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