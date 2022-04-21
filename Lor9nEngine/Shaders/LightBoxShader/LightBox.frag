#version 460 core

layout(location = 0) out vec4 FragColor;

uniform vec3 lightColor;
uniform sampler2D Diffuse;
in vec2 TexCoords;

void main()
{           
	vec4 diff = texture(Diffuse, TexCoords);
	if(diff.a < 0.5){discard;}
	FragColor = vec4(lightColor,1.0) * diff;
}