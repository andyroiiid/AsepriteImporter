using System.IO;

namespace Aseprite
{
    public struct Chunk
    {
        public readonly uint Size;
        public readonly ChunkType Type;
        public readonly byte[] Data;

        private const int DataOffset = sizeof(uint) + sizeof(ushort);

        public Chunk(BinaryReader reader)
        {
            Size = reader.ReadDWord();
            Type = (ChunkType) reader.ReadWord();
            Data = reader.ReadBytes((int) (Size - DataOffset));
        }
    }
}