using Classes;
using System.Collections.Generic;
using ZwSoft.ZwCAD.ApplicationServices;
using ZwSoft.ZwCAD.DatabaseServices;
using ZwSoft.ZwCAD.EditorInput;
using ZwSoft.ZwCAD.Geometry;

namespace Utils
{
    internal static class GetType
    {
        public static PromptDoubleResult TypeDouble(string message = "Type a double value:", double? defaultValue = null, bool allowZero = false)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            PromptDoubleOptions opts = new PromptDoubleOptions(message);
            opts.AllowNone = false;
            opts.AllowNegative = false;
            opts.AllowZero = allowZero;

            if (defaultValue.HasValue)
                opts.DefaultValue = defaultValue.Value;

            return ed.GetDouble(opts);
        }

        public static PromptEntityResult SelectPolyline(string message = "Select a polyline:",
                                                string rejectMessage = "Must be a Polyline...")
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            PromptEntityOptions obj = new PromptEntityOptions(message);
            obj.SetRejectMessage(rejectMessage);
            obj.AddAllowedClass(typeof(Polyline), true);

            return ed.GetEntity(obj);
        }

        public static PromptEntityResult SelectLine(string message = "Select a line:", string rejectMessage = "Must be a line...")
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            PromptEntityOptions obj = new PromptEntityOptions(message);
            obj.SetRejectMessage(rejectMessage);
            obj.AddAllowedClass(typeof(Line), true);

            return ed.GetEntity(obj);
        }

        public static Polyline GetPolyline(Point2dCollection points)
        {
            Polyline pl = new Polyline();

            int i = 0;

            foreach (Point2d vertex in points.ToArray())
                pl.AddVertexAt(i++, vertex, 0, 0, 0);

            return pl;
        }

        public static Line GetLine(Point3d startPoint, Point3d endPoint)
        {
            return new Line(startPoint, endPoint);
        }

        public static Line GetLine(Point3d startPoint, double yPosition)
        {
            return new Line(startPoint, new Point3d(startPoint.X, yPosition, startPoint.Z));
        }

        public static DBText GetDBText<T>(Profile profile, T content, Point3d pos)
        {
            DBText text = new DBText()
            { 
                TextString = content.ToString(),
                Position = pos,
                Height = profile.TextSet.TamanhoTexto,
                Rotation = profile.TextSet.TextRotation
            };

            return text;
        }

        public static DBText GetDBText(Profile profile, Point3d coords, string textString)
        {
            DBText text = new DBText()
            {
                TextString = textString,
                Height = profile.TextSet.TamanhoTexto,
                Rotation = profile.TextSet.TextRotation,
                Position = coords,
            };

            return text;
        }

        public static PromptResult GetKeyWords(string promptMessage, IEnumerable<string> keywords, bool allowNone, string defaultKeyword = "")
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            var promptKeywordOptions = new PromptKeywordOptions(promptMessage) { AllowNone = allowNone };

            foreach (var keyword in keywords)
            {
                promptKeywordOptions.Keywords.Add(keyword);
            }

            if (defaultKeyword != "")
            {
                promptKeywordOptions.Keywords.Default = defaultKeyword;
            }

            return ed.GetKeywords(promptKeywordOptions);
        }
    }
}
