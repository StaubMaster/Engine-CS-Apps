using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Engine3D;
using Engine3D.Abstract3D;
using Engine3D.Graphics;
using Engine3D.GraphicsOld;
using Engine3D.Noise;
using Engine3D.Entity;

namespace VoidFactory.Astronomical
{
    class CSatelliteMiner
    {
        public PolyHedra Body;
        public BodyElemBuffer Buffer;
        public CSatelliteNatural Source;
        public AOrbit Orbit;

        public uint Rock;

        public CSatelliteMiner(PolyHedra body, CSatelliteNatural source, AOrbit orbit)
        {
            Body = body;
            Buffer = body.ToBuffer();

            Source = source;
            Orbit = orbit;
            Rock = 0;
        }

        public void Update()
        {
            Orbit.Update();

            if (Source.Rock != 0)
            {
                Source.Rock--;
                Rock++;
            }
        }
    }
}
