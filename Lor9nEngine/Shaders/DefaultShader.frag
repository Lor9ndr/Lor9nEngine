#version 460 core

#define NUM_SAMPLES 20
#define NUM_SAMPLESF 20.0

out vec4 FragColor;


struct Material {
	sampler2D Diffuse;
	sampler2D Specular;
	sampler2D Normal;
	sampler2D Metallic;
	sampler2D Roughness;
	sampler2D Ao;
	sampler2D Alpha;
	float     shininess;
};


struct DirectLight {
    vec3 direction;
    vec3 color;
    float intensity;

    mat4 LightSpaceMatrix;
	sampler2D shadow;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};  
uniform DirectLight dirLight;


struct PointLight {    
    vec3 position;
    vec3 color;
    float intensity;

    float constant;
    float linear;
    float quadratic;  

    samplerCube shadow;
	float farPlane;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
}; 

struct SpotLight {
    vec3 position;
    vec3 direction;

    vec3 color;
    float intensity;

    float cutOff;
    float outerCutOff;
  
    float constant;
    float linear;
    float quadratic;
  
    mat4 LightSpaceMatrix;
	sampler2D shadow;
	float farPlane;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;       
};

in VS_OUT {
    vec3 FragPos;
    vec2 TexCoords;
    vec3 Normal;
    mat3 TBN;
    vec3 TangentFragPos;
    vec3 TangentViewPos;
} fs_in;

vec3 CalcDirLight(DirectLight light, vec3 normal, vec3 viewDir,vec3 diffColor, vec3 specColor,vec3 F0, float roughness, float metallic);
vec3 CalcPointLight(PointLight light, vec3 defaultNormal, vec3 defaultViewDir,vec3 diffColor, vec3 specColor,vec3 F0, float roughness, float metallic);
vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 viewDir,vec3 diffColor, vec3 specColor,vec3 F0, float roughness, float metallic);

float SpotLightShadowCalculation(SpotLight light, vec3 normal, vec3 lightDir);
float ShadowDirectCalculation(DirectLight light, vec3 lightDir);
float ShadowPointCalculation(PointLight light,vec3 normal, vec3 lightDir);
//PBR
float DistributionGGX(vec3 N, vec3 H, float roughness);
float GeometrySchlickGGX(float NdotV, float roughness);
float GeometrySmith(vec3 N, vec3 V, vec3 L, float roughness);
vec3 fresnelSchlick(float cosTheta, vec3 F0);

//Fog

float LinearizeDepth(float depth);

uniform DirectLight directLight;
uniform SpotLight spotLights[30];
uniform PointLight pointLights[30];
uniform Material material;
uniform int nrSpotLights;
uniform int nrPointLights;
uniform bool enableLight;


vec3 sampleOffsetDirections[NUM_SAMPLES] = vec3[]
(
   vec3(1, 1,  1), vec3( 1, -1,  1), vec3(-1, -1,  1), vec3(-1, 1,  1), 
   vec3(1, 1, -1), vec3( 1, -1, -1), vec3(-1, -1, -1), vec3(-1, 1, -1),
   vec3(1, 1,  0), vec3( 1, -1,  0), vec3(-1, -1,  0), vec3(-1, 1,  0),
   vec3(1, 0,  1), vec3(-1,  0,  1), vec3( 1,  0, -1), vec3(-1, 0, -1),
   vec3(0, 1,  1), vec3( 0, -1,  1), vec3( 0, -1, -1), vec3( 0, 1, -1)
);

const float PI = 3.14159265359;
const float near = 0.1;
const float far  = 1000.0; 
uniform vec3 viewPos;

void main()
{
    vec3 tanNormal =  texture(material.Normal, fs_in.TexCoords).rgb;
    tanNormal = normalize(tanNormal * 2.0 - 1.0);
    tanNormal = normalize(fs_in.TBN * tanNormal);
    
    vec3 defaultNormal = normalize(fs_in.Normal);
    vec3 defaultViewDir = normalize(viewPos - fs_in.FragPos);
    vec3 tanviewDir = normalize(fs_in.TangentViewPos - fs_in.TangentFragPos);

    vec3 diffuseMap = texture(material.Diffuse, fs_in.TexCoords).rgb;
    vec3 specularMap = texture(material.Specular, fs_in.TexCoords).rgb;
    float metallic  = texture(material.Metallic, fs_in.TexCoords).r;
    float roughness = texture(material.Roughness, fs_in.TexCoords).r;
    float ao        = texture(material.Ao, fs_in.TexCoords).r;
    float alpha     = texture(material.Alpha, fs_in.TexCoords).a;

    vec3 F0 = vec3(0.04); 
    F0 = mix(F0, diffuseMap, metallic);

    float gamma = 1.3;

    vec3 result = vec3(0);
    result += CalcDirLight(directLight, tanNormal, tanviewDir, diffuseMap, specularMap,F0, roughness, metallic);
     // phase 2: point lights
    for(int i = 0; i < nrPointLights; i++)
        result += CalcPointLight(pointLights[i],  tanNormal, tanviewDir, diffuseMap, specularMap, F0, roughness, metallic); 
   
    // phase 3: spot light
     for(int i = 0; i < nrSpotLights; i++)
          result += CalcSpotLight(spotLights[i], tanNormal, tanviewDir, diffuseMap, specularMap, F0, roughness, metallic);
    FragColor = vec4(result,1.0);
}

vec3 CalcDirLight(DirectLight light, vec3 normal, vec3 viewDir, vec3 diffColor, vec3 specColor,vec3 F0, float roughness, float metallic)
{
	// diffuse
	vec3 L = normalize(fs_in.TBN *(-light.direction));
    vec3 H = normalize(viewDir + L);
  
    float distance    = length(-light.direction);
    float attenuation = 1.0 / (distance * distance);
    vec3 radiance     = light.color * attenuation; 

    float NDF = DistributionGGX(normal, H, roughness);       
    float G   = GeometrySmith(normal, viewDir, L, roughness);     

    vec3 F  = fresnelSchlick(max(dot(H, viewDir), 0.0), F0);
    vec3 numerator    = NDF * G * F;
    float denominator = 4.0 * max(dot(normal, viewDir), 0.0) * max(dot(normal, L), 0.0)  + 0.000001;
    vec3 specular     = (numerator / denominator) * light.specular;  
    vec3 kS = F;
    vec3 kD = vec3(1.0) - kS;
      
    kD *= 1.0 - metallic;

    float NdotL = max(dot(normal, L), 0.0);  
    float shadow = ShadowDirectCalculation(light, L);
    return ((1.0 - shadow) * (kD * diffColor / PI + specular)) * radiance * NdotL * light.intensity;
}  

vec3 CalcPointLight(PointLight light, vec3 normal, vec3 viewDir, vec3 diffColor, vec3 specColor,vec3 F0, float roughness, float metallic)
{
	// calculate per-light radiance
    vec3 L = light.position - fs_in.TangentFragPos;
    L *= fs_in.TBN;
    float distance  = length(-L);
    L = normalize(L);
    float NdotL = max(dot(normal, L), 0.0);
    float attenuation = 0;
    if (NdotL > 0)
    {
         attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic*(distance * distance));
    }
    vec3 H = normalize(viewDir + L);
    vec3 radiance     = light.color * attenuation * light.intensity;
    
    // cook-torrance brdf
    float NDF = DistributionGGX(normal, H, roughness);
    float G   = GeometrySmith(normal, viewDir, L, roughness);
    vec3 F    = fresnelSchlick(max(dot(H, viewDir), 0.0), F0);
    
    vec3 kS = F;
    vec3 kD = vec3(1.0) - kS;
    kD *= 1.0 - metallic;	  
    
    vec3 numerator    = NDF * G * F;
    float denominator = 4.0 * max(dot(normal, viewDir), 0.0) * max(dot(normal, L), 0.0) + 0.0001;
    vec3 specular     = numerator / denominator;  

    float shadow = ShadowPointCalculation(light,  normal, L);

    return ((1.0 - shadow) * (kD * diffColor / PI + specular)) * radiance * NdotL; 
} 
// calculates the color when using a spot light.
vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 viewDir,vec3 diffColor, vec3 specColor,vec3 F0, float roughness, float metallic)
{
    
    // calculate per-light radiance
    vec3 L = light.position - fs_in.FragPos ;
    float distance  = length(fs_in.FragPos - light.position);
    L = normalize(L);
    float NdotL = max(dot(normal, L), 0.0);
    float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic*(distance * distance));
    // Spotlight (soft edges)
    float theta = dot(L, normalize(-light.direction));
    float epsilon = (light.cutOff - light.outerCutOff);
    float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);

	
    vec3 H = normalize(viewDir + L);
    vec3 radiance     = light.color * attenuation * light.intensity * intensity;
    
    // cook-torrance brdf
    float NDF = DistributionGGX(normal, H, roughness);
    float G   = GeometrySmith(normal, viewDir, L, roughness);
    vec3 F    = fresnelSchlick(max(dot(H, viewDir), 0.0), F0);
    
    vec3 kS = F;
    vec3 kD = vec3(1.0) - kS;
    kD *= 1.0 - metallic;	  
    
    vec3 numerator    = NDF * G * F;
    float denominator = 4.0 * max(dot(normal, viewDir), 0.0) * max(dot(normal, L), 0.0) + 0.0001;
    vec3 specular     = numerator / denominator;  
    float shadow = SpotLightShadowCalculation(light,  normal, L);

    return ((1.0 - shadow) * (kD * diffColor / PI + specular)) * radiance * NdotL; 
} 
        


float DistributionGGX(vec3 N, vec3 H, float roughness)
{
    float a = roughness*roughness;
    float a2 = a*a;
    float NdotH = max(dot(N, H), 0.0);
    float NdotH2 = NdotH*NdotH;

    float nom   = a2;
    float denom = (NdotH2 * (a2 - 1.0) + 1.0);
    denom = PI * denom * denom;

    return nom / denom;
}
// ----------------------------------------------------------------------------
float GeometrySchlickGGX(float NdotV, float roughness)
{
    float r = (roughness + 1.0);
    float k = (r*r) / 8.0;

    float nom   = NdotV;
    float denom = NdotV * (1.0 - k) + k;

    return nom / denom;
}
// ----------------------------------------------------------------------------
float GeometrySmith(vec3 N, vec3 V, vec3 L, float roughness)
{
    float NdotV = max(dot(N, V), 0.0);
    float NdotL = max(dot(N, L), 0.0);
    float ggx2 = GeometrySchlickGGX(NdotV, roughness);
    float ggx1 = GeometrySchlickGGX(NdotL, roughness);

    return ggx1 * ggx2;
}
// ----------------------------------------------------------------------------
vec3 fresnelSchlick(float cosTheta, vec3 F0)
{
    return F0 + (1.0 - F0) * pow(clamp(1.0 - cosTheta, 0.0, 1.0), 5.0);
}


float SpotLightShadowCalculation(SpotLight light, vec3 normal, vec3 lightDir)
{
	// perform perspective divide
	vec4 fragPosLightSpace = light.LightSpaceMatrix * vec4(fs_in.FragPos, 1.0);
    vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
    // transform to [0,1] range
    projCoords = projCoords * 0.5 + 0.5;
    // get closest depth value from light's perspective (using [0,1] range fragPosLight as coords)
    float closestDepth = texture(light.shadow, projCoords.xy).r; 
    // get depth of current fragment from light's perspective
    float currentDepth = projCoords.z;
    // calculate bias (based on depth map resolution and slope)
    float bias = max(0.05 * (1.0 - dot(normal, lightDir*2)), 0.05);
    // check whether current frag pos is in shadow
    // float shadow = currentDepth - bias > closestDepth  ? 1.0 : 0.0;
    // PCF
    float shadow = 0.0;
    vec2 texelSize = 1.0 / textureSize(light.shadow, 0);
	if (fragPosLightSpace.w > 1)
	{
		 for(int x = -1; x <= 1; ++x)
		{
			for(int y = -1; y <= 1; ++y)
			{
				float pcfDepth = texture(light.shadow, projCoords.xy + vec2(x, y) * texelSize).r; 
				shadow += currentDepth - bias >= pcfDepth  ? 1.0 : 0.0;        
			}
		}
		shadow /= 9.0;
	}
   
    
    // keep the shadow at 0.0 when outside the far_plane region of the light's frustum.
    if(projCoords.z >= 1.0)
        shadow = 1.0;
        
    return shadow;
}

float ShadowDirectCalculation(DirectLight light, vec3 lightDir)
{
	vec4 fragPosLightSpace = light.LightSpaceMatrix * vec4(fs_in.FragPos, 1.0);
    vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
    // transform to [0,1] range
    projCoords = projCoords * 0.5 + 0.5;
    // get closest depth value from light's perspective (using [0,1] range fragPosLight as coords)
    float closestDepth = texture(light.shadow, projCoords.xy).r; 
    // get depth of current fragment from light's perspective
    float currentDepth = projCoords.z;
    // calculate bias (based on depth map resolution and slope)
    vec3 normal = normalize(fs_in.Normal);
    float bias = max(0.05 * (1.0 - dot(normal, lightDir)), 0.005);
    // check whether current frag pos is in shadow
    // float shadow = currentDepth - bias > closestDepth  ? 1.0 : 0.0;
    // PCF
    float shadow = 0.0;
    vec2 texelSize = 1.0 / textureSize(light.shadow, 0);
    for(int x = -1; x <= 1; ++x)
    {
        for(int y = -1; y <= 1; ++y)
        {
            float pcfDepth = texture(light.shadow, projCoords.xy + vec2(x, y) * texelSize).r; 
            shadow += currentDepth - bias > pcfDepth  ? 1.0 : 0.0;        
        }    
    }
    shadow /= 9.0;
    
    // keep the shadow at 0.0 when outside the far_plane region of the light's frustum.
    if(projCoords.z > 1.0)
        shadow = 0.0;
        
    return shadow;
}



float ShadowPointCalculation(PointLight light,vec3 normal, vec3 lightDir)
{
	// get vector from the light to the fragment
	vec3 lightToFrag = fs_in.FragPos - light.position;

	// get current depth
	float currentDepth = length(lightToFrag);

	// calculate bias
	float minBias = 0.005;
	float maxBias = 0.05;
	float bias = max(maxBias * (1.0 - dot(normal, lightDir)), minBias);

	// PCF
	float shadow = 0.0;
	float viewDist = length(viewPos);
	float diskRadius = (1.0 + (viewDist / light.farPlane)) / 30.0;
	for (int i = 0; i < NUM_SAMPLES; i++) {
		float pcfDepth = texture(light.shadow, lightToFrag + sampleOffsetDirections[i] * diskRadius).r;
		pcfDepth *= light.farPlane;

		if (currentDepth - bias >= pcfDepth) {
			shadow += 1.0;
		}
	}

	shadow /= NUM_SAMPLESF;

	return shadow;
}
