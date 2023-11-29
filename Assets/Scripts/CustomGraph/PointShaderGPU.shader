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
		#pragma surface ConfigureSurface Standard fullforwardshadows addshadow
        #pragma instancing_options procedural:ConfigureProcedural
        #pragma target 4.5
        
        struct Input
        {
			float3 worldPos;
		};
        float _Smoothness;
        float _Metallic;
        StructuredBuffer<float3> _Positions;

        void ConfigureProcedural()
        {
	        
        }
        
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