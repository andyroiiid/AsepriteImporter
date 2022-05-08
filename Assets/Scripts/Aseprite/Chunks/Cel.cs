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
        public readonly Color32[] Pixels;

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

            Pixels = new Color32[Width * Height];
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    // flip vertically
                    var pixelOffset = x + (Height - y - 1) * Width;
                    var dataOffset = 4 * (x + y * Width);
                    Pixels[pixelOffset] = new Color32(
                        data[dataOffset],
                        data[dataOffset + 1],
                        data[dataOffset + 2],
                        data[dataOffset + 3]
                    );
                }
            }
        }
    }
}