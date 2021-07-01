using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace DarkKey.Editor
{
    public class AssetCleaner : EditorWindow
    {
        private GameObject _model;
        private GameObject _prefab;
        private bool _structuredFolder;

        private HashSet<string> _texturesPaths;
        private HashSet<string> _materialPaths;
        private string _modelPath;
        private string _prefabPath;

        // Optional
        private string _destinationFolder = "Assets/Example/../..";
        private string _assetName;
        private string _assetNewPath;

        private string _modelFolder = "Assets/Example/../..Models";
        private string _prefabFolder = "Assets/Example/../../Prefabs";
        private string _textureFolder = "Assets/Example/../../Textures";
        private string _materialFolder = "Assets/Example/../../Materials";

        private Vector2 _scrollPos;

        [MenuItem("Tools/Asset Cleaner")]
        public static void ShowWindow() => GetWindow<AssetCleaner>("Asset Cleaner");

        private void OnGUI()
        {
            _texturesPaths = new HashSet<string>();
            _materialPaths = new HashSet<string>();

            DrawGui();

            if (!AreValidGameObjects()) return;

            if (_structuredFolder)
            {
                if (!IsValidPath(_assetNewPath)) return;
            }
            else
            {
                if (!IsValidPath(_modelFolder)) return;
                if (!IsValidPath(_prefabFolder)) return;
                if (!IsValidPath(_materialFolder)) return;
                if (!IsValidPath(_textureFolder)) return;
            }

            InitializeAssetPaths();
            DrawLogs();
            DrawButtons();
        }

        private void InitializeAssetPaths()
        {
            _modelPath = AssetDatabase.GetAssetPath(_model);
            _prefabPath = AssetDatabase.GetAssetPath(_prefab);
            Renderer[] renderers = GetRenderers();
            foreach (var renderer in renderers)
            {
                var materials = renderer.sharedMaterials;
                _materialPaths = GetMaterialPaths(materials);
                _texturesPaths = GetTexturePaths(materials);
            }
        }

        private Renderer[] GetRenderers()
        {
            var renderers = new List<Renderer>();

            if (_prefab.TryGetComponent(out Renderer r)) renderers.Add(r);
            if (_prefab.GetComponentsInChildren<Renderer>().Length <= 0) return renderers.ToArray();

            var childrenRenderers = _prefab.GetComponentsInChildren<Renderer>();
            renderers.AddRange(childrenRenderers);
            return renderers.ToArray();
        }

        private static HashSet<string> GetMaterialPaths(Material[] materials)
        {
            var materialPaths = new HashSet<string>();
            foreach (var material in materials)
            {
                var materialPath = AssetDatabase.GetAssetPath(material);
                if (!string.IsNullOrEmpty(materialPath)) materialPaths.Add(materialPath);
            }

            return materialPaths;
        }

        private static HashSet<string> GetTexturePaths(Material[] materials)
        {
            var texturePaths = new HashSet<string>();
            foreach (var material in materials)
            {
                int[] textureIds = material.GetTexturePropertyNameIDs();
                foreach (var textureId in textureIds)
                {
                    var texture = material.GetTexture(textureId);
                    var texturePath = AssetDatabase.GetAssetPath(texture);
                    if (!string.IsNullOrEmpty(texturePath)) texturePaths.Add(texturePath);
                }
            }

            return texturePaths;
        }

        private static void MoveObjectAsset(string oldPath, string newPath)
        {
            if (!AssetDatabase.IsValidFolder(newPath)) CreateFolderStructure(newPath);

            var assetName = Path.GetFileNameWithoutExtension(oldPath);
            var assetExtension = Path.GetExtension(oldPath);

            if (AssetDatabase.ValidateMoveAsset(oldPath, $"{newPath}/{assetName}{assetExtension}") == "")
                AssetDatabase.MoveAsset(oldPath, $"{newPath}/{assetName}{assetExtension}");
        }

        private static void CreateFolderStructure(string newPath)
        {
            var subFolders = newPath.Split('/');
            string subFolder = null;
            string parentFolder = null;

            foreach (var folder in subFolders)
            {
                subFolder += $"{folder}";

                if (!AssetDatabase.IsValidFolder(subFolder))
                {
                    AssetDatabase.CreateFolder($"{parentFolder}", folder);
                }

                parentFolder = subFolder;
                subFolder += "/";
            }
        }

        private void MoveAllObjectAssets()
        {
            MoveObjectAsset(_modelPath, _modelFolder);
            MoveObjectAsset(_prefabPath, _prefabFolder);
            foreach (var materialPath in _materialPaths) MoveObjectAsset(materialPath, _materialFolder);
            foreach (var texturePath in _texturesPaths) MoveObjectAsset(texturePath, _textureFolder);
        }

        private void DeleteAllAsset()
        {
            AssetDatabase.DeleteAsset(_modelPath);
            AssetDatabase.DeleteAsset(_prefabPath);
            foreach (var materialPath in _materialPaths) AssetDatabase.DeleteAsset(materialPath);
            foreach (var texturePath in _texturesPaths) AssetDatabase.DeleteAsset(texturePath);
        }

        private void ResetAssetCleaner()
        {
            _model = null;
            _prefab = null;
        }

        private void DrawGui()
        {
            _model = EditorGUILayout.ObjectField("Model", _model, typeof(GameObject), false) as GameObject;
            _prefab = EditorGUILayout.ObjectField("Prefab", _prefab, typeof(GameObject), false) as GameObject;
            _structuredFolder = EditorGUILayout.Toggle("Structured Folder", _structuredFolder);

            if (_structuredFolder)
            {
                _destinationFolder = EditorGUILayout.TextField("Destination (FullPath)", _destinationFolder);
                _assetName = EditorGUILayout.TextField("Asset Folder Name", _assetName);

                // Sanitize _destinationFolder
                _assetNewPath = $"{_destinationFolder.TrimEnd('/')}";
                
                if (string.IsNullOrEmpty(_assetName))
                {
                    if (_model != null)
                        _assetNewPath += $"/{_model.name}";
                }
                else
                {
                    _assetNewPath += $"/{_assetName}";
                }

                EditorGUILayout.HelpBox(_assetNewPath, MessageType.None, false);

                _modelFolder = $"{_assetNewPath}/Models";
                _prefabFolder = $"{_assetNewPath}/Prefabs";
                _materialFolder = $"{_assetNewPath}/Materials";
                _textureFolder = $"{_assetNewPath}/Textures";
            }
            else
            {
                _modelFolder = EditorGUILayout.TextField("Model Path", _modelFolder);
                _prefabFolder = EditorGUILayout.TextField("Prefab Path", _prefabFolder);
                _materialFolder = EditorGUILayout.TextField("Material Path", _materialFolder);
                _textureFolder = EditorGUILayout.TextField("Texture Path", _textureFolder);
            }
        }

        private void DrawLogs()
        {
            EditorGUILayout.Space();

            // ScrollView
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginScrollView(default);
            EditorGUILayout.Space();

            GUI.color = Color.red;
            GUILayout.Label("DETECTED FILES", EditorStyles.boldLabel);
            GUI.color = Color.white;

            EditorGUILayout.Space();

            // Log Path
            GUI.color = Color.green;
            EditorGUILayout.HelpBox($" [ModelPath]: {_modelPath} --> {_modelFolder}", MessageType.None);
            EditorGUILayout.HelpBox($" [PrefabPath]: {_prefabPath} --> {_prefabFolder}", MessageType.None);

            foreach (var materialPath in _materialPaths)
                EditorGUILayout.HelpBox($" [MaterialPath]: {materialPath} --> {_materialFolder}", MessageType.None);

            foreach (var texturesPath in _texturesPaths)
                EditorGUILayout.HelpBox($" [TexturePath]: {texturesPath} --> {_textureFolder}", MessageType.None);

            GUI.color = Color.white;
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawButtons()
        {
            if (GUILayout.Button("Move Asset"))
            {
                MoveAllObjectAssets();
                ResetAssetCleaner();
            }

            if (GUILayout.Button("Delete Asset"))
            {
                DeleteAllAsset();
                ResetAssetCleaner();
            }
        }

        private static bool IsValidPath(string path)
        {
            var pathRegex = new Regex(@"^Assets(/){1}([0-9a-zA-Z-_\s/.]*/?)*/?$");
            if (pathRegex.IsMatch(path)) return true;

            EditorGUILayout.HelpBox($"Invalid Path, Path must start with \"Assets/\".", MessageType.Error);
            return false;
        }

        private bool AreValidGameObjects()
        {
            if (_model == null || _prefab == null)
            {
                EditorGUILayout.HelpBox("MODEL OR PREFAB IS MISSING", MessageType.Error);
                return false;
            }

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