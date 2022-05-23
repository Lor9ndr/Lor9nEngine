#version 460 core
layout (location = 0) in vec3 aPos;
layout (location = 5) in ivec4 boneIds;
layout (location = 6) in vec4 weights;

uniform mat4 model;
const int MAX_BONES = 100;
const int MAX_BONE_INFLUENCE = 4;
uniform mat4 finalBonesMatrices[MAX_BONES];

void main()
{
    vec4 totalPosition = vec4(0.0f);
    vec3 localNormal = vec3(0.0f);
    if(boneIds[0] != -1)
    {
        for(int i = 0 ; i < MAX_BONE_INFLUENCE ; i++)
        {
            if(boneIds[i] == -1) 
                continue;
            if(boneIds[i] >= MAX_BONES) 
            {
                totalPosition = vec4(aPos,1.0f);
                break;
            }
            vec4 localPosition = finalBonesMatrices[boneIds[i]] * vec4(aPos,1.0f);
            totalPosition +=  localPosition * weights[i];
        }
        //vs_out.FragPos = vec3(model * vec4(totalPosition.xyz, 1.0f));
    }
    else
    {
        totalPosition = vec4(aPos,1.0f);
    }
    gl_Position = model * vec4(totalPosition.xyz, 1.0);
}