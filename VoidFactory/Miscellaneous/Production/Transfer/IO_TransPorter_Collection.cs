using System.Collections.Generic;

using Engine3D.GraphicsOld;
using Engine3D.OutPut.Shader;
using Engine3D.Graphics;

namespace VoidFactory.Production.Transfer
{
    partial class IO_TransPorter
    {
        public class Collection
        {
            public TransPorterProgram Program;
            public TransPorterBuffer Buffer;

            private List<IO_TransPorter> TransPorter;
            private List<RenderData> Render;

            public int Count()
            {
                return TransPorter.Count;
            }

            public void Create()
            {
                Program.Create();

                Buffer = new TransPorterBuffer();
                Buffer.Create();

                TransPorter = new List<IO_TransPorter>();
                Render = new List<RenderData>();
            }
            public void Delete()
            {
                Program.Delete();

                Buffer.Delete();
                Buffer = null;

                TransPorter = null;
                Render = null;
            }

            public void Update()
            {
                for (int i = 0; i < TransPorter.Count; i++)
                {
                    if (TransPorter[i].ToRemove)
                    {
                        TransPorter.RemoveAt(i);

                        Render.RemoveAt(i);
                        Buffer.Data(Render.ToArray());

                        i--;
                    }
                    else
                    {
                        TransPorter[i].Update();
                    }
                }
            }
            public void Draw()
            {
                for (int i = 0; i < TransPorter.Count; i++)
                {
                    TransPorter[i].Draw();
                }

                Program.Use();
                Buffer.Draw();
            }

            public void Add(IO_TransPorter porter)
            {
                if (porter != null)
                {
                    TransPorter.Add(porter);
                    Render.Add(new RenderData(porter.Inn.Pos, porter.Out.Pos));
                    Buffer.Data(Render.ToArray());
                }
            }
        }
    }
}
