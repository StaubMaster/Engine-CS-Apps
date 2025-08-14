using System;

using Engine3D.Abstract3D;
using Engine3D.GraphicsOld;
using Engine3D.OutPut.Shader;
using Engine3D.OutPut.Uniform.Specific;

using VoidFactory.Production.Data;
using VoidFactory.Production.Transfer;
using VoidFactory.Inventory;
using VoidFactory.Surface2D;

namespace VoidFactory.Production.Buildings
{
    abstract partial class BLD_Base
    {
        public abstract class Template_Base
        {
            public uint Idx;
            public string Name;
            public string Processing;
            public string Category;

            public Point3D[] Inn;
            public Point3D[] Out;

            public DATA_Cost Cost;

            public Template_Base()
            {

            }


            public void Draw()
            {
                Bodys[Idx].BufferDraw();
            }
            public void Draw(TransUniProgram progMain, TransUniProgram progPort, Transformation3D trans)
            {
                progMain.UniTrans(new RenderTrans(trans));
                Bodys[Idx].BufferDraw();

                if (progPort != null)
                {
                    for (int i = 0; i < Inn.Length; i++)
                    {
                        progPort.UniTrans(new RenderTrans(trans.TFore(Inn[i])));
                        IO_Port.BodyInn.Draw();
                    }

                    for (int o = 0; o < Out.Length; o++)
                    {
                        progPort.UniTrans(new RenderTrans(trans.TFore(Out[o])));
                        IO_Port.BodyOut.Draw();
                    }
                }
            }
            public void Draw(CShaderTransformation progMain, CShaderTransformation progPort, Transformation3D trans)
            {
                progMain.Trans.Value(trans);
                Bodys[Idx].BufferDraw();

                if (progPort != null)
                {
                    for (int i = 0; i < Inn.Length; i++)
                    {
                        progPort.Trans.Value(new Transformation3D(trans.TFore(Inn[i])));
                        IO_Port.BodyInn.Draw();
                    }

                    for (int o = 0; o < Out.Length; o++)
                    {
                        progPort.Trans.Value(new Transformation3D(trans.TFore(Out[o])));
                        IO_Port.BodyOut.Draw();
                    }
                }
            }

            public override string ToString()
            {
                string str = "";

                str += Name + "(" + Processing + ")(" + Category + ")[" + Idx + "]";
                str += "\nInn[" + Inn.Length + "]";
                str += "\nOut[" + Out.Length + "]";

                return str;
            }
        }
    }
}
