using System.IO;
using System.Text;

namespace Aseprite
{
    public static class BinaryReaderExtensions
    {
        public static ushort ReadWord(this BinaryReader reader) => reader.ReadUInt16();

        public static short ReadShort(this BinaryReader reader) => reader.ReadInt16();

        public static uint ReadDWord(this BinaryReader reader) => reader.ReadUInt32();

        public static int ReadLong(this BinaryReader reader) => reader.ReadInt32();

        public static double ReadFixed(this BinaryReader reader) => reader.ReadInt32() / 65536.0;

        public static string ReadStr(this BinaryReader reader)
        {
            var length = reader.ReadWord();
            return Encoding.UTF8.GetString(reader.ReadBytes(length));
        }

        public static byte[] ReadRemainingBytes(this BinaryReader reader)
        {
            var stream = reader.BaseStream;
            var remainingBytes = stream.Length - stream.Position;
            return reader.ReadBytes((int) remainingBytes);
        }
    }
}