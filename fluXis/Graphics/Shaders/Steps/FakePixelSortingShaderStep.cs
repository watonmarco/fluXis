using System.Runtime.InteropServices;
using fluXis.Map.Structures.Events;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;

namespace fluXis.Graphics.Shaders.Steps;

public class FakePixelSortingShaderStep : ShaderStep<FakePixelSortingShaderStep.FakePixelSortingParameters>
{
    protected override string FragmentShader => "FakePixelSorting";
    public override ShaderType Type => ShaderType.FakePixelSorting;
    public override bool ShouldRender => Strength > 0;
    
    public override void UpdateParameters(IFrameBuffer current) => ParameterBuffer.Data = new FakePixelSortingParameters
    {
        TexSize = current.Size,
        Strength = Strength,
        Time = (float)(Time.Current / 1000f)
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public record struct FakePixelSortingParameters
    {
        public UniformVector2 TexSize;
        public UniformFloat Time;
        public UniformFloat Strength;
        
    }
}
