using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Engine3D.Abstract3D;
using Engine3D.Entity;

namespace VoidFactory.Astronomical
{
    class COrbitNormal : AOrbit
    {
        private double Dist;
        private SAngledRotation Circle;

        public COrbitNormal(AOrbit center, SAngledRotation spin,
            double dist, SAngledRotation circle) : base(center, spin)
        {
            Dist = dist;
            Circle = circle;
        }

        public override void Update()
        {
            Trans.Pos = (new Point3D(0, 0, (float)Dist) + Circle.Update()) + Center.Trans.Pos;
            Trans.Rot = Spin.Update();
        }
    }
}
