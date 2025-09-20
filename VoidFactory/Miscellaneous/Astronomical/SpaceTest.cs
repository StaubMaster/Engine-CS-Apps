using System;
using System.Collections.Generic;

using Engine3D;
using Engine3D.Abstract3D;

using Engine3D.Graphics;
using Engine3D.Graphics.Shader.Manager;
using Engine3D.Graphics.PolyHedraInstance.PH_3D;

using OpenTK.Windowing.GraphicsLibraryFramework;

namespace VoidFactory.Astronomical
{
    class SpaceTest
    {
        private DisplayArea MainWindow;
        private DisplayCamera MainCamera;

        private CSatelliteNatural ViewHoverCenter;
        private Transformation3D ViewHover;



        private PolyHedra_Shader_Manager PH_Man;
        private PolyHedraInstance_3D_Array PH_Arr;

        private bool UpdateSatellites;
        private List<CSatelliteNatural> Astro;
        private List<CSatelliteMiner> Miner;



        public SpaceTest()
        {
            MainWindow = new DisplayArea(640, 480, CloseFunc, FrameFunc);
            MainWindow.ChangeColor(0, 0, 0);

            InitView();
            InitShaders();
            InitSatellites();

            MainWindow.Run();
            MainWindow.Term();
        }

        private void InitView()
        {
            MainCamera = new DisplayCamera();
            MainCamera.Depth = new Engine3D.DataStructs.DepthData(1.0f, 1600.0f);

            ViewHoverCenter = null;
            ViewHover = Transformation3D.NaN();
        }
        private void InitShaders()
        {
            string shaderDir = "E:/Programmieren/VS_Code/OpenTK/Engine3D/Engine3D/Shaders/";

            PH_Man = new PolyHedra_Shader_Manager(shaderDir);
            PH_Man.Depth.ChangeData(MainCamera.Depth);
        }
        private void InitSatellites()
        {
            UpdateSatellites = true;

            PolyHedra[] ph = new PolyHedra[]
            {
                PolyHedra.Generate.SphereCube(12, 200.0f),
                PolyHedra.Generate.SphereCube(12, 16.0f),
                PolyHedra.Generate.SphereCube(12, 4.0f),
                PolyHedra.Generate.Cube(),
            };
            PH_Arr = new PolyHedraInstance_3D_Array(ph);

            Astro = new List<CSatelliteNatural>();
            Astro.Add(new CSatelliteNatural(PH_Arr.Alloc(0, 1), new COrbitHover(null, new SAngledRotation(0.0009, 0, 0), Point3D.Default()), 200));
            Astro.Add(new CSatelliteNatural(PH_Arr.Alloc(1, 1), new COrbitNormal(Astro[0].Orbit, new SAngledRotation(0.001, 0.0, 0.0), 1000, new SAngledRotation(0.0001, 0.2, 0)), 16));
            Astro.Add(new CSatelliteNatural(PH_Arr.Alloc(2, 1), new COrbitNormal(Astro[1].Orbit, new SAngledRotation(0.008, 0.0, 0.0), 100, new SAngledRotation(0.008, 0, 0)), 4));

            Miner = new List<CSatelliteMiner>();
        }

        private Intersekt.RayInterval Select_Astro()
        {
            Intersekt.RayInterval func(Ray3D ray, CSatelliteNatural body)
            {
                return body.InstData.Intersekt(ray, 0);
            }
            return Intersekt.Ray_Multiple(MainCamera.Ray, Astro, func);
        }
        private Intersekt.RayInterval Select_Miner()
        {
            Intersekt.RayInterval func(Ray3D ray, CSatelliteMiner body)
            {
                return body.InstData.Intersekt(ray, 0);
            }
            return Intersekt.Ray_Multiple(MainCamera.Ray, Miner, func);
        }

        private void Update_View()
        {
            if (!ViewHover.IsNaN())
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
                TMovement.Unrestricted(ref temp, MainWindow.MoveByKeys(0.01f, 100), MainWindow.SpinByMouse());

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
                TMovement.Unrestricted(ref MainCamera.Trans, MainWindow.MoveByKeys(0.1f, 100), MainWindow.SpinByMouse());
            }
            MainCamera.Update(MainWindow.MouseRay());
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
                    rel = rel * (float)((nat.Radius) / rel.Len);
                    Miner.Add(new CSatelliteMiner(PH_Arr.Alloc(3, 1), nat, new COrbitHover(nat.Orbit, new SAngledRotation(0.01, 0, 0), rel)));
                }
                if (interMiner.Is && interMiner.Interval < interAstro.Interval)
                {
                    Miner.RemoveAt(interMiner.Index);
                }
            }

            if (MainWindow.CheckKey(Keys.T).IsPressed())
            {
                if (ViewHover.IsNaN())
                {
                    if (interAstro.Is)
                    {
                        InitViewHover(Astro[interAstro.Index]);
                    }
                }
                else
                {
                    ViewHover = Transformation3D.NaN();
                }
            }

            if (MainWindow.CheckKey(Keys.G).IsPressed())
            {
                string str = "";
                for (int i = 0; i < PH_Arr.Length; i++)
                {
                    str += "[" + i.ToString("00") + "]" + PH_Arr[i].Count + "\n";
                }
                ConsoleLog.Log(str);
            }



            //  Draw
            PH_Man.View.ChangeData(MainCamera.Trans);
            PH_Man.ViewPortSizeRatio.ChangeData(MainWindow.SizeRatio());
            PH_Man.InstShader.Use();
            PH_Arr.Update();
            PH_Arr.Draw();



            //Shader_Default.Use();
            //
            //for (int i = 0; i < Astro.Count; i++)
            //{
            //    Shader_Default.Trans.Value(Astro[i].Orbit.Trans);
            //    Astro[i].Buffer.Draw();
            //}
            //
            //for (int i = 0; i < Miner.Count; i++)
            //{
            //    Shader_Default.Trans.Value(Miner[i].Orbit.Trans);
            //    Miner[i].Buffer.Draw();
            //}
            //
            //Shader_Default.Trans.Value(Transformation3D.Default());
            //TestBuffer.Draw();
        }

        private void CloseFunc()
        {

        }
    }
}
