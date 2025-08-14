using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Engine3D.Abstract;
using Engine3D.Entity;

namespace VoidFactory.Astronomical
{
    class SatelliteFixed : Satellite
    {
        private Punkt Relative;

        public SatelliteFixed(Satellite center, double spin_speed, double spin_ang1, double spin_ang2, Punkt rel)
            : base(center, spin_speed, spin_ang1, spin_ang2)
        {
            Relative = rel;

            Update();
        }

        public override void Update()
        {
            Trans.Pos = Center.Trans.TFore(Relative);
            //Trans.Dir = Center.Trans.Pos - Trans.Pos;
            Trans.Rot = (Trans.Rot + Winkl.Xp) + Spin.Update();
        }
    }
}
