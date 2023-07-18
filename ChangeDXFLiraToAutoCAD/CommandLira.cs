using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using ChangeDXFLiraToAutoCAD.Infrastructure.Interfaces;
using ChangeDXFLiraToAutoCAD.Infrastructure;

namespace ChangeDXFLiraToAutoCAD
{
    public class CommandLira : IExtensionApplication
    {
        /// <summary>
        /// Entry point
        /// </summary>
        [CommandMethod("LIRA")]
        public void LiraStart()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            // Выбираем все элементы и применяем к ним команду burst (Расчленение выбранных блоков с сохранением слоя блока и преобразованием значений атрибутов в текстовые объекты)
            doc.SendStringToExecute("_ai_selall ", false, false, true);
            doc.SendStringToExecute("burst ", false, false, true);
            // Выбираем все элементы и применяем к ним команду FLATTEN (Ищменяет значение Z всех элементов на 0)
            doc.SendStringToExecute("_ai_selall ", false, false, true);
            doc.SendStringToExecute("FLATTEN  ", false, false, true);
            //Так как для завершения предыдущих команд нужно закончить выполнение метода, все следующие операции помещены в отдельную команду LiraEnd
            doc.SendStringToExecute("LiraEnd ", false, false, true);
        }

        [CommandMethod("LiraEnd")]
        public void LiraEnd()
        {
            // получаем БД и Editor текущего документа
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor editor = doc.Editor;
            IWorkerWithElements workerWithElements = new WorkerWithElements(editor, db);
            IWorkerWithLayer workerWithLayer = new WorkerWithLayer(db, doc);
            // Удаляем элементы из слоя PLAST
            string layerName = "PLAST";
            workerWithElements.RemoveAllElementsLayer(layerName);
            // Смещение текста в центры элементов
            layerName = "USILKLEEN";
            double shiftX = 0.5;
            double shiftY = 0.7;
            workerWithElements.RelocateAllTextInLayer(shiftX, shiftY, layerName);
            // Перенос серых линий в отдельный слой
            layerName = "SOLID";
            string colorName = "9";
            workerWithElements.ChangeLayerForAllElementWithColor(layerName, colorName);
            // Увеличение толщины линий
            layerName = "KLEENKA";
            LineWeight lineWeight = LineWeight.LineWeight060;
            workerWithElements.IncreaseWightLines(layerName, lineWeight);
            // Распологаем линии по цвету в нужном порядке
            string[] color = { "94", "5", "1", "190", "3", "30", "210", "11" };
            foreach (string i in color)
            {
                workerWithElements.ColorMoveToTop(i);
            }
            // Удаляем слои PLAST, STER, SUPEL 
            layerName = "PLAST";
            workerWithLayer.RemoveLayer(layerName);
            layerName = "STER";
            workerWithLayer.RemoveLayer(layerName);
            layerName = "SUPEL";
            workerWithLayer.RemoveLayer(layerName);
            // Переименовываем слои SOLID, KLEENKA, USILKLEEN
            layerName = "SOLID";
            string newLayerName = "Фон";
            workerWithLayer.RenameLayer(layerName, newLayerName);
            layerName = "KLEENKA";
            newLayerName = "Доп. армирование";
            workerWithLayer.RenameLayer(layerName, newLayerName);
            layerName = "USILKLEEN";
            newLayerName = "Площадь арматуры";
            workerWithLayer.RenameLayer(layerName, newLayerName);
        }
        void IExtensionApplication.Initialize() { }
        void IExtensionApplication.Terminate() { }
    }
}
