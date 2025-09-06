using System.Collections.Generic;

using Engine3D.Abstract3D;
using Engine3D.Entity;
using Engine3D.GraphicsOld;
using Engine3D.OutPut.Shader;
using Engine3D.OutPut.Uniform.Specific;
using Engine3D.Graphics;
using Engine3D.Graphics.Display3D;

using Engine3D.Miscellaneous;

using VoidFactory.Production.Data;
using VoidFactory.Production.Transfer;
using VoidFactory.Inventory;

namespace VoidFactory.Production.Buildings
{
    abstract partial class BLD_Base
    {
        /*
        public static BodyStatic[] Bodys;
        public static void BodysCreate()
        {
            for (int i = 0; i < Bodys.Length; i++)
            {
                Bodys[i].BufferCreate();
                Bodys[i].BufferFill();
            }
        }
        public static void BodysDelete()
        {
            for (int i = 0; i < Bodys.Length; i++)
            {
                Bodys[i].BufferDelete();
            }
            Bodys = null;
        }
        */


        public static PHEI[] Mains;
        public static void CreateMains(BodyStatic[] bodys)
        {
            Mains = new PHEI[bodys.Length];
            for (int i = 0; i < bodys.Length; i++)
            {
                Mains[i] = new PHEI(bodys[i].ToPolyHedra());
            }
        }
        public static void DeleteMains()
        {
            Mains = null;
        }


        public bool ToRemove;

        private uint Idx;
        private string Name;
        private string Ablt;
        private string Cat;

        private Transformation3D Trans;
        private EntryContainer<PHEIData>.Entry InstEntry;

        protected IO_Port[] Inn;
        protected IO_Port[] Out;

        private DATA_Cost MaterialCost;

        public BLD_Base(Template_Base temp, Transformation3D trans)
        {
            ToRemove = false;

            Idx = temp.Idx;
            Name = temp.Name;
            Ablt = temp.Processing;
            Cat = temp.Category;

            Trans = trans;

            InstEntry = Mains[Idx].Alloc(1);
            InstEntry[0] = new PHEIData(trans);
            Mains[Idx].TransUpdate();

            Inn = new IO_Port[temp.Inn.Length];
            for (int i = 0; i < Inn.Length; i++)
            {
                Inn[i] = new IO_Port(trans.TFore(temp.Inn[i]), false);
            }

            Out = new IO_Port[temp.Out.Length];
            for (int o = 0; o < Out.Length; o++)
            {
                Out[o] = new IO_Port(trans.TFore(temp.Out[o]), true);
            }

            MaterialCost = temp.Cost;
            Inventory_Storage.CostDeduct(MaterialCost);
        }
        ~BLD_Base()
        {
            InstEntry.Free();
            Mains[Idx].TransUpdate();
        }
        public void Remove()
        {
            if (!Inventory_Storage.CostCanRefund(MaterialCost))
                return;

            for (int i = 0; i < Inn.Length; i++)
                Inn[i].Remove();

            for (int o = 0; o < Out.Length; o++)
                Out[o].Remove();

            Inventory_Storage.CostRefund(MaterialCost);
            ToRemove = true;
        }

        public abstract void Update();
        public void Draw(TransUniProgram programMain, TransUniProgram programPort)
        {
            programMain.UniTrans(new RenderTrans(Trans));
            //Bodys[Idx].BufferDraw();
            Mains[Idx].Draw_Main();

            if (programPort != null)
            {
                for (int i = 0; i < Inn.Length; i++)
                    Inn[i].Draw(programPort);
                for (int o = 0; o < Out.Length; o++)
                    Out[o].Draw(programPort);
            }
        }
        public void Draw(CShaderTransformation program)
        {
            program.Use();
            program.Trans.Value(Trans);
            //Bodys[Idx].BufferDraw();
            Mains[Idx].Draw_Main();

            {
                for (int i = 0; i < Inn.Length; i++) { Inn[i].Draw(program); }
                for (int o = 0; o < Out.Length; o++) { Out[o].Draw(program); }
            }
        }
        public void Draw(BodyElemUniShader program)
        {
            //program.Use();
            //program.Trans.Value(Trans);
            //Bodys[Idx].BufferDraw();

            {
                //for (int i = 0; i < Inn.Length; i++)
                //    Inn[i].Draw(program);
                //for (int o = 0; o < Out.Length; o++)
                //    Out[o].Draw(program);
            }
        }



        public override string ToString()
        {
            string str = "";

            str += Name + "("+ Ablt + ")(" + Cat + ")[" + Idx + "]";

            str += "\nInn[" + Inn.Length + "]";
            for (int i = 0; i < Inn.Length; i++)
                str += "\n  " + Inn[i].ToString();

            str += "\nOut[" + Out.Length + "]";
            for (int o = 0; o < Out.Length; o++)
                str += "\n  " + Out[o].ToString();

            return str;
        }

        public static IO_TransPorter Connect(IO_Port.Select_Port Inn, IO_Port.Select_Port Out, List<BLD_Base> Buildings)
        {
            if (!Inn.Valid || !Out.Valid || !IO_Port.Select_Port.Inn_Out(ref Inn, ref Out))
            {
                return null;
            }

            if (!(0 <= Inn.Converter_Idx && Inn.Converter_Idx < Buildings.Count))
            {
                return null;
            }
            if (!(0 <= Out.Converter_Idx && Out.Converter_Idx < Buildings.Count))
            {
                return null;
            }

            BLD_Base InnConv = Buildings[Inn.Converter_Idx];
            BLD_Base OutConv = Buildings[Out.Converter_Idx];

            if (!(0 <= Inn.Port_Idx && Inn.Port_Idx < InnConv.Out.Length))
            {
                return null;
            }
            if (!(0 <= Out.Port_Idx && Out.Port_Idx < OutConv.Inn.Length))
            {
                return null;
            }

            IO_Port InnPort = InnConv.Out[Inn.Port_Idx];
            IO_Port OutPort = OutConv.Inn[Out.Port_Idx];

            if (InnPort.TransPorter != null)
            {
                return null;
            }
            if (OutPort.TransPorter != null)
            {
                return null;
            }

            return new IO_TransPorter(InnPort, OutPort);
        }


        public IO_Port.Select_Port Closest_Port(Ray3D ray)
        {
            IO_Port.Select_Port closest = new IO_Port.Select_Port();
            IO_Port.Select_Port temp = new IO_Port.Select_Port();

            Intersekt.RayInterval t;
            Point3D p;

            closest.Reset();

            for (int i = 0; i < Inn.Length; i++)
            {
                if (Inn[i].TransPorter != null)
                    continue;

                temp.Pos = +Inn[i].Pos;
                t = Intersekt.Ray_Point(ray, temp.Pos);
                p = t.Pos;

                temp.Ray_Dist = (p - temp.Pos).Len;
                temp.Valid = true;
                if (temp.IsCloser(closest))
                {
                    temp.InnOut = false;
                    temp.Port_Idx = i;
                    closest = temp;
                }
            }

            for (int o = 0; o < Out.Length; o++)
            {
                if (Out[o].TransPorter != null)
                    continue;

                temp.Pos = +Out[o].Pos;
                t = Intersekt.Ray_Point(ray, temp.Pos);
                p = t.Pos;

                temp.Ray_Dist = (p - temp.Pos).Len;
                temp.Valid = true;
                if (temp.IsCloser(closest))
                {
                    temp.Pos = +Out[o].Pos;
                    temp.InnOut = true;
                    temp.Port_Idx = o;
                    closest = temp;
                }
            }

            return closest;
        }
        public Select_Building RaySelect(Ray3D ray)
        {
            Select_Building selected = new Select_Building();

            Intersekt.RayInterval t;
            Point3D p;

            selected.Pos = +Trans.Pos;
            t = Intersekt.Ray_Point(ray, selected.Pos);
            p = t.Pos;

            selected.Ray_Dist = (p - selected.Pos).Len;
            selected.Valid = true;

            return selected;
        }


        public struct Select_Building
        {
            public bool Valid;

            public int Building_Idx;
            public double Ray_Dist;

            public Point3D Pos;

            public void Reset()
            {
                Valid = false;

                Building_Idx = -1;
                Ray_Dist = double.NaN;

                //Pos = null;
            }

            public bool IsCloser(Select_Building other)
            {
                if (Valid && other.Valid)
                    return (Ray_Dist < other.Ray_Dist);
                return Valid;
            }

            public static Select_Building Closest(Ray3D ray, List<BLD_Base> converter)
            {
                Select_Building closest = new Select_Building();
                Select_Building temp;

                closest.Reset();

                for (int c = 0; c < converter.Count; c++)
                {
                    temp = converter[c].RaySelect(ray);
                    if(temp.IsCloser(closest))
                    {
                        temp.Building_Idx = c;
                        closest = temp;
                    }
                }

                return closest;
            }

            public override string ToString()
            {
                string str = "";

                str += "Conv [idx]" + Building_Idx + "\n";

                str += "RayDist " + Ray_Dist;

                return str;
            }
        }
    }
}
