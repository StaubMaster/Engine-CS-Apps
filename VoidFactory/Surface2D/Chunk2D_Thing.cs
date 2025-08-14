using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Engine3D;
using Engine3D.Abstract3D;
using Engine3D.Entity;
using Engine3D.GraphicsOld;
using Engine3D.OutPut.Shader;
using Engine3D.OutPut.Uniform.Specific;
using Engine3D.Graphics;
using Engine3D.Graphics.Display;

using VoidFactory.Production.Data;

namespace VoidFactory.Surface2D
{
    partial class Chunk2D
    {
        public class SURF_Object
        {
            public static DisplayPolyHedra[] Bodys;
            public static void BodysCreate()
            {
                for (int i = 0; i < Bodys.Length; i++)
                {
                    //Bodys[i].BufferCreate();
                    //Bodys[i].BufferFill();
                }
            }
            public static void BodysDelete()
            {
                for (int i = 0; i < Bodys.Length; i++)
                {
                    //Bodys[i].BufferDelete();
                }
                Bodys = null;
            }



            public bool ToRemove;
            public bool ToDraw;

            private uint Idx;
            private Transformation3D Trans;
            public DATA_Buffer Content;

            public SURF_Object(uint idx, DATA_Thing thing, uint num, Transformation3D trans)
            {
                ToRemove = false;

                Idx = idx;
                Trans = trans;
                Content = new DATA_Buffer(thing, num);
            }

            public void Draw()
            {
                Bodys[Idx].Draw();
            }
            public void Draw(TransUniProgram program)
            {
                program.UniTrans(new RenderTrans(Trans));
                Bodys[Idx].Draw();
            }
            public void Draw(CShaderTransformation program)
            {
                program.Trans.Value(Trans);
                Bodys[Idx].Draw();
            }
            public void Draw(BodyElemUniShader program)
            {
                program.Trans.Value(Trans);
                Bodys[Idx].Draw();
            }
            public DATA_Cost Collect()
            {
                DATA_Cost cost = new DATA_Cost(Content.Thing, 1);
                Content.Num--;

                if (Content.Num == 0)
                    ToRemove = true;

                return cost;
            }

            public override string ToString()
            {
                string str = "";

                str += Content;
                str += "[" + Idx + "]";

                return str;
            }

            public class Template
            {
                public uint Idx1;
                public uint Idx2;
                public DATA_Thing Thing1;
                public DATA_Thing Thing2;

                public uint MinVal1;
                public uint MaxVal1;

                public uint MinVal2;
                public uint MaxVal2;

                public double MinDist;
                public double MaxDist;

                public Engine3D.Noise.Static Gen;
                public uint Color;

                public Template()
                {

                }

                public void Draw()
                {
                    Bodys[Idx2].Draw();
                }
                public void Draw(TransUniProgram program, Transformation3D trans)
                {
                    program.UniTrans(new RenderTrans(trans));
                    Bodys[Idx2].Draw();
                }
                public void Draw(CShaderTransformation program, Transformation3D trans)
                {
                    program.Trans.Value(trans);
                    Bodys[Idx2].Draw();
                }

                public SURF_Object ToInstance1(Transformation3D trans, double perc)
                {
                    uint num;
                    num = (uint)Math.Round((MaxVal1 - MinVal1) * perc);
                    num += MinVal1;

                    return new SURF_Object(Idx1, Thing1, num, trans);
                }
                public SURF_Object ToInstance2(Transformation3D trans, double perc)
                {
                    uint num;
                    num = (uint)Math.Round((MaxVal2 - MinVal2) * perc);
                    num += MinVal2;

                    return new SURF_Object(Idx2, Thing2, num, trans);
                }
                public override string ToString()
                {
                    string str = "";

                    str += "[" + Idx1 + ":" + Idx2 + "]";
                    str += "[" + MinVal1 + ":" + MaxVal1 + "]";
                    str += "[" + MinVal2 + ":" + MaxVal2 + "]";
                    str += Thing1 + " ";
                    str += Thing2;

                    return str;
                }

            }
            public static class Interpret
            {
                private static List<Template> ListThing;
                private static List<DisplayPolyHedra> ListBody;

                public static void Create()
                {
                    ListThing = new List<Template>();
                    ListBody = new List<DisplayPolyHedra>();
                }
                public static void Delete()
                {
                    ListThing = null;
                    ListBody = null;
                }

                public static void SetFile(FileInterpret.FileStruct fileData, DATA_Thing[] things)
                {
                    FileInterpret.Query[] querys = new FileInterpret.Query[]
                    {
                        new FileInterpret.Query("interp", 1, 1, 1, 1),

                        new FileInterpret.Query("thing1", 1, 1, 1, 1),
                        new FileInterpret.Query("thing2", 1, 1, 1, 1),

                        new FileInterpret.Query("range_val1", 1, 1, 2, 2),
                        new FileInterpret.Query("range_val2", 1, 1, 2, 2),
                        new FileInterpret.Query("range_dist", 1, 1, 2, 2),

                        new FileInterpret.Query("static", 1, 1, 2, 2),
                        new FileInterpret.Query("color", 1, 1, 1, 1),

                        new FileInterpret.Query("file1", 1, 1, 1, 1),
                        new FileInterpret.Query("file2", 1, 1, 1, 1),
                    };
                    int q;

                    Template temp;
                    string file1;
                    string file2;

                    for (int ent = 0; ent < fileData.Entrys.Length; ent++)
                    {
                        if (FileInterpret.Query.RunQuerys(fileData.Entrys[ent], querys))
                        {
                            q = 0;
                            if (querys[q].Found[0][0] == "surfObj")
                            {
                                temp = new Template();
                                q++;

                                temp.Idx1 = (uint)(ListBody.Count + 0);
                                temp.Idx2 = (uint)(ListBody.Count + 1);
                                temp.Thing1 = DATA_Thing.FindID(things, querys[q].Found[0][0]); q++;
                                temp.Thing2 = DATA_Thing.FindID(things, querys[q].Found[0][0]); q++;

                                temp.MinVal1 = uint.Parse(querys[q].Found[0][0]);
                                temp.MaxVal1 = uint.Parse(querys[q].Found[0][1]);
                                q++;
                                temp.MinVal2 = uint.Parse(querys[q].Found[0][0]);
                                temp.MaxVal2 = uint.Parse(querys[q].Found[0][1]);
                                q++;

                                temp.MinDist = double.Parse(querys[q].Found[0][0]);
                                temp.MaxDist = double.Parse(querys[q].Found[0][1]);
                                q++;

                                temp.Gen = Engine3D.Noise.Static.FromString(querys[q].Found[0]);
                                q++;
                                temp.Color = uint.Parse(querys[q].Found[0][0].Replace("0x", ""), System.Globalization.NumberStyles.HexNumber);
                                q++;

                                file1 = querys[q].Found[0][0]; q++;
                                file2 = querys[q].Found[0][0];

                                ListThing.Add(temp);
                                //ListBody.Add(BodyStatic.File.Load(file1));
                                //ListBody.Add(BodyStatic.File.Load(file2));
                                ListBody.Add(new DisplayPolyHedra(file1));
                                ListBody.Add(new DisplayPolyHedra(file2));
                            }
                        }
                    }
                }

                public static Template[] GetTemplates()
                {
                    return ListThing.ToArray();
                }
                public static DisplayPolyHedra[] GetBodys()
                {
                    return ListBody.ToArray();
                }
            }
        }
    }
}
