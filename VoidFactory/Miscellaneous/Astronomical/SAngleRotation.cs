using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Engine3D.Abstract3D;

namespace VoidFactory.Astronomical
{
    struct SAngledRotation
    {
        private double Current;
        private double Speed;
        private double Angle1;
        private double Angle2;

        public SAngledRotation(double speed, double ang1, double ang2)
        {
            Current = 0;
            Speed = speed;
            Angle1 = ang1;
            Angle2 = ang2;
        }

        public Angle3D Update()
        {
            Current += Speed;
            return new Angle3D(Current, 0, 0) - new Angle3D(0, Angle1, Angle2);
        }
    };
}
