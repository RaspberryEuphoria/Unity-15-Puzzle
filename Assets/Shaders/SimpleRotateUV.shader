Shader "Custom/SimpleRotateUV" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _BumpMap ("Bumpmap", 2D) = "bump" {}
        _Angle ("Angle", Range(-5.0,  5.0)) = 0.0
    }

    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows vertex:vert
        #pragma target 3.0
        sampler2D _MainTex;
        sampler2D _BumpMap;
        struct Input {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float2 val;
        };
        float _RotationSpeed;
        float _Angle;
        void vert (inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input,o);
            float sinX = sin(_Angle);
            float cosX = cos(_Angle);
            float2x2 rotationMatrix = float2x2( cosX, -sinX, sinX, cosX);
            o.val.xy = mul ( v.texcoord.xy - 0.5, rotationMatrix ) + 0.5;
        }
        fixed4 _Color;
        void surf (Input IN, inout SurfaceOutputStandard o) {
            fixed4 c = tex2D (_MainTex, IN.val) * _Color;
            o.Albedo = c.rgb;
            o.Normal = UnpackNormal (tex2D (_BumpMap, IN.val));
        }
        ENDCG
    }

    FallBack "Diffuse"
}
