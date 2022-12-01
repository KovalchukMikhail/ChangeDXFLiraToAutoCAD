
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;



namespace ChangeDXFLiraToAutoCAD
{
    public class CommonClass
    {
        public class CommandsFirst : IExtensionApplication
        {
            /// <summary>
            /// Entry point
            /// </summary>
            [CommandMethod("LIRA")]
            public void LiraStart()
            {
                Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;

                // Выбираем все элементы и применяем к ним команду burst (Расчленение выбранных блоков с сохранением слоя блока и преобразованием значений атрибутов в текстовые объекты)
                doc.SendStringToExecute("_ai_selall ", false, false, true);
                doc.SendStringToExecute("burst ", false, false, true);

                // Выбираем все элементы и применяем к ним команду FLATTEN (Ищменяет значение Z всех элементов на 0)
                doc.SendStringToExecute("_ai_selall ", false, false, true);
                doc.SendStringToExecute("FLATTEN  ", false, false, true);

                //Так как для завершения предыдущих команд нужно закончить выполнение метода, все следующие операции помещены в отдельную команду LiraEnd
                doc.SendStringToExecute("LiraEnd ", false, false, true);

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
            [CommandMethod("LiraEnd")]
            public void LiraEnd()
            {
                // получаем БД и Editor текущего документа
                Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                Database db = doc.Database;
                Editor ed = doc.Editor;
                // создаем переменную, в которой будут содержаться данные для фильтра
                TypedValue[] filterList = new TypedValue[1];

                // Удаляем элементы из слоя PLAST
                string layerName = "PLAST";
                Model.RemoveAllElementsLayer(layerName, filterList, ed, db);

                // Смещение текста в центры элементов
                layerName = "USILKLEEN";
                double shiftX = 0.5;
                double shiftY = 0.7;
                Model.RelocateAllTextInLayer(shiftX, shiftY, layerName, filterList, ed, db);


                // Перенос серых линий в отдельный слой
                layerName = "SOLID";
                string colorName = "9";
                Model.ChangeLayerForAllElementWithColor(layerName, colorName, filterList, ed, db);

                // Увеличение толщины линий
                layerName = "KLEENKA";
                Model.IncreaseWightLines(layerName, filterList, ed, db);

                // Распологаем линии по цвету в нужном порядке
                string[] color = { "94", "5", "1", "190", "3", "30", "210", "11" };
                foreach (string i in color)
                {
                    Model.ColorMoveToTop(i, filterList, ed, db);
                }

                // Удаляем слои PLAST, STER, SUPEL 
                layerName = "PLAST";
                Model.RemoveLayer(layerName, db, doc);
                layerName = "STER";
                Model.RemoveLayer(layerName, db, doc);
                layerName = "SUPEL";
                Model.RemoveLayer(layerName, db, doc);

                // Переименовываем слои SOLID, KLEENKA, USILKLEEN
                layerName = "SOLID";
                string newLayerName = "Фон";
                Model.RenameLayer(layerName, newLayerName, db, doc);
                layerName = "KLEENKA";
                newLayerName = "Доп. армирование";
                Model.RenameLayer(layerName, newLayerName, db, doc);
                layerName = "USILKLEEN";
                newLayerName = "Площадь арматуры";
                Model.RenameLayer(layerName, newLayerName, db, doc);
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
