using System;
using System.Collections.Generic;
using OpenTK;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template_P3
{
    public class Node
        //camera * root * children * children....
    {
        Node parent;
        List<Node> children;
        Mesh nodeMesh;
        Texture texture;
        Texture normal;

        Matrix4 objectMatrix;
        SceneGraph sceneGraph;
       
        string id;

        public Node(string id, Node parent, Mesh nodeObject, Matrix4 positionFromParent, Texture texture, Texture normal, SceneGraph sceneGraph)
        {
            this.sceneGraph = sceneGraph;
            this.id = id;

            this.children = new List<Node>();
            this.parent = parent;
            this.nodeMesh = nodeObject;
            objectMatrix = positionFromParent;
            this.texture = texture;
            this.normal = normal;

            if (parent == null)
            {
                if(sceneGraph.Root == null)
                    sceneGraph.Root = this;
                else
                    throw new Exception("Only one node can be set as root");
            } 
            else
                this.parent.children.Add(this);
            
        }

        public List<Node> Children
        {
            get { return children; }
        }

        public Matrix4 Matrix
        {
            get { return objectMatrix; }
            set { objectMatrix = value; }
        }

        public Mesh NodeMesh
        {
            get { return nodeMesh; }
        }

        public Node Parent
        {
            get { return parent; }
        }

        public string ID
        {
            get { return id; }
        }

        public Texture Texture
        {
            get { return texture; }
        }

        public Texture Normal
        {
            get { return normal; }
        }
    }
}
