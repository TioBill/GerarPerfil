using System;
using System.Collections.Generic;
using System.Linq;
using ZwSoft.ZwCAD.ApplicationServices;
using ZwSoft.ZwCAD.DatabaseServices;
using ZwSoft.ZwCAD.EditorInput;
using ZwSoft.ZwCAD.Geometry;
using ZwSoft.ZwCAD.Runtime;

using Debug;
using Classes;
using GerarPerfil.components.debug;

namespace GerarPerfil
{
    public static class Extensions
    {
        public static Point3d[] ToArray(this Point3dCollection pts)
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


        [CommandMethod("GerarPerfil")]
        public static void main()
        {
            Dwg currentDrawing = Dwg.GetInstance();

            PromptEntityResult giRes = GetPolyline("Select your invert:");

            if (giRes.Status != PromptStatus.OK)
                return;

            PromptEntityResult terrenoRes = GetPolyline("Select your terrain:");

            if (terrenoRes.Status != PromptStatus.OK)
                return;

            PromptEntityResult baseRes = GetLine("Select your baseLine:");

            if (baseRes.Status != PromptStatus.OK)
                return;

            PromptDoubleResult valorInicial = GetDouble("Type your baseLine level:");

            if (valorInicial.Status != PromptStatus.OK)
                return;

            PromptDoubleResult tamanhoTexto = GetDouble("Type a font-height:");

            if (tamanhoTexto.Status != PromptStatus.OK)
                return;

            PromptDoubleResult distancia = GetDouble("Increase your station number every how much distance?", true);

            if (distancia.Status != PromptStatus.OK)
                return;

            PromptResult geraPontos = GetKeyWords("Do you wish to generate main points?", new[] {"Yes", "No"}, false, "Yes");

            if (geraPontos.Status != PromptStatus.OK)
                return;

            PromptResult keepLine = GetKeyWords("Erase original line?", new[] { "Yes", "No" }, false, "Yes");

            if (keepLine.Status != PromptStatus.OK) 
                return;

            

            using (currentDrawing.Transation = currentDrawing.Document.TransactionManager.StartTransaction())
            {
                currentDrawing.BlockTable = currentDrawing.Transation.GetObject(currentDrawing.Database.BlockTableId, OpenMode.ForRead) as BlockTable;
                currentDrawing.BlockTabelRecord = currentDrawing.Transation.GetObject(currentDrawing.BlockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Profile profile = new Profile(
                    currentDrawing.Transation.GetObject(giRes.ObjectId, OpenMode.ForWrite) as Polyline,
                    currentDrawing.Transation.GetObject(terrenoRes.ObjectId, OpenMode.ForRead) as Polyline,
                    currentDrawing.Transation.GetObject(baseRes.ObjectId, OpenMode.ForRead) as Line
                );

                if (geraPontos.StringResult == "Yes" || geraPontos.Status == PromptStatus.None)
                {
                    if (keepLine.StringResult == "Yes")
                        profile.Invert.Erase(true);

                    profile.Invert = GeraPontos(profile.Invert, distancia.Value, blockTableRec, trans);
                }
                

                double curPosicaoTexto = 0;

                for (int i = 0; i < profile.Invert.NumberOfVertices; i++)
                {
                    // Desenhando Linhas das Estacas


                    
                    DesenhaLinhaEstacas(profile.Invert.GetPoint3dAt(i), baseLine.StartPoint.Y, blockTableRec, trans);

                    curPosicaoTexto = baseLine.StartPoint.Y - tamanhoTexto.Value * 20;

                    // Gera numeração das estacas
                    GeraNumeracaoEstacas(profile.Invert.GetPoint3dAt(0),    profile.Invert.GetPoint3dAt(i), i, tamanhoTexto.Value, curPosicaoTexto,
                                         distancia.Value, blockTableRec, trans);

                    curPosicaoTexto = baseLine.StartPoint.Y - tamanhoTexto.Value * 40;

                    // Gera Nivel da Geratrix Inferior (G.I)
                    GeraGINivel(profile.Invert.GetPoint3dAt(i), baseLine.StartPoint.Y, valorInicial.Value, tamanhoTexto.Value,
                                curPosicaoTexto, blockTableRec, trans);

                    curPosicaoTexto = baseLine.StartPoint.Y - tamanhoTexto.Value * 60;

                    // Gera a Distancia Progressiva
                    GeraDistanciaProgressiva(profile.Invert.GetPoint3dAt(0), profile.Invert.GetPoint3dAt(i), curPosicaoTexto, tamanhoTexto.Value,
                                             blockTableRec, trans);

                    curPosicaoTexto = baseLine.StartPoint.Y - tamanhoTexto.Value * 80;

                    // Gera extensão
                    if (i > 0)
                    {
                        GeraExtensao(profile.Invert.GetPoint3dAt(i - 1), profile.Invert.GetPoint3dAt(i), curPosicaoTexto, tamanhoTexto.Value,
                                     blockTableRec, trans);

                        curPosicaoTexto = baseLine.StartPoint.Y - tamanhoTexto.Value * 100;
                        
                        // Gera declividade
                        GeraDeclividade(profile.Invert.GetPoint3dAt(i - 1), profile.Invert.GetPoint3dAt(i), curPosicaoTexto, tamanhoTexto.Value,
                                        blockTableRec, trans);
                    }

                    curPosicaoTexto = baseLine.StartPoint.Y - tamanhoTexto.Value * 120;

                    // Gera nivel de terreno e Profundiade
                    var result = GeraNivelTerrenoEProf(profile.Invert.GetPoint3dAt(i), terreno, baseLine, valorInicial.Value, curPosicaoTexto,
                                          tamanhoTexto.Value, blockTableRec, trans);

                    result.UnwrapOrElse(err =>
                    {
                        switch (err)
                        {
                            case ResultType.NoIntersection:
                                var promptResult = GetKeyWords("No intersection found? Wish to continue?", new[] { "Yes", "No"}, false, "No");
                                break;

                            default:
                                break;
                        };
                    });
                }

                trans.Commit();
            }
        }
    }
} // namespace GerarPerfil
