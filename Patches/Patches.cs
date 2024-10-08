using System;
using MelonLoader;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using TMPro;
using System.Reflection;
using Mono.Security;
using AnimalCodegen.Logging;
using System.Security.Cryptography;
using System.Runtime.CompilerServices;

namespace AnimalCodegen.Patches
{
    [HarmonyPatch(typeof(UnityEngine.AndroidJNIHelper))]
    [HarmonyPatch("GetSignature")]
    public class JNISignaturePatch
    {
        public static void Postfix(ref string __signature)
        {
            _ = WebhookLogger.SendMessage($"AndroidJNIHelper.GetSignature -> {__signature} | Overriding original value to null...");
            __signature = String.Empty;

            var StackTrace = new System.Diagnostics.StackTrace();
            var CallingMethod = StackTrace.GetFrame(2)?.GetMethod();

            if (CallingMethod != null) {
                _ = WebhookLogger.SendMessage($"Android.JNIHelper.GetSignature:Caller -> {CallingMethod.DeclaringType?.FullName}.{CallingMethod.Name}");
            };

            var ForfeitSignatureCheck = true; // if true exit process if signature checks are called, useful to prevent any possible bans and be able to inspect the process signature check
            if (ForfeitSignatureCheck)
            {
                AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer")
                    .GetStatic<AndroidJavaObject>("currentActivity");
                activity.Call("finish");
                Application.Quit();
            }
        }
    }

    [HarmonyPatch(typeof(AnimalCompany.Terminal))]
    [HarmonyPatch("UpdateText")]
    public class TerminalUpdatePatch
    {
        [HarmonyPrefix]
        public static void Prefix(AnimalCompany.Terminal __instance)
        {
            // variables
            Type TerminalType = typeof(AnimalCompany.Terminal);

            if (__instance != null)
            {
                __instance.pvpEnabledText.text = "PVP MODDED | AnimalCodegen"; // easy way to see if dlls are loaded and game is modded w/ harmony
            };
        }
    }

    [HarmonyPatch(typeof(AppDomain))]
    [HarmonyPatch("GetAssemblies")]
    public class AppDomainAssemblyPatch
    {
        [HarmonyPostfix]
        public static void Postfix(ref Assembly[] __assemblies)
        {
            // assembly names based off of KSHR anti, not real testing yet.
            string[] hidden_assemblies =
            {
                "melon",
                "lemon",
                "harmony",
                "devx", // ???
                "melon loader",
                "melonloader",
                "lemonloader",
                "lemon loader",
                "harmony patches",
            };

            // print assemblies to webhook for debugging
            foreach (Assembly assembly in __assemblies)
            {
                _ = WebhookLogger.SendMessage($"AppDomain.GetAssemblies -> {assembly.FullName}");
            }

            // filter assemblies, improved by chatgpt :thumbs_up:
            __assemblies = __assemblies.Where(a =>
                !(hidden_assemblies.Contains(a.FullName.ToLower()) ||
                hidden_assemblies.Contains(a.FullName.ToUpper()) ||
                hidden_assemblies.Contains(a.FullName))
            ).ToArray();

            // stack trace
            var StackTrace = new System.Diagnostics.StackTrace();
            var CallingMethod = StackTrace.GetFrame(2)?.GetMethod();

            if (CallingMethod != null && CallingMethod.DeclaringType?.FullName != null || CallingMethod.Name != null)
            {
                _ = WebhookLogger.SendMessage($"AppDomain.CurrentDomain.GetAssemblies:Caller -> {CallingMethod.DeclaringType?.FullName}.{CallingMethod.Name}");
            };

            // temporary
            AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer")
                    .GetStatic<AndroidJavaObject>("currentActivity");
            activity.Call("finish");
            Application.Quit();
        }
    }

    [HarmonyPatch(typeof(System.Security.Cryptography.X509Certificates.X509Certificate2))]
    [HarmonyPatch("Verify")]
    public class SystemX509Cert2Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ref bool __valid)
        {
            __valid = true; // make sure signatures always valid

            // stack trace for debugging
            var stackTrace = new System.Diagnostics.StackTrace();
            var CallingMethod = stackTrace.GetFrame(2)?.GetMethod();
            if (CallingMethod != null && CallingMethod.DeclaringType?.FullName != null || CallingMethod.Name != null)
            {
                _ = WebhookLogger.SendMessage($"System.Security...X509Certificate2.Verify:Caller -> {CallingMethod.DeclaringType?.FullName}.{CallingMethod.Name}");
            };
            // do not end process as it is forced to return the signature as valid, allowing other debugging processes to run
        }
    };

    [HarmonyPatch(typeof(Mono.Security.X509.X509Certificate))]
    public class MonoSecurityX509Patch
    {
        [HarmonyPatch(typeof(Mono.Security.X509.X509Certificate), "VerifySignature", new Type[] { typeof(DSA) })]
        [HarmonyPostfix]
        public static void VerifySignaturePostfixDSA(DSA dsa, ref bool __result, Mono.Security.X509.X509Certificate __instance)
        {
            __result = true;
            // stack trace for debugging
            var stackTrace = new System.Diagnostics.StackTrace();
            var CallingMethod = stackTrace.GetFrame(2)?.GetMethod();
            if (CallingMethod != null && CallingMethod.DeclaringType?.FullName != null || CallingMethod.Name != null)
            {
                _ = WebhookLogger.SendMessage($"Mono.Security...X509Certificate.VerifySignature[1]:Caller -> {CallingMethod.DeclaringType?.FullName}.{CallingMethod.Name}");
            };
        }

        [HarmonyPatch(typeof(Mono.Security.X509.X509Certificate), "VerifySignature", new Type[] { typeof(RSA) })]
        [HarmonyPostfix]
        public static void VerifySignaturePostfixRSA(RSA rsa, ref bool __result, Mono.Security.X509.X509Certificate __instance)
        {
            __result = true;
            // stack trace for debugging
            var stackTrace = new System.Diagnostics.StackTrace();
            var CallingMethod = stackTrace.GetFrame(2)?.GetMethod();
            if (CallingMethod != null && CallingMethod.DeclaringType?.FullName != null || CallingMethod.Name != null)
            {
                _ = WebhookLogger.SendMessage($"Mono.Security...X509Certificate.VerifySignature[2]:Caller -> {CallingMethod.DeclaringType?.FullName}.{CallingMethod.Name}");
            };
        }
        [HarmonyPatch(typeof(Mono.Security.X509.X509Certificate), "VerifySignature", new Type[] { typeof(AsymmetricAlgorithm) })]
        [HarmonyPostfix]
        public static void VerifySignaturePostfixAA(AsymmetricAlgorithm aa, ref bool __result, Mono.Security.X509.X509Certificate __instance)
        {
            __result = true;
            // stack trace for debugging
            var stackTrace = new System.Diagnostics.StackTrace();
            var CallingMethod = stackTrace.GetFrame(2)?.GetMethod();
            if (CallingMethod != null && CallingMethod.DeclaringType?.FullName != null || CallingMethod.Name != null)
            {
                _ = WebhookLogger.SendMessage($"Mono.Security...X509Certificate.VerifySignature[3]:Caller -> {CallingMethod.DeclaringType?.FullName}.{CallingMethod.Name}");
            };
        }
    }

    [HarmonyPatch(typeof(Directory))]
    [HarmonyPatch("Exists")]
    public class HiddenDirectoryPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(string path, ref bool __result)
        {
            string[] hidden_dirs =
            {
                "melon",
                "lemon",
                "harmony",
                "devx", // ???
                "melon loader",
                "melonloader",
                "lemonloader",
                "lemon loader",
                "harmony patches",
            };

            foreach (string hidden_directory_path in hidden_dirs)
            {
                if (path.ToLower().Contains(hidden_directory_path.ToLower()))
                {
                    __result = Directory.Exists(Application.persistentDataPath); // simulate success
                    return false; // Skip the original method
                }
            };
            return true;
        }
    }



}
