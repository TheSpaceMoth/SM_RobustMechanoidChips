using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace RobustMechanoidChips
{
    [StaticConstructorOnStartup]
    static public class HarmonyPatches
    {
        public static Harmony harmonyInstance;


        static HarmonyPatches()
        {
            harmonyInstance = new Harmony("rimworld.rwmods.RobustMechanoidChips");
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
        }

    }


    [HarmonyPatch(typeof(Thing))]
    [HarmonyPatch("ButcherProducts")]
    internal static class ButcherProductsTweak
    {
        public static bool Prefix(ref Thing __instance, ref IEnumerable<Thing> __result, Pawn butcher, float efficiency)
        {
            bool RetVal = true;

            if (__instance.def.butcherProducts != null)
            {
                List<Thing> RetEnum = new List<Thing>();

                for (int i = 0; i < __instance.def.butcherProducts.Count; i++)
                {
                    ThingDefCountClass butcherProduct = __instance.def.butcherProducts[i];

                    Thing butcherThing = ThingMaker.MakeThing(butcherProduct.thingDef);

                    if ((butcherProduct.thingDef.defName == "SignalChip") ||
                        (butcherProduct.thingDef.defName == "PowerfocusChip") ||
                        (butcherProduct.thingDef.defName == "NanostructuringChip"))
                    {
                        RetVal = false; // We dont want to run the old code

                        butcherThing.stackCount = butcherProduct.count; // Do not reduce based on effeciency for these items.
                        RetEnum.Add(butcherThing);
                    }
                    else
                    {
                        int num = GenMath.RoundRandom((float)butcherProduct.count * efficiency);
                        if (num > 0)
                        {
                            butcherThing.stackCount = num;
                            RetEnum.Add(butcherThing);
                        }
                    }
                }

                // Only set the return if we are not going to call the base function.
                if (RetVal == false)
                    __result = RetEnum;

            }

            return RetVal;
        }
    }

    /*
    [HarmonyPatch(typeof(CompProperties_Useable_CallBossgroup))]
    [HarmonyPatch("SendBossgroupDetailsLetter")]
    internal static class NotifyBossDeadTweak
    {
        public static bool Prefix(ref CompProperties_Useable_CallBossgroup __instance, string labelKey, string textKey, ThingDef parent)
        {
            List<ThingDef> hyperlinkThingDefs = new List<ThingDef>()
              {
                parent
              };


            //hyperlinkThingDefs.AddRange(this.bossgroupDef.boss.kindDef.race.killedLeavingsPlayerHostile.Select<ThingDefCountClass, ThingDef>((Func<ThingDefCountClass, ThingDef>)(t => t.thingDef)));

            Find.LetterStack.ReceiveLetter((TaggedString)__instance.FormatLetterLabel(labelKey), (TaggedString)__instance.FormatLetterText(textKey, parent), LetterDefOf.NeutralEvent, (LookTargets)null, hyperlinkThingDefs: hyperlinkThingDefs);


            return false; // Replace old function
        }
    }*/

}
