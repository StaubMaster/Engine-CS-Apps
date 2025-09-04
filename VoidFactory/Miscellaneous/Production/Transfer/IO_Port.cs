using System.Collections.Generic;

using Engine3D.Abstract3D;
using Engine3D.Entity;
using Engine3D.GraphicsOld;
using Engine3D.OutPut.Shader;
using Engine3D.OutPut.Uniform.Specific;
using Engine3D.Graphics;
using Engine3D.Graphics.Display3D;
using Engine3D.Graphics.Display;

using Engine3D.Miscellaneous;

using VoidFactory.Production.Data;
using VoidFactory.Production.Buildings;
using VoidFactory.Inventory;

namespace VoidFactory.Production.Transfer
{
    class IO_Port
    {
        public static DisplayPolyHedra BodyError;
        public static DisplayPolyHedra BodyTransPorter;
        public static DisplayPolyHedra BodyAxis;

        public static DisplayPolyHedra BodyHex;
        public static DisplayPolyHedra BodyOct;

        public static DisplayPolyHedra BodyInn;
        public static DisplayPolyHedra BodyInnHex;
        public static DisplayPolyHedra BodyInnOct;

        public static DisplayPolyHedra BodyOut;
        public static DisplayPolyHedra BodyOutHex;
        public static DisplayPolyHedra BodyOutOct;



        public static PHEIBuffer InstMainInn;
        public static PHEIBuffer InstMainOut;

        public static EntryContainer<PHEIData> InstDataInn;
        public static EntryContainer<PHEIData> InstDataOut;



        public readonly Point3D Pos;
        public readonly bool InnOut;
        public readonly EntryContainer<PHEIData>.Entry InstEntry;

        public DATA_Buffer Buffer;
        public uint Limit;
        public IO_TransPorter TransPorter;

        public IO_Port(Point3D pos, bool io)
        {
            Pos = pos;
            InnOut = io;

            if (!InnOut)
            {
                InstEntry = InstDataInn.Alloc(1);
            }
            else
            {
                InstEntry = InstDataOut.Alloc(1);
            }

            InstEntry[0] = new PHEIData(new Transformation3D(pos));

            if (!InnOut)
            {
                InstMainInn.Bind_InstTrans(InstDataInn.Data);
            }
            else
            {
                InstMainOut.Bind_InstTrans(InstDataOut.Data);
            }

            Buffer = new DATA_Buffer();
            Limit = 0;
            TransPorter = null;
        }
        ~IO_Port()
        {
            InstEntry.Free();
            if (!InnOut)
            {
                InstMainInn.Bind_InstTrans(InstDataInn.Data);
            }
            else
            {
                InstMainOut.Bind_InstTrans(InstDataOut.Data);
            }
        }
        public void Remove()
        {
            DATA_Cost cost = new DATA_Cost(Buffer.Thing, Buffer.Num);
            Inventory_Storage.CostRefund(cost);

            if (TransPorter != null)
            {
                TransPorter.Remove();
            }
        }

        public bool canInn()
        {
            return (Buffer.Num < Limit);
        }
        public bool canOut()
        {
            return (Buffer.Num > 0);
        }

        public void tryInnS(ref DATA_Thing thing)
        {
            if (Buffer.Num < Limit)
                Buffer.tryInnS(ref thing);
        }
        public void tryOutS(ref DATA_Thing thing)
        {
            if (Buffer.Num > 0)
                Buffer.tryOutS(ref thing);
        }

        public void tryInnA(DATA_Buffer buffer)
        {
            Buffer.tryInnA(buffer);
        }
        public void tryOutA(DATA_Buffer buffer)
        {
            Buffer.tryOutA(buffer);
        }

        public void Draw(TransUniProgram program)
        {
            if (TransPorter != null) { return; }

            program.UniTrans(new RenderTrans(Pos));
            if (!InnOut)
                BodyInn.Draw();
            else
                BodyOut.Draw();
        }
        public void Draw(CShaderTransformation program)
        {
            if (TransPorter != null) { return; }

            program.Trans.Value(new Transformation3D(Pos));
            if (!InnOut)
            {
                BodyInn.Draw();
            }
            else
            {
                BodyOut.Draw();
            }
        }
        public void Draw(BodyElemUniShader program)
        {
            if (TransPorter != null) { return; }

            program.Use();
            program.Trans.Value(new Transformation3D(Pos));
            program.LightRange.Value(1.0f, 1.0f);

            if (!InnOut)
            {
                BodyInn.Draw();
            }
            else
            {
                BodyOut.Draw();
            }

            program.LightRange.Value(0.1f, 1.0f);
        }

        public override string ToString()
        {
            return (Buffer.Num + "/" + Limit + " " + Buffer.Thing);
        }



        public struct Select_Port
        {
            public bool Valid;

            public int Converter_Idx;
            public int Port_Idx;
            public double Ray_Dist;
            public bool InnOut;

            public Point3D Pos;

            public void Reset()
            {
                Valid = false;

                Converter_Idx = -1;
                Port_Idx = -1;
                Ray_Dist = double.NaN;
                InnOut = false;

                //Pos = null;
            }

            public bool IsSame(Select_Port other)
            {
                return (
                    Converter_Idx == other.Converter_Idx &&
                    Port_Idx == other.Port_Idx &&
                    InnOut && other.InnOut
                    );
            }
            public bool IsCloser(Select_Port other)
            {
                if (Valid && other.Valid)
                    return (Ray_Dist < other.Ray_Dist);
                return Valid;
            }
            public bool CanConnect(Select_Port other)
            {
                if (!Valid || !other.Valid)
                    return false;

                if (Converter_Idx == other.Converter_Idx)
                    return false;

                if (InnOut == other.InnOut)
                    return false;

                return true;
            }

            public override string ToString()
            {
                string str = "";

                str += "Conv [idx] " + Converter_Idx + "\n";

                str += "Port [idx] " + Port_Idx + "\n";

                str += "IO ";
                if (Valid)
                {
                    if (!InnOut)
                        str += "Inn";
                    else
                        str += "Out";
                }
                else
                {
                    str += "N/A";
                }
                str += "\n";

                str += "RayDist " + Ray_Dist;

                return str;
            }
            public void Draw_Hover(TransUniProgram program)
            {
                if (!Valid) { return; }

                program.UniTrans(new RenderTrans(Pos));

                if (!InnOut)
                    BodyInnHex.Draw();
                else
                    BodyOutHex.Draw();
            }
            public void Draw_Select(TransUniProgram program)
            {
                if (!Valid) { return; }

                program.UniTrans(new RenderTrans(Pos));

                if (!InnOut)
                    BodyInnOct.Draw();
                else
                    BodyOutOct.Draw();
            }
            public void Draw_Hover(CShaderTransformation program)
            {
                if (!Valid) { return; }

                program.Trans.Value(new Transformation3D(Pos));

                if (!InnOut)
                    BodyInnHex.Draw();
                else
                    BodyOutHex.Draw();
            }
            public void Draw_Select(CShaderTransformation program)
            {
                if (!Valid) { return; }

                program.Trans.Value(new Transformation3D(Pos));

                if (!InnOut)
                    BodyInnOct.Draw();
                else
                    BodyOutOct.Draw();
            }
            public void Draw_Hover(BodyElemUniShader program)
            {
                if (!Valid) { return; }

                program.Use();
                program.Trans.Value(new Transformation3D(Pos));
                program.LightRange.Value(1.0f, 1.0f);

                if (!InnOut)
                {
                    BodyInnHex.Draw();
                }
                else
                {
                    BodyOutHex.Draw();
                }

                program.LightRange.Value(0.1f, 1.0f);
            }
            public void Draw_Select(BodyElemUniShader program)
            {
                if (!Valid) { return; }

                program.Use();
                program.Trans.Value(new Transformation3D(Pos));
                program.LightRange.Value(1.0f, 1.0f);

                if (!InnOut)
                {
                    BodyInnOct.Draw();
                }
                else
                {
                    BodyOutOct.Draw();
                }

                program.LightRange.Value(0.1f, 1.0f);
            }

            public static bool Inn_Out(ref Select_Port Inn, ref Select_Port Out)
            {
                Select_Port temp_Inn = Inn;
                Select_Port temp_Out = Out;

                if (Inn.InnOut == Out.InnOut)
                    return false;

                if (Inn.InnOut && !Out.InnOut)
                {
                    Inn = temp_Inn;
                    Out = temp_Out;
                }
                else
                {
                    Inn = temp_Out;
                    Out = temp_Inn;
                }

                return true;
            }

            public static Select_Port Closest(Ray3D ray, List<BLD_Base> converter)
            {
                Select_Port closest = new Select_Port();
                Select_Port temp;

                closest.Reset();

                for (int c = 0; c < converter.Count; c++)
                {
                    temp = converter[c].Closest_Port(ray);
                    if (temp.IsCloser(closest))
                    {
                        temp.Converter_Idx = c;
                        closest = temp;
                    }
                }

                return closest;
            }
        }
    }
}
