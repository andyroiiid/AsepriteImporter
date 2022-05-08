using System.Collections.Generic;
using System.IO;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Aseprite
{
    [ScriptedImporter(1, "aseprite")]
    public class AsepriteImporter : ScriptedImporter
    {
        [SerializeField] private AsepriteImportConfig config;

        public override void OnImportAsset(AssetImportContext ctx)
        {
            using var stream = File.OpenRead(ctx.assetPath);
            using var reader = new BinaryReader(stream);

            var header = new Header(reader);
            Debug.Log($"{header}\n{header.DebugFieldsToString()}");

            var textures = new List<Texture>();
            var sprites = new List<Sprite>();

            for (var iFrame = 0; iFrame < header.Frames; iFrame++)
            {
                var frame = new Frame(reader);
                Debug.Log($"{frame}\n{frame.DebugFieldsToString()}");

                var chunks = new List<Chunk>();
                for (var iChunk = 0; iChunk < frame.NumChunks; iChunk++)
                {
                    chunks.Add(new Chunk(reader));
                }

                var importer = new FrameImporter(config, iFrame, chunks);
                textures.AddRange(importer.Textures);
                sprites.AddRange(importer.Sprites);
            }

            foreach (var texture in textures)
            {
                ctx.AddObjectToAsset(texture.name, texture);
            }

            foreach (var sprite in sprites)
            {
                ctx.AddObjectToAsset(sprite.name, sprite);
            }

            var mainTexture = new Texture2D(128, 128);
            for (var x = 0; x < 64; x++)
            {
                for (var y = 0; y < 64; y++)
                {
                    mainTexture.SetPixel(x, y, Color.red);
                }
            }

            mainTexture.Apply();

            ctx.AddObjectToAsset("mainTexture", mainTexture);
            ctx.SetMainObject(mainTexture);
        }
    }
}