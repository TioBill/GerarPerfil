﻿using System;
using System.Linq;
using ZwSoft.ZwCAD.ApplicationServices;
using ZwSoft.ZwCAD.DatabaseServices;
using ZwSoft.ZwCAD.EditorInput;
using ZwSoft.ZwCAD.Geometry;
using ZwSoft.ZwCAD.Runtime;

namespace GerarPerfil
{
public static class Extensions
{
  public
    static Point3d[] ToArray(this Point3dCollection pts)
    {
        var arr = new Point3d[pts.Count];
        pts.CopyTo(arr, 0);

        return arr;
    }
}

// Main project
public class Class1
{
    const double TEXT_ROTATION = 90 * Math.PI / 180;

    [CommandMethod("GerarPerfil")] public static void main()
    {
        Document acDoc = Application.DocumentManager.MdiActiveDocument;
        Editor editor = acDoc.Editor;
        Database database = acDoc.Database;

        PromptEntityResult giRes = getPolyline("Select your invert:");

        if (giRes.Status != PromptStatus.OK)
            return;

        PromptEntityResult terrenoRes = getPolyline("Select your terrain:");

        if (terrenoRes.Status != PromptStatus.OK)
            return;

        PromptEntityResult baseRes = getLine("Select your baseLine:");

        if (baseRes.Status != PromptStatus.OK)
            return;

        PromptDoubleResult valorInicial = getDouble("Type your baseLine level:");

        if (valorInicial.Status != PromptStatus.OK)
            return;

        PromptDoubleResult tamanhoTexto = getDouble("Type a font-height:");

        if (valorInicial.Status != PromptStatus.OK)
            return;

        PromptDoubleResult distancia = getDouble("Increase your station number every how much distance?", true);

        if (distancia.Status != PromptStatus.OK)
            return;

        using(Transaction trans = acDoc.TransactionManager.StartTransaction())
        {
            BlockTable blockTable = trans.GetObject(database.BlockTableId, OpenMode.ForRead) as BlockTable;
            BlockTableRecord blockTableRec =
                trans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

            Polyline gi = trans.GetObject(giRes.ObjectId, OpenMode.ForRead) as Polyline;
            Polyline terreno = trans.GetObject(terrenoRes.ObjectId, OpenMode.ForRead) as Polyline;
            Line baseLine = trans.GetObject(baseRes.ObjectId, OpenMode.ForRead) as Line;

            double curPosicaoTexto = 0;

            for (int i = 0; i < gi.NumberOfVertices; i++)
            {
                // Desenhando Linhas das Estacas
                desenhaLinhaEstacas(gi.GetPoint3dAt(i), baseLine.StartPoint.Y, blockTableRec, trans);

                curPosicaoTexto = baseLine.StartPoint.Y - tamanhoTexto.Value * 20;

                // Gera numeração das estacas
                geraNumeracaoEstacas(gi.GetPoint3dAt(0), gi.GetPoint3dAt(i), i, tamanhoTexto.Value, curPosicaoTexto,
                                     distancia.Value, blockTableRec, trans);

                curPosicaoTexto = baseLine.StartPoint.Y - tamanhoTexto.Value * 40;

                // Gera Nivel da Geratrix Inferior (G.I)
                geraGINivel(gi.GetPoint3dAt(i), baseLine.StartPoint.Y, valorInicial.Value, tamanhoTexto.Value,
                            curPosicaoTexto, blockTableRec, trans);

                curPosicaoTexto = baseLine.StartPoint.Y - tamanhoTexto.Value * 60;

                // Gera a Distancia Progressiva
                geraDistanciaProgressiva(gi.GetPoint3dAt(0), gi.GetPoint3dAt(i), curPosicaoTexto, tamanhoTexto.Value,
                                         blockTableRec, trans);

                curPosicaoTexto = baseLine.StartPoint.Y - tamanhoTexto.Value * 80;

                // Gera extensão
                if (i > 0)
                {
                    geraExtensao(gi.GetPoint3dAt(i - 1), gi.GetPoint3dAt(i), curPosicaoTexto, tamanhoTexto.Value,
                                 blockTableRec, trans);

                    curPosicaoTexto = baseLine.StartPoint.Y - tamanhoTexto.Value * 100;

                    // Gera declividade
                    geraDeclividade(gi.GetPoint3dAt(i - 1), gi.GetPoint3dAt(i), curPosicaoTexto, tamanhoTexto.Value,
                                    blockTableRec, trans);
                }

                curPosicaoTexto = baseLine.StartPoint.Y - tamanhoTexto.Value * 120;

                // Gera nivel de terreno e Profundiade
                geraNivelTerrenoEProf(gi.GetPoint3dAt(i), terreno, baseLine, valorInicial.Value, curPosicaoTexto,
                                      tamanhoTexto.Value, blockTableRec, trans);
            }

            trans.Commit();
        }
    }

    // GerarPerfil Functions
  private
    static void geraNivelTerrenoEProf(Point3d gi, Polyline terreno, Line baseLine, double valorInicial,
                                      double posicaoYTexto, double tamanhoTexto, BlockTableRecord blockTableRec,
                                      Transaction trans, int scale = 10)
    {

        using(Line line = new Line())
        {
            Point3dCollection pts = new Point3dCollection();

            line.StartPoint = gi;
            line.EndPoint = new Point3d(gi.X, gi.Y + 1, 0);

            line.IntersectWith(terreno, Intersect.ExtendThis, pts, IntPtr.Zero, IntPtr.Zero);

            double yTerreno = pts.ToArray().First().Y;

            // Escreve nivel de terreno
            using(DBText text = new DBText())
            {
                text.TextString = Math.Round((yTerreno - baseLine.StartPoint.Y) / scale + valorInicial, 3).ToString();

                text.Height = tamanhoTexto;
                text.Rotation = TEXT_ROTATION;
                text.Position = new Point3d(line.StartPoint.X, posicaoYTexto, 0);

                blockTableRec.AppendEntity(text);
                trans.AddNewlyCreatedDBObject(text, true);
            }

            // Escreve profundidade
            using(DBText text = new DBText())
            {
                text.TextString = Math.Round((yTerreno - line.StartPoint.Y) / scale, 2).ToString();

                text.Height = tamanhoTexto;
                text.Rotation = TEXT_ROTATION;
                text.Position = new Point3d(line.StartPoint.X, baseLine.StartPoint.Y - tamanhoTexto * 140, 0);

                blockTableRec.AppendEntity(text);
                trans.AddNewlyCreatedDBObject(text, true);
            }
        }
    }

  private
    static void geraDeclividade(Point3d startPoint, Point3d endPoint, double posicaoYTexto, double tamanhoTexto,
                                BlockTableRecord blockTableRec, Transaction trans, int scale = 10)
    {
        using(DBText text = new DBText())
        {
            text.TextString =
                Math.Round(Math.Abs(endPoint.Y - startPoint.Y) / scale / (endPoint.X - startPoint.X), 4).ToString();
            text.Position = new Point3d((endPoint.X + startPoint.X) / 2, posicaoYTexto, 0);
            text.Height = tamanhoTexto;
            text.Rotation = TEXT_ROTATION;

            blockTableRec.AppendEntity(text);
            trans.AddNewlyCreatedDBObject(text, true);
        }
    }

  private
    static void geraExtensao(Point3d startPoint, Point3d endPoint, double posicaoYTexto, double tamanhoTexto,
                             BlockTableRecord blockTableRec, Transaction trans)
    {
        using(DBText text = new DBText())
        {
            text.TextString = Math.Round((endPoint.X - startPoint.X), 2).ToString();
            text.Position = new Point3d(((endPoint.X + startPoint.X) / 2), posicaoYTexto, 0);
            text.Height = tamanhoTexto;
            text.Rotation = TEXT_ROTATION;

            blockTableRec.AppendEntity(text);
            trans.AddNewlyCreatedDBObject(text, true);
        }
    }

  private
    static void geraDistanciaProgressiva(Point3d startPoint, Point3d endPoint, double posicaoYTexto,
                                         double tamanhoTexto, BlockTableRecord blockTableRec, Transaction trans)
    {
        using(DBText text = new DBText())
        {
            text.TextString = Math.Round(endPoint.X - startPoint.X, 2).ToString();
            text.Position = new Point3d(endPoint.X, posicaoYTexto, 0);
            text.Height = tamanhoTexto;
            text.Rotation = TEXT_ROTATION;

            blockTableRec.AppendEntity(text);
            trans.AddNewlyCreatedDBObject(text, true);
        }
    }

  private
    static void geraNumeracaoEstacas(Point3d startPoint, Point3d endPoint, int posicao, double tamanhoTexto, double posicaoYTexto,
                                     double separadorEstacas, BlockTableRecord blockTableRec, Transaction trans)
    {
        using(DBText text = new DBText())
        {
            // Pega a distancia do primeiro ponto até o atual, divide pelo
            // separadorEstacas. A parte inteira é o número da estaca atual. Pega o
            // resto da divisão e adicionar ao lado do numero: ex: 1 + 23
            double distanciaCur = endPoint.X - startPoint.X;

            if (separadorEstacas > 0)
            {
                text.TextString = ((int)(distanciaCur / separadorEstacas)).ToString() +
                                  ((int)(distanciaCur % 50) != 0 ? " + " + ((int)(distanciaCur % 50)).ToString() : "");
            }
            else
            {
                text.TextString = posicao.ToString();
            }

            text.Height = tamanhoTexto;
            text.Position = new Point3d(endPoint.X, posicaoYTexto, 0);
            text.Rotation = TEXT_ROTATION;

            // Add to Block Table Record
            blockTableRec.AppendEntity(text);
            trans.AddNewlyCreatedDBObject(text, true);
        }
    }

  private
    static void desenhaLinhaEstacas(Point3d gi, double baseLine, BlockTableRecord blockTableRec, Transaction trans)
    {
        using(Line currentLine = new Line(gi, new Point3d(gi.X, baseLine, 0)))
        {
            blockTableRec.AppendEntity(currentLine);
            trans.AddNewlyCreatedDBObject(currentLine, true);
        }
    }

  private
    static void geraGINivel(Point3d gi, double baseline, double valorInicial, double tamanhoTexto, double posicaoTexto,
                            BlockTableRecord blockTableRec, Transaction trans, int scale = 10)
    {
        double curNivel = (gi.Y - baseline) / scale + valorInicial;

        using(DBText text = new DBText())
        {
            text.TextString = Math.Round(curNivel, 3).ToString();
            text.Position = new Point3d(gi.X, posicaoTexto, 0);
            text.Height = tamanhoTexto;
            text.Rotation = TEXT_ROTATION;

            blockTableRec.AppendEntity(text);
            trans.AddNewlyCreatedDBObject(text, true);
        }
    }

    // Utilities
  private
    static PromptDoubleResult getDouble(string message = "Type a double value:", bool allowZero = false)
    {
        Document doc = Application.DocumentManager.MdiActiveDocument;
        Editor ed = doc.Editor;

        PromptDoubleOptions opts = new PromptDoubleOptions(message);
        opts.AllowNone = false;
        opts.AllowNegative = false;
        opts.AllowZero = allowZero;

        return ed.GetDouble(opts);
    }

  private
    static PromptEntityResult getPolyline(string message = "Select a polyline:",
                                          string rejectMessage = "Must be a Polyline...")
    {
        Document doc = Application.DocumentManager.MdiActiveDocument;
        Editor ed = doc.Editor;

        PromptEntityOptions obj = new PromptEntityOptions(message);
        obj.SetRejectMessage(rejectMessage);
        obj.AddAllowedClass(typeof(Polyline), true);

        return ed.GetEntity(obj);
    }

  private
    static PromptEntityResult getLine(string message = "Select a line:", string rejectMessage = "Must be a line...")
    {
        Document doc = Application.DocumentManager.MdiActiveDocument;
        Editor ed = doc.Editor;

        PromptEntityOptions obj = new PromptEntityOptions(message);
        obj.SetRejectMessage(message);
        obj.AddAllowedClass(typeof(Line), true);

        return ed.GetEntity(obj);
    }
}
} // namespace GerarPerfil
