
using System.Collections.Generic;

using Engine3D;

namespace VoidFactory.Production.Data
{
    partial class DATA_Recipy
    {
        public static class Interpret
        {
            private static List<DATA_Recipy> ListRecipy;

            public static void Create()
            {
                ListRecipy = new List<DATA_Recipy>();
            }
            public static void Delete()
            {
                ListRecipy = null;
            }

            public static void SetFile(FileInterpret.FileStruct fileData, DATA_Thing[] things)
            {
                FileInterpret.Query[] querys = new FileInterpret.Query[]
                {
                    new FileInterpret.Query("interp", 1, 1, 1, 1),
                    new FileInterpret.Query("cat", 1, 1, 1, 1),

                    new FileInterpret.Query("proc", 1, 1, 1, 1),
                    new FileInterpret.Query("inn", 0, 255, 2, 2),
                    new FileInterpret.Query("out", 0, 255, 2, 2),

                    new FileInterpret.Query("tick", 1, 1, 1, 1),
                };
                int q;

                string cat;
                string proc;
                DATA_Buffer[] Inn;
                DATA_Buffer[] Out;
                uint tick;

                for (int ent = 0; ent < fileData.Entrys.Length; ent++)
                {
                    if (FileInterpret.Query.RunQuerys(fileData.Entrys[ent], querys))
                    {
                        q = 0;
                        if (querys[q].Found[0][0] == "recipy")
                        {
                            q++;

                            cat = querys[q].Found[0][0]; q++;

                            proc = querys[q].Found[0][0]; q++;
                            Inn = DATA_Buffer.FromString(querys[q].Found, things); q++;
                            Out = DATA_Buffer.FromString(querys[q].Found, things); q++;

                            tick = querys[q].ToUInt();

                            ListRecipy.Add(new DATA_Recipy(Inn, Out, tick, proc, cat));
                        }
                    }
                }
            }

            public static DATA_Recipy[] GetRecipy()
            {
                return ListRecipy.ToArray();
            }
        }
    }
}
