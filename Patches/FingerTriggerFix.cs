using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using UnityEngine;

namespace Grate.Patches
{
    [HarmonyPatch(typeof(GorillaTriggerColliderHandIndicator))]
    [HarmonyPatch("Update", MethodType.Normal)]
    class FingerTriggerFix
    {
        private static void Postfix(GorillaTriggerColliderHandIndicator __instance)
        {
            if (__instance.name.ToUpper().Contains("HAND"))
            {
                if (__instance.GetComponent<TransformFollow>() != null)
                {
                    __instance.transform.SetParent(__instance.GetComponent<TransformFollow>().transformToFollow);
                }
                if (__instance.transform.parent == __instance.GetComponent<TransformFollow>().transformToFollow)
                {
                    __instance.GetComponent<TransformFollow>().enabled = false;
                    __instance.transform.localPosition = Vector3.zero;
                }
            }
        }
    }
}
