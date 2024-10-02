using Classes;
using System;
using Utils;
using ZwSoft.ZwCAD.DatabaseServices;
using ZwSoft.ZwCAD.EditorInput;
using ZwSoft.ZwCAD.Runtime;

namespace GerarPerfil
{
    public class GerarPerfil
    {
        const double TEXT_ROTATION = 90 * Math.PI / 180;
        const double HORIZONTAL_SPACE = 20;

        [CommandMethod("GerarPerfil")]
        public static void Main()
        {
            Drawing currentDrawing = Drawing.GetInstance();
            
            PromptEntityResult giRes = Utils.GetType.SelectPolyline("Select your invert:");

            if (giRes.Status != PromptStatus.OK)
                return;

            PromptEntityResult terrenoRes = Utils.GetType.SelectPolyline("Select your terrain:");

            if (terrenoRes.Status != PromptStatus.OK)
                return;

            PromptEntityResult baseRes = Utils.GetType.SelectLine("Select your baseLine:");

            if (baseRes.Status != PromptStatus.OK)
                return;

            PromptDoubleResult scaleRes = Utils.GetType.TypeDouble("Scale: ", 10);

            if (baseRes.Status != PromptStatus.OK)
                return;

            PromptDoubleResult valorInicial = Utils.GetType.TypeDouble("Type your baseLine level:");

            if (valorInicial.Status != PromptStatus.OK)
                return;

            PromptDoubleResult tamanhoTexto = Utils.GetType.TypeDouble("Type a font-height:");

            if (tamanhoTexto.Status != PromptStatus.OK)
                return;

            PromptDoubleResult distancia = Utils.GetType.TypeDouble("Increase your station number every how much distance?", null, true);

            if (distancia.Status != PromptStatus.OK)
                return;

            PromptResult geraPontos = Utils.GetType.GetKeyWords("Do you wish to generate main points?", new[] {"Yes", "No"}, false, "Yes");

            if (geraPontos.Status == PromptStatus.Error)
                return;

            using (currentDrawing.Transation = currentDrawing.Document.TransactionManager.StartTransaction())
            {
                currentDrawing.BlockTable = currentDrawing.Transation.GetObject(currentDrawing.Database.BlockTableId, OpenMode.ForRead) as BlockTable;
                currentDrawing.BlockTableRecord = currentDrawing.Transation.GetObject(currentDrawing.BlockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                Profile profile = new Profile(
                    currentDrawing.Transation.GetObject(giRes.ObjectId, OpenMode.ForWrite) as Polyline,
                    currentDrawing.Transation.GetObject(terrenoRes.ObjectId, OpenMode.ForRead) as Polyline,
                    currentDrawing.Transation.GetObject(baseRes.ObjectId, OpenMode.ForRead) as Line,
                    scaleRes.Value,
                    new Text(tamanhoTexto.Value, TEXT_ROTATION),
                    distancia.Value,
                    valorInicial.Value,
                    HORIZONTAL_SPACE
                );

                if (geraPontos.StringResult == "Yes" || geraPontos.Status == PromptStatus.None)
                {
                    PromptResult keepLine = Utils.GetType.GetKeyWords("Erase original line?", new[] { "Yes", "No" }, false, "Yes");

                    if (keepLine.Status == PromptStatus.Error)
                        return;

                    if (keepLine.StringResult == "Yes")
                        profile.Invert.Erase(true);
  
                    profile.Invert = profile.GeraPontos();

                    currentDrawing.DrawPolyline(profile.Invert);
                }

                profile.PopulateData();
                currentDrawing.AppendToDrawing(profile);
                currentDrawing.DrawLine(profile.GetRefLines());
                currentDrawing.Transation.Commit();
            }
        }
    }
}
