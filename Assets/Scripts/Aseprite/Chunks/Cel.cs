using System.IO;
using Ionic.Zlib;
using UnityEngine;
using UnityEngine.Assertions;

namespace Aseprite.Chunks
{
    public readonly struct Cel
    {
        public readonly ushort LayerIndex;
        public readonly short XPosition;
        public readonly short YPosition;
        public readonly byte OpacityLevel;
        public readonly CelType Type;

        public readonly ushort Width; // compressed image: pixels
        public readonly ushort Height; // compressed image: pixels
        public readonly Texture2D Texture;
        public readonly Sprite Sprite;

        public Cel(BinaryReader reader)
        {
            LayerIndex = reader.ReadWord();
            XPosition = reader.ReadShort();
            YPosition = reader.ReadShort();
            OpacityLevel = reader.ReadByte();
            Type = (CelType) reader.ReadWord();
            reader.ReadBytes(7);

            // only support compressed image for now
            Assert.AreEqual(Type, CelType.CompressedImage);

            Width = reader.ReadWord();
            Height = reader.ReadWord();

            using var compressedStream = new MemoryStream(reader.ReadRemainingBytes());
            using var stream = new ZlibStream(compressedStream, CompressionMode.Decompress);

            var dataLength = 4 * Width * Height;
            var data = new byte[dataLength];
            Assert.AreEqual(stream.Read(data, 0, dataLength), dataLength);

            var colors = new Color32[Width * Height];
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    // flip vertically
                    var colorOffset = x + (Height - y - 1) * Width;
                    var dataOffset = 4 * (x + y * Width);
                    colors[colorOffset] = new Color32(
                        data[dataOffset],
                        data[dataOffset + 1],
                        data[dataOffset + 2],
                        data[dataOffset + 3]
                    );
                }
            }

            Texture = new Texture2D(Width, Height)
            {
                name = $"Cel{LayerIndex}{XPosition}{YPosition}{Width}{Height}Texture",
                filterMode = FilterMode.Point,
                alphaIsTransparency = true,
            };
            Texture.SetPixels32(colors);
            Texture.Apply();

            Sprite = Sprite.Create(
                Texture,
                new Rect(Vector2.zero, new Vector2(Texture.width, Texture.height)),
                Vector2.zero
            );
            Sprite.name = $"Cel{LayerIndex}{XPosition}{YPosition}{Width}{Height}Sprite";
        }
    }
}