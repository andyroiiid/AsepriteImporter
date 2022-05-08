using System.IO;

namespace Aseprite.Chunks
{
    public readonly struct ColorProfile
    {
        public readonly ushort Type;
        public readonly ushort Flags;
        public readonly double FixedGamma;

        public struct IccProfileData
        {
            public readonly uint Length;
            public readonly byte[] Data;

            public IccProfileData(uint length, byte[] data)
            {
                Length = length;
                Data = data;
            }
        }

        public readonly IccProfileData? IccProfile;

        public ColorProfile(BinaryReader reader)
        {
            Type = reader.ReadWord();
            Flags = reader.ReadWord();
            FixedGamma = reader.ReadFixed();

            // ICC profile
            if (Type == 2)
            {
                var length = reader.ReadDWord();
                var data = reader.ReadBytes((int) length);
                IccProfile = new IccProfileData(length, data);
            }
            else
            {
                IccProfile = null;
            }
        }
    }
}