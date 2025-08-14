using System;
using System.Collections.Generic;

using Engine3D;
using Engine3D.Abstract3D;
using Engine3D.OutPut;
using Engine3D.OutPut.Shader;
using Engine3D.OutPut.Uniform;
using Engine3D.OutPut.Uniform.Specific;
using Engine3D.OutPut.Uniform.Generic.Float;
using Engine3D.BodyParse;
using Engine3D.Graphics.Display;

using OpenTK.Windowing.GraphicsLibraryFramework;

namespace VoidFactory.GameSelect
{
    class Plane
    {
        private CShaderTransformation ShaderDefault;

        private SUniformLocation IndicatorUniColor;
        private SUniformLocation IndicatorUniColorInterPol;
        private CUniformColor IndicatorColor;
        private CUniformInterPolate IndicatorColorInterPol;

        private void Frame()
        {
            TMovement.FlatX(ref MainCamera.Trans, MainWindow.MoveByKeys(0.1, 10), MainWindow.SpinByMouse());
            MainCamera.Update(MainWindow.MouseRay());


        }





        private DisplayArea MainWindow;
        private DisplayCamera MainCamera;

        public Plane()
        {
            MainWindow = new DisplayArea(1000, 1000, null, Frame);
            MainCamera = new DisplayCamera();
            MainCamera.Depth.Far = 10000.0f;

            

            MainWindow.Run();
            MainWindow.Term();
        }
    }
}
