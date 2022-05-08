using System.IO;
using UnityEngine.Assertions;

namespace Aseprite
{
    public struct Header
    {
        public readonly uint FileSize;
        public readonly ushort MagicNumber;
        public readonly ushort Frames;
        public readonly ushort WidthInPixels;
        public readonly ushort HeightInPixels;
        public readonly ushort ColorDepth;
        public readonly uint Flags;
        public readonly byte TransparentColorIndex;
        public readonly ushort NumColors;
        public readonly byte PixelWidth; // width per pixel
        public readonly byte PixelHeight; // height per pixel
        public readonly short GridPositionX;
        public readonly short GridPositionY;
        public readonly ushort GridWidth;
        public readonly ushort GridHeight;

        public Header(BinaryReader reader)
        {
            FileSize = reader.ReadDWord();
            MagicNumber = reader.ReadWord();
            Frames = reader.ReadWord();
            WidthInPixels = reader.ReadWord();
            HeightInPixels = reader.ReadWord();
            ColorDepth = reader.ReadWord();
            Flags = reader.ReadDWord();
            reader.ReadWord(); // Speed
            reader.ReadDWord();
            reader.ReadDWord();
            TransparentColorIndex = reader.ReadByte();
            reader.ReadBytes(3);
            NumColors = reader.ReadWord();
            PixelWidth = reader.ReadByte();
            PixelHeight = reader.ReadByte();
            GridPositionX = reader.ReadShort();
            GridPositionY = reader.ReadShort();
            GridWidth = reader.ReadWord();
            GridHeight = reader.ReadWord();
            reader.ReadBytes(84);

            Assert.AreEqual(MagicNumber, 0xA5E0u);
        }
    }
}