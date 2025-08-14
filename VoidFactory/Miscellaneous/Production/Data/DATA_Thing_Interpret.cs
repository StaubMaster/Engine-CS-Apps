
using System.Collections.Generic;

using Engine3D;
using Engine3D.Entity;

namespace VoidFactory.Production.Data
{
    partial class DATA_Thing
    {
        public static class Interpret
        {
            private static List<DATA_Thing> ListThing;
            private static List<BodyStatic> ListBody;

            public static void Create()
            {
                ListThing = new List<DATA_Thing>();
                ListBody = new List<BodyStatic>();
            }
            public static void Delete()
            {
                ListThing = null;
                ListBody = null;
            }

            public static void SetFile(FileInterpret.FileStruct fileData)
            {
                FileInterpret.Query[] querys = new FileInterpret.Query[]
                {
                    new FileInterpret.Query("interp", 1, 1, 1, 1),

                    new FileInterpret.Query("name", 1, 1, 1, 1),
                    new FileInterpret.Query("id", 1, 1, 1, 1),
                    new FileInterpret.Query("cat", 1, 1, 1, 1),

                    new FileInterpret.Query("file", 1, 1, 1, 1),
                };
                int q;

                uint idx;
                string name;
                string id;
                string cat;
                string file;

                for (int ent = 0; ent < fileData.Entrys.Length; ent++)
                {
                    if (FileInterpret.Query.RunQuerys(fileData.Entrys[ent], querys))
                    {
                        q = 0;
                        if (querys[q].Found[0][0] == "thing")
                        {
                            q++;

                            idx = (uint)ListThing.Count;
                            name = querys[q].Found[0][0]; q++;
                            id = querys[q].Found[0][0]; q++;
                            cat = querys[q].Found[0][0]; q++;
                            file = querys[q].Found[0][0];

                            ListThing.Add(new DATA_Thing(idx, name, id, cat));
                            ListBody.Add(BodyStatic.File.Load(file));
                        }
                    }
                }
            }

            public static DATA_Thing[] GetThings()
            {
                return ListThing.ToArray();
            }
            public static BodyStatic[] GetBodys()
            {
                return ListBody.ToArray();
            }
        }
    }
}
