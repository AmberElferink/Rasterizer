﻿using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Diagnostics;

// minimal OpenTK rendering framework for UU/INFOGR
// Jacco Bikker, 2016

namespace Template_P3
{

    public class Game
    {
        // member variables
        public Surface screen;                  // background surface for printing etc.
        public Shader shader;                   // shader to use for rendering
        public Shader postproc;                 // shader to use for post processing
        public Texture wood;                    // texture to use for rendering


        Mesh teapot, floor;                     // a mesh to draw using OpenGL
        Node teapotNode, floorNode;             //the corresponding Nodes
        SceneGraph sceneGraph;
        Matrix4 Tworld, Tcam, TcamPerspective;
        const float PI = 3.1415926535f;			// PI
        float a = 0;                            // world rotation angle
        Stopwatch timer;                        // timer for measuring frame duration


        float Yteapot;
        bool Upwards;



        // initialize
        public void Init()
        {
            // initialize stopwatch
            timer = new Stopwatch();
            timer.Reset();
            timer.Start();

            //set initial basic matrices
            Tcam = Matrix4.CreateTranslation(0, 4, 15);
            TcamPerspective = Matrix4.CreatePerspectiveFieldOfView(1.2f, 1.3f, .1f, 1000);

            // create shaders
            shader = new Shader("../../shaders/vs.glsl", "../../shaders/fs.glsl");
            postproc = new Shader("../../shaders/vs_post.glsl", "../../shaders/fs_post.glsl");
            // load a texture
            wood = new Texture("../../assets/wood.jpg");

            // set the ambient color
            int ambientID = GL.GetUniformLocation(shader.programID, "ambientColor");
            GL.UseProgram(shader.programID);
            GL.Uniform3(ambientID, 0.5f, 0.05f, 0.05f);

            //--------------NEW----------------------
            // set the lights
            List<Light> allLights = new List<Light>();

            Light light1 = new Light(
                new Vector3(7.0f, 5.0f, 2.0f), // position
                new Vector3(10, 10, 8), // color
                new Vector3(5, 0, 0)); // specular color
            allLights.Add(light1);
            Matrix3 lightMat1 = new Matrix3(light1.lightPos, light1.lightColor, light1.specLightColor); // store position, color, specular color in matrix
            lightMat1 = Matrix3.Transpose(lightMat1); // now the position is the first column, color second, specular color third
            int lightMat1ID = GL.GetUniformLocation(shader.programID, "light1");
            GL.UseProgram(shader.programID);
            GL.UniformMatrix3(lightMat1ID, true, ref lightMat1);
            // TODO: what does the bool transpose do? We now forwarded the matrix in the correct setting, so it must not be transposed again.
            // It seems to go right (same output as before), but the bool does not make much sense. (Not too important; works)

            Light light2 = new Light(
                new Vector3(-7.0f, 5.0f, 2.0f), // position
                new Vector3(1, 1, 6), // color
                new Vector3(0, 0, 5)); // specular color
            allLights.Add(light2);
            Matrix3 lightMat2 = new Matrix3(light2.lightPos, light2.lightColor, light2.specLightColor); // store position, color, specular color in matrix
            lightMat2 = Matrix3.Transpose(lightMat2); // now the position is the first column, color second, specular color third
            int lightMat2ID = GL.GetUniformLocation(shader.programID, "light2");
            GL.UseProgram(shader.programID);
            GL.UniformMatrix3(lightMat2ID, true, ref lightMat2);

            Light light3 = new Light(
                new Vector3(7.0f, 5.0f, -2.0f), // position
                new Vector3(0, 0, 0), // color
                new Vector3(0, 5, 0)); // specular color
            allLights.Add(light3);
            Matrix3 lightMat3 = new Matrix3(light3.lightPos, light3.lightColor, light3.specLightColor); // store position, color, specular color in matrix
            lightMat3 = Matrix3.Transpose(lightMat3); // now the position is the first column, color second, specular color third
            int lightMat3ID = GL.GetUniformLocation(shader.programID, "light3");
            GL.UseProgram(shader.programID);
            GL.UniformMatrix3(lightMat3ID, true, ref lightMat3);

            Light light4 = new Light(
                new Vector3(-7.0f, 5.0f, 2.0f), // position
                new Vector3(0, 0, 0), // color
                new Vector3(0, 0, 0)); // specular color
            allLights.Add(light4);
            Matrix3 lightMat4 = new Matrix3(light4.lightPos, light4.lightColor, light4.specLightColor); // store position, color, specular color in matrix
            lightMat4 = Matrix3.Transpose(lightMat4); // now the position is the first column, color second, specular color third
            int lightMat4ID = GL.GetUniformLocation(shader.programID, "light4");
            GL.UseProgram(shader.programID);
            GL.UniformMatrix3(lightMat4ID, true, ref lightMat4);

            int lightCountID = GL.GetUniformLocation(shader.programID, "lightCount");
            GL.UseProgram(shader.programID);
            GL.Uniform1(lightCountID, allLights.Count);

            // TODO: is there a better ('prettier' or more efficient) way to initialize all these lights and pass them to the shader?
            // Storing them all in an array or list would be ideal (see also fs.glsl).
            //------------------------------------


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

            //increase and decrease the Ypos of the teapot to create a bounce effect
            if (Yteapot > 1)
                Upwards = false;
            if (Yteapot < 0)
                Upwards = true;

            if (Upwards)
                Yteapot += 0.01f;
            else
                Yteapot -= 0.01f;


            // measure frame duration
            float frameDuration = timer.ElapsedMilliseconds;
            timer.Reset();
            timer.Start();
            // update rotation
            a += 0.001f * frameDuration;
            if (a > 2 * PI) a -= 2 * PI;
            Tworld = Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), a);

            teapotNode.Matrix = Matrix4.CreateTranslation(0, Yteapot, -0.5f);
        }




        // tick for OpenGL rendering code
        public void RenderGL()
        {
            sceneGraph.RenderSceneGraph();
        }

        public Matrix4 TCamera
        {
            get {return Tcam; }
        }

        public Matrix4 TWorld
        {
            get { return Tworld; }
        }

        public Matrix4 TCamPerspective
        {
            get { return TcamPerspective; }
        }
    }

} // namespace Template_P3