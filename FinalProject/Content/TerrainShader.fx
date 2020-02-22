// Parameters that should be set from the program
float4x4 World; // World Matrix
float4x4 View; // View Matrix
float4x4 Projection; // Projection Matrix
float3 LightPosition; // in world space
float3 CameraPosition; // in world space
float Shininess; // scalar value
float3 AmbientColor;
float3 DiffuseColor;
float3 SpecularColor;
texture NormalMap;

sampler NormalMapSampler = sampler_state
{
	Texture = <NormalMap>;

	AddressU = Wrap;

	AddressV = Wrap;
};

struct VertexInput
{
	float4 Position : POSITION0; // Here, POSITION0 and NORMAL0

	float2 UV: TEXCOORD0;
};

struct PhongVertexOutput
{
	float4 Position : POSITION0;

	float2 UV: TEXCOORD0;

	float4 WorldPosition : TEXCOORD1;
};

PhongVertexOutput PhongVertex(VertexInput input)
{
	PhongVertexOutput output;

	output.WorldPosition = mul(input.Position, World);

	float4 viewPosition = mul(output.WorldPosition, View);

	output.Position = mul(viewPosition, Projection);

	output.UV = input.UV;

	return output;
}

float4 PhongPixel(PhongVertexOutput input) : COLOR0
{
	float3 lightDirection = normalize(LightPosition - input.WorldPosition.xyz);

	float3 viewDirection = normalize(CameraPosition - input.WorldPosition.xyz);

	float3 normal = normalize(mul(tex2D(NormalMapSampler, input.UV).xyz * 2 - 1, World));

	float3 reflectDirection = -reflect(lightDirection, normal);

	float4 diffuse = max(dot(lightDirection, normal), 0);

	float specular = pow(max(dot(reflectDirection, viewDirection), 0), Shininess);

	return float4(AmbientColor + diffuse.x * DiffuseColor + specular * SpecularColor, 1);
}

technique Phong
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 PhongVertex();

		PixelShader = compile ps_4_0 PhongPixel();
	}
}