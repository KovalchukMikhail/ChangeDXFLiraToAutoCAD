using Autodesk.AutoCAD.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;

namespace ChangeDXFLiraToAutoCAD
{
    public class Model
    {
        public static void RemoveAllElementLayer(String layerName, TypedValue[] filterList, Editor ed, Database db)
        {
            filterList[0] = new TypedValue((int)DxfCode.LayerName, "PLAST");

            // создаем фильтр
            SelectionFilter filter = new SelectionFilter(filterList);

            // пытаемся получить ссылки на объекты с учетом фильтра
            PromptSelectionResult selRes = ed.SelectAll(filter);

            // если произошла ошибка - сообщаем о ней
            if (selRes.Status != PromptStatus.OK)
            {
                ed.WriteMessage("\nError!\n");
                return;
            }
            // получаем массив ID объектов
            ObjectId[] ids = selRes.Value.GetObjectIds();

            // начинаем транзакцию
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                // "пробегаем" по всем полученным объектам
                foreach (ObjectId id in ids)
                {
                    // приводим каждый из них к типу Entity
                    Entity entity = (Entity)tr.GetObject(id, OpenMode.ForRead);

                    // открываем объект на запись
                    entity.UpgradeOpen();

                    // удаляем объект
                    entity.Erase();
                }
                tr.Commit();
            }
        }

        public static void RelocateAllTextInLayer(double shiftX, double shiftY, String layerName, TypedValue[] filterList, Editor ed, Database db)
        {
            filterList[0] = new TypedValue((int)DxfCode.LayerName, layerName);

            // создаем фильтр
            SelectionFilter filter = new SelectionFilter(filterList);

            // пытаемся получить ссылки на объекты с учетом фильтра
            PromptSelectionResult selResSecond = ed.SelectAll(filter);

            // если произошла ошибка - сообщаем о ней
            if (selResSecond.Status != PromptStatus.OK)
            {
                ed.WriteMessage("\nError!\n");
                return;
            }
            // получаем массив ID объектов
            ObjectId[] idsSecond = selResSecond.Value.GetObjectIds();

            // начинаем транзакцию
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                // открываем таблицу блоков документа
                BlockTable acBlkTbl;
                acBlkTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;

                // открываем пространство модели (Model Space) - оно является одной из записей в таблице блоков документа
                BlockTableRecord ms = tr.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                // "пробегаем" по всем полученным объектам. Для изменения положения тут по факту создаетются новые объекты текста содержащие тот же текст что и старые но имеющие изменненые координаты. старый текст удаляется
                foreach (ObjectId id in idsSecond)
                {
                    // приводим каждый из старых объектов к типу DBText
                    DBText textSecond = (DBText)tr.GetObject(id, OpenMode.ForRead);

                    // создаем новые обыекты текста
                    DBText text = new DBText();

                    // копируем в новые обекты текста содержимое старых и задаем новое положение и размер текста для новых объектов
                    text.Position = new Point3d(textSecond.Position.X + shiftX, textSecond.Position.Y + shiftY, 0);
                    text.Height = 0.05;
                    text.TextString = textSecond.TextString;
                    //устанавливаем для новых объекта нужный слой и цвет
                    text.Layer = layerName;
                    text.Color = Autodesk.AutoCAD.Colors.Color.FromRgb(0, 255, 255);
                    // добавляем созданный объект в пространство модели и в транзакцию
                    ms.AppendEntity(text);
                    tr.AddNewlyCreatedDBObject(text, true);

                    // открываем старые объект на запись и удаляем их
                    textSecond.UpgradeOpen();
                    textSecond.Erase();
                }
                tr.Commit();
            }
        }

        public static void ChangeLayerForAllElementWithColor(String layerName, String ColorName, TypedValue[] filterList, Editor ed, Database db)
        {
            filterList[0] = new TypedValue((int)DxfCode.Color, ColorName);

            // создаем фильтр
            SelectionFilter filter = new SelectionFilter(filterList);

            // пытаемся получить ссылки на объекты с учетом фильтра
            PromptSelectionResult selRes = ed.SelectAll(filter);

            // если произошла ошибка - сообщаем о ней
            if (selRes.Status != PromptStatus.OK)
            {
                ed.WriteMessage("\nError!\n");
                return;
            }
            // получаем массив ID объектов
            ObjectId[] idsGray = selRes.Value.GetObjectIds();

            // начинаем транзакцию
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                // "пробегаем" по всем полученным объектам
                foreach (ObjectId id in idsGray)
                {
                    // приводим каждый из них к типу Entity
                    Entity gray = (Entity)tr.GetObject(id, OpenMode.ForWrite);

                    // изменяем слой объектов
                    gray.Layer = layerName;
                }
                tr.Commit();
            }
        }


        public static void IncreaseeWightLines(String layerName, TypedValue[] filterList, Editor ed, Database db)
        {
            filterList[0] = new TypedValue((int)DxfCode.LayerName, layerName);

            // создаем фильтр
            SelectionFilter filter = new SelectionFilter(filterList);

            // пытаемся получить ссылки на объекты с учетом фильтра
            PromptSelectionResult selResLayer = ed.SelectAll(filter);

            // если произошла ошибка - сообщаем о ней
            if (selResLayer.Status != PromptStatus.OK)
            {
                ed.WriteMessage("\nError!\n");
                return;
            }
            // получаем массив ID объектов
            ObjectId[] idsLayer = selResLayer.Value.GetObjectIds();

            // начинаем транзакцию
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                // "пробегаем" по всем полученным объектам
                foreach (ObjectId id in idsLayer)
                {
                    // приводим каждый из них к типу Entity
                    Entity width = (Entity)tr.GetObject(id, OpenMode.ForWrite);

                    // Меняем толщину объектов
                    width.LineWeight = LineWeight.LineWeight060;
                }
                tr.Commit();
            }
        }

    
        public static void ColorMoveToTop(String colorName, TypedValue[] filterList, Editor ed, Database db)
        {

            filterList[0] = new TypedValue((int)DxfCode.Color, colorName);
            // создаем фильтр
            SelectionFilter filter = new SelectionFilter(filterList);
            // пытаемся получить ссылки на объекты с учетом фильтра
            PromptSelectionResult selRes = ed.SelectAll(filter);
            // если цвет объекты существуют то выполняем метод MoveToTop() который последовательно размещает объекты на переднем плане
            if (selRes.Status == PromptStatus.OK)
            {
                // получаем массив ID объектов
                ObjectId[] ids = selRes.Value.GetObjectIds();
                MoveToTop(ids, db);
            }
        }

        public static void RemoveLayer(String layerName, Database db, Document doc)
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

        public static void RenameLayer(String layerName, string newLayerName, Database db, Document doc)
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


        static void MoveToTop(ObjectId[] ids, Database db)
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTableRecord block;
                DrawOrderTable drawOrder;
                ObjectIdCollection idsLine = new ObjectIdCollection();

                // "пробегаем" по всем полученным объектам
                foreach (ObjectId id in ids)
                {
                    // приводим каждый из них к типу Entity
                    Entity gray = (Entity)tr.GetObject(id, OpenMode.ForRead);
                    block = (BlockTableRecord)tr.GetObject(gray.BlockId, OpenMode.ForWrite);
                    drawOrder = (DrawOrderTable)tr.GetObject(block.DrawOrderTableId, OpenMode.ForWrite);
                    idsLine.Add(id);

                    if (id == ids[ids.Length - 1])
                    {
                        drawOrder.MoveToTop(idsLine);
                    }
                }

                tr.Commit();
            }
        }

    }
}
