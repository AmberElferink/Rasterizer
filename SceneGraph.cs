using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Template_P3
{
    public class SceneGraph
    {
        Node root;
        Stopwatch timer;                        // timer for measuring frame duration
        float a = 0;                            // teapot rotation angle
        Shader shader;                          // shader to use for rendering
        Shader postproc;                        // shader to use for post processing
        Texture wood;                           // texture to use for rendering
        RenderTarget target;                    // intermediate render target
        ScreenQuad quad;                        // screen filling quad for post processing
        bool useRenderTarget = true;
        const float PI = 3.1415926535f;			// PI
        Game game;

        public SceneGraph(Game game)
        {
            this.game = game;
            // initialize stopwatch
            timer = new Stopwatch();
            timer.Reset();
            timer.Start();

            // create shaders
            shader = new Shader("../../shaders/vs.glsl", "../../shaders/fs.glsl");
            postproc = new Shader("../../shaders/vs_post.glsl", "../../shaders/fs_post.glsl");
            // load a texture
            wood = new Texture("../../assets/wood.jpg");
            // create the render target
            target = new RenderTarget(game.screen.width, game.screen.height);
            quad = new ScreenQuad();
            
        }

        public void RenderSceneGraph()
        {
            // prepare matrix for vertex shader
            /*Matrix4 transform = Matrix4.CreateFromAxisAngle( new Vector3( 0, 1, 0 ), a );
            transform *= Matrix4.CreateTranslation( 0, -4, -15 );
            transform *= Matrix4.CreatePerspectiveFieldOfView( 1.2f, 1.3f, .1f, 1000 );*/

            // measure frame duration
            float frameDuration = timer.ElapsedMilliseconds;
            timer.Reset();
            timer.Start();



            // update rotation
            a += 0.001f * frameDuration;
            if (a > 2 * PI) a -= 2 * PI;

            if (useRenderTarget)
            {
                // enable render target
                target.Bind();

                root.NodeMesh.Render(shader, root.Matrix, wood);

                foreach(Node node in root.Children)
                {
                    node.NodeMesh.Render(shader, node.Matrix, wood);
                }

                // render quad
                target.Unbind();
                quad.Render(postproc, target.GetTextureID());
            }
            else
            {
                // render scene directly to the screen
                root.NodeMesh.Render(shader, root.Matrix, wood);

                foreach (Node node in root.Children)
                {
                    node.NodeMesh.Render(shader, node.Matrix, wood);
                }
            }
        }


        public Node Root
        {
            get { return root; }
            set { root = value; }
        }
    }


}
