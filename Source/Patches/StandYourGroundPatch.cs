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
             bool pawn_faction_is_player = pawn.Faction != null && pawn.Faction.IsPlayer;
             bool pawn_host_faction_is_player = pawn.HostFaction != null && pawn.HostFaction.IsPlayer;
    
             if ((pawn_faction_is_player || pawn_host_faction_is_player || pawn.IsOnHoldingPlatform) && pawn.playerSettings == null)
             {
                 pawn.playerSettings = new Pawn_PlayerSettings(pawn);
                 
                 string hostilityResponse = (SettingsManager.GetSetting("protobeard.madagascarvanilla", "hostilityResponse"));
                 pawn.playerSettings.hostilityResponse = (HostilityResponseMode) Enum.Parse(typeof(HostilityResponseMode), hostilityResponse);
             }
        }
    }
}