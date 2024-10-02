using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using ZwSoft.ZwCAD.DatabaseServices;
using ZwSoft.ZwCAD.Geometry;
using static Utils.Extensions;
using static Utils.GetType;

namespace Classes
{
    public partial class Profile
    {
        public Polyline GeraPontos()
        {
            Point3dCollection pts = new Point3dCollection();
            Point2dCollection plDraw = new Point2dCollection();

            // Gather line by intersection
            using (Line line = new Line())
            {
                int i = 0;

                for (double currentDistance = Invert.StartPoint.X; currentDistance <= Invert.EndPoint.X; currentDistance += SeparadorEstacas)
                {
                    while (i < Invert.NumberOfVertices && Invert.GetPoint2dAt(i).X < currentDistance)
                        plDraw.Add(Invert.GetPoint2dAt(i++));

                    line.StartPoint = new Point3d(currentDistance, Invert.StartPoint.Y, 0);
                    line.EndPoint = new Point3d(currentDistance, Invert.StartPoint.Y + 1, 0);

                    line.IntersectWith(Invert, Intersect.ExtendThis, pts, IntPtr.Zero, IntPtr.Zero);

                    Point2d last = new Point2d(pts.ToArray().Last().X, pts.ToArray().Last().Y);

                    if (Invert.GetPoint2dAt(i) == last)
                        i++;

                    plDraw.Add(last);
                }

                while (i < Invert.NumberOfVertices)
                    plDraw.Add((Invert.GetPoint2dAt(i++)));

                return GetPolyline(plDraw);
            }
        }
        public string GeraNumeracaoEstacas(int invertIndex)
        {
            string textString;

            double distanciaCur = GetGradualDistance(invertIndex);

            if (SeparadorEstacas > 0)
                textString = ((int)(distanciaCur / SeparadorEstacas)).ToString() +
                                  ((int)(distanciaCur % SeparadorEstacas) != 0 ? " + " + ((int)(distanciaCur % SeparadorEstacas)).ToString() : "");
            else
                textString = invertIndex.ToString();

            return textString;
        }
        public double GetTerrainLevel(int index)
        {
            var position = Invert.GetPoint3dAt(index);

            using (Line line = new Line())
            {
                Point3dCollection pts = new Point3dCollection();

                line.StartPoint = position;
                line.EndPoint = new Point3d(position.X, position.Y + 1, 0);

                line.IntersectWith(Terrain, Intersect.ExtendThis, pts, IntPtr.Zero, IntPtr.Zero);

                if (pts.Count == 0)        
                    return double.NaN; 
                
                double yTerreno = pts.ToArray().First().Y;

                // Escreve nivel de terreno
                return Math.Round((yTerreno - BaseLine.StartPoint.Y) / Scale + ValorInicial, 3);
            }
        }
        public double GetInvertLevel(int index)
        {
            double curNivel = (Invert.GetPoint3dAt(index).Y - BaseLine.StartPoint.Y) / Scale + ValorInicial;

            return Math.Round(curNivel, 3);
        }
        public double GetDepth(int index)
        {
            return Math.Round(Data[index].TerrainLevel - Data[index].InvertLevel, 2);
        }
        public double GetGradualDistance(int index)
        {
            return Math.Round((Invert.GetPoint3dAt(index).X - Invert.GetPoint3dAt(0).X), 2);
        }
        public double GetSlope(Data firstData, Data lastData)
        {
            return Math.Round(
                Math.Abs(
                    (firstData.InvertLevel - lastData.InvertLevel) / (firstData.GradualDistance - lastData.GradualDistance)), 4);
        }
        public double GetLength(Data firstData, Data lastData)
        {
            return Math.Round(firstData.GradualDistance - lastData.GradualDistance, 2);
        }
        public double GetLength(double lastPosition, double firstPosition)
        {
            return Math.Round(lastPosition - firstPosition, 2);
        }
        public List<Line> GetRefLines()
        {
            List<Line> lines = new List<Line>(Invert.NumberOfVertices);

            for (int i = 0; i < Invert.NumberOfVertices; i++)
            {
                var position = Invert.GetPoint3dAt(i);

                lines.Add(GetLine(position, new Point3d(position.X, BaseLine.StartPoint.Y, position.Z)));
            }

            return lines;
        }
    }
}
