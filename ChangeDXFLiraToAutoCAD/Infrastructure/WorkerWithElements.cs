using System;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using ChangeDXFLiraToAutoCAD.Infrastructure.Interfaces;

namespace ChangeDXFLiraToAutoCAD.Infrastructure
{
    internal class WorkerWithElements : IWorkerWithElements
    {
        Editor editor;
        Database db;
        IObjectSelecter objectSelecter;
        public WorkerWithElements(Editor editor, Database db)
        {
            this.editor = editor;
            this.db = db;
            objectSelecter = new ObjectSelecter(editor);
        }
        public void RemoveAllElementsLayer(string layerName)
        {
            // получаем массив ID объектов
            ObjectId[] ids;
            try
            {
                ids = objectSelecter.SelectObjectIds((int)DxfCode.LayerName, layerName);
            }
            catch (Exception ex)
            {
                editor.WriteMessage("\nError!\n");
                return;
            }
            // начинаем транзакцию
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                // "пробегаем" по всем полученным объектам
                foreach (ObjectId id in ids)
                {
                    // приводим каждый из них к типу Entity
                    Entity entity = (Entity)tr.GetObject(id, OpenMode.ForRead);
                    //Удаляем объект
                    DeletEntity(entity);
                }
                tr.Commit();
            }
        }
        public void RelocateAllTextInLayer(double shiftX, double shiftY, string layerName)
        {
            // получаем массив ID объектов
            ObjectId[] ids;
            try
            {
                ids = objectSelecter.SelectObjectIds((int)DxfCode.LayerName, layerName);
            }
            catch (Exception ex)
            {
                editor.WriteMessage("\nError!\n");
                return;
            }
            // начинаем транзакцию
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                // открываем таблицу блоков документа
                BlockTable blockTable;
                blockTable = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                // открываем пространство модели (Model Space) - оно является одной из записей в таблице блоков документа
                BlockTableRecord ms = tr.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                // "пробегаем" по всем полученным объектам. Для изменения положения тут по факту создаетются новые объекты текста содержащие тот же текст что и старые но имеющие изменненые координаты. старый текст удаляется
                foreach (ObjectId id in ids)
                {
                    // приводим каждый из старых объектов к типу DBText
                    DBText textOld = (DBText)tr.GetObject(id, OpenMode.ForRead);
                    // создаем новый объект текст
                    DBText text = CreateText(textOld.Position.X + shiftX,
                                                textOld.Position.Y + shiftY,
                                                0,
                                                textOld.TextString,
                                                layerName,
                                                0.05,
                                                Color.FromRgb(0, 255, 255));
                    // добавляем созданный объект в пространство модели и в транзакцию
                    ms.AppendEntity(text);
                    tr.AddNewlyCreatedDBObject(text, true);
                    //Удаляем объект
                    DeletEntity(textOld);
                }
                tr.Commit();
            }
        }
        public void ChangeLayerForAllElementWithColor(string layerName, string colorName)
        {
            // получаем массив ID объектов
            ObjectId[] ids;
            try
            {
                ids = objectSelecter.SelectObjectIds((int)DxfCode.Color, colorName);
            }
            catch (Exception ex)
            {
                editor.WriteMessage("\nError!\n");
                return;
            }
            // начинаем транзакцию
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                // "пробегаем" по всем полученным объектам
                foreach (ObjectId id in ids)
                {
                    // приводим каждый из них к типу Entity
                    Entity entity = (Entity)tr.GetObject(id, OpenMode.ForWrite);

                    // изменяем слой объектов
                    entity.Layer = layerName;
                }
                tr.Commit();
            }
        }
        public void IncreaseWightLines(string layerName, LineWeight lineWeight)
        {
            // получаем массив ID объектов
            ObjectId[] ids;
            try
            {
                ids = objectSelecter.SelectObjectIds((int)DxfCode.LayerName, layerName);
            }
            catch (Exception ex)
            {
                editor.WriteMessage("\nError!\n");
                return;
            }
            // начинаем транзакцию
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                // "пробегаем" по всем полученным объектам
                foreach (ObjectId id in ids)
                {
                    // приводим каждый из них к типу Entity
                    Entity entity = (Entity)tr.GetObject(id, OpenMode.ForWrite);

                    // Меняем толщину объектов
                    entity.LineWeight = lineWeight;
                }
                tr.Commit();
            }
        }
        public void ColorMoveToTop(string colorName)
        {
            ObjectId[] ids;
            try
            {
                ids = objectSelecter.SelectObjectIds((int)DxfCode.Color, colorName);
            }
            catch (Exception ex)
            {
                editor.WriteMessage("\nError!\n");
                return;
            }
            MoveToTop(ids, db);
        }
        public void MoveToTop(ObjectId[] ids, Database db)
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
                    Entity entity = (Entity)tr.GetObject(id, OpenMode.ForRead);
                    block = (BlockTableRecord)tr.GetObject(entity.BlockId, OpenMode.ForWrite);
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
        public void DeletEntity(Entity entity)
        {
            // открываем объект на запись
            entity.UpgradeOpen();
            // удаляем объект
            entity.Erase();
        }
        public DBText CreateText(double positionByX, double positionByY, double positionByZ, string textToWrite, string layerName, double heightOfText, Color color)
        {
            // создаем новые обыекты текста
            DBText text = new DBText();
            // Задание положения текста, толщины текста и установка текста
            text.Position = new Point3d(positionByX, positionByY, positionByZ);
            text.Height = heightOfText;
            text.TextString = textToWrite;
            //устанавливаем для новых объекта нужный слой и цвет
            text.Layer = layerName;
            text.Color = color;
            return text;
        }
    }
}
