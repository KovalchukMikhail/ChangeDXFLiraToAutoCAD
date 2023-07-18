using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using ChangeDXFLiraToAutoCAD.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeDXFLiraToAutoCAD.Infrastructure
{
    internal class WorkerWithLayer : IWorkerWithLayer
    {
        Database db;
        Document doc;
        public WorkerWithLayer(Database db, Document doc)
        {
            this.db = db;
            this.doc = doc;
        }
        public void RemoveLayer(String layerName)
        {
            // блокируем документ
            using (DocumentLock docloc = doc.LockDocument())
            {
                // начинаем транзакцию
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    // открываем таблицу слоев документа
                    LayerTable acLyrTbl = tr.GetObject(db.LayerTableId, OpenMode.ForWrite) as LayerTable;
                    // получаем запись слоя для изменений
                    LayerTableRecord layer = (LayerTableRecord)tr.GetObject(acLyrTbl[layerName], OpenMode.ForWrite);
                    layer.Erase(true);
                    tr.Commit();
                }
            }
        }
        public void RenameLayer(String layerName, string newLayerName)
        {
            // блокируем документ
            using (DocumentLock docloc = doc.LockDocument())
            {
                // начинаем транзакцию
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    // открываем таблицу слоев документа
                    LayerTable acLyrTbl = tr.GetObject(db.LayerTableId, OpenMode.ForWrite) as LayerTable;   
                    // получаем запись слоя для изменений
                    LayerTableRecord layer = (LayerTableRecord)tr.GetObject(acLyrTbl[layerName], OpenMode.ForWrite);
                    layer.Name = newLayerName;
                    tr.Commit();
                }
            }
        }
    }
}
