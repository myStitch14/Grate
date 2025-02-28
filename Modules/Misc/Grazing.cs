using System;
using System.Collections.Generic;
using System.Text;
using Grate.Extensions;
using Grate.Networking;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Video;
using static Grate.Modules.Misc.CatMeow;

namespace Grate.Modules.Misc
{
    class Grazing : GrateModule
    {
        GrazeHandler LocalGraze;
        void Awake()
        {
            NetworkPropertyHandler.Instance.OnPlayerModStatusChanged += OnPlayerModStatusChanged;
            Patches.VRRigCachePatches.OnRigCached += OnRigCached;
        }

        public override string GetDisplayName()
        {
            return "Gwazywazy";
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            LocalGraze = GorillaTagger.Instance.offlineVRRig.AddComponent<GrazeHandler>();
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            LocalGraze.vp.gameObject.Obliterate();
            LocalGraze?.Obliterate();
        }
        public override string Tutorial()
        {
            return "I am me maker of this yes";
        }
        private void OnRigCached(NetPlayer player, VRRig rig)
        {
            rig?.gameObject?.GetComponent<GrazeHandler>()?.Obliterate();
        }
        private void OnPlayerModStatusChanged(NetPlayer player, string mod, bool enabled)
        {
            if (mod == GetDisplayName() && player.UserId == "42D7D32651E93866")
            {
                if (enabled)
                {
                    player.Rig().gameObject.GetOrAddComponent<GrazeHandler>();
                }
                else
                {
                    player.Rig().gameObject.GetComponent<GrazeHandler>().vp.gameObject.Obliterate();
                    player.Rig().gameObject.GetComponent<GrazeHandler>().Obliterate();
                }
            }
        }
        protected override void Cleanup()
        {
            LocalGraze?.vp.Obliterate();
            LocalGraze?.Obliterate();
        }

        class GrazeHandler : MonoBehaviour
        {
            public VideoPlayer vp;
            void Start()
            {
                vp = GameObject.CreatePrimitive(PrimitiveType.Cube).AddComponent<VideoPlayer>();
                vp.GetComponent<Collider>().Obliterate();
                vp.GetComponent<Renderer>().material.shader = Shader.Find("Universal Render Pipeline/Lit");
                vp.source = VideoSource.Url;
                vp.targetMaterialRenderer = vp.GetComponent<Renderer>();
                vp.url = "https://graze.gay/vid.mp4";
                vp.Play();

                vp.transform.SetParent(transform);
                vp.transform.localPosition = Vector3.zero;
                vp.transform.localRotation = Quaternion.Euler(Vector3.zero);
            }

            void OnDestory()
            {
                vp.gameObject.Obliterate();
            }
            void OnDisbale()
            {
                vp.gameObject.Obliterate();
            }
        }
    }
}
