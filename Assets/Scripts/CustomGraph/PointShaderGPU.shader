

Shader "Graph/Point Surface GPU" 
{
	Properties 
	{
		_Smoothness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.5
	}
	
    SubShader
    {
        CGPROGRAM
		#pragma surface ConfigureSurface Standard fullforwardshadows
        #pragma target 3.0
        struct Input
        {
			float3 worldPos;
		};
        float _Smoothness;
        float _Metallic;
        
		void ConfigureSurface (Input input, inout SurfaceOutputStandard surface)
		{
			surface.Smoothness = _Smoothness;
			surface.Albedo.rg = input.worldPos.xy * 0.5 + 0.5;
			surface.Metallic = _Metallic;
		}
        
		ENDCG
    }
    
    Fallback "Diffuse"
}