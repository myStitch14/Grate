using Fusion;
using GorillaLocomotion;
using Grate.Extensions;
using Grate.GUI;
using Grate.Networking;
using Grate.Tools;
using System;
using System.Collections.Generic;
using System.Text;
using Grate.Gestures;
using UnityEngine;
using UnityEngine.XR;

namespace Grate.Modules.Misc
{
    class CatMeow : GrateModule
    {
        private static List<AudioClip> meowSounds = new List<AudioClip>();
        private static GameObject meowerPrefab;
        static System.Random rnd = new System.Random();
        private VRRig rig;
        private GameObject meowbox;
        private ParticleSystem meowParticles;
        private AudioSource meowAudio;
        private InputTracker inputL = GestureTracker.Instance.GetInputTracker("grip", XRNode.LeftHand);
        private InputTracker inputR = GestureTracker.Instance.GetInputTracker("grip", XRNode.RightHand);
        
        public override string GetDisplayName()
        {
            return "Meow";
        }

        public override string Tutorial()
        {
            return "Mrrrrpp....";
        }

        protected override void Cleanup()
        {
            GripOff();
        }
        
        protected override void OnDisable()
        {
            GripOff();
        }

        void Awake()
        {   
            NetworkPropertyHandler.Instance.OnPlayerModStatusChanged += OnPlayerModStatusChanged;
            Patches.VRRigCachePatches.OnRigCached += OnRigCached;
        }

        void Start()
        {
            rig = GorillaTagger.Instance.offlineVRRig;
            meowerPrefab = Plugin.grateExtrasBundle.LoadAsset<GameObject>("ParticleEmitter");
            meowbox = Instantiate(meowerPrefab, rig.gameObject.transform);
            meowbox.transform.localPosition = Vector3.zero;
            meowParticles = meowbox.GetComponent<ParticleSystem>();
            meowAudio = meowbox.GetComponent<AudioSource>();
                        
            meowSounds.Add(Plugin.grateExtrasBundle.LoadAsset<AudioClip>("meow1"));
            meowSounds.Add(Plugin.grateExtrasBundle.LoadAsset<AudioClip>("meow2"));
            meowSounds.Add(Plugin.grateExtrasBundle.LoadAsset<AudioClip>("meow3"));
            meowSounds.Add(Plugin.grateExtrasBundle.LoadAsset<AudioClip>("meow4"));
        }

        void OnLocalGrip(InputTracker _) => DoMeow(meowParticles, meowAudio);

        void GripOn()
        {
            inputL.OnPressed += OnLocalGrip;
            inputR.OnPressed += OnLocalGrip;
        }

        void GripOff()
        {
            inputL.OnPressed -= OnLocalGrip;
            inputR.OnPressed -= OnLocalGrip;
        }
        
        private void OnRigCached(NetPlayer player, VRRig rig)
        {
            rig?.gameObject?.GetComponent<TheMeower>()?.Obliterate();
        }

        private void OnPlayerModStatusChanged(NetPlayer player, string mod, bool enabled)
        {
            if (mod == GetDisplayName() && player.UserId == "FBE3EE50747CB892")
            {
                if (enabled)
                {
                    player.Rig().gameObject.GetOrAddComponent<TheMeower>();
                }
                else
                {
                    Destroy(player.Rig().gameObject.GetComponent<TheMeower>());
                }
            }
        }

        protected override void OnEnable()
        {
            if (!MenuController.Instance.Built) return;
            base.OnEnable();
            GripOn();
        }

        static void DoMeow(ParticleSystem meowParticles, AudioSource meowAudioSource)
        {
            meowAudioSource.PlayOneShot(meowSounds[rnd.Next(meowSounds.Count)]);
            meowParticles.Play();
            meowParticles.Emit(1);
        }
        
        class TheMeower : MonoBehaviour
        {
            VRRig rigNet;
            private NetworkedPlayer netPlayer;
            private GameObject meowboxNet;
            private ParticleSystem meowParticlesNet;
            private AudioSource meowAudioNet;

            void Start()
            {
                rigNet = GetComponent<VRRig>();
                netPlayer = rigNet.GetComponent<NetworkedPlayer>();
                meowboxNet = Instantiate(meowerPrefab, rigNet.gameObject.transform);
                meowboxNet.transform.localPosition = Vector3.zero;
                meowParticlesNet = meowboxNet.GetComponent<ParticleSystem>();
                meowAudioNet = meowboxNet.GetComponent<AudioSource>();
                
                netPlayer.OnGripPressed += DoMeowNetworked;
            }

            void DoMeowNetworked(NetworkedPlayer player, bool isLeft)
            {
                DoMeow(meowParticlesNet, meowAudioNet);
            }

            void OnDestroy()
            {
                netPlayer.OnGripPressed -= DoMeowNetworked;
            }
        }
    }
}
