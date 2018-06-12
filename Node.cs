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
        Matrix4 objectMatrix;
        SceneGraph sceneGraph;
        string id;

        public Node(string id, Node parent, Mesh nodeObject, Matrix4 positionFromParent, SceneGraph sceneGraph)
        {
            this.sceneGraph = sceneGraph;
            this.id = id;
            this.children = new List<Node>();
            this.parent = parent;
            this.nodeMesh = nodeObject;
            objectMatrix = positionFromParent;

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
        }

        public Mesh NodeMesh
        {
            get { return nodeMesh; }
        }

        public string ID
        {
            get { return id; }
        }

    }
}
