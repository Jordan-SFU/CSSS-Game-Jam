Shader "Custom/PosterizeEffect"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Levels ("Color Levels", Range(2, 16)) = 4
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Overlay" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Levels;

            fixed4 frag(v2f_img i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col.rgb = floor(col.rgb * _Levels) / _Levels;
                return col;
            }
            ENDCG
        }
    }
}
