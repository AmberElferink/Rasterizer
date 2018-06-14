#version 330
// shader input
in vec2 uv;                 // interpolated texture coordinates
in vec4 normal;             // interpolated normal, world space
in vec4 worldPos;           // world space position of fragment

//shader output
out vec4 outputColor;       

// texture sampler
uniform sampler2D pixels;   

// light position in world space
uniform vec3 lightPos;      

// fragment shader
void main()             
{
	vec3 L = lightPos - worldPos.xyz;
	float dist = L.length();
	L = normalize( L );
	vec3 lightColor = vec3( 10, 10, 8 );
	vec3 specLightColor = vec3( 2, 2, 2);

	vec3 ambientColor = vec3(1, 0.1f, 0.1f );
	vec3 materialColor = texture( pixels, uv).xyz;
	vec3 diffuseColor = materialColor;
	vec3 specularColor = vec3(1, 1, 1);

	float attenuation = 1.0f / (dist* dist);

	float alfa = 0.3f;

	vec3 Rv = L;

	outputColor = vec4( ambientColor + diffuseColor * max( 0.0f, dot( L, normal.xyz) ) * attenuation * lightColor + 
		specularColor * pow(max( 0.0f, dot( L, Rv) ), alfa) * attenuation * specLightColor, 1 );
}
