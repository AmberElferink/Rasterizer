using System.Linq;

using OpenTK;

namespace Template_P3
{
    public class SceneGraph
    {
        Node root;


                          
        RenderTarget target;                    // intermediate render target
        ScreenQuad quad;                        // screen filling quad for post processing
        bool useRenderTarget = false;           //ZET OP TRUE VOOR POST-PROCESSING

        Game game;
     

        public SceneGraph(Game game)
        {
            this.game = game;
            // create the render target
            target = new RenderTarget(game.screen.width, game.screen.height);
            quad = new ScreenQuad();
            
        }

        public void RenderSceneGraph()
        {
            Matrix4 transform = game.TWorld * game.TCamera.Inverted() * game.TCamPerspective;

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
            Matrix4 TransformedMatrix = transformParents * node.Matrix;

            node.NodeMesh.Render(game.shader, TransformedMatrix, game.TWorld, game.wood);

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
