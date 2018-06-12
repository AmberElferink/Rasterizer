using System.Diagnostics;
using OpenTK;
using System.Collections.Generic;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

// minimal OpenTK rendering framework for UU/INFOGR
// Jacco Bikker, 2016

namespace Template_P3 {

public class Game
{
        // member variables
    public Surface screen;              // background surface for printing etc.
    Mesh teapot, floor;                     // a mesh to draw using OpenGL
        SceneGraph sceneGraph;




	// initialize
	public void Init()
	{
            sceneGraph = new SceneGraph(this);
            LoadMeshes();



	}

    void LoadMeshes()
    {
            floor = new Mesh("../../assets/floor.obj");

            Matrix4 transform = Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), 0);
            transform *= Matrix4.CreateTranslation(0, -4, -15);
            transform *= Matrix4.CreatePerspectiveFieldOfView(1.2f, 1.3f, .1f, 1000);

            Node floorNode = new Node("floor", null, floor, transform, sceneGraph);
            // load teapot
            teapot = new Mesh("../../assets/teapot.obj");
            Node teapotNode = new Node("teapot", floorNode, teapot, transform, sceneGraph);


    }

	// tick for background surface
	public void Tick()
	{
		screen.Clear( 0 );
		screen.Print( "hello world", 2, 2, 0xffff00 );
	}

	// tick for OpenGL rendering code
	public void RenderGL()
	{
            sceneGraph.RenderSceneGraph();
	}
}

} // namespace Template_P3