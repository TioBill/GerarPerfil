using Debug;
using GerarPerfil.components.classes;
using GerarPerfil.components.debug;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ZwSoft.ZwCAD.DatabaseServices;
using ZwSoft.ZwCAD.Geometry;

namespace Utils
{
    internal class InfoGen : InfoGenInterface
    {


        // GerarPerfil Functions
        [Obsolete("", true)]
        private static Polyline GeraPontos(Polyline gi, double distancia, BlockTableRecord blockTableRec, Transaction trans)
        {
            Point3dCollection pts = new Point3dCollection();
            Point2dCollection plDraw = new Point2dCollection();

            int i;

            // Gather line by intersection
            using (Line line = new Line())
            {
                i = 0;

                for (double currentDistance = gi.StartPoint.X; currentDistance <= gi.EndPoint.X; currentDistance += distancia)
                {
                    while (i < gi.NumberOfVertices && gi.GetPoint2dAt(i).X < currentDistance)
                        plDraw.Add(gi.GetPoint2dAt(i++));

                    line.StartPoint = new Point3d(currentDistance, gi.StartPoint.Y, 0);
                    line.EndPoint = new Point3d(currentDistance, gi.StartPoint.Y + 1, 0);

                    line.IntersectWith(gi, Intersect.ExtendThis, pts, IntPtr.Zero, IntPtr.Zero);

                    Point2d last = new Point2d(pts.ToArray().Last().X, pts.ToArray().Last().Y);

                    if (gi.GetPoint2dAt(i) == last)
                        i++;

                    plDraw.Add(last);
                }
            }

            while (i < gi.NumberOfVertices)
                plDraw.Add((gi.GetPoint2dAt(i++)));

            // Draw Line 
            i = 0;

            Polyline pl = new Polyline();

            foreach (Point2d vertex in plDraw.ToArray())
                pl.AddVertexAt(i++, vertex, 0, 0, 0);

            blockTableRec.AppendEntity(pl);
            trans.AddNewlyCreatedDBObject(pl, true);

            return pl;
        }

        
        [Obsolete("", true)]
        private static Result<bool> GeraNivelTerrenoEProf(Point3d gi, Polyline terreno, Line baseLine, double valorInicial,
                                            double posicaoYTexto, double tamanhoTexto, BlockTableRecord blockTableRec,
                                            Transaction trans, int scale = 10)
        {

            using (Line line = new Line())
            {
                Point3dCollection pts = new Point3dCollection();

                line.StartPoint = gi;
                line.EndPoint = new Point3d(gi.X, gi.Y + 1, 0);

                line.IntersectWith(terreno, Intersect.ExtendThis, pts, IntPtr.Zero, IntPtr.Zero);

                double? yTerreno = pts.ToArray().FirstOrDefault(null).Y;

                if (yTerreno == null)
                    return Result<bool>.Err(ResultType.NoIntersection);

                // Escreve nivel de terreno
                using (DBText text = new DBText())
                {
                    text.TextString = Math.Round((double)((yTerreno - baseLine.StartPoint.Y) / scale + valorInicial), 3).ToString();

                    text.Height = tamanhoTexto;
                    text.Rotation = TextRotation;
                    text.Position = new Point3d(line.StartPoint.X, posicaoYTexto, 0);

                    blockTableRec.AppendEntity(text);
                    trans.AddNewlyCreatedDBObject(text, true);
                }

                // Escreve profundidade
                using (DBText text = new DBText())
                {
                    text.TextString = Math.Round(((double)yTerreno - line.StartPoint.Y) / scale, 2).ToString();

                    text.Height = tamanhoTexto;
                    text.Rotation = TextRotation;
                    text.Position = new Point3d(line.StartPoint.X, baseLine.StartPoint.Y - tamanhoTexto * 140, 0);

                    blockTableRec.AppendEntity(text);
                    trans.AddNewlyCreatedDBObject(text, true);
                }

                return Result<bool>.Ok(true);
            }
        }
        [Obsolete("", true)]
        private static void GeraDeclividade(Point3d startPoint, Point3d endPoint, double posicaoYTexto, double tamanhoTexto,
                                      BlockTableRecord blockTableRec, Transaction trans, int scale = 10)
        {
            using (DBText text = new DBText())
            {
                text.TextString =
                    Math.Round(Math.Abs(endPoint.Y - startPoint.Y) / scale / (endPoint.X - startPoint.X), 4).ToString();
                text.Position = new Point3d((endPoint.X + startPoint.X) / 2, posicaoYTexto, 0);
                text.Height = tamanhoTexto;
                text.Rotation = TextRotation;

                blockTableRec.AppendEntity(text);
                trans.AddNewlyCreatedDBObject(text, true);
            }
        }
        [Obsolete("", true)]
        private static void GeraExtensao(Point3d startPoint, Point3d endPoint, double posicaoYTexto, double tamanhoTexto,
                                   BlockTableRecord blockTableRec, Transaction trans)
        {
            using (DBText text = new DBText())
            {
                text.TextString = Math.Round((endPoint.X - startPoint.X), 2).ToString();
                text.Position = new Point3d(((endPoint.X + startPoint.X) / 2), posicaoYTexto, 0);
                text.Height = tamanhoTexto;
                text.Rotation = TextRotation;

                blockTableRec.AppendEntity(text);
                trans.AddNewlyCreatedDBObject(text, true);
            }
        }
        [Obsolete("", true)]
        private static void GeraDistanciaProgressiva(Point3d startPoint, Point3d endPoint, double posicaoYTexto,
                                               double tamanhoTexto, BlockTableRecord blockTableRec, Transaction trans)
        {
            using (DBText text = new DBText())
            {
                text.TextString = Math.Round(endPoint.X - startPoint.X, 2).ToString();
                text.Position = new Point3d(endPoint.X, posicaoYTexto, 0);
                text.Height = tamanhoTexto;
                text.Rotation = TextRotation;

                blockTableRec.AppendEntity(text);
                trans.AddNewlyCreatedDBObject(text, true);
            }
        }
        [Obsolete("", true)]
        private static void GeraNumeracaoEstacas(Point3d startPoint, Point3d endPoint, int posicao, double tamanhoTexto, double posicaoYTexto,
                                           double separadorEstacas, BlockTableRecord blockTableRec, Transaction trans)
        {
            using (DBText text = new DBText())
            {
                // Pega a distancia do primeiro ponto até o atual, divide pelo
                // separadorEstacas. A parte inteira é o número da estaca atual. Pega o
                // resto da divisão e adicionar ao lado do numero: ex: 1 + 23
                double distanciaCur = endPoint.X - startPoint.X;

                if (separadorEstacas > 0)
                {
                    text.TextString = ((int)(distanciaCur / separadorEstacas)).ToString() +
                                      ((int)(distanciaCur % separadorEstacas) != 0 ? " + " + ((int)(distanciaCur % separadorEstacas)).ToString() : "");
                }
                else
                {
                    text.TextString = posicao.ToString();
                }

                text.Height = tamanhoTexto;
                text.Position = new Point3d(endPoint.X, posicaoYTexto, 0);
                text.Rotation = TextRotation;

                // Add to Block Table Record
                blockTableRec.AppendEntity(text);
                trans.AddNewlyCreatedDBObject(text, true);
            }
        }
        [Obsolete("", true)]
        private static void DesenhaLinhaEstacas(Point3d gi, double baseLine, BlockTableRecord blockTableRec, Transaction trans)
        {
            using (Line currentLine = new Line(gi, new Point3d(gi.X, baseLine, 0)))
            {
                blockTableRec.AppendEntity(currentLine);
                trans.AddNewlyCreatedDBObject(currentLine, true);
            }
        }
        [Obsolete("", true)]
        private static void GeraGINivel(Point3d gi, double baseline, double valorInicial, double tamanhoTexto, double posicaoTexto,
                                  BlockTableRecord blockTableRec, Transaction trans, int scale = 10)
        {
            double curNivel = (gi.Y - baseline) / scale + valorInicial;

            using (DBText text = new DBText())
            {
                text.TextString = Math.Round(curNivel, 3).ToString();
                text.Position = new Point3d(gi.X, posicaoTexto, 0);
                text.Height = tamanhoTexto;
                text.Rotation = TextRotation;

                blockTableRec.AppendEntity(text);
                trans.AddNewlyCreatedDBObject(text, true);
            }
        }

    }
}
