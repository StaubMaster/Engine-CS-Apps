using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Engine3D;
using Engine3D.Abstract3D;
using Engine3D.Graphics;

namespace VoidFactory.Astronomical
{
    class CSatelliteNatural
    {
        public PolyHedra Body;
        public BodyElemBuffer Buffer;
        public AOrbit Orbit;
        public double Radius;

        public uint Rock;

        public CSatelliteNatural(PolyHedra body, AOrbit orbit, double radius)
        {
            Body = body;
            Buffer = body.ToBufferElem();

            Orbit = orbit;
            Radius = radius;

            Rock = (uint)(radius * radius * radius);
        }

        public void Update()
        {
            Orbit.Update();
        }
    }
}
