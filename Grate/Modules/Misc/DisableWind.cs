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
        public override string GetDisplayName()
        {
            return "Disable Wind";
        }

        public override string Tutorial()
        {
            return "No more annoying wind barrier";
        }

        protected override void OnEnable()
        {
            FindObjectOfType<GorillaSurfaceOverride>().enabled = false;
        }

        protected override void OnDisable()
        {
            FindObjectOfType<GorillaSurfaceOverride>().enabled = true;
        }

        protected override void Cleanup()
        {
            throw new NotImplementedException();
        }
    }
}
