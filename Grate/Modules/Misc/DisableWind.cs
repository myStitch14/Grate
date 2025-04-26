using System;
using System.Collections.Generic;
using System.Text;
using GorillaGameModes;
using Grate.Extensions;
using Grate.Networking;
using Grate.Patches;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Grate.Modules.Misc
{
    internal class DisableWind : GrateModule
    {
        public static bool Enabled;
        public override string GetDisplayName()
        {
            return "Disable Wind";
        }

        protected override void Start()
        {
            Enabled = true;
            OnEnable();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Enabled = true;
        }

        public override string Tutorial()
        {
            return "Disables the wind barriers";
        }

        protected override void Cleanup()
        {
            Enabled = false;
        }
    }
}
