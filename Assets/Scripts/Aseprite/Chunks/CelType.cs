namespace Aseprite.Chunks
{
    public enum CelType : ushort
    {
        RawImageData = 0,
        LinkedCel = 1,
        CompressedImage = 2,
        CompressedTilemap = 3
    }
}