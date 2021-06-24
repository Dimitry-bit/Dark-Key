    // private void MoveAllObjectAssets()
    // {
    //     var tmpAssetName = _assetName;
    //     if (string.IsNullOrEmpty(_assetName)) tmpAssetName = _model.name;
    //
    //     // Move Model
    //     MoveObjectAssets(tmpAssetName, "Models", _modelPath);
    //
    //     // Move Prefab
    //     MoveObjectAssets(tmpAssetName, "Prefabs", _prefabPath);
    //
    //     // Move Materials
    //     foreach (var materialPath in _materialPaths)
    //     {
    //         MoveObjectAssets(tmpAssetName, "Materials", materialPath.Value);
    //     }
    //
    //     // Move Textures
    //     foreach (var texturePath in _texturesPaths)
    //     {
    //         MoveObjectAssets(tmpAssetName, "Textures", texturePath.Value);
    //     }
    // }


        /*     public void MoveObjectAssets(string assetFolderName, string newFolderName, string path)
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
             }*/
