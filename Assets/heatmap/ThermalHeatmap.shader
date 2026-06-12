Shader "Hidden/Shader/ThermalHeatmap"
{
    HLSLINCLUDE

    #pragma target 3.5
    #pragma exclude_renderers gles

    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

    float _Blend;
    float _RangeMin;
    float _RangeMax;
    int   _Palette;

    #define MAX_RAY_HITS 1024

    int    _RayHitCount;
    float4 _RayHitPositions[MAX_RAY_HITS];
    float  _RayHitRadii[MAX_RAY_HITS];
    float  _RayHitRadius;
    float  _RayHitIntensity;

  float3 VOSHeat(float t)
{
    t = saturate(t);

    float3 deepBlue = float3(0.00, 0.00, 1.00);
    float3 blue     = float3(0.08, 0.18, 1.00);
    float3 cyan     = float3(0.00, 0.85, 1.00);
    float3 green    = float3(0.00, 1.00, 0.00);
    float3 yellow   = float3(1.00, 1.00, 0.00);
    float3 orange   = float3(1.00, 0.45, 0.00);
    float3 red      = float3(1.00, 0.00, 0.00);

    if (t < 0.18)
        return lerp(deepBlue, blue, t / 0.18);

    if (t < 0.34)
        return lerp(blue, cyan, (t - 0.18) / 0.16);

    if (t < 0.50)
        return lerp(cyan, green, (t - 0.34) / 0.16);

    if (t < 0.68)
        return lerp(green, yellow, (t - 0.50) / 0.18);

    if (t < 0.84)
        return lerp(yellow, orange, (t - 0.68) / 0.16);

    return lerp(orange, red, (t - 0.84) / 0.16);
}

    float3 Infrared(float t)
    {
        return VOSHeat(t);
    }

    float3 Isotherm(float t)
    {
        const float bands = 8.0;
        float b = floor(saturate(t) * bands) / bands;
        return VOSHeat(b);
    }

    float3 Plasma(float t)
    {
        t = saturate(t);

        float3 c;

        if      (t < 0.25)
            c = lerp(float3(0.05, 0.03, 0.53), float3(0.46, 0.07, 0.63), t / 0.25);
        else if (t < 0.50)
            c = lerp(float3(0.46, 0.07, 0.63), float3(0.80, 0.18, 0.44), (t - 0.25) / 0.25);
        else if (t < 0.75)
            c = lerp(float3(0.80, 0.18, 0.44), float3(0.97, 0.58, 0.13), (t - 0.50) / 0.25);
        else
            c = lerp(float3(0.97, 0.58, 0.13), float3(0.94, 0.98, 0.13), (t - 0.75) / 0.25);

        return c;
    }

    float3 HeatZones(float t)
    {
        t = saturate(t);

        if (t < 0.20) return float3(0.00, 0.00, 1.00);
        if (t < 0.40) return float3(0.00, 0.85, 1.00);
        if (t < 0.60) return float3(0.00, 1.00, 0.00);
        if (t < 0.80) return float3(1.00, 1.00, 0.00);

        return float3(1.00, 0.00, 0.00);
    }

    float3 Delta(float t)
    {
        t = saturate(t);

        float d = t - 0.5;

        if (d < 0.0)
        {
            float s = -d * 2.0;
            return lerp(float3(0.50, 0.50, 0.50), float3(0.08, 0.31, 0.78), s);
        }

        float s = d * 2.0;
        return lerp(float3(0.50, 0.50, 0.50), float3(0.78, 0.08, 0.08), s);
    }

    float3 ApplyPalette(float t)
    {
        if (_Palette == 1) return t.xxx;
        if (_Palette == 2) return Isotherm(t);
        if (_Palette == 3) return Plasma(t);
        if (_Palette == 4) return HeatZones(t);
        if (_Palette == 5) return Delta(t);

        return VOSHeat(t);
    }

    float4 ThermalFrag(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        float2 uv = input.texcoord;

        float3 sceneColor = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv).rgb;

        float lum = Luminance(sceneColor);

        float heat = 0.0;

        float rawDepth = SampleSceneDepth(uv);

        bool isSky;

        #if defined(UNITY_REVERSED_Z)
            isSky = rawDepth <= 0.0001;
        #else
            isSky = rawDepth >= 0.9999;
        #endif

        float3 pixelWorldPos = ComputeWorldSpacePosition(
            uv,
            rawDepth,
            UNITY_MATRIX_I_VP
        );

       float rayHeat = 0.0;

if (!isSky)
{
    [loop]
    for (int i = 0; i < _RayHitCount; i++)
    {
        float3 hitPos = _RayHitPositions[i].xyz;
        float pointIntensity = _RayHitPositions[i].w;

        float3 diff = pixelWorldPos - hitPos;
        float distSq = dot(diff, diff);

        float radius = max(0.001, _RayHitRadii[i]);
        float sigma = radius * 1.5;
        float limit = sigma * 3.0;

        // Optimization: Skip sqrt() and exp() calculations for pixels outside the falloff area
        if (distSq < limit * limit)
        {
            float h = exp(-distSq / (2.0 * sigma * sigma));
            rayHeat = max(rayHeat, h * pointIntensity);
        }
    }
}

        heat += rayHeat;
        heat = saturate(heat);

        heat = pow(heat, 0.6);

        float3 thermal = ApplyPalette(heat);

        // Blending using full _Blend: all areas (including cold regions) are tinted blue
        float3 outColor = lerp(sceneColor, thermal, _Blend);

        return float4(outColor, 1.0);
    }

    ENDHLSL

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }

        ZWrite Off
        ZTest Always
        Blend Off
        Cull Off

        Pass
        {
            Name "ThermalHeatmap"

            HLSLPROGRAM
                #pragma vertex Vert
                #pragma fragment ThermalFrag
            ENDHLSL
        }
    }

    Fallback Off
}