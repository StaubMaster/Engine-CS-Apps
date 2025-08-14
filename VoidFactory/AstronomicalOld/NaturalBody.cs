using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Engine3D;
using Engine3D.Abstract;
using Engine3D.Graphics;
using Engine3D.Noise;
using Engine3D.Entity;

namespace VoidFactory.Astronomical
{
    class NaturalBody
    {
        public TransBody Body;
        public Satellite Orbit;
        public double Radius;

        public uint Rock;

        public NaturalBody(BodyStatic body, Satellite orbit, double radius)
        {
            Body = new TransBody(body, false);
            Orbit = orbit;
            Radius = radius;

            Rock = (uint)(radius * radius * radius);

            Body.Trans = Orbit.Trans;
        }


        public bool Intersekt(Ray ray, ref double dist)
        {
            double d = Body.Intersekt(ray);
            if (Ray.IsPositive(d) && d < dist)
            {
                dist = d;
                return true;
            }
            return false;
        }

        public void Update()
        {
            Orbit.Update();
        }
    }
}
