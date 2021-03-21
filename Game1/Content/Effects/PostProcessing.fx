#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix WorldViewProjection;

Texture2D _MainTex;
sampler2D _MainTexSampler = sampler_state
{
	Texture = <_MainTex>;
};

float4 VignettePS(float2 uv : VPOS) : COLOR
{
	uv = (uv + 0.5) * float2(1.0 / 1920.0, 1.0 / 1080.0);


	float4 color = tex2D(_MainTexSampler, uv);
	float4 vignette = 1;

	float2 offset = uv * 2 - 1;

	vignette.rgb *= smoothstep(1, .2, abs(sin(offset.x)));
	vignette.rgb *= smoothstep(1, .2, abs(sin(offset.y)));
	vignette *= color;

	return vignette;
}

float4 BlurPS(float2 uv : VPOS) : COLOR
{
	uv = (uv + 0.5) * float2(1.0 / 1920.0, 1.0 / 1080.0);

	float4 color = tex2D(_MainTexSampler, uv);
	color += tex2D(_MainTexSampler, uv + (.005)) * 2;
	
	return color/2;
}

float4 ChromaticAberrationPS(float2 uv : VPOS) : COLOR
{
	uv = (uv + 0.5) * float2(1.0 / 1920.0, 1.0 / 1080.0);

	float strenght = 5.0;
	float3 rgbOffset = 1 + float3(0.01, 0.005, 0) * strenght;
	float dist = distance(uv, float2(.5, .5));
	float2 dir = uv - float2(.5, .5);

	rgbOffset = normalize(rgbOffset * dist);

	float2 uvR = float2(.5, .5) + rgbOffset.r * dir;
	float2 uvG = float2(.5, .5) + rgbOffset.g * dir;
	float2 uvB = float2(.5, .5) + rgbOffset.b * dir;

	float4 colorR = tex2D(_MainTexSampler, uvR);
	float4 colorG = tex2D(_MainTexSampler, uvG);
	float4 colorB = tex2D(_MainTexSampler, uvB);

	return float4(colorR.r, colorG.g, colorB.b, 1.0);
}

technique ChromaticAberration
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL ChromaticAberrationPS();
	}
};

technique Vignette
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL VignettePS();
	}
};

technique Blur
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL BlurPS();
	}
};