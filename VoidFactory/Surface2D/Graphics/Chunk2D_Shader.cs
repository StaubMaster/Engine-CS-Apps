using Engine3D.DataStructs;
using Engine3D.Graphics.Shader;

using OpenTK.Graphics.OpenGL4;

namespace VoidFactory.Surface2D.Graphics
{
    struct Chunk2D_Int : IData
    {
        public int Data;

        public Chunk2D_Int(int data)
        {
            Data = data;
        }

        public void ToUniform(params int[] locations)
        {
            GL.Uniform1(locations[0], 1, new int[] { Data });
        }
    }

    struct Chunk2D_Pos : IData
    {
        public int Y;
        public int X;
        public int C;

        public Chunk2D_Pos(int y, int x, int c)
        {
            Y = y;
            X = x;
            C = c;
        }

        public void ToUniform(params int[] locations)
        {
            GL.Uniform3(locations[0], 1, new int[] { Y, X, C });
        }
    }

    static class Chunk2D_Graphics
    {
        public static GenericShader Chunk_Shader;
        public static GenericDataUniform<Chunk2D_Int> UniChunk_TileSize;
        public static GenericDataUniform<Chunk2D_Int> UniChunk_TilesPerSide;
        public static GenericDataUniform<Chunk2D_Pos> UniChunk_Pos;
    }
}
