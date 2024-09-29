using Classes;
using System.Collections.Generic;
using ZwSoft.ZwCAD.ApplicationServices;
using ZwSoft.ZwCAD.DatabaseServices;
using ZwSoft.ZwCAD.EditorInput;
using ZwSoft.ZwCAD.Geometry;

namespace Utils
{
    internal class GetType
    {
        public static PromptDoubleResult GetDouble(string message = "Type a double value:", bool allowZero = false)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            PromptDoubleOptions opts = new PromptDoubleOptions(message);
            opts.AllowNone = false;
            opts.AllowNegative = false;
            opts.AllowZero = allowZero;

            return ed.GetDouble(opts);
        }

        public static PromptEntityResult GetPolyline(string message = "Select a polyline:",
                                                string rejectMessage = "Must be a Polyline...")
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            PromptEntityOptions obj = new PromptEntityOptions(message);
            obj.SetRejectMessage(rejectMessage);
            obj.AddAllowedClass(typeof(Polyline), true);

            return ed.GetEntity(obj);
        }

        public static PromptEntityResult GetLine(string message = "Select a line:", string rejectMessage = "Must be a line...")
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            PromptEntityOptions obj = new PromptEntityOptions(message);
            obj.SetRejectMessage(rejectMessage);
            obj.AddAllowedClass(typeof(Line), true);

            return ed.GetEntity(obj);
        }
        public static Polyline GenPoly(Point2dCollection points)
        {
            Polyline pl = new Polyline();

            int i = 0;

            foreach (Point2d vertex in points.ToArray())
                pl.AddVertexAt(i++, vertex, 0, 0, 0);

            return pl;
        }

        public static Line DesenhaLinhaEstacas(Point3d startPoint, Point3d endPoint)
        {
            return new Line(startPoint, endPoint);
        }

        public static DBText GenDBText(Profile profile, Point3d coords, string textString)
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
