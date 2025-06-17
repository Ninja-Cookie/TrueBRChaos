using System;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System.IO;

namespace TrueBRChaos
{
    internal class CrewBoomHook : MonoBehaviour
    {
        public static BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static;

        private const string _CrewBoomName = "CrewBoom";

        private static bool _CheckedForCrewBoom = false;
        private static bool _CrewBoomInstalled = false;
        public  static bool CrewBoomInstalled
        {
            get
            {
                bool crewBoomInstalled = !_CheckedForCrewBoom && !_CrewBoomInstalled ? _CrewBoomInstalled = FindObjectsOfType<BepInEx.BaseUnityPlugin>().Any(x => x.Info.Metadata.GUID == _CrewBoomName) : _CrewBoomInstalled;
                _CheckedForCrewBoom = true;

                return crewBoomInstalled;
            }
        }

        private static BepInEx.BaseUnityPlugin _CrewBoomPlugin = null;
        public  static BepInEx.BaseUnityPlugin CrewBoomPlugin
        {
            get
            {
                return CrewBoomInstalled && _CrewBoomPlugin == null ? _CrewBoomPlugin = FindObjectsOfType<BepInEx.BaseUnityPlugin>().FirstOrDefault(x => x.Info.Metadata.GUID == _CrewBoomName) : _CrewBoomPlugin;
            }
        }

        private static Type _CrewBoom_CharacterDatabase = null;
        public  static Type CrewBoom_CharacterDatabase
        {
            get
            {
                if (CrewBoomInstalled && _CrewBoom_CharacterDatabase == null)
                {
                    if (BepInEx.Utility.TryParseAssemblyName(CrewBoomPlugin.Info.Metadata.Name, out AssemblyName assemblyName) && BepInEx.Utility.TryResolveDllAssembly(assemblyName, Path.GetDirectoryName(CrewBoomPlugin.Info.Location), out Assembly assembly))
                    {
                        return _CrewBoom_CharacterDatabase = Type.GetType($"{_CrewBoomName}.CharacterDatabase, {assembly}");
                    }
                }
                return _CrewBoom_CharacterDatabase;
            }
        }

        private static int _NewCharacterCount = 0;
        public  static int NewCharacterCount
        {
            get
            {
                return _NewCharacterCount == 0 && CrewBoom_CharacterDatabase != null ? _NewCharacterCount = (int)CrewBoom_CharacterDatabase.GetProperty("NewCharacterCount", flags).GetValue(null, null) : _NewCharacterCount;
            }
        }
    }
}
