using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Engine3D.Abstract3D;
using VoidFactory.Production.Data;

namespace VoidFactory.Production.Buildings
{
    class BLD_Converter : BLD_Base
    {
        private DATA_Recipy Recipy;
        private uint Ticks;

        public BLD_Converter(Template temp, Transformation3D trans)
            : base(temp, trans)
        {
            Recipy = null;
            Ticks = 0;
        }

        public void SetRecipy(DATA_Recipy recipy)
        {
            if (recipy.RInn.Length == Inn.Length && recipy.ROut.Length == Out.Length)
            {
                Recipy = recipy;

                for (int i = 0; i < Inn.Length; i++)
                    Inn[i].Limit = Recipy.RInn[i].Num * 2;

                for (int o = 0; o < Out.Length; o++)
                    Out[o].Limit = Recipy.ROut[o].Num * 2;
            }
            else
            {
                Recipy = null;

                for (int i = 0; i < Inn.Length; i++)
                    Inn[i].Limit = 0;

                for (int o = 0; o < Out.Length; o++)
                    Out[o].Limit = 0;
            }

            Ticks = 0;
        }

        public override void Update()
        {
            if (Recipy != null && Ticks >= Recipy.Ticks)
            {
                if (Recipy != null && Recipy.Check(Inn, Out))
                    Recipy.Convert(Inn, Out);
                Ticks = 0;
            }

            if (Recipy != null && Recipy.Check(Inn, Out))
                Ticks++;
            else
                Ticks = 0;
        }

        public override string ToString()
        {
            string str = base.ToString();

            str += "\nTick  " + Ticks + "/";
            if (Recipy != null)
            {
                str += Recipy.Ticks;

                int i, o;
                Recipy.Check(Inn, Out, out i, out o);
                str += "\nI(" + i + ")";
                str += "\nO(" + o + ")";
            }

            str += "\n\nRecipy\n" + Recipy;

            return str;
        }



        public class Template : Template_Base
        {
            public BLD_Converter ToInstance(Transformation3D trans)
            {
                return new BLD_Converter(this, trans);
            }
        }
    }
}
