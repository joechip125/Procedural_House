#include <UnityShaderVariables.cginc>

void Ripple_float (
float3 PositionIn, float3 Origin,
float Period, float Speed, float Amplitude,
out float3 PositionOut
) {
    float3 p = PositionIn - Origin;
    static const float PI = 3.14159265f;
    float d = length(p);
    float f = 2.0 * PI * Period * (d - Speed * _Time.y);
	
    PositionOut = PositionIn + float3(0.0, Amplitude * sin(f), 0.0);
}
