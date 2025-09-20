using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Engine3D;
using Engine3D.Abstract3D;
using Engine3D.Graphics;
using Engine3D.Graphics.PolyHedraInstance.PH_3D;
using Engine3D.Noise;
using Engine3D.Entity;

namespace VoidFactory.Astronomical
{
    class CSatelliteMiner
    {
        public PolyHedraInstance_3D_Array.Entry InstData;

        public CSatelliteNatural Source;
        public AOrbit Orbit;

        public uint Rock;

        public CSatelliteMiner(PolyHedraInstance_3D_Array.Entry inst_data, CSatelliteNatural source, AOrbit orbit)
        {
            InstData = inst_data;

            Source = source;
            Orbit = orbit;
            Rock = 0;

            InstData[0] = new PolyHedraInstance_3D_Data(Orbit.Trans);
        }

        public void Update()
        {
            Orbit.Update();

            if (Source.Rock != 0)
            {
                Source.Rock--;
                Rock++;
            }

            PolyHedraInstance_3D_Data data;
            data = InstData[0];
            data.Trans = Orbit.Trans;
            InstData[0] = data;
        }
    }
}
