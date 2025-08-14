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
    class SatMiner
    {
        public TransBody Body;
        public NaturalBody Source;
        public Satellite Orbit;

        public uint Rock;

        public SatMiner(BodyStatic body, NaturalBody source, Satellite orbit)
        {
            Body = new TransBody(body, false);
            Source = source;
            Orbit = orbit;
            Rock = 0;

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
            Body.Trans = Orbit.Trans;

            if (Source.Rock != 0)
            {
                Source.Rock--;
                Rock++;
            }
        }
    }
}
