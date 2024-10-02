using System.Collections.Generic;
using System.Linq;
using Utils;
using ZwSoft.ZwCAD.ApplicationServices;
using ZwSoft.ZwCAD.DatabaseServices;
using ZwSoft.ZwCAD.EditorInput;
using ZwSoft.ZwCAD.Geometry;


namespace Classes
{
    public sealed class Drawing
    {
        public Document Document { get; }
        public Editor Editor { get; }
        public Database Database { get; }
        public BlockTable BlockTable { get; set; }
        public BlockTableRecord BlockTableRecord { get; set; }
        public Transaction Transation { get; set; }

        static Drawing _instance;

        private Drawing(Document currentDocument)
        {
            Document = currentDocument;
            Editor = currentDocument.Editor;
            Database = currentDocument.Database;
        }

        public static Drawing GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Drawing(Application.DocumentManager.CurrentDocument);
            }

            return _instance;
        }

        public void AppendToDrawing(Profile profile)
        {
            foreach (Data data in profile.Data)
            {
                double curHorizontalSpace = data.Position.Y;

                foreach (var field in typeof(Data).GetProperties().Where(p => p.Name != "Position"))
                {
                    curHorizontalSpace -= profile.TextSet.TamanhoTexto * profile.HorizontalSpace;

                    var value = field.GetValue(data);
                    var entity = Utils.GetType.GetDBText(profile, value, new Point3d(data.Position.X, curHorizontalSpace, data.Position.Z));
                    BlockTableRecord.AppendEntity(entity);
                    Transation.AddNewlyCreatedDBObject(entity, true);
                }
            }
        }

        public void DrawPolyline(Polyline polyline)
        {
            BlockTableRecord.AppendEntity(polyline);
            Transation.AddNewlyCreatedDBObject(polyline, true);
        }

        public void DrawLine(List<Line> lines)
        {
            foreach (var line in lines)
            {
                BlockTableRecord.AppendEntity(line);
                Transation.AddNewlyCreatedDBObject(line, true);
            }
        }
    }


    
}
