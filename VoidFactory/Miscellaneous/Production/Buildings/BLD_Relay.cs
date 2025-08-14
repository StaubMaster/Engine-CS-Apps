
using Engine3D.Abstract3D;
using VoidFactory.Production.Data;

namespace VoidFactory.Production.Buildings
{
    class BLD_Relay : BLD_Base
    {
        private DATA_Thing thing;
        private int InnIdx;
        private int OutIdx;

        public BLD_Relay(Template temp, Transformation3D trans)
            : base (temp, trans)
        {
            for (int i = 0; i < Inn.Length; i++)
                Inn[i].Limit = 1;

            for (int o = 0; o < Out.Length; o++)
                Out[o].Limit = 1;

            thing = null;
            InnIdx = 0;
            OutIdx = 0;
        }


        private void CycleInn()
        {
            for (int i = 0; i < Inn.Length; i++)
            {
                InnIdx = (InnIdx + 1) % Inn.Length;
                Inn[InnIdx].tryOutS(ref thing);

                if (thing != null)
                    return;
            }
        }
        private void CycleOut()
        {
            for (int o = 0; o < Out.Length; o++)
            {
                OutIdx = (OutIdx + 1) % Out.Length;
                Out[OutIdx].tryInnS(ref thing);

                if (thing == null)
                    return;
            }
        }
        public override void Update()
        {
            for (int i = 0; i < 4; i++)
            {
                if (thing == null)
                    CycleInn();
                if (thing == null)
                    return;

                if (thing != null)
                    CycleOut();
                if (thing != null)
                    return;
            }
        }

        public override string ToString()
        {
            string str = base.ToString();

            str += "\nThing:" + thing;

            return str;
        }



        public class Template : Template_Base
        {
            public BLD_Relay ToInstance(Transformation3D trans)
            {
                return new BLD_Relay(this, trans);
            }
        }
    }
}
