using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace DarkKey.Editor
{
    public class AssetCleaner : EditorWindow
    {
        private string _destinationFolder = "Assets/Example/../..";
        private string _assetName;
        private GameObject _model;
        private GameObject _prefab;

        private Dictionary<Texture, string> _texturesPaths;
        private Dictionary<Material, string> _materialPaths;
        private string _modelPath;
        private string _prefabPath;
        private string _assetPath;

        private Vector2 _scrollPos;
        private bool _showPosition;

        [MenuItem("Tools/Asset Cleaner")]
        public static void ShowWindow() => GetWindow<AssetCleaner>("Asset Cleaner");

        private void OnGUI()
        {
            _texturesPaths = new Dictionary<Texture, string>();
            _materialPaths = new Dictionary<Material, string>();

            _model = EditorGUILayout.ObjectField("Model", _model, typeof(GameObject), false) as GameObject;
            _prefab = EditorGUILayout.ObjectField("Prefab", _prefab, typeof(GameObject), false) as GameObject;

            if (!CheckForNull()) return;
            if (!AreValidGameObjects()) return;

            _destinationFolder = EditorGUILayout.TextField("Destination (FullPath)", _destinationFolder);

            if (!IsValidPath()) return;

            _assetName = EditorGUILayout.TextField("Asset Folder Name", _assetName);
            _assetPath = $"{_destinationFolder}/{_assetName}";

            if (string.IsNullOrEmpty(_assetName))
                _assetPath = $"{_destinationFolder}/{_model.name}";

            EditorGUILayout.HelpBox(_assetPath, MessageType.None, false);

            EditorGUILayout.Space();

            if (!GetRenderer()) return;

            // Get Model and Prefab paths
            _modelPath = AssetDatabase.GetAssetPath(_model);
            _prefabPath = AssetDatabase.GetAssetPath(_prefab);

            LogAllInspector();

            if (!GUILayout.Button("Move Asset")) return;

            MoveAllObjectAssets();
        }
        
        private bool GetRenderer()
        {
            if (_prefab.TryGetComponent(out Renderer r))
            {
                var sharedMaterials = r.sharedMaterials;

                GetMaterialsPath(sharedMaterials);
                GetTexturesPath(sharedMaterials);
            }

            if (_prefab.GetComponentsInChildren<Renderer>().Length > 0)
            {
                var renderers = _prefab.GetComponentsInChildren<Renderer>();

                foreach (var renderer in renderers)
                {
                    var sharedMaterials = renderer.sharedMaterials;
                    GetMaterialsPath(sharedMaterials);
                    GetTexturesPath(sharedMaterials);
                }

                return true;
            }

            EditorGUILayout.HelpBox("Couldn't find a renderer", MessageType.Error);
            return false;
        }

        private void GetMaterialsPath(Material[] sharedMaterials)
        {
            foreach (var material in sharedMaterials)
            {
                var materialPath = AssetDatabase.GetAssetPath(material);
                if (string.IsNullOrEmpty(materialPath)) return;

                RegisterMaterialPath(material, materialPath);
            }
        }

        private void GetTexturesPath(Material[] materials)
        {
            foreach (var material in materials)
            {
                int[] textures = material.GetTexturePropertyNameIDs();

                foreach (var id in textures)
                {
                    var texture = material.GetTexture(id);
                    var texturePath = AssetDatabase.GetAssetPath(texture);

                    if (string.IsNullOrEmpty(texturePath)) return;

                    RegisterPathTexture(texture, texturePath);
                }
            }
        }

        private void RegisterPathTexture(Texture texture, string path)
        {
            if (_texturesPaths.ContainsKey(texture)) return;
            _texturesPaths.Add(texture, path);
        }

        private void RegisterMaterialPath(Material material, string path)
        {
            if (_materialPaths.ContainsKey(material)) return;
            _materialPaths.Add(material, path);
        }

        private void MoveObjectAssets(string assetFolderName, string newFolderName, string path)
        {
            if (!AssetDatabase.IsValidFolder($"{_destinationFolder}/{assetFolderName}"))
                AssetDatabase.CreateFolder($"{_destinationFolder}", $"{assetFolderName}");

            if (!AssetDatabase.IsValidFolder($"{_destinationFolder}/{assetFolderName}/{newFolderName}"))
                AssetDatabase.CreateFolder($"{_destinationFolder}/{assetFolderName}", $"{newFolderName}");

            var assetName = Path.GetFileNameWithoutExtension(path);
            var assetExtension = Path.GetExtension(path);

            if (AssetDatabase.ValidateMoveAsset(path,
                $"{_destinationFolder}/{assetFolderName}/{newFolderName}/{assetName}{assetExtension}") == "")
                AssetDatabase.MoveAsset(path,
                    $"{_destinationFolder}/{assetFolderName}/{newFolderName}/{assetName}{assetExtension}");
        }

        private void MoveAllObjectAssets()
        {
            var tmpAssetName = _assetName;
            if (string.IsNullOrEmpty(_assetName)) tmpAssetName = _model.name;

            // Move Model
            MoveObjectAssets(tmpAssetName, "Models", _modelPath);

            // Move Prefab
            MoveObjectAssets(tmpAssetName, "Prefabs", _prefabPath);

            // Move Materials
            foreach (var materialPath in _materialPaths)
            {
                MoveObjectAssets(tmpAssetName, "Materials", materialPath.Value);
            }

            // Move Textures
            foreach (var texturePath in _texturesPaths)
            {
                MoveObjectAssets(tmpAssetName, "Textures", texturePath.Value);
            }
        }
        
        private void LogAllInspector()
        {
            // ScrollView
            EditorGUILayout.BeginHorizontal();
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            EditorGUILayout.Space();

            GUI.color = Color.red;
            GUILayout.Label("THESE FILES WILL BE MOVED", EditorStyles.boldLabel);
            GUI.color = Color.white;
            EditorGUILayout.Space();

            GUI.color = Color.green;

            // Log Path
            EditorGUILayout.HelpBox($" [ModelPath]: ({_modelPath})", MessageType.None);
            EditorGUILayout.HelpBox($" [PrefabPath]: ({_prefabPath})", MessageType.None);

            foreach (var texturesPath in _texturesPaths)
                EditorGUILayout.HelpBox($" [TexturePath]: ({texturesPath.Value})", MessageType.None);

            foreach (var materialPath in _materialPaths)
                EditorGUILayout.HelpBox($" [MaterialPath]: ({materialPath.Value})", MessageType.None);

            GUI.color = Color.white;

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndHorizontal();
        }

        private bool CheckForNull()
        {
            if (_model != null && _prefab != null) return true;
            EditorGUILayout.HelpBox("MODEL OR PREFAB IS MISSING", MessageType.Error);
            return false;
        }

        private bool IsValidPath()
        {
            if (AssetDatabase.IsValidFolder(_destinationFolder))
                return true;

            EditorGUILayout.HelpBox("Invalid DESTINATION PATH.", MessageType.Error);
            return false;
        }

        private bool AreValidGameObjects()
        {
            if (PrefabUtility.GetPrefabAssetType(_model) != PrefabAssetType.Model)
            {
                EditorGUILayout.HelpBox("INVALID MODEL. PLEASE REASSIGN MODEL", MessageType.Error);
                return false;
            }

            if (PrefabUtility.GetPrefabAssetType(_prefab) != PrefabAssetType.Regular)
            {
                EditorGUILayout.HelpBox("INVALID PREFAB. PLEASE REASSIGN PREFAB", MessageType.Error);
                return false;
            }

            return true;
        }
    }
}