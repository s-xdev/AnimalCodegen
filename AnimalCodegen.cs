using System;
using MelonLoader;
using HarmonyLib;
using AnimalCompany;
using AnimalCodegen.Patches;
using AnimalCodegen.Logging;
using ExitGames.Client.Photon.StructWrapping;

namespace AnimalCodegen
{
    public class AnimalCodegen : MelonMod
    {
        public override void OnInitializeMelon()
        {
            WebhookLogger.WebhookUrl = ""; // webhook url

            _ = WebhookLogger.SendMessage("Hello from Animal Codegen!");
            _ = WebhookLogger.SendMessage("Applying Harmony Patches...");
            HarmonyPatches.ApplyHarmonyPatches();
            _ = WebhookLogger.SendMessage("Removed harmony patches.");


            
        }

        public override void OnApplicationQuit()
        {
            _ = WebhookLogger.SendMessage("Disabling AnimalCodegen Harmony Patches...");
            HarmonyPatches.RemoveHarmonyPatches();
            _ = WebhookLogger.SendMessage("Removed harmony patches.");
        }
    }
}
