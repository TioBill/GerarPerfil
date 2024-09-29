using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZwSoft.ZwCAD.DatabaseServices;
using ZwSoft.ZwCAD.Geometry;

using Utils;

namespace Classes
{
    internal struct Profile
    {
        public Polyline Invert { set; get; }
        public Polyline Terrain { set; get; }
        public Line BaseLine { set; get; }
        public double Scale { set; get; }
        public Text TextSet { get; set; }
        public double SeparadorEstacas { get; set; }
        public double ValorInicial { get; set; }

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

        public Profile(Polyline invert, Polyline terrain, Line baseLine, double scale, Text textSet, double separadorEstacas, double valorInicial)
        {
            Invert = invert;
            Terrain = terrain;
            BaseLine = baseLine;
            Scale = scale;
            TextSet = textSet;
            SeparadorEstacas = separadorEstacas;
            ValorInicial = valorInicial;
        }
    }
}
