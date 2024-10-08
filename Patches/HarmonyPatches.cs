using System;
using System.Reflection;
using HarmonyLib;

namespace AnimalCodegen.Patches
{
    public class HarmonyPatches
    {
        private static HarmonyLib.Harmony instance;

        public static bool IsPatched { get; private set; }
        public const string InstanceId = "com.adosx.AnimalCodegen";

        internal static void ApplyHarmonyPatches()
        {
            if (!IsPatched)
            {
                if (instance == null)
                {
                    instance = new HarmonyLib.Harmony(InstanceId);
                };
                instance.PatchAll(Assembly.GetExecutingAssembly());
                IsPatched = true;
            };
        }

        internal static void RemoveHarmonyPatches()
        {
            if (instance != null && IsPatched)
            {
                instance.UnpatchSelf();
                IsPatched = false;
            };
        }
    }
};
