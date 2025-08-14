using System;
using System.Collections.Generic;

using Engine3D;
using Engine3D.Abstract;
using Engine3D.Graphics;
using Engine3D.Graphics.Forms;
using Engine3D.Entity;

using VoidFactory.Astronomical;

namespace VoidFactory.GameSelect
{
    class GameSpace : Game3D
    {
        private Cache<string, BodyStatic> CBody;

        private NaturalBody[] AstBody;
        private List<SatMiner> Miner;
        private uint Rock;

        private TransUniCollection TransUni_Coll;

        private TextProgram Text_Prog;
        private TextBuffers Text_Buff;

        public GameSpace(Action externDelete) : base(externDelete)
        {
            TransUni_Coll = new TransUniCollection(
                new TransUniProgram("TransUni",
                    "Vert_Unif_noCol.vert",
                    "Geom_Norm_Color.geom",
                    "Frag/Light.frag")
                );

            Text_Prog = new TextProgram("Text",
                "Text/Vert_Text.vert",
                "Text/Geom_Text.geom",
                "Frag/Direct.frag");
            Text_Buff = new TextBuffers();
        }

        private void Closest_Ast(Ray ray, out double dist, out int idx)
        {
            dist = double.PositiveInfinity;
            idx = -1;
            for (int i = 0; i < AstBody.Length; i++)
            {
                if (AstBody[i].Intersekt(ray, ref dist))
                    idx = i;
            }
        }
        private void Closest_Miner(Ray ray, out double dist, out int idx)
        {
            dist = double.PositiveInfinity;
            idx = -1;
            for (int i = 0; i < Miner.Count; i++)
            {
                if (Miner[i].Intersekt(ray, ref dist))
                    idx = i;
            }
        }

        protected override void Frame()
        {
            //  Update
            {
                TMovement.Unrestricted(ref view.Trans, win.VelPos_Key(), win.VelRot_Mouse());                
                view.Update();
                /*if (OrbitRel == null)
                    view.Update_3D(win.VelPos_Key(), win.VelRot_Mouse());
                else
                {
                    view.Update_3D(null, win.VelRot_Mouse());
                    view.Trans.Pos = AstBody[OrbitIdx].Orbit.Trans.TFore(OrbitRel.Pos);
                }*/


                Ray ray = view.Trans.ToRay();

                double nat_dist;
                int nat_idx;
                Closest_Ast(ray, out nat_dist, out nat_idx);

                double mine_dist;
                int mine_idx;
                Closest_Miner(ray, out mine_dist, out mine_idx);

                //if (KeyOrbit.pressed)
                //{
                //    if (OrbitRel == null && nat_idx != -1)
                //    {
                //        OrbitIdx = nat_idx;
                //        OrbitRel = AstBody[OrbitIdx].Orbit.Trans.TBack(view.Trans);
                //    }
                //    else
                //        OrbitRel = null;
                //}

                /*if (KeyCollect.Check() && mine_idx != -1)
                {
                    Rock += Miner[mine_idx].Rock;
                    Miner[mine_idx].Rock = 0;
                }*/

                /*if (KeyPlace.Check())
                {
                    if (nat_dist < mine_dist && nat_idx != -1)
                    {
                        NaturalBody nat = AstBody[nat_idx];
                        Punkt rel = nat.Orbit.Trans.TBack(ray.Scale(nat_dist));
                        rel = rel * ((nat.Radius + 10) / rel.Len);
                        Miner.Add(new SatMiner(CBody.GetOut("miner"), nat, new SatelliteFixed(nat.Orbit, 0.01, 0, 0, rel)));
                    }
                    else if (mine_dist < nat_dist && mine_idx != -1)
                    {
                        Rock += Miner[mine_idx].Rock;
                        Miner.RemoveAt(mine_idx);
                        mine_idx = -1;
                        mine_dist = double.PositiveInfinity;
                    }
                }*/



                string str = "";
                //str += "Paused: " + KeyPause.Check() + "\n";
                str += "Ast Body: " + (nat_idx == -1 ? "?" : AstBody[nat_idx].Rock.ToString()) + "\n";
                str += "MinerNum: " + Miner.Count.ToString() + "\n";
                str += "Miner   : " + (mine_idx == -1 ? "?" : Miner[mine_idx].Rock.ToString()) + "\n";
                str += "Rock    : " + (Rock.ToString()) + "\n";
                Text_Buff.Insert(-0.975f, +0.90f, 0xFFFFFF, false, str);

                win.UText.BufferFill(Text_Buff);
            }

            //  Render
            {
                //TransUni_Coll.Program.UniView(Rechnen.RenderFloats(view.Trans));
                //TransUni_Coll.Program.UniView(new RenderTrans(view.Trans));
                view.UniTrans(TransUni_Coll.Program);

                /*if (!KeyPause.Check())
                {
                    for (int i = 0; i < AstBody.Length; i++)
                        AstBody[i].Update();
                    for (int i = 0; i < Miner.Count; i++)
                        Miner[i].Update();
                }*/

                for (int i = 0; i < AstBody.Length; i++)
                    AstBody[i].Body.Draw(TransUni_Coll.Program);
                for (int i = 0; i < Miner.Count; i++)
                    Miner[i].Body.Draw(TransUni_Coll.Program);

                Text_Prog.Use();
                Text_Buff.Fill_Strings();
                Text_Buff.Draw();
            }
        }

        public override void Create()
        {
            if (Running) { return; }
            ConsoleLog.Log("Create GameSpace");
            ConsoleLog.TabInc();
            base.Create();
            view.renderDepth.Far = 10000;

            CBody = new Cache<string, BodyStatic>();
            CBody.Insert("miner", "E:/Programmieren/VS_Code/Spiel Zeug/3D/SpaceMiner1.txt");
            CBody.FuncAllIO(BodyStatic.File.Load);
            CBody.Insert("sphere1", BodyStatic.Create.SphereTri(16, 36, 200.0));
            CBody.Insert("sphere2", BodyStatic.Create.SphereTri(8, 12, 16.0));
            CBody.Insert("sphere3", BodyStatic.Create.SphereTri(2, 5, 4.0));
            ConsoleLog.Log("");

            TransUni_Coll.Create();
            for (int i = 0; i < CBody.Length; i++)
            {
                CBody.GetOut(i).BufferCreate();
                CBody.GetOut(i).BufferFill();
            }
            //TransUni_Coll.Program.UniProj(new RenderDepthFactors(view.DepthN, view.DepthF), view.Fov);
            view.UniDepth(TransUni_Coll.Program);
            ConsoleLog.Log("");

            AstBody = new NaturalBody[3];
            AstBody[0] = new NaturalBody(CBody.GetOut("sphere1"), new Satellite(                      0.0009, 0, 0), 200);
            AstBody[1] = new NaturalBody(CBody.GetOut("sphere2"), new SatelliteFloat(AstBody[0].Orbit, 0.001, 0.0, 0.0, 1000, 0.0001, 0.2, 0), 16);
            AstBody[2] = new NaturalBody(CBody.GetOut("sphere3"), new SatelliteFloat(AstBody[1].Orbit, 0.008, 0.0, 0.0, 100,  0.0008, 0, 0), 4);

            Miner = new List<SatMiner>();

            Rock = 0;

            Text_Prog.Create();
            Text_Buff.Create();
            Text_Buff.Fill_Pallets();

            ConsoleLog.TabDec();
            ConsoleLog.Log("");
            Running = true;
        }
        public override void Delete()
        {
            if (!Running) { return; }
            ConsoleLog.Log("Delete GameSpace");
            ConsoleLog.TabInc();
            base.Delete();

            Text_Prog.Delete();
            Text_Buff.Delete();

            TransUni_Coll.Delete();

            for (int i = 0; i < CBody.Length; i++)
                CBody.GetOut(i).BufferDelete();
            CBody = null;

            AstBody = null;
            Miner = null;

            ConsoleLog.TabDec();
            ConsoleLog.Log("");
            Running = false;
        }
    }
}
