using System;

using Engine3D.Graphics.Shader;

using Engine3D.Miscellaneous;
using Engine3D.Miscellaneous.EntryContainer;

using OpenTK.Graphics.OpenGL4;

namespace VoidFactory.Surface2D.Graphics
{
    class Chunk2D_Static_Buffer : BaseBuffer
    {
        public readonly int Buffer_Data;

        public Chunk2D_Static_Buffer() : base()
        {
            Buffer_Data = GL.GenBuffer();
        }
        ~Chunk2D_Static_Buffer()
        {
            GL.DeleteBuffer(Buffer_Data);
        }

        public override void Draw()
        {
            throw new NotImplementedException();
        }
    }
    class Chunk2D_Buffer : BaseBuffer
    {
        private static Chunk2D_Static_Buffer SBuffer;
        public static EntryContainerFixed<Chunk2D.TileData> SBufferEntrys;
        private static bool BufferNeedsUpdate;

        public static void SCreate()
        {
            SBuffer = new Chunk2D_Static_Buffer();
            SBufferEntrys = new EntryContainerFixed<Chunk2D.TileData>(1_000_000, Chunk2D.TileData.Size);
            BufferNeedsUpdate = false;
        }
        public static void SDelete()
        {
            SBuffer = null;
            SBufferEntrys = null;
        }
        public static void SUpdate()
        {
            if (BufferNeedsUpdate)
            {
                Engine3D.ConsoleLog.Log("Chunk Bind Update");

                SBuffer.Use();

                GL.BindBuffer(BufferTarget.ArrayBuffer, SBuffer.Buffer_Data);
                GL.BufferData(BufferTarget.ArrayBuffer, SBufferEntrys.Data.Length * Chunk2D.TileData.Size, SBufferEntrys.Data, BufferUsageHint.DynamicDraw);

                GL.EnableVertexAttribArray(0);
                GL.VertexAttribIPointer(0, 1, VertexAttribIntegerType.UnsignedInt, Chunk2D.TileData.Size, (IntPtr)Chunk2D.TileData.Size_Color);

                GL.EnableVertexAttribArray(1);
                GL.VertexAttribIPointer(1, 1, VertexAttribIntegerType.Int, Chunk2D.TileData.Size, (IntPtr)Chunk2D.TileData.Size_Height_Mid);

                GL.EnableVertexAttribArray(2);
                GL.VertexAttribIPointer(2, 4, VertexAttribIntegerType.Int, Chunk2D.TileData.Size, (IntPtr)Chunk2D.TileData.Size_Height_Corn);

                BufferNeedsUpdate = false;
            }
        }
        private static void SChange()
        {
            BufferNeedsUpdate = true;
        }



        private EntryContainerFixed<Chunk2D.TileData>.Entry BufferEntry;

        public Chunk2D_Buffer() : base()
        {
            BufferEntry = null;
        }
        ~Chunk2D_Buffer()
        {

        }

        public void Bind(Chunk2D.TileData[] tiles)
        {
            if (BufferEntry != null)
            {
                BufferEntry.Dispose();
                BufferEntry = null;
            }

            BufferEntry = SBufferEntrys.Alloc(tiles.Length);
            if (BufferEntry == null)
            {
                Engine3D.ConsoleLog.Log("Chunk Bind Entry: null");
                return;
            }

            for (int i = 0; i < BufferEntry.Length; i++)
            {
                BufferEntry[i] = tiles[i];
            }
            SChange();
        }

        public override void Draw()
        {
            if (BufferEntry != null)
            {
                SBuffer.Use();
                GL.DrawArrays(PrimitiveType.Points, BufferEntry.Offset, BufferEntry.Length);
            }
        }
    }
}
