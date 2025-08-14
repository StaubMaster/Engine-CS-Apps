using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Engine3D.Abstract3D;
using Engine3D.Entity;

namespace VoidFactory.Astronomical
{
    class COrbitHover : AOrbit
    {
        private Point3D Relative;

        public COrbitHover(AOrbit center, SAngledRotation spin, Point3D rel)
            : base(center, spin)
        {
            Relative = rel;
        }

        public override void Update()
        {
            if (Center != null)
            {
                Trans.Pos = Center.Trans.TFore(Relative);
                //Trans.Dir = Center.Trans.Pos - Trans.Pos;
                //Trans.Rot = (Trans.Rot + Winkl.Xp) + Spin.Update();
                //Trans.Rot = Center.Trans.TFore(RelativeRot);
                Trans.Rot = (new Angle3D(Center.Trans.Pos - Trans.Pos) + Angle3D.Xp) + Spin.Update();
            }
            else
            {
                Trans.Pos = Relative;
                Trans.Rot = Spin.Update();
            }
        }
    }
}
