using Engine3D.Graphics.Shader;
using Engine3D.Graphics.Shader.Uniform.Float;
using Engine3D.Graphics.Shader.Uniform.Int;

namespace VoidFactory.Surface2D.Graphics
{
    class Chunk2D_Shader : BaseShader
    {
        public readonly UniScreenRatio ScreenRatio;

        public readonly UniTransformation View;


        public readonly UniDepth Depth;
        public readonly UniRange DepthFadeRange;
        public readonly UniColor DepthFadeColor;

        public readonly UniPoint LightSolar;
        public readonly UniRange LightRange;

        public readonly UniColor OtherColor;
        public readonly UniInter OtherColorInter;

        public readonly UniInter GrayInter;


        public readonly UniInt1 TilesSize;
        public readonly UniInt1 TilesPreSide;
        public readonly UniInt3 ChunkPos;

        public Chunk2D_Shader(string shaderDir) : base(new ShaderCode[]
        {
            ShaderCode.FromFile(shaderDir + "Surface/Chunk_Flat2.vert"),
            ShaderCode.FromFile(shaderDir + "Surface/Chunk_Flat2.geom"),
            ShaderCode.FromFile(shaderDir + "OAR/OAR.frag"),
        })
        {
            ScreenRatio = new UniScreenRatio(this, "screenRatios");

            View = new UniTransformation(this, "view");

            Depth = new UniDepth(this, "depthFactor");
            DepthFadeRange = new UniRange(this, "depthFadeRange");
            DepthFadeColor = new UniColor(this, "depthFadeColor");

            LightSolar = new UniPoint(this, "solar");
            LightRange = new UniRange(this, "lightRange");

            OtherColor = new UniColor(this, "colorOther");
            OtherColorInter = new UniInter(this, "colorInterPol");

            GrayInter = new UniInter(this, "GrayInter");


            TilesSize = new UniInt1(this, "tiles_size", 1);
            TilesPreSide = new UniInt1(this, "tiles_per_side", 1);

            ChunkPos = new UniInt3(this, "chunk_pos", 1);
        }

        protected override void UpdateUniforms()
        {
            ScreenRatio.Update();

            View.Update();

            Depth.Update();
            DepthFadeRange.Update();
            DepthFadeColor.Update();

            LightSolar.Update();
            LightRange.Update();

            OtherColor.Update();
            OtherColorInter.Update();

            GrayInter.Update();


            TilesSize.Update();
            TilesPreSide.Update();

            ChunkPos.Update();
        }
    }
}
