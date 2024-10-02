using System;
using ZwSoft.ZwCAD.Geometry;

namespace Utils
{
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

    public struct Data
    {
        public string Estaca { set; get; }
        public double TerrainLevel { set; get; }
        public double InvertLevel { set; get; }
        public double Depth { get; set; }
        public double GradualDistance { set; get; }
        public double Slope { set; get; }
        public double Length { set; get; }
        public Point3d Position { get; set; }

        public void CalculateDepth()
        {
            Depth = Math.Round(TerrainLevel - InvertLevel, 2);
        }
    }
}
