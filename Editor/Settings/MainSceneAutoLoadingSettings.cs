using SaG.MainSceneAutoLoading.MainSceneLoadedHandlers;
using SaG.MainSceneAutoLoading.MainSceneProviders;
using SaG.MainSceneAutoLoading.PlaymodeExitedHandlers;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SaG.MainSceneAutoLoading.Settings
{
    public sealed class MainSceneAutoLoadingSettings : ScriptableObject
    {
        private const string AssetPath = "ProjectSettings/MainSceneAutoLoadingSettings.asset";

        public bool Enabled = true;

        [SerializeReference]
        internal IMainSceneProvider _mainSceneProvider = new FirstSceneInBuildSettings();

        [SerializeReference]
        internal IMainSceneLoadedHandler _mainSceneLoadedHandler = new LoadAllLoadedScenes();

        [SerializeReference]
        internal IPlaymodeExitedHandler _playmodeExitedHandler = new RestoreSceneManagerSetup();

        internal IMainSceneProvider GetMainSceneProvider()
        {
            return _mainSceneProvider;
        }

        internal IMainSceneLoadedHandler GetLoadMainSceneHandler()
        {
            return _mainSceneLoadedHandler;
        }

        internal IPlaymodeExitedHandler GetPlaymodeExitedHandler()
        {
            return _playmodeExitedHandler;
        }

        internal static MainSceneAutoLoadingSettings GetOrCreate()
        {
            if (TryLoadAsset(out var settings))
            {
                return settings;
            }

            settings = CreateInstance<MainSceneAutoLoadingSettings>();
            settings.Save();

            return settings;
        }

        internal void Save()
        {
            UnityEditorInternal.InternalEditorUtility.SaveToSerializedFileAndForget(new Object[]
            {
                this
            }, AssetPath, true);
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreate());
        }

        internal static bool TryLoadAsset(out MainSceneAutoLoadingSettings settings)
        {
            settings = AssetDatabase.LoadAssetAtPath<MainSceneAutoLoadingSettings>(AssetPath);
            if (settings != null)
                return true;

            var objects =
                UnityEditorInternal.InternalEditorUtility.LoadSerializedFileAndForget(AssetPath);
            if (objects is { Length: > 0 })
            {
                settings = objects[0] as MainSceneAutoLoadingSettings;
            }

            return settings != null;
        }
    }
}
