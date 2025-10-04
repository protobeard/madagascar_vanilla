using System;
using RimWorld;
using Verse;
using HarmonyLib;
using XmlExtensions;

namespace MadagascarVanilla.Patches
{
    
    [HarmonyPatch(typeof(PawnComponentsUtility))]
    [HarmonyPatch(nameof(PawnComponentsUtility.AddAndRemoveDynamicComponents))]
    public static class StandYourGroundPatch
    {
        private const string HostilityRewardsPreference = "hostilityResponse";
        // by performing the same checks as AddAndRemoveDynamicComponents does, instead of it
        // creating the PlayerSettings when they are null, we should be doing so (and thus preventing it from ever doing so)
        // we can then change the hostilityResponse (or other values "set" in the constructor) without having to actually
        // patch the constructor.
        //
        // flags on original method
        //bool flag1 = pawn.Faction != null && pawn.Faction.IsPlayer;
        //bool flag2 = pawn.HostFaction != null && pawn.HostFaction.IsPlayer;
        // ...
        // later
        // ...
        // if ((flag1 | flag2 || pawn.IsOnHoldingPlatform) && pawn.playerSettings == null)
        //    pawn.playerSettings = new Pawn_PlayerSettings(pawn);
        public static void Prefix(ref Pawn pawn)
        {
             bool pawnFactionIsPlayer = pawn.Faction != null && pawn.Faction.IsPlayer;
             bool pawnHostFactionIsPlayer = pawn.HostFaction != null && pawn.HostFaction.IsPlayer;
    
             if ((pawnFactionIsPlayer || pawnHostFactionIsPlayer || pawn.IsOnHoldingPlatform) && pawn.playerSettings == null)
             {
                 pawn.playerSettings = new Pawn_PlayerSettings(pawn);
                 
                 string hostilityResponse = (SettingsManager.GetSetting(MadagascarVanillaMod.modId, HostilityRewardsPreference));
                 pawn.playerSettings.hostilityResponse = (HostilityResponseMode) Enum.Parse(typeof(HostilityResponseMode), hostilityResponse);
             }
        }
    }
}