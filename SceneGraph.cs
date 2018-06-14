using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using OpenTK;

namespace Template_P3
{
    public class SceneGraph
    {
        Node root;
        Stopwatch timer;                        // timer for measuring frame duration
        float a = 0;                            // world rotation angle
                          
        RenderTarget target;                    // intermediate render target
        ScreenQuad quad;                        // screen filling quad for post processing
        bool useRenderTarget = false;           //ZET OP TRUE VOOR POST-PROCESSING
        const float PI = 3.1415926535f;			// PI
        Game game;
        Matrix4 Tworld, Tcam, TcamPerspective;

        public SceneGraph(Game game)
        {
            this.game = game;
            // initialize stopwatch
            timer = new Stopwatch();
            timer.Reset();
            timer.Start();
           
            Tcam = Matrix4.CreateTranslation(0, 4, 15);
            TcamPerspective = Matrix4.CreatePerspectiveFieldOfView(1.2f, 1.3f, .1f, 1000);

            
            // create the render target
            target = new RenderTarget(game.screen.width, game.screen.height);
            quad = new ScreenQuad();
            
        }

        public void RenderSceneGraph()
        {

            // measure frame duration
            float frameDuration = timer.ElapsedMilliseconds;
            timer.Reset();
            timer.Start();

            // update rotation
            a += 0.001f * frameDuration;
            if (a > 2 * PI) a -= 2 * PI;
            Tworld = Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), a);
            Matrix4 transform = Tworld * Tcam.Inverted() * TcamPerspective;

            if (useRenderTarget)
            {
                // enable render target
                target.Bind();
                TransformNodesToCamera(root, transform * root.Matrix);

                // render quad
                target.Unbind();
                quad.Render(game.postproc, target.GetTextureID());
            }
            else
            {
                // render scene directly to the screen

                TransformNodesToCamera(root, transform * root.Matrix);
            }
        }

        /// <summary>
        /// Will loop through the scenegraph, from the root to the leaves. It will Render each node with orientation with respect to the parents above it.
        /// </summary>
        /// <param name="transformParents">the multiplied transformation matrices from all parent above the current node</param>
        void TransformNodesToCamera(Node node, Matrix4 transformParents)
        {
            Matrix4 TransformedMatrix = node.Matrix;
            TransformedMatrix = transformParents * node.Matrix;

            node.NodeMesh.Render(game.shader, TransformedMatrix, Tworld, game.wood);

            if (node.Children.Any()) //if there exists something within the children list:
            {
                foreach (Node childnode in node.Children)
                {
                    TransformNodesToCamera(childnode, TransformedMatrix);
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
