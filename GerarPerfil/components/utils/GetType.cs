using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZwSoft.ZwCAD.ApplicationServices;
using ZwSoft.ZwCAD.DatabaseServices;
using ZwSoft.ZwCAD.EditorInput;

namespace Utils
{
    internal class GetType
    {
        private static PromptDoubleResult GetDouble(string message = "Type a double value:", bool allowZero = false)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            PromptDoubleOptions opts = new PromptDoubleOptions(message);
            opts.AllowNone = false;
            opts.AllowNegative = false;
            opts.AllowZero = allowZero;

            return ed.GetDouble(opts);
        }

        private static PromptEntityResult GetPolyline(string message = "Select a polyline:",
                                                string rejectMessage = "Must be a Polyline...")
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            PromptEntityOptions obj = new PromptEntityOptions(message);
            obj.SetRejectMessage(rejectMessage);
            obj.AddAllowedClass(typeof(Polyline), true);

            return ed.GetEntity(obj);
        }

        private static PromptEntityResult GetLine(string message = "Select a line:", string rejectMessage = "Must be a line...")
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            PromptEntityOptions obj = new PromptEntityOptions(message);
            obj.SetRejectMessage(rejectMessage);
            obj.AddAllowedClass(typeof(Line), true);

            return ed.GetEntity(obj);
        }

        private static PromptResult GetKeyWords(string promptMessage, IEnumerable<string> keywords, bool allowNone, string defaultKeyword = "")
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
