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
	//vec3 lightColor = vec3( 7, 7, 5 );
	//vec3 specLightColor = vec3( 2, 2, 2);
	vec3 specLightColor = vec3( 5, 5, 5);

	//vec3 ambientColor = vec3(1, 0.1f, 0.1f );
	vec3 ambientColor = vec3(0.5f, 0.05f, 0.05f );
	vec3 materialColor = texture( pixels, uv).xyz;
	vec3 diffuseColor = materialColor;
	vec3 specularColor = vec3(1, 1, 1);
	//vec3 specularColor = vec3(0.5f, 0.5f, 0.5f);

	float attenuation = 1.0f / (dist* dist);

	float alfa = 1.0f;
	
	vec3 Rv = -L + 2*dot(L,normal.xyz)*normal.xyz;
	Rv = normalize(Rv);

	outputColor = vec4( ambientColor + diffuseColor * max( 0.0f, dot( L, normal.xyz) ) * attenuation * lightColor + 
		specularColor * pow(max( 0.0f, dot( L, Rv) ), alfa) * attenuation * specLightColor, 1 );

		// TODO: is het idee niet dat de felle vlek kleiner wordt naarmate alfa groter wordt? Hier lijkt de vlek gewoon te verdwijnen.
		// TODO: waarom zitten er meerdere lichte plekken op de theepot, terwijl er maar één lichtbron is? (Er is altijd een vlek aan de linkerkant.)
}
