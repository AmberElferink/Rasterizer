using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Diagnostics;
using OpenTK.Input;
using System;

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


        Mesh sun, earth, moon, floor, earthpot, skybox;                     // a mesh to draw using OpenGL
        Node sunNode, earthNode, moonNode, floorNode, earthpotnode, skyboxnode;         //the corresponding Nodes
        public Texture sunTexture, earthTexture, moonTexture, wood, earthpottexture, skyboxTexture;          // texture to use for rendering
        public Texture earthNormal; //normals
        SceneGraph sceneGraph;
        Matrix4 Tworld, Tcam, TcamPerspective;
        const float PI = 3.1415926535f;			// PI
        float a = 0; // world rotation angle
        float moonorbit = 0, earthorbit = 0; //planet orbit angle
        float moonrotation = 0, earthrotation = 0; // rotation angle
        Stopwatch timer;                        // timer for measuring frame duration
        KeyboardState keyboardstate;
        MouseState mousestate;

        int prevMouseY = 0;
        int prevMouseX = 0;

        Matrix4 TcamTranslation;

        //float c = 0; // color increase



        // initialize
        public void Init()
        {
            // initialize stopwatch
            timer = new Stopwatch();
            timer.Reset();
            timer.Start();

            //set initial basic matrices
            TcamTranslation = Matrix4.CreateTranslation(0, 4, 15);
            Tcam = TcamTranslation;
            TcamPerspective = Matrix4.CreatePerspectiveFieldOfView(1.2f, 1.3f, .1f, 1000);


            // create shaders
            shader = new Shader("../../shaders/vs.glsl", "../../shaders/fs.glsl");
            postproc = new Shader("../../shaders/vs_post.glsl", "../../shaders/fs_post.glsl");

            // set the ambient color
            int ambientID = GL.GetUniformLocation(shader.programID, "ambientColor");
            GL.UseProgram(shader.programID);
            GL.Uniform3(ambientID, 0.1f, 0.1f, 0.1f);

            // set the lights (positions, colors)
            passLights();


            sceneGraph = new SceneGraph(this);
            LoadMeshes();
        }



        void LoadMeshes()
        {
            earthNormal = new Texture("../../assets/Earth/Textures/Earth_Normal.jpg");

            // load a texture
            wood = new Texture("../../assets/wood.jpg");
            floor = new Mesh("../../assets/floor.obj");
            floorNode = new Node("floor", null, floor, Matrix4.Identity, wood, null, sceneGraph);
            // load teapot

            skyboxTexture = new Texture("../../assets/skybox1.jpg");
            skybox = new Mesh("../../assets/skyboxH.obj");
            skyboxnode = new Node("skybox", floorNode, skybox, Matrix4.Identity, skyboxTexture, null, sceneGraph);

            sunTexture = new Texture("../../assets/Sun/2k_sun.jpg");
            sun = new Mesh("../../assets/Earth/Earth.obj");
            sunNode = new Node("sun", floorNode, sun,
                 Matrix4.CreateRotationY(moonrotation) * //rotation around its center
                Matrix4.CreateTranslation(0, 0, 0) * //distance from moon center to the earth center
                Matrix4.CreateRotationY(moonorbit)* //rotation around the earth
                Matrix4.CreateScale(1.5f, 1.5f, 1.5f),
                sunTexture, null, sceneGraph);

            earthpottexture = new Texture("../../assets/Earth/Textures/Earth_Diffuse.jpg");
            earthpot = new Mesh("../../assets/teapot.obj");
            earthpotnode = new Node("earthpot", sunNode, earthpot, Matrix4.CreateTranslation(0, 3, 0), earthpottexture, earthNormal, sceneGraph);

            earthTexture = new Texture("../../assets/Earth/Textures/Earth_Diffuse.jpg");

            earth = new Mesh("../../assets/Earth/Earth.obj");
            earthNode = new Node("earth", sunNode, earth,
                                Matrix4.CreateRotationY(moonrotation) * //rotation around its center
                Matrix4.CreateTranslation(130, 0, 0) * //distance from moon center to the earth center
                Matrix4.CreateRotationY(moonorbit), //rotation around the earth
                earthTexture, earthNormal, sceneGraph);

            moonTexture = new Texture("../../assets/Moon/Textures/2k_moon.jpg");
            moon = new Mesh("../../assets/Earth/Earth.obj");
            moonNode = new Node("earth", earthNode, earth, Matrix4.Identity, moonTexture, null, sceneGraph);  //the matrix of moving objects is set in Tick()

        }




        // tick for background surface
        public void Tick()
        {
            HandleInput();
            


            screen.Clear(0);
            //screen.Print("hello world", 2, 2, 0xffff00);


            // measure frame duration
            float frameDuration = timer.ElapsedMilliseconds;
            timer.Reset();
            timer.Start();
            // update rotation
            //a += 0.001f * frameDuration;
            if (a > 2 * PI) a -= 2 * PI;
            Tworld = Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), a);

            moonorbit = Rotate(moonorbit, 0.0001f, frameDuration);
            moonrotation = Rotate(moonrotation, 0.001f, frameDuration);

            moonNode.Matrix = Matrix4.CreateRotationY(moonrotation) * //rotation around its center
               Matrix4.CreateTranslation(-250f, 0, 0) * //distance from moon center to the earth center
               Matrix4.CreateRotationY(moonorbit) * //rotation around the earth
               Matrix4.CreateScale(0.25f, 0.25f, 0.25f); //size relative to earth


            earthorbit = Rotate(earthorbit, 0.001f, frameDuration);
            earthrotation = Rotate(earthrotation, 0.002f, frameDuration);

            earthNode.Matrix = Matrix4.CreateRotationY(moonrotation) * //rotation around its center
                Matrix4.CreateTranslation(130, 0, 0) * //distance from moon center to the earth center
                Matrix4.CreateRotationY(moonorbit); //rotation around the earth
        }


        float Rotate(float variable, float rotationangle, float frameDuration)
        {
            variable += 0.0001f * frameDuration;
            if (variable > 2 * PI) variable -= 2 * PI;

            return variable;
        }

        void HandleInput()
        {
            if (Keyboard[Key.Up])
            {
                TCamera = Matrix4.CreateTranslation(0, 0, -0.5f) * TCamera;
            }
            else if (Keyboard[Key.Down])
            {
                TCamera = Matrix4.CreateTranslation(0, 0, 0.5f) * TCamera;
            }
            else if (Keyboard[Key.Left])
            {
                TCamera = Matrix4.CreateTranslation(-0.5f, 0, 0) * TCamera;
            }
            else if (Keyboard[Key.Right])
            {
                TCamera = Matrix4.CreateTranslation(0.5f, 0, -0.5f) * TCamera;
            }
            else if (Keyboard[Key.Space])
            {
                TCamera = Matrix4.CreateTranslation(0, 0.5f, 0) * TCamera;
            }
            else if (Keyboard[Key.LShift] || Keyboard[Key.RShift])
            {
                TCamera = Matrix4.CreateTranslation(0, -0.5f, 0) * TCamera;
            }
            else if (Keyboard[Key.W])
            {
                TCamera = Matrix4.CreateRotationX(0.01f) * TCamera;
            }
            else if (Keyboard[Key.S])
            {
                TCamera = Matrix4.CreateRotationX(-0.01f) * TCamera;
            }
            else if (Keyboard[Key.A])
            {
                TCamera = Matrix4.CreateRotationY(0.01f) * TCamera;
            }
            else if (Keyboard[Key.D])
            {
                TCamera = Matrix4.CreateRotationY(-0.01f) * TCamera;
            }

            //CameraActionMouse();

        }


        //provides cameraMovement
        public void CameraActionMouse()
        {

            MouseState mousestate = OpenTK.Input.Mouse.GetState();

            int currMouseX = mousestate.X;
            int currMouseY = mousestate.Y;

            int mouseDiffX = currMouseX - prevMouseX;
            int mouseDiffY = currMouseY - prevMouseY;

            Console.WriteLine(mouseDiffX + " " + mouseDiffY);

            prevMouseX = currMouseX;
            prevMouseY= currMouseY;

            TCamera = Matrix4.CreateRotationY(-mouseDiffX * 0.007f) * TCamera;
            TCamera = Matrix4.CreateRotationX(-mouseDiffY * 0.007f) * TCamera;
        }

        // pass the lights to the shader
        public void passLights()
        {
            Light light1 = new Light(
                new Vector3(7.0f, 5.0f, 2.0f), // position
                new Vector3(10, 10, 8), // color
                new Vector3(5, 5, 5)); // specular color
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
            Matrix3 lightMat2 = new Matrix3(light2.lightPos, light2.lightColor, light2.specLightColor);
            lightMat2 = Matrix3.Transpose(lightMat2);
            int lightMat2ID = GL.GetUniformLocation(shader.programID, "light2");
            GL.UseProgram(shader.programID);
            GL.UniformMatrix3(lightMat2ID, true, ref lightMat2);

            Light light3 = new Light(
                new Vector3(7.0f, 5.0f, -2.0f), // position
                100*new Vector3(7, 5, 8), // color
                new Vector3(0, 5, 0)); // specular color
            Matrix3 lightMat3 = new Matrix3(light3.lightPos, light3.lightColor, light3.specLightColor);
            lightMat3 = Matrix3.Transpose(lightMat3);
            int lightMat3ID = GL.GetUniformLocation(shader.programID, "light3");
            GL.UseProgram(shader.programID);
            GL.UniformMatrix3(lightMat3ID, true, ref lightMat3);

            Light light4 = new Light(
                new Vector3(180, 5.0f, 0.2f), // position
                new Vector3(8000, 9000, 10000), // color
                new Vector3(90, 90, 90)); // specular color
            Matrix3 lightMat4 = new Matrix3(light4.lightPos, light4.lightColor, light4.specLightColor);
            lightMat4 = Matrix3.Transpose(lightMat4);
            int lightMat4ID = GL.GetUniformLocation(shader.programID, "light4");
            GL.UseProgram(shader.programID);
            GL.UniformMatrix3(lightMat4ID, true, ref lightMat4);

            Light light5 = new Light(
                new Vector3(-180, 5.0f, 0.2f), // position
                new Vector3(8000, 9000, 10000), // color
                new Vector3(90, 90, 90)); // specular color
            Matrix3 lightMat5 = new Matrix3(light5.lightPos, light5.lightColor, light5.specLightColor);
            lightMat5 = Matrix3.Transpose(lightMat5);
            int lightMat5ID = GL.GetUniformLocation(shader.programID, "light5");
            GL.UseProgram(shader.programID);
            GL.UniformMatrix3(lightMat5ID, true, ref lightMat5);

            Light light6 = new Light(
                new Vector3(0, 180f, 0.2f), // position
                new Vector3(8000, 9000, 10000), // color
                new Vector3(90, 90, 90)); // specular color
            Matrix3 lightMat6 = new Matrix3(light6.lightPos, light6.lightColor, light6.specLightColor);
            lightMat6 = Matrix3.Transpose(lightMat6);
            int lightMat6ID = GL.GetUniformLocation(shader.programID, "light6");
            GL.UseProgram(shader.programID);
            GL.UniformMatrix3(lightMat6ID, true, ref lightMat6);

            Light light7 = new Light(
                new Vector3(0, -180f, 0.2f), // position
                new Vector3(8000, 9000, 10000), // color
                new Vector3(90, 90, 90)); // specular color
            Matrix3 lightMat7 = new Matrix3(light7.lightPos, light7.lightColor, light7.specLightColor);
            lightMat7 = Matrix3.Transpose(lightMat7);
            int lightMat7ID = GL.GetUniformLocation(shader.programID, "light7");
            GL.UseProgram(shader.programID);
            GL.UniformMatrix3(lightMat7ID, true, ref lightMat7);

            Light light8 = new Light(
                new Vector3(0, -180f, 0.2f), // position
                new Vector3(8000, 9000, 10000), // color
                new Vector3(90, 90, 90)); // specular color
            Matrix3 lightMat8 = new Matrix3(light8.lightPos, light8.lightColor, light8.specLightColor);
            lightMat8 = Matrix3.Transpose(lightMat8);
            int lightMat8ID = GL.GetUniformLocation(shader.programID, "light8");
            GL.UseProgram(shader.programID);
            GL.UniformMatrix3(lightMat8ID, true, ref lightMat8);
        }




    // tick for OpenGL rendering code
    public void RenderGL()
        {
            sceneGraph.RenderSceneGraph();
        }

        public Matrix4 TCamera
        {
            get {return Tcam; }
            set { Tcam = value; }
        }

        public Matrix4 TWorld
        {
            get { return Tworld; }
        }

        public Matrix4 TCamPerspective
        {
            get { return TcamPerspective; }
        }

        public KeyboardState Keyboard
        {
            get { return keyboardstate; }
            set { keyboardstate = value; }
        }

        public MouseState Mouse
        {
            get { return mousestate; }
            set { mousestate = value; }
        }

    }

} // namespace Template_P3