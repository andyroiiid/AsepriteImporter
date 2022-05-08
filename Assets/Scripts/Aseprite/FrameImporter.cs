using System.Collections.Generic;
using System.IO;
using Aseprite.Chunks;
using UnityEngine;

namespace Aseprite
{
    public class FrameImporter
    {
        private readonly AsepriteImportConfig _config;
        private readonly int _frameIndex;

        public readonly List<Texture> Textures = new();
        public readonly List<Sprite> Sprites = new();

        private void CreateTextureAndSpriteFromCel(Cel cel)
        {
            var texture = new Texture2D(cel.Width, cel.Height)
            {
                name = $"Texture{_frameIndex}-{cel.LayerIndex}",
                alphaIsTransparency = _config.alphaIsTransparency,
                filterMode = _config.filterMode,
                wrapMode = _config.wrapMode,
                anisoLevel = _config.anisoLevel
            };
            texture.SetPixels32(cel.Pixels);
            texture.Apply();
            Textures.Add(texture);

            var sprite = Sprite.Create(
                texture,
                new Rect(Vector2.zero, new Vector2(texture.width, texture.height)),
                Vector2.zero
            );
            sprite.name = $"Sprite{_frameIndex}-{cel.LayerIndex}";
            Sprites.Add(sprite);
        }

        private void ProcessChunk(Chunk chunk)
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
                    CreateTextureAndSpriteFromCel(cel);
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

        public FrameImporter(AsepriteImportConfig config, int frameIndex, List<Chunk> chunks)
        {
            _config = config;
            _frameIndex = frameIndex;
            foreach (var chunk in chunks)
            {
                ProcessChunk(chunk);
            }
        }
    }
}