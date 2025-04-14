using Grate.GUI;
using GorillaNetworking;

namespace Grate.Modules.Misc
{
    public class Lobby : GrateModule
    {

        public static readonly string DisplayName = "Join Grate Code";

        protected override void OnEnable()
        {
            if (!MenuController.Instance.Built) return;
            base.OnEnable();
            Plugin.Instance.JoinLobby("GRATE_MOD");
            this.enabled = false;
        }
        public override string GetDisplayName()
        {
            return DisplayName;
        }

        public override string Tutorial()
        {
            return "Joins the Grate Mod code";
        }

        protected override void Cleanup() { }   
    }
}
