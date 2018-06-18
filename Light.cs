using OpenTK;
using System;

public class Light
{
    public Vector3 position; // position of the light in world space
    public Vector3 color; // light color
    public Vector3 specular; // specular light color

    public Light(Vector3 pos, Vector3 col, Vector3 spec)
    {
        position = pos;
        color = col;
        specular = spec;
    }

    public Vector3 lightPos
    {
        get { return position; }
    }

    public Vector3 lightColor
    {
        get { return color; }
    }

    public Vector3 specLightColor
    {
        get { return specular; }
    }
}

