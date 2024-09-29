using Classes;
using GerarPerfil;
using System;
using System.Linq;
using ZwSoft.ZwCAD.DatabaseServices;
using ZwSoft.ZwCAD.Geometry;

namespace Utils
{
    internal class InfoGen : InfoGenInterface
    {
        public Polyline GeraPontos(Polyline invert, double distance, int index)
        {

        }

        public DBText GeraNivelTerreno(Profile profile, double posicaoYTexto, int index)
        {
            var position = profile.Invert.GetPoint3dAt(index);

            using (Line line = new Line())
            {
                Point3dCollection pts = new Point3dCollection();

                line.StartPoint = position;
                line.EndPoint = new Point3d(position.X, position.Y + 1, 0);

                line.IntersectWith(profile.Terrain, Intersect.ExtendThis, pts, IntPtr.Zero, IntPtr.Zero);

                // TODO: Fazer verificação, pois o .First() pode não existir!!!
                double yTerreno = pts.ToArray().First().Y;

                // Escreve nivel de terreno
                return Utils.GetType.GenDBText (
                    profile, 
                    new Point3d(line.StartPoint.X, posicaoYTexto, 0), 
                    Math.Round((yTerreno - profile.BaseLine.StartPoint.Y) / profile.Scale + profile.ValorInicial, 3).ToString());
            }
        }

        public DBText GerarProf(Profile profile, Position coords, string textString, double posicaoYTexto)
        {


            DBText text = new DBText()
            {
                TextString = textString,
                Height = profile.TextSet.TamanhoTexto,
                Rotation = profile.TextSet.TextRotation,
                Position = new Point3d(coords.EndPoint.X, posicaoYTexto, 0),
            };

            return text;
        }

        public DBText GeraDeclividade(Profile profile, Position coords, double posicaoYTexto)
        {
            DBText text = new DBText
            {
                TextString =
                Math.Round(
                    Math.Abs(coords.EndPoint.Y - coords.StartPoint.Y) / profile.Scale / (coords.EndPoint.X - coords.StartPoint.X), 4).ToString(),
                Position = new Point3d((coords.EndPoint.X + coords.StartPoint.X) / 2, posicaoYTexto, 0),
                Height = profile.TextSet.TamanhoTexto,
                Rotation = profile.TextSet.TextRotation
            };


            return text;
        }

        public DBText GeraExtensao(Profile profile, Position coords, double posicaoYTexto)
        {
            DBText text = new DBText
            {
                TextString = Math.Round((coords.EndPoint.X - coords.StartPoint.X), 2).ToString(),
                Position = new Point3d(((coords.EndPoint.X + coords.StartPoint.X) / 2), posicaoYTexto, 0),
                Height = profile.TextSet.TamanhoTexto,
                Rotation = profile.TextSet.TextRotation,
            };

            return text;
        }

        public DBText GeraDistanciaProgressiva(Profile profile, Position coords, double posicaoYTexto)
        {
            DBText text = new DBText()
            {
                TextString = Math.Round(coords.EndPoint.X - coords.StartPoint.X, 2).ToString(),
                Position = new Point3d(coords.EndPoint.X, posicaoYTexto, 0),
                Height = profile.TextSet.TamanhoTexto,
                Rotation = profile.TextSet.TextRotation,
            };

            return text;
        }

        // Pega a distancia do primeiro ponto até o atual, divide pelo
        // separadorEstacas. A parte inteira é o número da estaca atual. Pega o
        // resto da divisão e adicionar ao lado do numero: ex: 1 + 23
        public DBText GeraNumeracaoEstacas(Profile profile, Position coords, int index, double posicaoYTexto)
        {
            double distanciaCur = coords.EndPoint.X - coords.StartPoint.X;

            String textString;

            if (profile.SeparadorEstacas > 0)
                textString = ((int)(distanciaCur / profile.SeparadorEstacas)).ToString() +
                                  ((int)(distanciaCur % profile.SeparadorEstacas) != 0 ? " + " + ((int)(distanciaCur % profile.SeparadorEstacas)).ToString() : "");
            else
                textString = index.ToString();

            DBText text = new DBText()
            {
   
                TextString = textString,
                Height = profile.TextSet.TamanhoTexto,
                Rotation = profile.TextSet.TextRotation,
                Position = new Point3d(coords.EndPoint.X, posicaoYTexto, 0),
            };


            return text;
        }

        public DBText GeraGINivel(Profile profile, double posicaoYTexto, int index)
        {
            double curNivel = (profile.Invert.GetPoint3dAt(index).Y - profile.BaseLine.StartPoint.Y) / profile.Scale + profile.ValorInicial;

            DBText text = new DBText()
            {
                TextString = Math.Round(curNivel, 3).ToString(),
                Position = new Point3d(profile.Invert.GetPoint3dAt(index).X, posicaoYTexto, 0),
                Height = profile.TextSet.TamanhoTexto,
                Rotation = profile.TextSet.TextRotation,
            };

            return text;
        }
    }
}
