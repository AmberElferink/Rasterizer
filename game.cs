using System.Diagnostics;
using OpenTK;
using System;
using System.Collections.Generic;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

// minimal OpenTK rendering framework for UU/INFOGR
// Jacco Bikker, 2016

namespace Template_P3
{

    public class Game
    {
        // member variables
        public Surface screen;              // background surface for printing etc.
        Mesh teapot, floor; // a mesh to draw using OpenGL
        Node teapotNode, floorNode; //the corresponding Nodes
        SceneGraph sceneGraph;
        float Yteapot;
        bool Upwards;
    



    // initialize
        public void Init()
        {
            Upwards = true;
            sceneGraph = new SceneGraph(this);
            LoadMeshes();



        }

        void LoadMeshes()
        {
            floor = new Mesh("../../assets/floor.obj");



            floorNode = new Node("floor", null, floor, Matrix4.Identity, sceneGraph);
            // load teapot
            teapot = new Mesh("../../assets/teapot.obj");
            teapotNode = new Node("teapot", floorNode, teapot, Matrix4.CreateTranslation(0, 0.5f, -0.5f), sceneGraph);


        }

        // tick for background surface
        public void Tick()
        {
            screen.Clear(0);
            screen.Print("hello world", 2, 2, 0xffff00);
            if (Yteapot > 1)
                Upwards = false;
            if (Yteapot < 0)
                Upwards = true;

            if(Upwards)
                Yteapot += 0.01f;
            else
                Yteapot -= 0.01f;
            Console.WriteLine(Yteapot);
            teapotNode.Matrix = Matrix4.CreateTranslation(0, Yteapot, -0.5f);
        }

        // tick for OpenGL rendering code
        public void RenderGL()
        {
            sceneGraph.RenderSceneGraph();
        }
    }

} // namespace Template_P3