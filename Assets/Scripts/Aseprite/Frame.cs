using System.IO;
using UnityEngine.Assertions;

namespace Aseprite
{
    public struct Frame
    {
        public readonly uint BytesInThisFrame;
        public readonly ushort MagicNumber;
        public readonly ushort OldNumChunks;
        public readonly ushort FrameDuration; // in milliseconds
        public readonly uint NumChunks;

        public Frame(BinaryReader reader)
        {
            BytesInThisFrame = reader.ReadDWord();
            MagicNumber = reader.ReadWord();
            OldNumChunks = reader.ReadWord();
            FrameDuration = reader.ReadWord();
            reader.ReadBytes(2);
            NumChunks = reader.ReadDWord();

            Assert.AreEqual(MagicNumber, 0xF1FAu);
        }
    }
}