namespace MKDS_Course_Editor.Export3DTools
{
    using OpenTK;
    using System;
    using System.Collections.Generic;

    public class OBJWriter
    {
        private List<Vector3> Normals = new List<Vector3>();
        private List<Vector2> TexCoords = new List<Vector2>();
        private List<Vector3> Vertices = new List<Vector3>();

        public void AddTriangle(Vector3[] Vertice)
        {
            this.Vertices.AddRange(Vertice);
        }

        public void AddTriangle(Vector3[] Vertice, Vector2 TexCoord)
        {
            this.Vertices.AddRange(Vertice);
            this.TexCoords.Add(TexCoord);
        }

        public void AddTriangle(Vector3[] Vertice, Vector3 Normal)
        {
            this.Vertices.AddRange(Vertice);
            this.Normals.Add(Normal);
        }

        public void AddTriangle(Vector3[] Vertice, Vector2 TexCoord, Vector3 Normal)
        {
            this.Vertices.AddRange(Vertice);
            this.TexCoords.Add(TexCoord);
            this.Normals.Add(Normal);
        }
    }
}

