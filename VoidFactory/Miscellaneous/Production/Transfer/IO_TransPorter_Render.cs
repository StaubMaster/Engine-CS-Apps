using System;

using OpenTK.Graphics.OpenGL4;

using Engine3D.Abstract3D;
using Engine3D.GraphicsOld;

namespace VoidFactory.Production.Transfer
{
    partial class IO_TransPorter
    {
        public struct RenderData
        {
            public const int Size_Inn = 0;
            public const int Size_Out = Size_Inn + sizeof(float) * 3;
            public const int Size = Size_Out + sizeof(float) * 3;

            private float InnY;
            private float InnX;
            private float InnC;

            private float OutY;
            private float OutX;
            private float OutC;

            public RenderData(Point3D Inn, Point3D Out)
            {
                InnY = (float)Inn.Y;
                InnX = (float)Inn.X;
                InnC = (float)Inn.C;

                OutY = (float)Out.Y;
                OutX = (float)Out.X;
                OutC = (float)Out.C;
            }
        }
    }

    class TransPorterProgram : ViewRenderProgram
    {
        public TransPorterProgram(string name, string vert_file, string geom_file, string frag_file)
            : base(name, vert_file, geom_file, frag_file)
        {

        }

        public override void Create()
        {
            if (Program != -1) { return; }
            base.Create();
        }
        public override void Delete()
        {
            if (Program == -1) { return; }
            base.Delete();
        }
    }
    class TransPorterBuffer : RenderBuffers
    {
        private int Buffer_Data;
        private int Data_Count;

        public TransPorterBuffer() : base()
        {

        }

        public override void Create()
        {
            if (Buffer_Array != -1) { return; }
            Create("TransPorter");

            Buffer_Data = GL.GenBuffer();
            Data_Count = 0;
        }
        public override void Delete()
        {
            if (Buffer_Array == -1) { return; }
            Delete("TransPorter");

            GL.DeleteBuffer(Buffer_Data);
        }


        public void Data(IO_TransPorter.RenderData[] data)
        {
            GL.BindVertexArray(Buffer_Array);

            GL.BindBuffer(BufferTarget.ArrayBuffer, Buffer_Data);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * IO_TransPorter.RenderData.Size, data, BufferUsageHint.DynamicDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, IO_TransPorter.RenderData.Size, (IntPtr)IO_TransPorter.RenderData.Size_Inn);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, IO_TransPorter.RenderData.Size, (IntPtr)IO_TransPorter.RenderData.Size_Out);

            Data_Count = data.Length;
        }

        public override void Draw()
        {
            GL.BindVertexArray(Buffer_Array);

            GL.DrawArrays(PrimitiveType.Points, 0, Data_Count);
        }
    }

}
