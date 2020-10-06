namespace MKDS_Course_Editor.Export3DTools
{
    using System.Windows;
    using System.Windows.Media.Media3D;

    public class Quad : Face
    {
        public Vector3D[] Normal = new Vector3D[4];
        public Point[] TexCoord = new Point[4];
        public Point3D[] Vertex = new Point3D[4];
    }
}

