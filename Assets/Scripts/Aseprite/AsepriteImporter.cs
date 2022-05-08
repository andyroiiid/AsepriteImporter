using System.IO;
using UnityEditor.AssetImporters;
using UnityEngine;
using Aseprite.Chunks;

namespace Aseprite
{
    [ScriptedImporter(1, "aseprite")]
    public class AsepriteImporter : ScriptedImporter
    {
        [SerializeField] private bool alphaIsTransparency = true;
        [SerializeField] private FilterMode filterMode = FilterMode.Point;
        [SerializeField] private TextureWrapMode wrapMode = TextureWrapMode.Repeat;
        [SerializeField] [Range(0, 16)] private int anisoLevel = 1;

        private void CreateTextureAndSpriteFromCel(AssetImportContext ctx, int frameIdx, Cel cel)
        {
            var texture = new Texture2D(cel.Width, cel.Height)
            {
                name = $"Texture{frameIdx}-{cel.LayerIndex}",
                alphaIsTransparency = alphaIsTransparency,
                filterMode = filterMode,
                wrapMode = wrapMode,
                anisoLevel = anisoLevel
            };
            texture.SetPixels32(cel.Pixels);
            texture.Apply();

            var sprite = Sprite.Create(
                texture,
                new Rect(Vector2.zero, new Vector2(texture.width, texture.height)),
                Vector2.zero
            );
            sprite.name = $"Sprite{frameIdx}-{cel.LayerIndex}";

            ctx.AddObjectToAsset(texture.name, texture);
            ctx.AddObjectToAsset(sprite.name, sprite);
        }

        private void ProcessChunk(AssetImportContext ctx, int frameIdx, Chunk chunk)
        {
            using var stream = new MemoryStream(chunk.Data);
            using var reader = new BinaryReader(stream);

            switch (chunk.Type)
            {
                case ChunkType.OldPalette8Bit:
                    Debug.LogError("Old palette format is not implemented.");
                    return;
                case ChunkType.OldPalette6Bit:
                    Debug.LogError("Old palette format is not implemented.");
                    return;
                case ChunkType.Layer:
                    var layer = new Layer(reader);
                    Debug.Log($"{layer}\n{layer.DebugFieldsToString()}");
                    return;
                case ChunkType.Cel:
                    var cel = new Cel(reader);
                    Debug.Log($"{cel}\n{cel.DebugFieldsToString()}");
                    CreateTextureAndSpriteFromCel(ctx, frameIdx, cel);
                    return;
                case ChunkType.CelExtra:
                    break;
                case ChunkType.ColorProfile:
                    var colorProfile = new ColorProfile(reader);
                    Debug.Log($"{colorProfile}\n{colorProfile.DebugFieldsToString()}");
                    return;
                case ChunkType.ExternalFiles:
                    break;
                case ChunkType.Mask:
                    break;
                case ChunkType.Path:
                    break;
                case ChunkType.Tags:
                    break;
                case ChunkType.Palette:
                    var palette = new Palette(reader);
                    Debug.Log($"{palette}\n{palette.DebugFieldsToString()}\n{palette.DebugEntriesToString()}");
                    return;
                case ChunkType.UserData:
                    break;
                case ChunkType.Slice:
                    break;
                case ChunkType.Tileset:
                    break;
                default:
                    Debug.LogError("Unknown chunk type");
                    return;
            }

            Debug.Log(chunk.Type);
        }

        public override void OnImportAsset(AssetImportContext ctx)
        {
            using var stream = File.OpenRead(ctx.assetPath);
            using var reader = new BinaryReader(stream);

            var header = new Header(reader);
            Debug.Log($"{header}\n{header.DebugFieldsToString()}");

            for (var iFrame = 0; iFrame < header.Frames; iFrame++)
            {
                var frame = new Frame(reader);
                Debug.Log($"{frame}\n{frame.DebugFieldsToString()}");

                for (var iChunk = 0; iChunk < frame.NumChunks; iChunk++)
                {
                    ProcessChunk(ctx, iFrame, new Chunk(reader));
                }
            }

            var texture = new Texture2D(128, 128);
            for (var x = 0; x < 64; x++)
            {
                for (var y = 0; y < 64; y++)
                {
                    texture.SetPixel(x, y, Color.red);
                }
            }

            texture.Apply();
            ctx.AddObjectToAsset("texture", texture);
            ctx.SetMainObject(texture);
        }
    }
}