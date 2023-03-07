using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpriteProcessor : AssetPostprocessor
{
    private void OnPostprocessTexture(Texture2D texture)
    {
        var lowerCaseAssetPath = assetPath.ToLower();
        var isInSpriteDirectory = lowerCaseAssetPath.IndexOf("/sprites/", StringComparison.Ordinal) != -1;

        if (!isInSpriteDirectory) return;
        
        var importer = (TextureImporter)assetImporter;
        importer.textureType = TextureImporterType.Sprite;
    }
}
