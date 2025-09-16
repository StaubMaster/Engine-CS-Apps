using System;
using System.Collections.Generic;

using Engine3D;
using Engine3D.Entity;

using VoidFactory.Production.Data;
using VoidFactory.Inventory;

namespace VoidFactory.Production.Buildings
{
    abstract partial class BLD_Base
    {
        public static GameSelect.Game3D game;

        public static class Interpreter
        {
            private static List<Template_Base> ListTemplate;
            private static List<BodyStatic> ListBodys;

            public static void Create()
            {
                ListTemplate = new List<Template_Base>();
                ListBodys = new List<BodyStatic>();
            }
            public static void Delete()
            {
                ListTemplate = null;
                ListBodys = null;
            }

            public static void SetFile(FileInterpret.FileStruct fileData, DATA_Thing[] things)
            {
                FileInterpret.Query[] querys = new FileInterpret.Query[]
                {
                    new FileInterpret.Query("interp", 1, 1, 1, 1),
                    new FileInterpret.Query("type", 1, 1, 1, 1),
                    new FileInterpret.Query("cat", 1, 1, 1, 1),

                    new FileInterpret.Query("name", 1, 1, 1, 1),
                    new FileInterpret.Query("cost", 0, 255, 2, 2),

                    new FileInterpret.Query("proc", 1, 1, 0, 1),
                    new FileInterpret.Query("inn", 0, 255, 3, 3),
                    new FileInterpret.Query("out", 0, 255, 3, 3),

                    new FileInterpret.Query("file", 1, 1, 1, 1),
                };
                int q;

                Template_Base temp;
                string file;

                for (int ent = 0; ent < fileData.Entrys.Length; ent++)
                {
                    if (FileInterpret.Query.RunQuerys(fileData.Entrys[ent], querys))
                    {
                        q = 0;
                        if (querys[q].Found[0][0] == "building")
                        {
                            q++;

                            switch (querys[q].Found[0][0])
                            {
                                case "conv" : temp = new BLD_Converter.Template();      break;
                                case "relay": temp = new BLD_Relay.Template();          break;
                                case "coll" : temp = new BLD_Surf_Collector.Template(); break;
                                default: continue;
                            }; q++;
                            temp.Category = querys[q].Found[0][0]; q++;

                            temp.Idx = (uint)(ListTemplate.Count + game.PolyHedras.Length);
                            temp.Name = querys[q].Found[0][0]; q++;
                            //cost = DATA_Cost.FromString(querys[q].Found, things); q++;
                            temp.Cost = new DATA_Cost(); q++;

                            if (querys[q].Found[0].Length != 0)
                                temp.Processing = querys[q].Found[0][0];
                            q++;
                            temp.Inn = querys[q].ToPunktArr(); q++;
                            temp.Out = querys[q].ToPunktArr(); q++;

                            file = querys[q].Found[0][0];

                            ListTemplate.Add(temp);
                            ListBodys.Add(BodyStatic.File.Load(file));
                        }
                    }
                }
            }

            public static Template_Base[] GetTemplates()
            {
                return ListTemplate.ToArray();
            }
            public static BodyStatic[] GetBodys()
            {
                return ListBodys.ToArray();
            }
        }
    }
}
