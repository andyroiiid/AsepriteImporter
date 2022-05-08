using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aseprite.Chunks;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Aseprite
{
    [ScriptedImporter(1, "aseprite")]
    public class AsepriteImporter : ScriptedImporter
    {
        [SerializeField] private bool alphaIsTransparency = true;
        [SerializeField] private FilterMode filterMode = FilterMode.Point;
        [SerializeField] private TextureWrapMode wrapMode = TextureWrapMode.Repeat;
        [SerializeField] [Range(0, 16)] private int anisoLevel = 1;

        private Texture2D ImportFrame(Vector2Int size, int frameIndex, IEnumerable<Chunk> chunks)
        {
            var cels = new List<Cel>();

            foreach (var chunk in chunks.Where(chunk => chunk.Type == ChunkType.Cel))
            {
                using var stream = new MemoryStream(chunk.Data);
                using var reader = new BinaryReader(stream);
                cels.Add(new Cel(reader));
            }

            cels.Sort((lhs, rhs) => lhs.LayerIndex - rhs.LayerIndex);

            var pixels = new Color[size.x * size.y];
            for (var y = 0; y < size.y; y++)
            {
                for (var x = 0; x < size.x; x++)
                {
                    var i = x + y * size.x;
                    pixels[i] = new Color(0.0f, 0.0f, 0.0f, 0.0f);
                }
            }

            // currently only normal blend is supported
            foreach (var cel in cels)
            {
                for (var y = 0; y < cel.Height; y++)
                {
                    for (var x = 0; x < cel.Width; x++)
                    {
                        var iCel = x + y * cel.Width;
                        var xPixel = x + cel.XPosition;
                        var yPixel = size.y - (y + cel.YPosition) - 1;

                        if (xPixel < 0 || xPixel >= size.x || yPixel < 0 || yPixel >= size.y)
                        {
                            continue;
                        }

                        var i = xPixel + yPixel * size.x;
                        pixels[i] = pixels[i].NormalBlend(cel.Pixels[iCel]);
                    }
                }
            }

            var texture = new Texture2D(size.x, size.y)
            {
                name = $"texture_{frameIndex}",
                alphaIsTransparency = alphaIsTransparency,
                filterMode = filterMode,
                wrapMode = wrapMode,
                anisoLevel = anisoLevel
            };
            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }

        public override void OnImportAsset(AssetImportContext ctx)
        {
            using var stream = File.OpenRead(ctx.assetPath);
            using var reader = new BinaryReader(stream);

            var header = new Header(reader);
            Debug.Log($"{header}\n{header.DebugFieldsToString()}");
            var size = new Vector2Int(header.WidthInPixels, header.HeightInPixels);

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

                var texture = ImportFrame(size, iFrame, chunks);
                textures.Add(texture);

                var sprite = Sprite.Create(texture, new Rect(Vector2.zero, size), Vector2.zero);
                sprite.name = $"sprite_{iFrame}";
                sprites.Add(sprite);
            }

            foreach (var texture in textures)
            {
                ctx.AddObjectToAsset(texture.name, texture);
            }

            foreach (var sprite in sprites)
            {
                ctx.AddObjectToAsset(sprite.name, sprite);
            }

            ctx.SetMainObject(textures[0]);
        }
    }
}