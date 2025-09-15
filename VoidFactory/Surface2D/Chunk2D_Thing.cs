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
using Engine3D.Graphics.Display3D;

using Engine3D.Miscellaneous.EntryContainer;

using VoidFactory.Production.Data;

namespace VoidFactory.Surface2D
{
    partial class Chunk2D
    {
        public partial class SURF_Object
        {
            public bool ToRemove;
            public bool ToDraw;

            private readonly uint Idx;
            private readonly Transformation3D Trans;
            private readonly TileIndex TileIdx;
            private EntryContainerDynamic<PolyHedraInstance_3D_Data>.Entry InstEntry;
            public DATA_Buffer Content;

            public SURF_Object(uint idx, DATA_Thing thing, uint num, Transformation3D trans, TileIndex tileIdx)
            {
                ToRemove = false;

                Idx = idx;
                Trans = trans;
                Content = new DATA_Buffer(thing, num);

                TileIdx = tileIdx;

                InstEntry = null;
            }
            public void Place()
            {
                if (InstEntry == null)
                {
                    InstEntry = Bodys[Idx].Alloc(1);
                    InstEntry[0] = new PolyHedraInstance_3D_Data(Trans);
                }
            }
            public void Remove()
            {
                if (!ToRemove)
                {
                    //ConsoleLog.Log("SURF Thing Remove " + this);
                    InstEntry.Dispose();
                    InstEntry = null;
                    ToRemove = true;
                }
            }

            public DATA_Cost Collect()
            {
                uint cost = 1;
                if (Content.Num <= cost)
                {
                    cost = Content.Num;
                    Remove();
                }
                Content.Num -= cost;
                return new DATA_Cost(Content.Thing, cost);
            }

            public override string ToString()
            {
                string str = "";

                str += Content;
                str += "[" + Idx + "]" + " " + TileIdx.ToLine();

                return str;
            }
        }
        public partial class SURF_Object
        {
            public static PHEI_Array Bodys;
            public static string BodysCountInfo()
            {
                string str = "";
                for (int i = 0; i < Bodys.Length; i++)
                {
                    str += Bodys[i].Info() + ":[" + i + "]Inst\n";
                }
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

                public SURF_Object ToInstance1(Transformation3D trans, double perc, TileIndex tileIdx)
                {
                    uint num;
                    num = (uint)Math.Round((MaxVal1 - MinVal1) * perc);
                    num += MinVal1;

                    return new SURF_Object(Idx1, Thing1, num, trans, tileIdx);
                }
                public SURF_Object ToInstance2(Transformation3D trans, double perc, TileIndex tileIdx)
                {
                    uint num;
                    num = (uint)Math.Round((MaxVal2 - MinVal2) * perc);
                    num += MinVal2;

                    return new SURF_Object(Idx2, Thing2, num, trans, tileIdx);
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
                private static List<BodyStatic> ListBody;

                public static void Create()
                {
                    ListThing = new List<Template>();
                    ListBody = new List<BodyStatic>();
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
                                ListBody.Add(BodyStatic.File.Load(file1));
                                ListBody.Add(BodyStatic.File.Load(file2));
                            }
                        }
                    }
                }

                public static Template[] GetTemplates()
                {
                    return ListThing.ToArray();
                }
                public static BodyStatic[] GetBodys()
                {
                    return ListBody.ToArray();
                }

                /*public static void Do(FileInterpret.FileStruct[] fileDatas, DATA_Thing[] Things)
                {
                    Create();
                    for (int i = 0; i < fileDatas.Length; i++)
                    {
                        SetFile(fileDatas[i], Things);
                    }
                    SURF_Object.Bodys = new PHEI_Array(GetBodys());

                    SurfThingTemplates = GetTemplates();
                    Delete();
                }*/
            }
        }
    }
}
