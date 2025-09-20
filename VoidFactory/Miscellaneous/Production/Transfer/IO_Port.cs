using System.Collections.Generic;

using Engine3D.Abstract3D;
using Engine3D.Entity;
using Engine3D.Graphics;
using Engine3D.Graphics.PolyHedraInstance.PH_3D;

using Engine3D.Miscellaneous.EntryContainer;

using VoidFactory.Production.Data;
using VoidFactory.Production.Buildings;
using VoidFactory.Inventory;

namespace VoidFactory.Production.Transfer
{
    class IO_Port
    {
        public enum MetaBodyIndex : int
        {
            Error,
            TransPorter,
            Axis,

            Hex,
            Oct,

            Inn,
            InnHex,
            InnOct,

            Out,
            OutHex,
            OutOct,
        };

        //public static PHEI_Array Bodys;
        public static GameSelect.Game3D game;





        public readonly Point3D Pos;
        public readonly bool InnOut;
        public readonly EntryContainerDynamic<PolyHedraInstance_3D_Data>.Entry InstEntry;

        public DATA_Buffer Buffer;
        public uint Limit;
        public IO_TransPorter TransPorter;

        public IO_Port(Point3D pos, bool io)
        {
            Pos = pos;
            InnOut = io;

            if (!InnOut)
            {
                //InstEntry = Bodys[(int)MetaBodyIndex.Inn].Alloc(1);
                InstEntry = game.PH_3D[(int)MetaBodyIndex.Inn].Alloc(1);
                InstEntry[0] = new PolyHedraInstance_3D_Data(new Transformation3D(pos));
            }
            else
            {
                //InstEntry = Bodys[(int)MetaBodyIndex.Out].Alloc(1);
                InstEntry = game.PH_3D[(int)MetaBodyIndex.Out].Alloc(1);
                InstEntry[0] = new PolyHedraInstance_3D_Data(new Transformation3D(pos));
            }

            Buffer = new DATA_Buffer();
            Limit = 0;
            TransPorter = null;
        }
        ~IO_Port()
        {
        }
        public void Remove()
        {
            DATA_Cost cost = new DATA_Cost(Buffer.Thing, Buffer.Num);
            Inventory_Storage.CostRefund(cost);

            InstEntry.Dispose();

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
            public void Draw_Hover()
            {
                if (!Valid) { return; }

                //program.Use();
                //program.Trans.Value(new Transformation3D(Pos));
                //program.LightRange.Value(1.0f, 1.0f);

                if (!InnOut)
                {
                    //BodyInnHex.DrawMain();
                    //Bodys[(int)MetaBodyIndex.InnHex].DrawMain();
                    //game.PH_3D[(int)MetaBodyIndex.InnHex].DrawMain();
                }
                else
                {
                    //BodyOutHex.DrawMain();
                    //Bodys[(int)MetaBodyIndex.OutHex].DrawMain();
                    //game.PH_3D[(int)MetaBodyIndex.OutHex].DrawMain();
                }

                //program.LightRange.Value(0.1f, 1.0f);
            }
            public void Draw_Select()
            {
                if (!Valid) { return; }

                //program.Use();
                //program.Trans.Value(new Transformation3D(Pos));
                //program.LightRange.Value(1.0f, 1.0f);

                if (!InnOut)
                {
                    //BodyInnOct.DrawMain();
                    //Bodys[(int)MetaBodyIndex.InnOct].DrawMain();
                    //game.PH_3D[(int)MetaBodyIndex.InnOct].DrawMain();
                }
                else
                {
                    //BodyOutOct.DrawMain();
                    //Bodys[(int)MetaBodyIndex.OutOct].DrawMain();
                    //game.PH_3D[(int)MetaBodyIndex.OutOct].DrawMain();
                }

                //program.LightRange.Value(0.1f, 1.0f);
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
