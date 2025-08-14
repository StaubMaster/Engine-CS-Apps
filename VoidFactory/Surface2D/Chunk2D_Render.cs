using System;

using OpenTK.Graphics.OpenGL4;

using Engine3D.Abstract3D;
using Engine3D.GraphicsOld;

namespace VoidFactory.Surface2D
{
    class Chunk2DProgram : ViewRenderProgram
    {
        private int Uni_Chunk_Pos;
        private int Uni_Tiles_Size;
        private int Uni_Tiles_Per_Side;

        private int Uni_Solar;

        public Chunk2DProgram(string name, string vert_file, string geom_file, string frag_file)
            : base(name, vert_file, geom_file, frag_file)
        {

        }

        public override void Create()
        {
            if (Program != -1) { return; }
            base.Create();

            Uni_Chunk_Pos = GL.GetUniformLocation(Program, "chunk_pos");
            Uni_Tiles_Size = GL.GetUniformLocation(Program, "tiles_size");
            Uni_Tiles_Per_Side = GL.GetUniformLocation(Program, "tiles_per_side");

            Uni_Solar = GL.GetUniformLocation(Program, "solar");
        }
        public override void Delete()
        {
            if (Program == -1) { return; }
            base.Delete();

            Uni_Chunk_Pos = -1;
            Uni_Tiles_Size = -1;
            Uni_Tiles_Per_Side = -1;

            Uni_Solar = -1;
        }

        public void UniTileSize(int tiles_size, int tiles_per_side)
        {
            Use();
            GL.Uniform1(Uni_Tiles_Size, tiles_size);
            GL.Uniform1(Uni_Tiles_Per_Side, tiles_per_side);
        }
        public void UniChunkIdx(int y, int x, int c)
        {
            Use();
            GL.Uniform3(Uni_Chunk_Pos, y, x, c);
        }
        public void UniSolar(Point3D p)
        {
            Use();
            GL.Uniform3(Uni_Solar, (float)p.Y, (float)p.X, (float)p.C);
        }
    }
    class Chunk2DBuffers : RenderBuffers
    {
        private int Buffer_Tiles;
        private int Tiles_Count;

        public Chunk2DBuffers() : base()
        {

        }

        public override void Create()
        {
            if (Buffer_Array != -1) { return; }
            Create("Chunk2D");

            Buffer_Tiles = GL.GenBuffer();
            Tiles_Count = 0;
        }
        public override void Delete()
        {
            if (Buffer_Array == -1) { return; }
            Delete("Chunk2D");

            GL.DeleteBuffer(Buffer_Tiles);
        }

        public void Tiles(Chunk2D.TileData[] tiles)
        {
            GL.BindVertexArray(Buffer_Array);

            GL.BindBuffer(BufferTarget.ArrayBuffer, Buffer_Tiles);
            GL.BufferData(BufferTarget.ArrayBuffer, tiles.Length * Chunk2D.TileData.Size, tiles, BufferUsageHint.DynamicDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribIPointer(0, 1, VertexAttribIntegerType.UnsignedInt, Chunk2D.TileData.Size, (IntPtr)Chunk2D.TileData.Size_Color);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribIPointer(1, 1, VertexAttribIntegerType.Int, Chunk2D.TileData.Size, (IntPtr)Chunk2D.TileData.Size_Height_Mid);

            GL.EnableVertexAttribArray(2);
            GL.VertexAttribIPointer(2, 4, VertexAttribIntegerType.Int, Chunk2D.TileData.Size, (IntPtr)Chunk2D.TileData.Size_Height_Corn);

            Tiles_Count = tiles.Length;
        }

        public override void Draw()
        {
            GL.BindVertexArray(Buffer_Array);

            GL.DrawArrays(PrimitiveType.Points, 0, Tiles_Count);
        }
    }
}
