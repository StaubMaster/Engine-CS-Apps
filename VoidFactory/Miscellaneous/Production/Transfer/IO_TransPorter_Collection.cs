using System.Collections.Generic;

using Engine3D.Graphics;

namespace VoidFactory.Production.Transfer
{
    partial class IO_TransPorter
    {
        public class Collection
        {
            public static Engine3D.Graphics.Shader.GenericShader Shader;
            //public TransPorterProgram Program;
            public TransPorterBuffer Buffer;

            private List<IO_TransPorter> TransPorter;
            private List<RenderData> Render;

            public int Count()
            {
                return TransPorter.Count;
            }

            public void Create()
            {
                //Program.Create();

                Buffer = new TransPorterBuffer();

                TransPorter = new List<IO_TransPorter>();
                Render = new List<RenderData>();
            }
            public void Delete()
            {
                //Program.Delete();

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

                //Program.Use();
                Shader.Use();
                Buffer.Draw();
            }

            public void Add(IO_TransPorter porter)
            {
                if (porter != null)
                {
                    TransPorter.Add(porter);
                    Engine3D.ConsoleLog.Log("TransPorter: " + porter.Inn.Pos.ToString_Line() + " | " + porter.Out.Pos.ToString_Line());
                    Render.Add(new RenderData(porter.Inn.Pos, porter.Out.Pos));
                    Buffer.Data(Render.ToArray());
                }
            }
        }
    }
}
