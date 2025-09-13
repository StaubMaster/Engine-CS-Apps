
using Engine3D.Abstract3D;
using Engine3D.GraphicsOld;

using VoidFactory.Production.Transfer;

namespace VoidFactory.Production.Data
{
    partial class DATA_Recipy
    {
        public readonly DATA_Buffer[] RInn;
        public readonly DATA_Buffer[] ROut;
        public readonly uint Ticks;
        public readonly string Ablt;
        public readonly string Cat;

        public DATA_Recipy(DATA_Buffer[] Inn, DATA_Buffer[] Out, uint ticks, string ablt, string cat)
        {
            RInn = Inn;
            ROut = Out;
            Ticks = ticks;
            Ablt = ablt;
            Cat = cat;
        }

        public static bool CanInnR(DATA_Buffer recipy, DATA_Buffer buffer)
        {
            return (DATA_Thing.CompareInn(recipy.Thing, buffer.Thing) && buffer.Num >= recipy.Num);
        }
        public static bool CanOutR(DATA_Buffer recipy, DATA_Buffer buffer)
        {
            return (DATA_Thing.CompareOut(recipy.Thing, buffer.Thing) && buffer.Num <= recipy.Num);
        }

        public static int RequiresInn(DATA_Buffer recipy, IO_Port[] ports)
        {
            for (int i = 0; i < ports.Length; i++)
            {
                if (CanInnR(recipy, ports[i].Buffer))
                    return i;
            }
            return -1;
        }
        public static int RequiresOut(DATA_Buffer recipy, IO_Port[] ports)
        {
            for (int o = 0; o < ports.Length; o++)
            {
                if (CanOutR(recipy, ports[o].Buffer))
                    return o;
            }
            return -1;
        }
 
        public bool Check(IO_Port[] Inn, IO_Port[] Out, out int checkInn, out int checkOut)
        {
            checkInn = 0;
            checkOut = 0;
            if (RInn.Length != Inn.Length || ROut.Length != Out.Length)
                return false;

            for (int i = 0; i < RInn.Length; i++)
            {
                if (RequiresInn(RInn[i], Inn) != -1)
                    checkInn++;
            }

            for (int o = 0; o < ROut.Length; o++)
            {
                //if (RequiresOut(ROut[o], Out) != -1)
                if (CanOutR(ROut[o], Out[o].Buffer))
                    checkOut++;
            }

            return (checkInn == RInn.Length && checkOut == ROut.Length);
        }
        public bool Check(IO_Port[] Inn, IO_Port[] Out)
        {
            return Check(Inn, Out, out _, out _);
        }

        public void Convert(IO_Port[] Inn, IO_Port[] Out)
        {
            if (RInn.Length != Inn.Length || ROut.Length != Out.Length)
                return;

            int r;

            for (int i = 0; i < RInn.Length; i++)
            {
                r = RequiresInn(RInn[i], Inn);
                Inn[r].tryOutA(RInn[i]);
            }

            for (int o = 0; o < ROut.Length; o++)
            {
                //r = RequiresOut(ROut[o], Out);
                //Out[r].Buffer.tryInn(ROut[o]);
                Out[o].tryInnA(ROut[o]);
            }
        }

        public void Draw(IconProgram program, double tick, float x, float y, float scale)
        {
            Angle3D wnk = new Angle3D(0, -0.5, 0);
            Point3D middle = new Point3D(x, y, 0);
            Point3D pos;
            float[] flt = new float[6];
            uint sum, idx;

            float icon_scale = scale * 2;

            sum = 0;
            for (int o = 0; o < ROut.Length; o++)
                sum += ROut[o].Num;
            idx = 0;
            for (int o = 0; o < ROut.Length; o++)
            {
                for (int n = 0; n < ROut[o].Num; n++)
                {
                    wnk.A = ((idx * Angle3D.Full) / sum) - tick;
                    wnk.FloatsSinCos(flt);

                    pos = (new Point3D(0.5f * scale, 0, 0) + wnk);
                    pos.Y /= 2.0f;
                    pos += middle;

                    program.UniRot(flt);

                    if (ROut[o].Thing != null)
                        ROut[o].Thing.Draw(program, (float)pos.Y, (float)pos.X, icon_scale);
                    else
                    {
                        //IO_Port.BodyError.DrawMain();
                        IO_Port.Bodys[(int)IO_Port.MetaBodyIndex.Error].DrawMain();
                    }

                    idx++;
                }
            }

            sum = 0;
            for (int i = 0; i < RInn.Length; i++)
                sum += RInn[i].Num;
            idx = 0;
            for (int i = 0; i < RInn.Length; i++)
            {
                for (int n = 0; n < RInn[i].Num; n++)
                {
                    wnk.A = ((idx * Angle3D.Full) / sum) + tick;
                    wnk.FloatsSinCos(flt);

                    pos = (new Point3D(1.0f * scale, 0, 0) + wnk);
                    pos.Y /= 2.0f;
                    pos += middle;

                    program.UniRot(flt);

                    if (RInn[i].Thing != null)
                        RInn[i].Thing.Draw(program, (float)pos.Y, (float)pos.X, icon_scale);
                    else
                    {
                        //IO_Port.BodyError.DrawMain();
                        IO_Port.Bodys[(int)IO_Port.MetaBodyIndex.Error].DrawMain();
                    }

                    idx++;
                }
            }
        }

        public override string ToString()
        {
            string str = "";
            str += "(" + Ablt + ")(" + Cat + ")\n";

            str += "Inn\n";
            for (int i = 0; i < RInn.Length; i++)
                str += "  " + RInn[i].ToString() + "\n";

            str += "Out\n";
            for (int o = 0; o < ROut.Length; o++)
                str += "  " + ROut[o].ToString() + "\n";

            str += "Ticks:" + Ticks;

            return str;
        }
    }
}
