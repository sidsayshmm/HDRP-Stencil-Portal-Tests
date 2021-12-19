Shader "Avenger/TestMaskShader"
{
    Properties
    {
        _Colour("Base Colour", Color) = (1, 1, 1, 1)
        [IntRange]_MaskID("Mask ID", Range(0,255)) = 1
        [IntRange]_RefID("Ref ID", Range(0,255)) = 1
        _SomeInt("RenderQueueOrder",int) = 200
    }
    
    HLSLINCLUDE

    #pragma target 4.5
    #pragma only_renderers d3d11 ps4 xboxone vulkan metal switch
    #pragma multi_compile_instancing

    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Unlit/UnlitProperties.hlsl"
    ENDHLSL
    
    
    SubShader
    {
        Tags
        {
            "RenderPipeline" = "HDRenderPipeline"
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
        }
        Pass
        {
            Stencil
            {
                Ref [_RefID]                    //(Ref & WriteMask) replaces probably.
                WriteMask [_MaskID]             //write mask because HDRP...
                Comp Always                     //draw pixel always if pass depth test.
                Pass Replace                    //Replace existing reference value with this reference value.
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            uniform float4 _Colour;

            fixed4 frag(v2f i) : SV_Target
            {
                return _Colour;
            }
            ENDCG
        }
    }
}