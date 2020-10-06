namespace MKDS_Course_Editor.Export3DTools
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class Group
    {
        private List<Polygon> PolygonList = new List<Polygon>();

        public void Add(Polygon g)
        {
            this.PolygonList.Add(g);
        }

        public IEnumerator<Polygon> GetEnumerator()
        {
            return this.PolygonList.GetEnumerator();
        }

        public Polygon this[int i]
        {
            get
            {
                return this.PolygonList[i];
            }
            set
            {
                this.PolygonList[i] = value;
            }
        }

        public Polygon[] Polygons
        {
            get
            {
                return this.PolygonList.ToArray();
            }
        }
    }
}

