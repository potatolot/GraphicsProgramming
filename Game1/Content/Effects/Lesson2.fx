#if OPENGL
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D MainTex;
sampler2D MainTextureSampler = sampler_state
{
    Texture = <MainTex>;
    MipFilter = POINT;
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
};

Texture2D NormalTex;
sampler2D NormalTextureSampler = sampler_state
{
    Texture = <NormalTex>;
    MipFilter = POINT;
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
};

Texture2D SpecularTex;
sampler2D SpecularTextureSampler = sampler_state
{
    Texture = <SpecularTex>;
    MipFilter = POINT;
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
};

// Getting out vertex data from vertex shader to pixel shader
struct VertexShaderOutput {
    float4 position     : SV_POSITION;
    float4 color        : COLOR0;
    float2 uv           : TEXCOORD0;
    float3 worldPos     : TEXCOORD1;
    float3 worldNormal  : TEXCOORD2;
};

// External Properties
float4x4 World, View, Projection;

float3 CameraPosition;

float4 LightColors[6]; //= { float4(0.8f, 0.14f, 0.5f, 1.0f) };
float3 LightPositions[6]; //= { float3(0.1f, 2.5f, 0.1f) };


// Vertex shader, receives values directly from semantic channels
VertexShaderOutput MainVS(float4 position : POSITION, float4 color : COLOR0, float2 uv : TEXCOORD, float3 normal : NORMAL) 
{ 
    VertexShaderOutput output = (VertexShaderOutput)0;

    output.position = mul(mul(mul(position, World), View), Projection);
    output.color = color;
    output.uv = uv;
    output.worldPos = mul(position, World);
    output.worldNormal = mul(normal, World);

    return output;
}

// Pixel Shader, receives input from vertex shader, and outputs to COLOR semantic
float4 MainPS(VertexShaderOutput input) : COLOR
{
    // calculate the view direction
    float3 viewDirection = input.worldPos - CameraPosition;    

    // Load in textues
    float3 normal = tex2D(NormalTextureSampler, input.uv);
    float4 texColor = tex2D(MainTextureSampler, input.uv);
    //float3 specular = tex2D(SpecularTextureSampler, input.uv);

    float3 perturbedNormal = input.worldNormal;
    perturbedNormal.rg += (normal.rg * 2 - 1);   
    perturbedNormal = normalize(perturbedNormal);


    float3 outputColor = float3(0, 0, 0);
    float3 phong = float3(0, 0, 0);

    for (unsigned int i = 0; i < LightPositions.Length; i++)
    {
        float3 lightDirection = normalize(input.worldPos - LightPositions[i]);

        float3 refLightDir = normalize(-reflect(lightDirection, perturbedNormal));        

        float lightValue = max(dot(perturbedNormal, -lightDirection), 0.0);

        float3 lightColor = LightColors[i];
        lightColor *= lightValue;

        phong += /*specular * */pow(max(dot(refLightDir, normalize(viewDirection)), 0.0), 8);

        outputColor += lightColor;
    }

    return float4((outputColor + phong) * texColor.rgb , 1);
}

technique
{
    pass
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};