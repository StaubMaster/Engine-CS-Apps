using System;

using Engine3D.Abstract3D;
using Engine3D.GraphicsOld;

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
