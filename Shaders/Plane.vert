#version 410

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTexCoord;

out vec2 texCoord;

// Uniform-variables
uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform vec2 texScale;

void main()
{
	gl_Position = vec4(aPosition, 1.0) * model * view * projection; // clip coordinates

	texCoord = aTexCoord * texScale;
}