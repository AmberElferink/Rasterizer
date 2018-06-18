using OpenTK;
using OpenTK.Graphics.OpenGL;
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
        public Texture wood;                    // texture to use for rendering
        

        Mesh teapot, floor;                     // a mesh to draw using OpenGL
        Node teapotNode, floorNode;             //the corresponding Nodes
        SceneGraph sceneGraph;
        Matrix4 Tworld, Tcam, TcamPerspective;
        const float PI = 3.1415926535f;			// PI
        float a = 0;                            // world rotation angle
        Stopwatch timer;                        // timer for measuring frame duration
        KeyboardState keyboardstate;
        MouseState mousestate;

        int prevMouseY = 0;
        int prevMouseX = 0;

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

            // set the light
            int lightID = GL.GetUniformLocation(shader.programID, "lightPos");
            GL.UseProgram(shader.programID);
            //GL.Uniform3(lightID, 0.0f, 10.0f, 0.0f); //coordinates of light
            GL.Uniform3(lightID, 15.0f, 10.0f, 2.0f);


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
            HandleInput();

            screen.Clear(0);
            //screen.Print("hello world", 2, 2, 0xffff00);

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

            CameraActionMouse();

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