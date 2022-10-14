
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;

// Привет


namespace ChangeDXFLiraToAutoCAD
{
    public class CommonClass
    {
        public class CommandsFirst : IExtensionApplication
        {
            [CommandMethod("LIRA")]
            public void eraseCircles_2()
            {
                Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                doc.SendStringToExecute("_ai_selall ", false, false, true);
                doc.SendStringToExecute("burst ", false, false, true);

                doc.SendStringToExecute("_ai_selall ", false, false, true);
                doc.SendStringToExecute("FLATTEN  ", false, false, true);

                doc.SendStringToExecute("LiraPlus ", false, false, true);

            }

            void IExtensionApplication.Initialize()
            {

            }

            void IExtensionApplication.Terminate()
            {

            }
        }


        public class CommandsSecond : IExtensionApplication
        {
            [CommandMethod("LiraPlus")]
            public void eraseCircles_2()
            {
                // Удаляем элементы из слоя PLAST

                // получаем БД и Editor текущего документа
                Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                Database db = doc.Database;
                Editor ed = doc.Editor;

                // создаем переменную, в которой будут содержаться данные для фильтра
                TypedValue[] filterlist = new TypedValue[1];

                // первый аргумент ((int)DxfCode.LayerName) указывает, что мы задаем тип объекта, а именно имя слоя
                // второй аргумент ("PLAST") - собственно тип (имя слоя)
                filterlist[0] = new TypedValue((int)DxfCode.LayerName, "PLAST");

                // создаем фильтр
                SelectionFilter filter = new SelectionFilter(filterlist);

                // пытаемся получить ссылки на объекты с учетом фильтра
                // ВНИМАНИЕ! Нужно проверить работоспособность метода с замороженными и заблокированными слоями!
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

                        // открываем приговоренный объект на запись
                        entity.UpgradeOpen();

                        // удаляем объект
                        entity.Erase();
                    }

                    tr.Commit();
                }

                // Смещение текста в центры элементов

                TypedValue[] filterListSecond = new TypedValue[1];

                // первый аргумент (0) указывает, что мы задаем тип объекта
                // второй аргумент ("CIRCLE") - собственно тип
                filterListSecond[0] = new TypedValue((int)DxfCode.LayerName, "USILKLEEN");

                // создаем фильтр
                SelectionFilter filterSecond = new SelectionFilter(filterListSecond);


                // пытаемся получить ссылки на объекты с учетом фильтра
                // ВНИМАНИЕ! Нужно проверить работоспособность метода с замороженными и заблокированными слоями!
                PromptSelectionResult selResSecond = ed.SelectAll(filterSecond);

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

                    // "пробегаем" по всем полученным объектам
                    foreach (ObjectId id in idsSecond)
                    {
                        // приводим каждый из них к типу Entity
                        DBText textSecond = (DBText)tr.GetObject(id, OpenMode.ForRead);

                        DBText text = new DBText();

                        text.Position = new Point3d(textSecond.Position.X + 0.5, textSecond.Position.Y + 0.7, 0);
                        text.Height = 0.05;
                        text.TextString = textSecond.TextString;
                        //устанавливаем для объекта нужный слой и цвет
                        text.Layer = "USILKLEEN";
                        text.Color = Autodesk.AutoCAD.Colors.Color.FromRgb(0, 255, 255);
                        // добавляем созданный объект в пространство модели и в транзакцию
                        ms.AppendEntity(text);
                        tr.AddNewlyCreatedDBObject(text, true);

                        // открываем приговоренный объект на запись
                        textSecond.UpgradeOpen();
                        textSecond.Height = 0.05;
                        textSecond.Erase();

                    }

                    tr.Commit();
                }

                // Перенос серых линий в отдельный слой
                // создаем переменную, в которой будут содержаться данные для фильтра
                TypedValue[] filterGray = new TypedValue[1];

                // первый аргумент ((int)DxfCode.LayerName) указывает, что мы задаем тип объекта, а именно имя слоя
                // второй аргумент ("PLAST") - собственно тип (имя слоя)
                // переменную которая будет хранить цвет


                filterGray[0] = new TypedValue((int)DxfCode.Color, "9");

                // создаем фильтр
                SelectionFilter filterForGray = new SelectionFilter(filterGray);

                // пытаемся получить ссылки на объекты с учетом фильтра
                // ВНИМАНИЕ! Нужно проверить работоспособность метода с замороженными и заблокированными слоями!
                PromptSelectionResult selResGray = ed.SelectAll(filterForGray);

                // если произошла ошибка - сообщаем о ней
                if (selResGray.Status != PromptStatus.OK)
                {
                    ed.WriteMessage("\nError!\n");
                    return;
                }

                // получаем массив ID объектов
                ObjectId[] idsGray = selResGray.Value.GetObjectIds();

                // начинаем транзакцию
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {

                    // "пробегаем" по всем полученным объектам
                    foreach (ObjectId id in idsGray)
                    {
                        // приводим каждый из них к типу Entity
                        Entity gray = (Entity)tr.GetObject(id, OpenMode.ForWrite);

                        // изменяем слой объектов
                        gray.Layer = "SOLID";
                    }

                    tr.Commit();
                }


                // Увеличение толщины линий

                // создаем переменную, в которой будут содержаться данные для фильтра
                TypedValue[] filterLayer = new TypedValue[1];

                // первый аргумент ((int)DxfCode.LayerName) указывает, что мы задаем тип объекта, а именно имя слоя
                // второй аргумент ("PLAST") - собственно тип (имя слоя)
                // переменную которая будет хранить цвет


                filterLayer[0] = new TypedValue((int)DxfCode.LayerName, "KLEENKA");

                // создаем фильтр
                SelectionFilter filterForLayer = new SelectionFilter(filterLayer);

                // пытаемся получить ссылки на объекты с учетом фильтра
                // ВНИМАНИЕ! Нужно проверить работоспособность метода с замороженными и заблокированными слоями!
                PromptSelectionResult selResLayer = ed.SelectAll(filterForLayer);

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

                // Распологаем цвета в соответсвующем порядке
                // создаем переменную, в которой будут содержаться данные для фильтра
                TypedValue[] filterColor94 = new TypedValue[1];
                TypedValue[] filterColor5 = new TypedValue[1];
                TypedValue[] filterColor1 = new TypedValue[1];
                TypedValue[] filterColor190 = new TypedValue[1];
                TypedValue[] filterColor3 = new TypedValue[1];
                TypedValue[] filterColor30 = new TypedValue[1];
                TypedValue[] filterColor210 = new TypedValue[1];
                TypedValue[] filterColor11 = new TypedValue[1];

                // первый аргумент ((int)DxfCode.Color) указывает, что мы задаем тип объекта, а именно цвет
                // второй аргумент ("94") - собственно тип (цвет)
                // переменную которая будет хранить цвет

                filterColor94[0] = new TypedValue((int)DxfCode.Color, "94");
                filterColor5[0] = new TypedValue((int)DxfCode.Color, "5");
                filterColor1[0] = new TypedValue((int)DxfCode.Color, "1");
                filterColor190[0] = new TypedValue((int)DxfCode.Color, "190");
                filterColor3[0] = new TypedValue((int)DxfCode.Color, "3");
                filterColor30[0] = new TypedValue((int)DxfCode.Color, "30");
                filterColor210[0] = new TypedValue((int)DxfCode.Color, "210");
                filterColor11[0] = new TypedValue((int)DxfCode.Color, "11");


                // создаем фильтр
                SelectionFilter filterFor94 = new SelectionFilter(filterColor94);
                SelectionFilter filterFor5 = new SelectionFilter(filterColor5);
                SelectionFilter filterFor1 = new SelectionFilter(filterColor1);
                SelectionFilter filterFor190 = new SelectionFilter(filterColor190);
                SelectionFilter filterFor3 = new SelectionFilter(filterColor3);
                SelectionFilter filterFor30 = new SelectionFilter(filterColor30);
                SelectionFilter filterFor210 = new SelectionFilter(filterColor210);
                SelectionFilter filterFor11 = new SelectionFilter(filterColor11);

                // пытаемся получить ссылки на объекты с учетом фильтра
                // ВНИМАНИЕ! Нужно проверить работоспособность метода с замороженными и заблокированными слоями!
                PromptSelectionResult selRes94 = ed.SelectAll(filterFor94);
                PromptSelectionResult selRes5 = ed.SelectAll(filterFor5);
                PromptSelectionResult selRes1 = ed.SelectAll(filterFor1);
                PromptSelectionResult selRes190 = ed.SelectAll(filterFor190);
                PromptSelectionResult selRes3 = ed.SelectAll(filterFor3);
                PromptSelectionResult selRes30 = ed.SelectAll(filterFor30);
                PromptSelectionResult selRes210 = ed.SelectAll(filterFor210);
                PromptSelectionResult selRes11 = ed.SelectAll(filterFor11);

                // если произошла ошибка - сообщаем о ней
                if (selRes94.Status == PromptStatus.OK)
                {
                    // получаем массив ID объектов
                    ObjectId[] ids94 = selRes94.Value.GetObjectIds();
                    MoveToTop(ids94, db);

                }

                if (selRes5.Status == PromptStatus.OK)
                {
                    // получаем массив ID объектов
                    ObjectId[] ids5 = selRes5.Value.GetObjectIds();
                    MoveToTop(ids5, db);

                }

                if (selRes1.Status == PromptStatus.OK)
                {
                    // получаем массив ID объектов
                    ObjectId[] ids1 = selRes1.Value.GetObjectIds();
                    MoveToTop(ids1, db);

                }

                if (selRes190.Status == PromptStatus.OK)
                {
                    // получаем массив ID объектов
                    ObjectId[] ids190 = selRes190.Value.GetObjectIds();
                    MoveToTop(ids190, db);

                }

                if (selRes3.Status == PromptStatus.OK)
                {
                    // получаем массив ID объектов
                    ObjectId[] ids3 = selRes3.Value.GetObjectIds();
                    MoveToTop(ids3, db);

                }

                if (selRes30.Status == PromptStatus.OK)
                {
                    // получаем массив ID объектов
                    ObjectId[] ids30 = selRes3.Value.GetObjectIds();
                    MoveToTop(ids30, db);

                }

                if (selRes210.Status == PromptStatus.OK)
                {
                    // получаем массив ID объектов
                    ObjectId[] ids210 = selRes210.Value.GetObjectIds();
                    MoveToTop(ids210, db);

                }

                if (selRes11.Status == PromptStatus.OK)
                {
                    // получаем массив ID объектов
                    ObjectId[] ids11 = selRes11.Value.GetObjectIds();
                    MoveToTop(ids11, db);

                }

                // Удаляем слои PLAST, STER, SUPEL, переименовываем слои SOLID, KLEENKA, USILKLEEN

                // блокируем документ
                using (DocumentLock docloc = doc.LockDocument())
                {
                    // начинаем транзакцию
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        // открываем таблицу слоев документа
                        LayerTable acLyrTbl = tr.GetObject(db.LayerTableId, OpenMode.ForWrite) as LayerTable;

                        // получаем запись слоя для изменений
                        LayerTableRecord layerSolid = (LayerTableRecord)tr.GetObject(acLyrTbl["SOLID"], OpenMode.ForWrite);
                        LayerTableRecord layerKleenka = (LayerTableRecord)tr.GetObject(acLyrTbl["KLEENKA"], OpenMode.ForWrite);
                        LayerTableRecord layerUsilkleen = (LayerTableRecord)tr.GetObject(acLyrTbl["USILKLEEN"], OpenMode.ForWrite);
                        LayerTableRecord layerPlast = (LayerTableRecord)tr.GetObject(acLyrTbl["PLAST"], OpenMode.ForWrite);
                        LayerTableRecord layerSter = (LayerTableRecord)tr.GetObject(acLyrTbl["STER"], OpenMode.ForWrite);
                        LayerTableRecord layerSupel = (LayerTableRecord)tr.GetObject(acLyrTbl["SUPEL"], OpenMode.ForWrite);

                        layerSolid.Name = "Фон";
                        layerKleenka.Name = "Доп. армирование";
                        layerUsilkleen.Name = "Площадь арматуры";

                        layerPlast.Erase(true);
                        layerSter.Erase(true);
                        layerSupel.Erase(true);

                        tr.Commit();

                    }
                }

            }


            void MoveToTop(ObjectId[] ids, Database db)
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


            void IExtensionApplication.Initialize()
            {

            }

            void IExtensionApplication.Terminate()
            {

            }
        }
    }
}
