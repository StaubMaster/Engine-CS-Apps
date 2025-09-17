using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Engine3D.Abstract3D;

namespace VoidFactory.Astronomical
{
    abstract class AOrbit
    {
        public Transformation3D Trans;
        protected SAngledRotation Spin;
        public AOrbit Center;

        protected AOrbit(AOrbit center, SAngledRotation spin)
        {
            Center = center;
            Spin = spin;
            Trans = Transformation3D.Default();
        }

        public abstract void Update();
    }
}
