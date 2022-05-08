namespace Aseprite
{
    public enum ChunkType : ushort
    {
        OldPalette8Bit = 0x0004,
        OldPalette6Bit = 0x0011,
        Layer = 0x2004,
        Cel = 0x2005,
        CelExtra = 0x2006,
        ColorProfile = 0x2007,
        ExternalFiles = 0x2008,
        Mask = 0x2016, // deprecated
        Path = 0x2017, // never used
        Tags = 0x2108,
        Palette = 0x2019,
        UserData = 0x2020,
        Slice = 0x2022,
        Tileset = 0x2023
    }
}