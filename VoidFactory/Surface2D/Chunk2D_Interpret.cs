using System.Collections.Generic;

using Engine3D;
using Engine3D.Noise;

using VoidFactory.Production.Data;

namespace VoidFactory.Surface2D
{
    partial class Chunk2D
    {
        public static class Interpret
        {
            private static List<LayerGenerationData> LayerNoise;

            public static void Create()
            {
                LayerNoise = new List<LayerGenerationData>();
            }
            public static void Delete()
            {
                LayerNoise = null;
            }

            public static void SetFile(FileInterpret.FileStruct fileData, DATA_Thing[] things)
            {
                FileInterpret.Query[] querys = new FileInterpret.Query[]
                {
                    new FileInterpret.Query("interp", 1, 1, 1, 1),

                    new FileInterpret.Query("color", 1, 1, 2, 2),
                    new FileInterpret.Query("thing", 1, 1, 1, 1),

                    new FileInterpret.Query("const", 0, 255, 1, 1),
                    new FileInterpret.Query("perlin", 0, 255, 3, 3),
                    new FileInterpret.Query("minmax", 0, 255, 7, 7),
                };
                int q;

                LayerGenerationData Slayer;
                NoiseLayer[] Nlayers;
                int layerIdx;

                for (int ent = 0; ent < fileData.Entrys.Length; ent++)
                {
                    if (FileInterpret.Query.RunQuerys(fileData.Entrys[ent], querys))
                    {
                        q = 0;
                        if (querys[q].Found[0][0] == "surfNoise")
                        {
                            q++;

                            Slayer = new LayerGenerationData();
                            Slayer.Color1 = querys[q].ToColor(0, 0);
                            Slayer.Color2 = querys[q].ToColor(0, 1);
                            q++;

                            Slayer.Thing = DATA_Thing.FindID(things, querys[q].Found[0][0]);
                            q++;

                            Nlayers = new NoiseLayer[querys[q + 0].Num + querys[q + 1].Num + querys[q + 2].Num];
                            layerIdx = -1;

                            for (int l = 0; l < querys[q].Num; l++)
                                Nlayers[++layerIdx] = Constant.FromString(querys[q].Found[l]);
                            q++;

                            for (int l = 0; l < querys[q].Num; l++)
                                Nlayers[++layerIdx] = Perlin.FromString(querys[q].Found[l]);
                            q++;

                            for (int l = 0; l < querys[q].Num; l++)
                                Nlayers[++layerIdx] = DistanceMinMax.FromString(querys[q].Found[l]);

                            Slayer.Noise = new NoiseSum(Nlayers);
                            LayerNoise.Add(Slayer);
                        }
                    }
                }
            }

            public static LayerGenerationData[] GetLayers()
            {
                return LayerNoise.ToArray();
            }
        }
    }
}
