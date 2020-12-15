#version 400
uniform mat4 WorldViewProj;

in vec3 position;
in vec2 texCoord;

out vec2 psTexCoord;

void main()
{
	psTexCoord = texCoord;

	gl_Position = WorldViewProj * vec4(position, 1.0);
}
