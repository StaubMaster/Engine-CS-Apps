using System.Collections.Generic;

using Engine3D.Abstract3D;
using Engine3D.GraphicsOld;
using Engine3D.OutPut.Shader;
using Engine3D.OutPut.Uniform.Specific;
using Engine3D.Graphics;

using VoidFactory.Production.Data;
using VoidFactory.Production.Transfer;
using VoidFactory.Inventory;

namespace VoidFactory.Production.Buildings
{
    abstract partial class BLD_Base
    {
        public class Collection
        {
            private List<BLD_Base> Buildings;
            public int Count()
            {
                return Buildings.Count;
            }

            public void Create()
            {
                Buildings = new List<BLD_Base>();
            }
            public void Delete()
            {
                Buildings = null;
            }

            public BLD_Base this[int idx]
            {
                get
                {
                    if (idx >= 0 && idx < Buildings.Count)
                        return Buildings[idx];
                    return null;
                }
            }

            public void Update()
            {
                for (int i = 0; i < Buildings.Count; i++)
                {
                    if (Buildings[i].ToRemove)
                    {
                        Buildings.RemoveAt(i);
                        i--;
                    }
                    else
                    {

                        Buildings[i].Update();
                    }
                }
            }
            public void DrawCost(Select_Building hover)
            {
                Inventory_Storage.DrawOut(Buildings[hover.Building_Idx].MaterialCost);
            }
            public string StringOf(int idx)
            {
                if (idx >= 0 && idx < Buildings.Count)
                    return Buildings[idx].ToString();
                return null;
            }

            public void Add(BLD_Base bld)
            {
                if (bld != null)
                    Buildings.Add(bld);
            }
            public void Sub(Select_Building select)
            {
                if (select.Valid)
                {
                    Buildings[select.Building_Idx].Remove();
                }
            }

            public Select_Building Select(Ray3D ray)
            {
                return Select_Building.Closest(ray, Buildings);
            }
            public IO_Port.Select_Port Port_Select(Ray3D ray)
            {
                return IO_Port.Select_Port.Closest(ray, Buildings);
            }
            public IO_TransPorter Port_Connect(IO_Port.Select_Port port1, IO_Port.Select_Port port2)
            {
                return Connect(port1, port2, Buildings);
            }
            public void RecipySet(Select_Building select, DATA_Recipy recipy)
            {
                if (select.Valid)
                {
                    BLD_Base bld = Buildings[select.Building_Idx];
                    if (bld.GetType() == typeof(BLD_Converter))
                    {
                        ((BLD_Converter)bld).SetRecipy(recipy);
                    }
                }
            }
        }
    }
}
