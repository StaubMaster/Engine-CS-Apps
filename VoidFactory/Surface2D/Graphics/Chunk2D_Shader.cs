using Engine3D.DataStructs;
using Engine3D.Graphics.Shader;
using Engine3D.Graphics.Uniform;

using OpenTK.Graphics.OpenGL4;

namespace VoidFactory.Surface2D.Graphics
{
    public class UniInt1 : ShaderUniform<int>
    {
        private int Location;
        public UniInt1(string name, BaseShader shader) : base(name, shader)
        {
            Location = shader.UniformFind(name);
        }
        public override void PutData(int i)
        {
            GL.Uniform1(Location, i);
        }
    }
    public class UniInt3 : ShaderUniform<(int, int, int)>
    {
        private int Location;
        public UniInt3(string name, BaseShader shader) : base(name, shader)
        {
            Location = shader.UniformFind(name);
        }
        public override void PutData((int, int, int) i)
        {
            GL.Uniform3(Location, i.Item1, i.Item2, i.Item3);
        }
    }
    public class CChunk2DShader : BaseShader
    {
        public UniSizeRatio SizeRatio;

        public UniTrans3D View;

        public UniDepth Depth;
        public UniRange DepthFadeRange;
        public UniColorU DepthFadeColor;

        public UniPoint3D LightSolar;
        public UniRange LightRange;

        public UniColorU OtherColor;
        public UniLInter OtherColorInter;

        public UniLInter GrayInter;

        public UniInt1 TileSize;
        public UniInt1 TilesPerSide;
        public UniInt3 ChunkPos;

        public CChunk2DShader(string shaderDir) : base(new ShaderCode[]
        {
                ShaderCode.FromFile(shaderDir + "Chunk2D/Chunk2D.vert"),
                ShaderCode.FromFile(shaderDir + "Chunk2D/Chunk2D.geom"),
                ShaderCode.FromFile(shaderDir + "Chunk2D/Chunk2D.frag"),
        })
        {
            SizeRatio = new UniSizeRatio("screenRatios", this);

            View = new UniTrans3D("view", this);

            Depth = new UniDepth("depthFactor", this);
            DepthFadeRange = new UniRange("depthFadeRange", this);
            DepthFadeColor = new UniColorU("depthFadeColor", this);

            LightSolar = new UniPoint3D("solar", this);
            LightRange = new UniRange("lightRange", this);

            OtherColor = new UniColorU("colorOther", this);
            OtherColorInter = new UniLInter("colorInterPol", this);

            GrayInter = new UniLInter("GrayInter", this);

            TileSize = new UniInt1("tiles_size", this);
            TilesPerSide = new UniInt1("tiles_per_side", this);
            ChunkPos = new UniInt3("chunk_pos", this);
        }
    }

    static class Chunk2D_Graphics
    {
        public static UniformBase<UniInt1, int> TileSize;
        public static UniformBase<UniInt1, int> TilesPerSide;
        public static UniformBase<UniInt3, (int, int, int)> ChunkPos;
        public static CChunk2DShader Shader;
    }
}
