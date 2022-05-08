using System;
using UnityEngine;

namespace Aseprite
{
    [Serializable]
    public class AsepriteImportConfig
    {
        public bool alphaIsTransparency = true;
        public FilterMode filterMode = FilterMode.Point;
        public TextureWrapMode wrapMode = TextureWrapMode.Repeat;
        [Range(0, 16)] public int anisoLevel = 1;
    }
}