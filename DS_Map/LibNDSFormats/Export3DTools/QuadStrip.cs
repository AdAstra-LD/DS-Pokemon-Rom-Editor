namespace MKDS_Course_Editor.Export3DTools
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media.Media3D;

    public class QuadStrip : Face
    {
        public List<Vector3D> Normal = new List<Vector3D>();
        public List<Point> TexCoord = new List<Point>();
        public List<Point3D> Vertex = new List<Point3D>();
    }
}

