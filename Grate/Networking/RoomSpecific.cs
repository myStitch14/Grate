using System;
using System.Linq;
using Photon.Pun;
namespace Grate.Networking
{
    class RoomSpecific : MonoBehaviourPunCallbacks
    {
        public NetPlayer Owner;

        void FixedUpdate()
        {
            if (!NetworkSystem.Instance.InRoom)
            {
                Destroy(gameObject);
            }
            if (!NetworkSystem.Instance.AllNetPlayers.Contains(Owner)) 
            {
                Destroy(gameObject);
            }
        }
    }
}
