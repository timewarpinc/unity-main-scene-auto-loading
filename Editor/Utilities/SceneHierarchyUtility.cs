// Partially from: https://github.com/sandolkakos/unity-utilities/blob/main/Scripts/Editor/SceneHierarchyUtility.cs

using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace SaG.MainSceneAutoLoading.Utilities
{
    /// <summary>
    /// Editor functionalities from internal SceneHierarchyWindow and SceneHierarchy classes. 
    /// For that we are using reflection.
    /// </summary>
    public static class SceneHierarchyUtility
    {
        [NotNull]
        private static Type SceneHierarchyWindowType => typeof(EditorWindow).Assembly
            .GetType("UnityEditor.SceneHierarchyWindow");

        /// <summary>
        /// Check if the target GameObject is expanded (aka unfolded) in the Hierarchy view.
        /// </summary>
        public static bool IsExpanded(GameObject go)
        {
            return GetExpandedGameObjects().Contains(go);
        }

        /// <summary>
        /// Get a list of all GameObjects which are expanded (aka unfolded) in the Hierarchy view.
        /// </summary>
        public static List<GameObject> GetExpandedGameObjects()
        {
            var sceneHierarchy = GetSceneHierarchy();

            if (sceneHierarchy == null) return null;

            var methodInfo = sceneHierarchy
                .GetType()
                .GetMethod("GetExpandedGameObjects");

            var result = methodInfo?.Invoke(sceneHierarchy, Array.Empty<object>());

            return (List<GameObject>) result;
        }

        /// <summary>
        /// Set the target GameObject as expanded (aka unfolded) in the Hierarchy view.
        /// </summary>
        public static void SetExpanded(GameObject go, bool expand)
        {
            var sceneHierarchy = GetSceneHierarchy();
            if (sceneHierarchy == null) return;

            var methodInfo = sceneHierarchy
                .GetType()
                .GetMethod("ExpandTreeViewItem", BindingFlags.NonPublic | BindingFlags.Instance);

            methodInfo?.Invoke(sceneHierarchy, new object[] {go.GetInstanceID(), expand});
        }

        /// <summary>
        /// Set the target GameObject and all children as expanded (aka unfolded) in the Hierarchy view.
        /// </summary>
        public static void SetExpandedRecursive(GameObject go, bool expand)
        {
            var sceneHierarchy = GetSceneHierarchy();
            if (sceneHierarchy == null) return;

            var methodInfo = sceneHierarchy
                .GetType()
                .GetMethod("SetExpandedRecursive", BindingFlags.Public | BindingFlags.Instance);

            methodInfo?.Invoke(sceneHierarchy, new object[] {go.GetInstanceID(), expand});
        }

        private static object GetSceneHierarchy()
        {
            var window = GetHierarchyWindow();

            if (window == null)
            {
                Debug.LogWarning("Could not get scene hierarchy window.");
                return null;
            }

            return SceneHierarchyWindowType
                .GetProperty("sceneHierarchy")?.GetValue(window);
        }

        private static EditorWindow GetHierarchyWindow()
        {
            return EditorWindow.GetWindow(SceneHierarchyWindowType);
        }

        public static List<string> GetExpandedSceneNames()
        {
            var sceneHierarchy = GetSceneHierarchy();
            if (sceneHierarchy == null) return null;

            var methodInfo = sceneHierarchy
                .GetType()
                .GetMethod("GetExpandedSceneNames", BindingFlags.NonPublic | BindingFlags.Instance);

            var result = methodInfo?.Invoke(sceneHierarchy, Array.Empty<object>());

            return (List<string>) result;
        }

        public static void SetScenesExpanded(List<string> sceneNames)
        {
            var sceneHierarchy = GetSceneHierarchy() ;
            if (sceneHierarchy == null) return;

            var methodInfo = sceneHierarchy
                .GetType()
                .GetMethod("SetScenesExpanded", BindingFlags.NonPublic | BindingFlags.Instance);

            methodInfo?.Invoke(sceneHierarchy, new object[] {sceneNames});
        }
    }
}
