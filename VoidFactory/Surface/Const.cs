using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidFactory
{
    partial class Layered_Surface
    {
        public static class Const
        {
            public const uint Tile_Width = 16;

            public const uint Corners_Per_Side = 33;
            public const uint Tiles_Per_Side = Corners_Per_Side - 1;
            public const uint Corners_Pre_Surface = Corners_Per_Side * Corners_Per_Side;

            public const int Noise_Offset = -(int)(Corners_Per_Side >> 1);

            public const uint Cut_Lo = (Corners_Per_Side >> 1) - (Corners_Per_Side >> 2);
            public const uint Cut_Hi = (Corners_Per_Side >> 1) + (Corners_Per_Side >> 2);
        }
    }
}
