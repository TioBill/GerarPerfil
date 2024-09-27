using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZwSoft.ZwCAD.DatabaseServices;
using ZwSoft.ZwCAD.Geometry;

namespace Classes
{
    internal struct Profile
    {
        public Polyline Invert { set; get; }
        public Polyline Terrain { set; get; }
        public Line BaseLine { set; get; }


        public struct Text
        {
            public double TamanhoTexto { set; get; }
            public double TextRotation { get; set; }

            public Text(double tamanhoTexto, double textRotation)
            {
                TamanhoTexto = tamanhoTexto;
                TextRotation = textRotation;
            }
        }

        public struct Position
        {
            Point3d StartPoint { get; }
            Point3d EndPoint { get; }

            public Position(Point3d startPoint, Point3d endPoint)
            {
                StartPoint = startPoint;
                EndPoint = endPoint;
            }
        }

        public Profile(Polyline gi, Polyline terreno, Line baseLine, double tamanhoTexto)
        {
            Invert = gi;
            Terrain = terreno;
            BaseLine = baseLine;
            TamanhoTexto = tamanhoTexto;
        }
    }
}
