using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Engine3D.Abstract;
using Engine3D.Graphics;
using Engine3D.Entity;

namespace VoidFactory.Astronomical
{
    class Satellite
    {
        protected struct AngledRotation
        {
            private double Current;
            private double Speed;
            private double Angle1;
            private double Angle2;

            public AngledRotation(double speed, double a1, double a2)
            {
                Current = 0;
                Speed = speed;
                Angle1 = a1;
                Angle2 = a2;
            }

            public Winkl Update()
            {
                Current += Speed;
                return new Winkl(Current, 0, 0) - new Winkl(0, Angle1, Angle2);
            }
        };

        public Transformation Trans;
        protected AngledRotation Spin;
        public Satellite Center;

        public Satellite(double spin_speed, double spin_ang1, double spin_ang2)
        {
            Center = null;
            Spin = new AngledRotation(spin_speed, spin_ang1, spin_ang2);
            Trans = new Transformation();

            Update();
        }
        protected Satellite(Satellite center, double spin_speed, double spin_ang1, double spin_ang2)
        {
            Center = center;
            Spin = new AngledRotation(spin_speed, spin_ang1, spin_ang2);
            Trans = new Transformation();
        }


        public virtual void Update()
        {
            Trans.Rot = Spin.Update();
        }

        public void Draw(TransBody body, TransUniProgram program)
        {
            body.Trans = Trans;
            body.Draw(program);
        }
    }
}
