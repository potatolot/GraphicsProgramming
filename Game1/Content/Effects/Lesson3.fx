#if OPENGL
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D DayTex;
sampler2D DayTextureSampler = sampler_state
{
    Texture = <DayTex>;
    MipFilter = POINT;
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
};

Texture2D NightTex;
sampler2D NightTextureSampler = sampler_state
{
    Texture = <NightTex>;
    MipFilter = POINT;
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
};

Texture2D CloudsTex;
sampler2D CloudsTextureSampler = sampler_state
{
    Texture = <CloudsTex>;
    MipFilter = POINT;
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    AddressU = WRAP; //CLAMP, MIRROR
};

Texture2D MoonTex;
sampler2D MoonTextureSampler = sampler_state
{
    Texture = <MoonTex>;
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

float3 LightColor; 
float3 LightPosition; 

float Time;


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
    // Load in textues
    float4 dayColor = tex2D(DayTextureSampler, input.uv);
    float4 nightColor = tex2D(NightTextureSampler, input.uv);
    float4 cloudColor = tex2D(CloudsTextureSampler, input.uv + half2(-Time * 0.009f, 0));

    // Calculate view direction
    float3 viewDirection = normalize(input.worldPos - CameraPosition);

    float3 outputColor = float3(0, 0, 0);
    float3 phong = float3(0, 0, 0);
    float3 diffuse = float3(0, 0, 0);


    float3 lightDirection = normalize(input.worldPos - LightPosition);

    float3 refLightDir = normalize(-reflect(lightDirection, input.worldNormal));

    float lightValue = max(dot(input.worldNormal, -lightDirection), 0.0);

    float3 skyColor = float3(.529, .808, .992);
    float3 fresnel = pow(max(dot(input.worldNormal, viewDirection) * .5f + .5f, 0.0), 3) * 8 * lightValue * skyColor;

    float3 lightColor = LightColor;
    lightColor *= lightValue;

    phong += pow(max(dot(refLightDir, normalize(viewDirection)), 0.0), 8);

    outputColor += lightColor;
    diffuse = lerp(nightColor.rgb, dayColor.rgb, lightValue) + cloudColor.rgb * lightValue;


    return float4((max(outputColor, 0.2) + phong) * diffuse.rgb + fresnel, 1);
}

// Pixel Shader, receives input from vertex shader, and outputs to COLOR semantic
float4 MoonPS(VertexShaderOutput input) : COLOR
{
    // Load in textues
    float4 moonColor = tex2D(MoonTextureSampler, input.uv);

    float3 lightDirection = normalize(input.worldPos - LightPosition);

    float lightValue = min(max(dot(input.worldNormal, -lightDirection), 0.0) * 64, 1.0);

    return float4(max(lightValue, 0.1) * moonColor.rgb, 1);
}

technique Earth
{
    pass
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};


technique Moon
{
    pass
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MoonPS();
    }
};