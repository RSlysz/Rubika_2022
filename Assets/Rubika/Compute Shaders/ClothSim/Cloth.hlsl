#include "./ClothInclude.hlsl"

StructuredBuffer<Point> points;
uint gridCount;

void SGClothPosition_float(in uint vertexID, out float3 position, out float3 normal)
{
	position = points[vertexID].position;
	normal = points[vertexID].normal;
}