using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.Runtime.InteropServices;

namespace Renderer.Core
{
    [StructLayout(LayoutKind.Sequential)]
    struct VERTEX
    {
        public Vector3 pos;        // vertex untransformed position
        public uint color;         // diffuse color
        public Vector2 texPos;     // texture relative coordinates
    };
}
