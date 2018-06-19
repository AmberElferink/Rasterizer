#version 330
// shader input
in vec2 uv;                 // interpolated texture coordinates
in vec4 normal;             // interpolated normal, world space
in vec4 worldPos;           // world space position of fragment

//shader output
out vec4 outputColor;       

// texture sampler
uniform sampler2D pixels; 

// ambient light color
uniform vec3 ambientColor;

// lights, stored in matrices (position, color, specular color)
uniform mat3 light1;
uniform mat3 light2;
uniform mat3 light3;
uniform mat3 light4;
uniform int lightCount;
// TODO: kan ik ook direct de lijst met lichten doorgeven? (Of array?)
// Nu wordt wel het aantal lichten doorgegeven (lightCount), maar omdat we toch alle lichten apart moeten declareren hebben we daar niks aan.

// fragment shader
void main()             
{
	outputColor = vec4(ambientColor, 1);
	
	vec3 vecx = vec3(1,0,0);
	vec3 vecy = vec3(0,1,0);
	vec3 vecz = vec3(0,0,1);

	mat3 lights[4] = mat3[4](light1, light2, light3, light4);
	// TODO: hoe kun je eerst een array (of list?) declareren en daarna pas elementen toevoegen?
	// Nu hebben we namelijk nog altijd niks aan het weten van lightCount, omdat we alsnog alles 'met de hand' moeten toevoegen.

	for (int i = 0; i < 4; i++)
	{
	vec3 lightPos = lights[i]*vecx;
	vec3 lightColor = lights[i]*vecy;
	vec3 specLightColor = lights[i]*vecz;
	
	vec3 L = lightPos - worldPos.xyz;
	float dist = L.length();
	L = normalize( L );	

	vec3 materialColor = texture( pixels, uv).xyz;
	vec3 diffuseColor = materialColor;
	vec3 specularColor = vec3(1, 1, 1);

	float attenuation = 1.0f / (dist* dist);

	float alfa = 1.0f;	

	vec3 Rv = vec3(0,0,0);  
	if(dot(L,normal.xyz) > 0)
		Rv = -L + 2*dot(L,normal.xyz)*normal.xyz;
	// if the normal points away from the light, there is no light at that point
	 
	outputColor += vec4(diffuseColor * max( 0.0f, dot( L, normal.xyz) ) * attenuation * lightColor + 
		specularColor * pow(max( 0.0f, dot( L, Rv) ), alfa) * attenuation * specLightColor, 1 );
	}
}
