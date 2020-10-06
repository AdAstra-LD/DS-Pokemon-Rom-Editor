namespace MKDS_Course_Editor.Export3DTools
{
    using OpenTK;
    using System;

    public class Polygon
    {
        public Vector3[] Normals;
        public PolygonType PolyType;
        public Vector2[] TexCoords;
        public Vector3[] Vertex;

        public Polygon(PolygonType PolyType, Vector3[] Normals, Vector2[] TexCoords, Vector3[] Vertex)
        {
            this.PolyType = PolyType;
            this.Normals = Normals;
            this.TexCoords = TexCoords;
            this.Vertex = Vertex;
        }
    }
}

