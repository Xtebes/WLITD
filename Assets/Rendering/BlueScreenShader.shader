Shader "Custom/NewSurfaceShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255
        _ColorMask("Color Mask", Float) = 15
        _Mask("Mask", 2D) = "white" {}
    }
    SubShader
    {
        Cull Off Zwrite Off Ztest Always 
        Tags
        {
            "Queue" = "Transparent+1"
        }
        Stencil
        {
            Ref[_Stencil]
            Comp[_StencilComp]
            Pass[_StencilOp]
            ReadMask[_StencilReadMask]
            WriteMask[_StencilWriteMask]
        }
        ColorMask[_ColorMask]
        pass
        {
            Blend SrcAlpha OneMinusSrcAlpha         
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            sampler2D _MainTex;
            sampler2D _Mask;
            struct appdata
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };
            struct v2f
            {
                float4 vertex   : POSITION;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
            };
            
            v2f vert(appdata v)
            {
                v2f o;
                o.worldPosition = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                return o;
            }
            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 textureColor = tex2D(_MainTex, i.texcoord);
                fixed4 maskColor = tex2D(_Mask, i.texcoord);
                textureColor = textureColor * ((maskColor.r + maskColor.g + maskColor.b) / 3.0f);
                return textureColor;
            }                  
            ENDCG
        }
    }
}
