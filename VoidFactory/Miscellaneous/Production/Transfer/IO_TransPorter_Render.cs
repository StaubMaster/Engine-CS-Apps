using System;

using OpenTK.Graphics.OpenGL4;

using Engine3D.Abstract3D;
using Engine3D.Graphics.Shader;

namespace VoidFactory.Production.Transfer
{
    partial class IO_TransPorter
    {
        public struct RenderData
        {
            public const int Size = sizeof(float) * 3 + sizeof(float) * 3;

            private float InnY;
            private float InnX;
            private float InnC;

            private float OutY;
            private float OutX;
            private float OutC;

            public RenderData(Point3D Inn, Point3D Out)
            {
                InnY = Inn.Y;
                InnX = Inn.X;
                InnC = Inn.C;

                OutY = Out.Y;
                OutX = Out.X;
                OutC = Out.C;
            }
        }
    }
    /*class TransPorterProgram : ViewRenderProgram
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
    }*/
    class TransPorterBuffer : BaseBuffer
    {
        private int Buffer_Data;
        private int Data_Count;

        public TransPorterBuffer() : base()
        {
            Buffer_Data = GL.GenBuffer();
            Data_Count = 0;
        }
        ~TransPorterBuffer()
        {
            Use();
            GL.DeleteBuffer(Buffer_Data);
        }

        public void Data(IO_TransPorter.RenderData[] data)
        {
            Engine3D.ConsoleLog.Log("Bind TransPorter");

            Use();

            int stride = IO_TransPorter.RenderData.Size;

            GL.BindBuffer(BufferTarget.ArrayBuffer, Buffer_Data);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * stride, data, BufferUsageHint.DynamicDraw);

            IntPtr offset = IntPtr.Zero;

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, offset);
            offset += sizeof(float) * 3;

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, offset);
            offset += sizeof(float) * 3;

            Data_Count = data.Length;
            Engine3D.ConsoleLog.Log("TransPorter Count: " + Data_Count);
        }

        public override void Draw()
        {
            Use();
            GL.DrawArrays(PrimitiveType.Points, 0, Data_Count);
        }
    }

}
