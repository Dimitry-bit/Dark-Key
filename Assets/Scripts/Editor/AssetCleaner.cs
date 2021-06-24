using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace DarkKey.Editor
{
    public class AssetCleaner : EditorWindow
    {
        //private string _destinationFolder = "Assets/Example/../..";
        private string _assetName;
        private GameObject _model;
        private GameObject _prefab;
        private Vector2 _scrollPos;
        private bool _showPosition;
        private string _modelFolder;
        private string _prefabFolder;
        private string _textureFolder;
        private string _materialFolder;
        private string _shaderFolder;
        private string _modelPath;
        private string _prefabPath;
        private Dictionary<string, string> _folderPaths;

        [MenuItem("Tools/Asset Cleaner")]
        public static void ShowWindow() => GetWindow<AssetCleaner>("Asset Cleaner");

        private void OnGUI()
        {

            Paths texturePath = new Paths();
            Paths materialPath = new Paths();

            Dictionary<Texture, string> _texturesPaths = texturePath.TextureDictionary();
            Dictionary<Material, string> _materialPaths = materialPath.MaterialDictionary();

            _model = EditorGUILayout.ObjectField("Model", _model, typeof(GameObject), false) as GameObject;
            _prefab = EditorGUILayout.ObjectField("Prefab", _prefab, typeof(GameObject), false) as GameObject;

            Checkers nullChecker = new Checkers();
            Checkers validGameChecker = new Checkers();
            if (!nullChecker.CheckForNull(_model, _prefab)) return;
            if (!validGameChecker.AreValidGameObjects(_model, _prefab)) return;

            //_destinationFolder = EditorGUILayout.TextField("Destination (FullPath)", _destinationFolder);

            _modelFolder = EditorGUILayout.TextField("Model Path", _modelFolder);
            _prefabFolder = EditorGUILayout.TextField("Prefab Path", _prefabFolder);
            _materialFolder = EditorGUILayout.TextField("Material Path", _materialFolder);
            _textureFolder = EditorGUILayout.TextField("Texture Path", _textureFolder);

            _folderPaths = new Dictionary<string, string>
            {
                {"Model", _modelFolder},
                {"Prefab", _prefabFolder},
                {"Material", _materialFolder},
                {"Texture", _textureFolder}
            };

            Checkers validPathChecker = new Checkers();

            foreach (var folder in _folderPaths)
            {
                if (!validPathChecker.IsValidPath(folder.Key, folder.Value)) return;
            }


            EditorGUILayout.Space();

            RenderChecker renderChecker = new RenderChecker();
            if (!renderChecker.HasRender(_prefab)) return;

            // Get Model and Prefab paths
            _modelPath = AssetDatabase.GetAssetPath(_model);
            _prefabPath = AssetDatabase.GetAssetPath(_prefab);

            LogAllInspector();

            if (!GUILayout.Button("Move Asset")) return;

            Movers mover = new Movers();
            mover.MoveAllObjectAssets(_modelPath, _modelFolder, _prefabPath, _prefabFolder, _textureFolder, _materialFolder);
        }

        private void LogAllInspector()
        {

            Paths texturePaths = new Paths();
            Paths materialPaths = new Paths();

            Dictionary<Texture, string> _texturesPaths = texturePaths.TextureDictionary();
            Dictionary<Material, string> _materialPaths = materialPaths.MaterialDictionary();

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

    }

    public class Paths
    {
        public Dictionary<Texture, string> _texturesPaths;
        public Dictionary<Material, string> _materialPaths;

        public Dictionary<Texture, string> TextureDictionary()
        {
            _texturesPaths = new Dictionary<Texture, string>();
            return _texturesPaths;
        }
        public Dictionary<Material, string> MaterialDictionary()
        {
            _materialPaths = new Dictionary<Material, string>();
            return _materialPaths;
        }


    }

    public class Movers
    {
        public void MoveAllObjectAssets(string _modelPath, string _modelFolder, string _prefabPath, string _prefabFolder, string _textureFolder, string _materialFolder)
        {
            // Move Model
            MoveObjectAssets(_modelPath, _modelFolder);

            // Move Prefab
            MoveObjectAssets(_prefabPath, _prefabFolder);

            // Move Materials
            Paths texturePaths = new Paths();
            Paths materialPaths = new Paths();

            Dictionary<Texture, string> _texturesPaths = texturePaths.TextureDictionary();
            Dictionary<Material, string> _materialPaths = materialPaths.MaterialDictionary();

            foreach (var materialPath in _materialPaths)
            {
                MoveObjectAssets(materialPath.Value, _materialFolder);
            }

            // Move Textures
            foreach (var texturePath in _texturesPaths)
            {
                MoveObjectAssets(texturePath.Value, _textureFolder);
            }
        }

        public void MoveObjectAssets(string oldPath, string newPath)
        {
            var assetName = Path.GetFileNameWithoutExtension(oldPath);
            var assetExtension = Path.GetExtension(oldPath);

            if (AssetDatabase.ValidateMoveAsset(oldPath, $"{newPath}/{assetName}{assetExtension}") == "")
                AssetDatabase.MoveAsset(oldPath, $"{newPath}/{assetName}{assetExtension}");
        }


    }

    public class Checkers
    {
        public bool CheckForNull(GameObject _model, GameObject _prefab)
        {
            if (_model != null && _prefab != null) return true;
            EditorGUILayout.HelpBox("MODEL OR PREFAB IS MISSING", MessageType.Error);
            return false;
        }

        public bool IsValidPath(string type, string path)
        {
            if (AssetDatabase.IsValidFolder(path))
                return true;

            EditorGUILayout.HelpBox($"Invalid {type} PATH.", MessageType.Error);
            return false;
        }

        public bool AreValidGameObjects(GameObject _model, GameObject _prefab)
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

    public class RenderChecker
    {
        public bool HasRender(GameObject _prefab)
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
            Paths texturePath = new Paths();

            Dictionary<Texture, string> _texturesPath = texturePath.TextureDictionary();


            if (_texturesPath.ContainsKey(texture)) return;
            _texturesPath.Add(texture, path);
        }

        private void RegisterMaterialPath(Material material, string path)
        {
            Paths materialPath = new Paths();
            Dictionary<Material, string> _materialPath = materialPath.MaterialDictionary();

            if (_materialPath.ContainsKey(material)) return;
            _materialPath.Add(material, path);
        }

    }

}