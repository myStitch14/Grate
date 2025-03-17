using GorillaLocomotion;
using Grate.Extensions;
using Grate.GUI;
using Grate.Gestures;
using Grate.Tools;
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using BepInEx.Configuration;

namespace Grate.Modules.Multiplayer
{
    public class Kamehameha : GrateModule
    {
        public static readonly string DisplayName = "Kamehameha";

        private Transform orb;
        private Rigidbody orbBody;
        private LineRenderer bananaLine;
        public bool isCharging, isFiring;
        public static readonly float maxOrbSize = .4f;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (MenuController.Instance.Built)
            {
                GestureTracker.Instance.OnKamehameha += OnKamehameha;
                bananaLine = Instantiate(Plugin.assetBundle.LoadAsset<GameObject>("Banana Line")).GetComponent<LineRenderer>();
                bananaLine.material = Plugin.assetBundle.LoadAsset<Material>("Laser Sight Material");
                bananaLine.gameObject.SetActive(false);
                orb = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
                orb.gameObject.SetActive(false);
                orb.gameObject.GetComponent<Collider>().isTrigger = true;
                orbBody = orb.gameObject.AddComponent<Rigidbody>();
                orbBody.isKinematic = true;
                orbBody.useGravity = false;
                orb.gameObject.layer = GrateInteractor.InteractionLayer;
                orb.gameObject.GetComponent<Renderer>().material = bananaLine.material;
            }
        }

        private void OnKamehameha()
        {
            if (enabled && !isCharging && !isFiring)
                StartCoroutine(GrowBananas());
        }

        IEnumerator GrowBananas()
        {
            isCharging = true;
            orb.gameObject.SetActive(true);
            orbBody.isKinematic = true;
            orbBody.velocity = Vector3.zero;
            GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(Random.Range(40, 56), false, 0.1f);
            Transform
                leftHand = GestureTracker.Instance.leftPalmInteractor.transform,
                rightHand = GestureTracker.Instance.rightPalmInteractor.transform;
            float diameter = 0;
            float lastHaptic = Time.time;
            float hapticDuration = .1f;
            while (GestureTracker.Instance.PalmsFacingEachOther())
            {
                float scale = Player.Instance.scale;
                if (Time.time - lastHaptic > hapticDuration)
                {
                    float strength = Mathf.SmoothStep(0, 1, diameter / maxOrbSize * scale);
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(Random.Range(40, 48), false, strength / 10f);
                    GestureTracker.Instance.leftController.SendHapticImpulse(0u, strength, hapticDuration);
                    GestureTracker.Instance.rightController.SendHapticImpulse(0u, strength, hapticDuration);
                    lastHaptic = Time.time;
                }
                diameter = Vector3.Distance(leftHand.position, rightHand.position);
                diameter = Mathf.Clamp(diameter, 0, maxOrbSize * scale);
                orb.transform.position = (leftHand.position + rightHand.position) / 2;
                orb.transform.localScale = Vector3.one * diameter * scale;
                yield return new WaitForEndOfFrame();
            }
            isCharging = false;
            Logging.Debug("Charging is done");
            float chargeTime = Time.time;
            while (Time.time - chargeTime < 1f)
            {
                if (GestureTracker.Instance.PalmsFacingSameWay())
                    break;
                yield return new WaitForEndOfFrame();
            }

            bananaLine.gameObject.SetActive(true);
            isFiring = true;
            while (GestureTracker.Instance.PalmsFacingSameWay() && HandProximity() < .6f)
            {
                if (Time.time - lastHaptic > hapticDuration)
                {
                    float strength = 1;
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(Random.Range(40, 56), false, strength / 10f);
                    GestureTracker.Instance.leftController.SendHapticImpulse(0u, strength, hapticDuration);
                    GestureTracker.Instance.rightController.SendHapticImpulse(0u, strength, hapticDuration);
                    lastHaptic = Time.time;
                }
                float scale = Player.Instance.scale;
                diameter = Vector3.Distance(leftHand.position, rightHand.position);
                diameter = Mathf.Clamp(diameter, 0, maxOrbSize * scale * 2);
                bananaLine.startWidth = diameter * scale;
                bananaLine.endWidth = diameter * scale;
                Vector3 direction =
                    (GestureTracker.Instance.leftHandVectors.palmNormal +
                    GestureTracker.Instance.rightHandVectors.palmNormal) / 2;
                Vector3 start = (leftHand.position + rightHand.position) / 2 + direction * .1f;
                orb.position = start;
                orb.transform.localScale = Vector3.one * diameter * scale;
                bananaLine.SetPosition(0, start);
                bananaLine.SetPosition(1, start + direction * 100f);
                Player.Instance.AddForce(direction * -40 * diameter * Time.fixedDeltaTime);
                yield return new WaitForEndOfFrame();
            }
            Logging.Debug("Firing is done");
            orb.gameObject.SetActive(false);
            bananaLine.gameObject.SetActive(false);
            isFiring = false;
        }

        float HandProximity()
        {
            return Vector3.Distance(
                GestureTracker.Instance.leftPalmInteractor.transform.position,
                GestureTracker.Instance.rightPalmInteractor.transform.position
            );
        }

        void FixedUpdate()
        {
        }


        protected override void Cleanup()
        {
            orb?.gameObject?.Obliterate();
            bananaLine?.gameObject?.Obliterate();
        }

        public override string GetDisplayName()
        {
            return DisplayName;
        }

        public override string Tutorial()
        {
            return "Copy the Show! \n To Be Networked";
        }

    }
}
