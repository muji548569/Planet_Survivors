#ifndef TOON_LIGHTING_INCLUDED
#define TOON_LIGHTING_INCLUDED

#ifndef SHADERGRAPH_PREVIEW
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#endif

void GetMainLight_float(out float3 Direction)
{
#ifdef SHADERGRAPH_PREVIEW
    Direction = float3(0.5, 0.5, 0);
#else
    Light mainLight = GetMainLight();
    Direction = mainLight.direction;
#endif
}

#endif