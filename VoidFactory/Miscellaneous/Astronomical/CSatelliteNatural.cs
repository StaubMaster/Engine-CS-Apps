using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Engine3D;
using Engine3D.Abstract3D;
using Engine3D.Graphics;
using Engine3D.Graphics.PolyHedraInstance.PH_3D;

namespace VoidFactory.Astronomical
{
    class CSatelliteNatural
    {
        public PolyHedraInstance_3D_Array.Entry InstData;

        public AOrbit Orbit;
        public double Radius;

        public uint Rock;

        public CSatelliteNatural(PolyHedraInstance_3D_Array.Entry inst_data, AOrbit orbit, double radius)
        {
            InstData = inst_data;

            Orbit = orbit;
            Radius = radius;
            Rock = (uint)(radius * radius * radius);

            InstData[0] = new PolyHedraInstance_3D_Data(Orbit.Trans);
        }

        public void Update()
        {
            Orbit.Update();

            PolyHedraInstance_3D_Data data;
            data = InstData[0];
            data.Trans = Orbit.Trans;
            InstData[0] = data;
        }
    }
}
