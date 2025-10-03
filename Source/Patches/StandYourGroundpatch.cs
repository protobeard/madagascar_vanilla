using RimWorld;
using Verse;
using HarmonyLib;

namespace MadagascarVanilla.Patches
{
    
    [HarmonyPatch(typeof(PawnComponentsUtility))]
    [HarmonyPatch(nameof(PawnComponentsUtility.AddAndRemoveDynamicComponents))]
    public static class StandYourGroundPatch
    {
        // by performing the same checks as AddAndRemoveDynamicComponents does, instead of it
        // creating the playersettings when they are null, we should be doing so (and thus preventing it from ever doing so)
        // we can then change the hostilityResponse (or other values "set" in the constructor) without having to actually
        // patch the constructor.
        public static void Prefix(ref Pawn pawn)
        {
            // flags on original method
            //bool flag1 = pawn.Faction != null && pawn.Faction.IsPlayer;
            //bool flag2 = pawn.HostFaction != null && pawn.HostFaction.IsPlayer;
            
            // later
            
            // if ((flag1 | flag2 || pawn.IsOnHoldingPlatform) && pawn.playerSettings == null)
            //    pawn.playerSettings = new Pawn_PlayerSettings(pawn);
            
            
            //FileLog.Log("syg: prefix response enter");
    
             var original = typeof(PawnComponentsUtility).GetMethod("AddAndRemoveDynamicComponents");
    
             bool pawn_faction_is_player = pawn.Faction != null && pawn.Faction.IsPlayer;
             bool pawn_host_faction_is_player = pawn.HostFaction != null && pawn.HostFaction.IsPlayer;
    
             if ((pawn_faction_is_player || pawn_host_faction_is_player || pawn.IsOnHoldingPlatform) && pawn.playerSettings == null)
             {
                 //FileLog.Log("syg: creating playersettings");
                 pawn.playerSettings = new Pawn_PlayerSettings(pawn);
    
                 //FileLog.Log("syg: changing hostilityResponse to attack");
                 // FIXME: use the setting instead of changing it to Attack. 
                 pawn.playerSettings.hostilityResponse = HostilityResponseMode.Attack;
                 
                 //pawn.playerSettings.hostilityResponse = MadagascarVanillaSettings.default_hostility_response;
             }
        }
    }
}