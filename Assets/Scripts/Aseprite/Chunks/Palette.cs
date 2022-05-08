using System.IO;
using System.Text;
using UnityEngine;

namespace Aseprite.Chunks
{
    public readonly struct Palette
    {
        public readonly uint NewPaletteSize;
        public readonly uint FirstColorIndexToChange;
        public readonly uint LastColorIndexToChange;

        public struct Entry
        {
            public readonly ushort Flags;
            public readonly Color32 Color;
            public readonly string ColorName;

            public Entry(BinaryReader reader)
            {
                Flags = reader.ReadWord();
                var r = reader.ReadByte();
                var g = reader.ReadByte();
                var b = reader.ReadByte();
                var a = reader.ReadByte();
                Color = new Color32(r, g, b, a);
                ColorName = (Flags & 1) != 0 ? reader.ReadStr() : string.Empty;
            }
        }

        public readonly Entry[] Entries;

        public Palette(BinaryReader reader)
        {
            NewPaletteSize = reader.ReadDWord();
            FirstColorIndexToChange = reader.ReadDWord();
            LastColorIndexToChange = reader.ReadDWord();
            reader.ReadBytes(8);

            var numEntries = LastColorIndexToChange - FirstColorIndexToChange + 1;
            Entries = new Entry[numEntries];
            for (var i = 0; i < numEntries; i++)
            {
                Entries[i] = new Entry(reader);
            }
        }

        public string DebugEntriesToString()
        {
            var sb = new StringBuilder();
            foreach (var entry in Entries)
            {
                sb.AppendLine($"<color=#{ColorUtility.ToHtmlStringRGB(entry.Color)}>{entry.Color}</color>");
            }

            return sb.ToString();
        }
    }
}