using System;
using System.Linq;
using Photon.Pun;
using UnityEngine;
namespace Grate.Networking
{
    class RoomSpecific : MonoBehaviour
    {
        public NetPlayer? Owner;

        void FixedUpdate()
        {
            if (!NetworkSystem.Instance.InRoom)
            {
                Destroy(gameObject);
            }
            if (Owner != null)
            {
                if (!NetworkSystem.Instance.AllNetPlayers.Contains(Owner))
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
