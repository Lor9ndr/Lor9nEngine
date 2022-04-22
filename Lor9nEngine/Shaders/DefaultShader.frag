#version 460 core


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

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
}; 

struct SpotLight {
    vec3 position;
    vec3 direction;
    float cutOff;
    float outerCutOff;
  
    float constant;
    float linear;
    float quadratic;
  
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

vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 fragPos, vec3 viewDir);

//PBR
float DistributionGGX(vec3 N, vec3 H, float roughness);
float GeometrySchlickGGX(float NdotV, float roughness);
float GeometrySmith(vec3 N, vec3 V, vec3 L, float roughness);
vec3 fresnelSchlick(float cosTheta, vec3 F0);

//Fog

float LinearizeDepth(float depth);

uniform DirectLight directLight;
uniform SpotLight spotlights[10];
uniform PointLight pointLights[10];
uniform Material material;
uniform int nrSpotLights;
uniform int nrPointLights;
uniform bool enableLight;

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
    result = CalcDirLight(directLight, tanNormal, tanviewDir, diffuseMap, specularMap,F0, roughness, metallic);
     // phase 2: point lights
    for(int i = 0; i < nrPointLights; i++)
        result += CalcPointLight(pointLights[i],  defaultNormal,  defaultViewDir, diffuseMap, specularMap, F0, roughness, metallic); 
   
   //// phase 3: spot light
   //for(int i = 0; i < nrSpotLights; i++)
   //    result += CalcSpotLight(spotlights[i], norm,  fs_in.FragPos, viewDir);    
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
    kD *= light.diffuse;

    float NdotL = max(dot(normal, L), 0.0);        
    return (kD * diffColor / PI + specular) * radiance * NdotL * light.intensity;
}  

vec3 CalcPointLight(PointLight light, vec3 defaultNormal, vec3 defaultViewDir, vec3 diffColor, vec3 specColor,vec3 F0, float roughness, float metallic)
{
	// calculate per-light radiance
    vec3 L = light.position - fs_in.TangentFragPos;
    L *= fs_in.TBN;
    float distance  = length(L);
    L = normalize(L);
    float NdotL = max(dot(defaultNormal, L), 0.0);
    float attenuation = 0;
    if (NdotL > 0)
    {
         attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic*(distance * distance));
    }
    vec3 H = normalize(defaultViewDir + L);
    vec3 radiance     = light.color * attenuation * light.intensity;
    
    // cook-torrance brdf
    float NDF = DistributionGGX(defaultNormal, H, roughness);
    float G   = GeometrySmith(defaultNormal, defaultViewDir, L, roughness);
    vec3 F    = fresnelSchlick(max(dot(H, defaultViewDir), 0.0), F0);
    
    vec3 kS = F;
    vec3 kD = vec3(1.0) - kS;
    kD *= 1.0 - metallic;	  
    
    vec3 numerator    = NDF * G * F;
    float denominator = 4.0 * max(dot(defaultNormal, defaultViewDir), 0.0) * max(dot(defaultNormal, L), 0.0) + 0.0001;
    vec3 specular     = numerator / denominator;  
        

    return (kD * diffColor / PI + specular) * radiance * NdotL; 
} 
// calculates the color when using a spot light.
vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    vec3 lightDir = normalize( light.position - fragPos);
    // diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);
    // specular shading
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    // attenuation
    float distance = length(light.position - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));    
    // spotlight intensity
    float theta = dot(lightDir, normalize(-light.direction)); 
    float epsilon = light.cutOff - light.outerCutOff;
    float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);
    // combine results
    vec3 ambient  = light.ambient  * vec3(texture(material.Diffuse, fs_in.TexCoords));
    vec3 diffuse  = light.diffuse  * diff * vec3(texture(material.Diffuse, fs_in.TexCoords));
    vec3 specular = light.specular * spec * vec3(texture(material.Specular, fs_in.TexCoords));
    ambient *= attenuation * intensity;
    diffuse *= attenuation * intensity;
    specular *= attenuation * intensity;
    return (ambient + diffuse + specular);
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