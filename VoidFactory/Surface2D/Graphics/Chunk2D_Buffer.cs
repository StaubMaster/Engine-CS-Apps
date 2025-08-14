using System;

using Engine3D.Graphics.Shader;
using Engine3D.Graphics.Shader.Uniform.Float;
using Engine3D.Graphics.Shader.Uniform.Int;
using Engine3D.Graphics.Shader.Uniform;

using OpenTK.Graphics.OpenGL4;

namespace VoidFactory.Surface2D.Graphics
{
    class Chunk2D_Buffer : BaseBuffer
    {
        private readonly int Buffer_Data;
        private int Count;

        public Chunk2D_Buffer() : base()
        {
            Buffer_Data = GL.GenBuffer();
        }
        ~Chunk2D_Buffer()
        {
            GL.DeleteBuffer(Buffer_Data);
        }

        public void Bind(Chunk2D.TileData[] tiles)
        {
            Use();

            GL.BindBuffer(BufferTarget.ArrayBuffer, Buffer_Data);
            GL.BufferData(BufferTarget.ArrayBuffer, tiles.Length * Chunk2D.TileData.Size, tiles, BufferUsageHint.DynamicDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribIPointer(0, 1, VertexAttribIntegerType.UnsignedInt, Chunk2D.TileData.Size, (IntPtr)Chunk2D.TileData.Size_Color);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribIPointer(1, 1, VertexAttribIntegerType.Int, Chunk2D.TileData.Size, (IntPtr)Chunk2D.TileData.Size_Height_Mid);

            GL.EnableVertexAttribArray(2);
            GL.VertexAttribIPointer(2, 4, VertexAttribIntegerType.Int, Chunk2D.TileData.Size, (IntPtr)Chunk2D.TileData.Size_Height_Corn);

            Count = tiles.Length;
        }

        public override void Draw()
        {
            Use();
            GL.DrawArrays(PrimitiveType.Points, 0, Count);
        }
    }
}
