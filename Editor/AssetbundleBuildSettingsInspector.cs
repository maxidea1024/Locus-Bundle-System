using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Linq;
using UnityEditor.Build.Pipeline.Utilities;

namespace BundleSystem
{
    [DisallowMultipleComponent]
    [CustomEditor(typeof(AssetbundleBuildSettings))]
    public class AssetbundleBuildSettingsInspector : Editor
    {
        SerializedProperty settingsProperty_;
        SerializedProperty remoteOutputPath_;
        SerializedProperty localOutputPath_;
        SerializedProperty emulateBundle_;
        SerializedProperty emulateUseRemoteFolder_;
        SerializedProperty cleanCache_;
        SerializedProperty remoteUrl_;
        ReorderableList list;

        SerializedProperty forceRebuild_;
        SerializedProperty useCacheServer_;
        SerializedProperty cacheServerHost_;
        SerializedProperty cacheServerPort_;

        SerializedProperty useFtp_;
        SerializedProperty ftpHost_;
        SerializedProperty ftpUser_;
        SerializedProperty ftpPass_;

        // Add menu named "My Window" to the Window menu
        [MenuItem("Window/Asset Management/Select Active Assetbundle Build Settings")]
        static void SelectActiveSettings()
        {
            Selection.activeObject = AssetbundleBuildSettings.EditorInstance;
        }

        private void OnEnable()
        {
            settingsProperty_ = serializedObject.FindProperty("BundleSettings");
            remoteOutputPath_ = serializedObject.FindProperty("m_RemoteOutputFolder");
            localOutputPath_ = serializedObject.FindProperty("m_LocalOutputFolder");
            emulateBundle_ = serializedObject.FindProperty("EmulateInEditor");
            emulateUseRemoteFolder_ = serializedObject.FindProperty("EmulateWithoutRemoteURL");
            cleanCache_ = serializedObject.FindProperty("CleanCacheInEditor");
            remoteUrl_ = serializedObject.FindProperty("RemoteURL");

            forceRebuild_ = serializedObject.FindProperty("ForceRebuild");
            useCacheServer_ = serializedObject.FindProperty("UseCacheServer");
            cacheServerHost_ = serializedObject.FindProperty("CacheServerHost");
            cacheServerPort_ = serializedObject.FindProperty("CacheServerPort");

            useFtp_ = serializedObject.FindProperty("UseFtp");
            ftpHost_ = serializedObject.FindProperty("FtpHost");
            ftpUser_ = serializedObject.FindProperty("FtpUserName");
            ftpPass_ = serializedObject.FindProperty("FtpUserPass");

            var settings = target as AssetbundleBuildSettings;

            list = new ReorderableList(serializedObject, settingsProperty_, true, true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "Bundle List");
                },

                elementHeightCallback = index =>
                {
                    var element = settingsProperty_.GetArrayElementAtIndex(index);
                    return EditorGUI.GetPropertyHeight(element, element.isExpanded);
                },

                drawElementCallback = (rect, index, a, h) =>
                {
                    // get outer element
                    var element = settingsProperty_.GetArrayElementAtIndex(index);
                    rect.xMin += 10;
                    EditorGUI.PropertyField(rect, element, new GUIContent(settings.BundleSettings[index].BundleName), element.isExpanded);
                }
            };
        }

        public override void OnInspectorGUI()
        {
            var settings = target as AssetbundleBuildSettings;

            list.DoLayoutList();
            EditorGUILayout.PropertyField(remoteOutputPath_);
            EditorGUILayout.PropertyField(localOutputPath_);
            EditorGUILayout.PropertyField(remoteUrl_);
            EditorGUILayout.Space(10);
            EditorGUILayout.PropertyField(emulateBundle_);
            EditorGUILayout.PropertyField(emulateUseRemoteFolder_);
            EditorGUILayout.PropertyField(cleanCache_);
            EditorGUILayout.PropertyField(forceRebuild_);
            EditorGUILayout.Space(10);
            useCacheServer_.boolValue = EditorGUILayout.BeginToggleGroup("Cache Server", useCacheServer_.boolValue);
            EditorGUILayout.PropertyField(cacheServerHost_);
            EditorGUILayout.PropertyField(cacheServerPort_);
            EditorGUILayout.EndToggleGroup();

            useFtp_.boolValue = EditorGUILayout.BeginToggleGroup("Ftp", useFtp_.boolValue);
            EditorGUILayout.PropertyField(ftpHost_);
            EditorGUILayout.PropertyField(ftpUser_);
            ftpPass_.stringValue = EditorGUILayout.PasswordField("Ftp Password", ftpPass_.stringValue);
            EditorGUILayout.EndToggleGroup();

            bool allowBuild = true;

            if (!settings.IsValid())
            {
                GUILayout.Label("Duplicate or Empty BundleName detected");
                allowBuild = false;
            }

            GUILayout.Label($"Local Output folder : { settings.LocalOutputPath }");
            GUILayout.Label($"Remote Output folder : { settings.RemoteOutputPath }");
            serializedObject.ApplyModifiedProperties();

            if (AssetbundleBuildSettings.EditorInstance == settings)
            {
                EditorGUILayout.BeginHorizontal();
                if (allowBuild && GUILayout.Button("Build Remote"))
                {
                    AssetbundleBuilder.BuildAssetBundles(settings);
                    GUIUtility.ExitGUI();
                }

                if (allowBuild && GUILayout.Button("Build Local"))
                {
                    AssetbundleBuilder.BuildAssetBundles(settings, true);
                    GUIUtility.ExitGUI();
                }

                EditorGUI.BeginDisabledGroup(!settings.UseFtp);
                if (allowBuild && GUILayout.Button("Upload(FTP)"))
                {
                    AssetbundleUploader.UploadAllRemoteFiles(settings);
                    GUIUtility.ExitGUI();
                }
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.EndHorizontal();
            }
            else
            {
                if (GUILayout.Button("Set as active setting"))
                {
                    AssetbundleBuildSettings.EditorInstance = settings;
                }
            }

        }
    }

}
