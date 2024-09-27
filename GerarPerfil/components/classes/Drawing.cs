using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZwSoft.ZwCAD.DatabaseServices;
using ZwSoft.ZwCAD.EditorInput;
using ZwSoft.ZwCAD.ApplicationServices;
using System.Diagnostics;

namespace Classes
{
    public sealed class Dwg
    {
        public Document Document { get; }
        public Editor Editor { get; }
        public Database Database { get; }
        public BlockTable BlockTable { get; set; }
        public BlockTableRecord BlockTabelRecord { get; set; }
        public Transaction Transation { get; set; }

        static Dwg _instance;

        private Dwg(Document currentDocument)
        {
            Document = currentDocument;
            Editor = currentDocument.Editor;
            Database = currentDocument.Database;
        }

        public static Dwg GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Dwg(Application.DocumentManager.CurrentDocument);
            }

            return _instance;
        }
    }


    
}
