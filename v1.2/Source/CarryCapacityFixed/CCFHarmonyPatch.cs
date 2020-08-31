using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;
using UnityEngine;
using OpCodes = System.Reflection.Emit.OpCodes;
using System.Text;

namespace CarryCapacityFixed
{
    [StaticConstructorOnStartup]
    internal static class CCFHarmonyPatch
    {
        static CCFHarmonyPatch()
        {
            var harmonyCCF = new Harmony("smashphil.ccfbutbetter.rimworld");
            //Harmony.DEBUG = true;

            harmonyCCF.Patch(original: AccessTools.Method(typeof(MassUtility), nameof(MassUtility.Capacity)),
                prefix: new HarmonyMethod(typeof(CCFHarmonyPatch), nameof(CarryCapacityChange)));
        }

        public static bool CarryCapacityChange(Pawn p, ref float __result, StringBuilder explanation = null)
        {
            if (!p.ContainedWithinDoNotApplyList())
            {
                if (!MassUtility.CanEverCarryAnything(p))
			    {
                    __result = 0f;
                    return false;
			    }
                float num = p.BodySize * p.GetStatValue(StatDefOf.CarryingCapacity);
			    if (explanation != null)
			    {
				    if (explanation.Length > 0)
				    {
					    explanation.AppendLine();
				    }
				    explanation.Append("  - " + p.LabelShortCap + ": " + num.ToStringMassOffset());
			    }
                __result = num;
                return false;
            }
            return true;
        }

        public static bool ContainedWithinDoNotApplyList(this Pawn p) => p?.def?.HasModExtension<DoNotApply_ModExtension>() ?? true;
    }
}
