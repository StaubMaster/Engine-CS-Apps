using System;
using System.Collections.Generic;

using Engine3D;
using Engine3D.Abstract3D;

using Engine3D.OutPut;
using Engine3D.OutPut.Shader;
using Engine3D.OutPut.Uniform;
using Engine3D.OutPut.Uniform.Specific;
using Engine3D.Graphics;
using Engine3D.Graphics.Display;

using OpenTK.Windowing.GraphicsLibraryFramework;

namespace VoidFactory.Astronomical
{
    class GameSpace
    {
        private DisplayArea MainWindow;
        private DisplayCamera MainCamera;

        private CSatelliteNatural ViewHoverCenter;
        private Transformation3D ViewHover;


        private PolyHedra TestPoly;
        private BodyElemBuffer TestBuffer;

        private BodyElemUniShader Shader_Default;


        private bool UpdateSatellites;
        private List<CSatelliteNatural> Astro;
        private List<CSatelliteMiner> Miner;



        public GameSpace()
        {
            MainWindow = new DisplayArea(1000, 1000, CloseFunc, FrameFunc);
            MainWindow.ChangeColor(0, 0, 0);

            InitView();
            InitShaders();
            InitSatellites();

            TestPoly = PolyHedra.Generate.SphereCube(12, 10.0f);
            TestBuffer = TestPoly.ToBuffer();

            MainWindow.Run();
            MainWindow.Term();
        }

        private void InitView()
        {
            MainCamera = new DisplayCamera();
            MainCamera.Depth.Near = 0.1f;
            MainCamera.Depth.Far = 1600.0f;

            ViewHoverCenter = null;
            ViewHover = Transformation3D.Null();
        }
        private void InitShaders()
        {
            string shaderDir = "E:/Programmieren/VS_Code/OpenTK/Engine3D/Engine3D/Shaders/";

            Shader_Default = new BodyElemUniShader(shaderDir);
            Shader_Default.Depth.Value(MainCamera.Depth.Near, MainCamera.Depth.Far);
        }
        private void InitSatellites()
        {
            UpdateSatellites = true;

            PolyHedra sphere1 = PolyHedra.Generate.SphereCube(12, 200.0f);
            PolyHedra sphere2 = PolyHedra.Generate.SphereCube(12, 16.0f);
            PolyHedra sphere3 = PolyHedra.Generate.SphereCube(12, 4.0f);

            Astro = new List<CSatelliteNatural>();

            Astro.Add(new CSatelliteNatural(sphere1, new COrbitHover(null, new SAngledRotation(0.0009, 0, 0), Point3D.Default()), 200));
            Astro.Add(new CSatelliteNatural(sphere2, new COrbitNormal(Astro[0].Orbit, new SAngledRotation(0.001, 0.0, 0.0), 1000, new SAngledRotation(0.0001, 0.2, 0)), 16));
            Astro.Add(new CSatelliteNatural(sphere3, new COrbitNormal(Astro[1].Orbit, new SAngledRotation(0.008, 0.0, 0.0), 100, new SAngledRotation(0.008, 0, 0)), 4));

            Miner = new List<CSatelliteMiner>();
        }

        private Intersekt.RayInterval Select_Astro()
        {
            Intersekt.RayInterval func(Ray3D ray, CSatelliteNatural body)
            {
                return body.Body.Intersekt(ray, body.Orbit.Trans);
            }
            return Intersekt.Ray_Multiple(MainCamera.Ray, Astro, func);
        }
        private Intersekt.RayInterval Select_Miner()
        {
            Intersekt.RayInterval func(Ray3D ray, CSatelliteMiner body)
            {
                return body.Body.Intersekt(ray, body.Orbit.Trans);
            }
            return Intersekt.Ray_Multiple(MainCamera.Ray, Miner, func);
        }

        private void Update_View()
        {
            if (ViewHover.Is())
            {
                Transformation3D center = ViewHoverCenter.Orbit.Trans;
                Transformation3D temp = Transformation3D.Default();
                Angle3D perpRot;

                // Relative
                temp.Pos = ViewHover.Pos;
                temp.Rot = ViewHover.Rot;

                perpRot = new Angle3D(temp.Pos) + Angle3D.Xn;

                // Relative To Flat
                temp.Pos = temp.Pos + perpRot;
                temp.Rot = temp.Rot - perpRot.InvertMns();

                //temp.Rot.D = 0;
                TMovement.Unrestricted(ref temp, MainWindow.MoveByKeys(0.01, 100), MainWindow.SpinByMouse());

                // Flat To Relative
                temp.Pos = temp.Pos + perpRot.InvertMns();
                temp.Rot = temp.Rot - perpRot;

                ViewHover.Pos = temp.Pos;
                ViewHover.Rot = temp.Rot;

                // Relative To Absolut
                MainCamera.Trans.Pos = (temp.Pos - center.Rot) + center.Pos;
                MainCamera.Trans.Rot = (temp.Rot - center.Rot);
            }
            else
            {
                TMovement.Unrestricted(ref MainCamera.Trans, MainWindow.MoveByKeys(0.1, 100), MainWindow.SpinByMouse());
            }
            MainCamera.Update(MainWindow.MouseRay());

            Shader_Default.ScreenRatio.Value(MainWindow.Size_Float2());
            Shader_Default.View.Value(MainCamera.Trans);
        }
        private void InitViewHover(CSatelliteNatural nat)
        {
            Transformation3D center = nat.Orbit.Trans;
            Point3D pos = (MainCamera.Trans.Pos - center.Pos) + center.Rot;
            Angle3D rot = (MainCamera.Trans.Rot - center.Rot.InvertMns());

            ViewHoverCenter = nat;
            ViewHover = new Transformation3D(pos, rot);
        }
        private void FrameFunc()
        {
            //  Update
            UpdateSatellites = !System.Windows.Forms.Control.IsKeyLocked(System.Windows.Forms.Keys.Scroll);
            if (UpdateSatellites)
            {
                for (int i = 0; i < Astro.Count; i++) { Astro[i].Update(); }
                for (int i = 0; i < Miner.Count; i++) { Miner[i].Update(); }
            }
            Update_View();

            Intersekt.RayInterval interAstro = Select_Astro();
            Intersekt.RayInterval interMiner = Select_Miner();
            Intersekt.RayInterval inter = new Intersekt.RayInterval(MainCamera.Ray);
            if (interAstro.Is && interAstro.Interval < inter.Interval) { inter = interAstro; }
            if (interMiner.Is && interMiner.Interval < inter.Interval) { inter = interMiner; }

            if (MainWindow.MouseLocked && MainWindow.CheckKey(MouseButton.Left).IsPressed())
            {
                if (interAstro.Is && interAstro.Interval < interMiner.Interval)
                {
                    CSatelliteNatural nat = Astro[interAstro.Index];
                    Point3D rel = nat.Orbit.Trans.TBack(interAstro.Pos);
                    rel = rel * ((nat.Radius) / rel.Len);
                    Miner.Add(new CSatelliteMiner(PolyHedra.Generate.Cube(), nat, new COrbitHover(nat.Orbit, new SAngledRotation(0.01, 0, 0), rel)));
                }
                if (interMiner.Is && interMiner.Interval < interAstro.Interval)
                {
                    Miner.RemoveAt(interMiner.Index);
                }
            }

            if (MainWindow.CheckKey(Keys.T).IsPressed())
            {
                if (ViewHover.Is())
                {
                    if (interAstro.Is)
                    {
                        InitViewHover(Astro[interAstro.Index]);
                    }
                }
                else
                {
                    ViewHover = Transformation3D.Null();
                }
            }



            //  Draw
            Shader_Default.Use();

            for (int i = 0; i < Astro.Count; i++)
            {
                Shader_Default.Trans.Value(Astro[i].Orbit.Trans);
                Astro[i].Buffer.Draw();
            }

            for (int i = 0; i < Miner.Count; i++)
            {
                Shader_Default.Trans.Value(Miner[i].Orbit.Trans);
                Miner[i].Buffer.Draw();
            }

            Shader_Default.Trans.Value(Transformation3D.Default());
            TestBuffer.Draw();
        }

        private void CloseFunc()
        {

        }
    }
}
