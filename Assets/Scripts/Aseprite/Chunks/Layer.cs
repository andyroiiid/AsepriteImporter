using System.IO;

namespace Aseprite.Chunks
{
    public readonly struct Layer
    {
        public readonly LayerFlags Flags;
        public readonly LayerType Type;
        public readonly ushort ChildLevel;
        public readonly LayerBlendMode BlendMode;
        public readonly byte Opacity;
        public readonly string Name;
        public readonly uint TilesetIndex;

        public Layer(BinaryReader reader)
        {
            Flags = (LayerFlags) reader.ReadWord();
            Type = (LayerType) reader.ReadWord();
            ChildLevel = reader.ReadWord();
            reader.ReadWord();
            reader.ReadWord();
            BlendMode = (LayerBlendMode) reader.ReadWord();
            Opacity = reader.ReadByte();
            reader.ReadBytes(3);
            Name = reader.ReadStr();
            TilesetIndex = Type.HasFlag(LayerType.Tilemap) ? reader.ReadDWord() : 0;
        }
    }
}