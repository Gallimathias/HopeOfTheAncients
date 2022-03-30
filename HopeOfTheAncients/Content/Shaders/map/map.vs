#version 400
uniform mat4 WorldViewProj;

in vec3 position;
in vec2 texCoord;
in uint texIndex;

out vec2 psTexCoord;
flat out uint psTexIndex;

void main()
{
	psTexCoord = texCoord;
	psTexIndex = texIndex;

	gl_Position = WorldViewProj * vec4(position, 1.0);
}
