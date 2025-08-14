using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Engine3D.Abstract;
using Engine3D.Entity;

namespace VoidFactory.Astronomical
{
    class SatelliteFloat : Satellite
    {
        private double OrbitDist;
        private AngledRotation Orbit;

        public SatelliteFloat(Satellite center, double spin_speed, double spin_ang1, double spin_ang2,
            double dist, double orbit_speed, double orbit_ang1, double orbit_ang2)
            : base(center, spin_speed, spin_ang1, spin_ang2)
        {
            OrbitDist = dist;
            Orbit = new AngledRotation(orbit_speed, orbit_ang1, orbit_ang2);

            Update();
        }

        public override void Update()
        {
            Trans.Pos = (new Punkt(0, 0, OrbitDist) + Orbit.Update()) + Center.Trans.Pos;
            Trans.Rot = Spin.Update();
        }
    }
}
