using System.Collections.Generic;
using System.Linq;

using Utils;
using ZwSoft.ZwCAD.DatabaseServices;
using ZwSoft.ZwCAD.Geometry;

namespace Classes
{
    public partial class Profile
    {
        public Polyline Invert { set; get; }
        public Polyline Terrain { set; get; }
        public Line BaseLine { set; get; }
        public double Scale { set; get; }
        public Text TextSet { get; set; }
        public double SeparadorEstacas { get; set; }
        public double ValorInicial { get; set; }
        public List<Data> Data { get; set; }
        public double HorizontalSpace { get; set; }

        public Profile(Polyline invert, Polyline terrain, Line baseLine, double scale, Text textSet, double separadorEstacas, double valorInicial, double horizontalSpace)
        {
            Invert = invert;
            Terrain = terrain;
            BaseLine = baseLine;
            Scale = scale;
            TextSet = textSet;
            SeparadorEstacas = separadorEstacas;
            ValorInicial = valorInicial;
            HorizontalSpace = horizontalSpace;
        }

        public void PopulateData()
        {
            Data = new List<Data>(Invert.NumberOfVertices);

            double currentDistance = 0;

            for (int i = 0; i < Invert.NumberOfVertices; i++)
            {
                var invertPostion = Invert.GetPoint3dAt(i);

                var data = new Data
                {
                    Position = new Point3d(invertPostion.X, BaseLine.StartPoint.Y, invertPostion.Z),
                    Estaca = GeraNumeracaoEstacas(i),
                    TerrainLevel = GetTerrainLevel(i),
                    InvertLevel = GetInvertLevel(i),
                    GradualDistance = GetLength(invertPostion.X, Invert.StartPoint.X)
                };

                data.CalculateDepth();

                if (i > 0)
                {
                    data.Slope = GetSlope(data, Data.Last());
                    data.Length = GetLength(data, Data.Last());
                }
                else
                {
                    data.Slope = double.NaN;
                    data.Length = double.NaN;
                }

                Data.Add(data);

                currentDistance += SeparadorEstacas;
            }
        }
    }
}
