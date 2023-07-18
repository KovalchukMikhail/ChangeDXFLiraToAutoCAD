using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeDXFLiraToAutoCAD.Infrastructure.Interfaces
{
    internal interface IWorkerWithElements
    {
        /// <summary>
        /// Удаляет все элементы в слое с именем layerName
        /// </summary>
        void RemoveAllElementsLayer(String layerName);

        /// <summary>
        /// Перемещает весь текст в слое с имененм layerName на велечины shiftX по X и shiftY по Y
        /// </summary>
        void RelocateAllTextInLayer(double shiftX, double shiftY, String layerName);

        /// <summary>
        /// Изменяет слой объекта на layerName для всех объектов имеющих цвет colorName
        /// </summary>
        void ChangeLayerForAllElementWithColor(String layerName, String colorName);

        /// <summary>
        /// Меняет вес линий в слое с именем layer name. Новый вес линий 60
        /// </summary>
        void IncreaseWightLines(String layerName, LineWeight lineWeight);

        /// <summary>
        /// Перемещает объекты с цветом collorName на передний план
        /// </summary>
        void ColorMoveToTop(String colorName);
        /// <summary>
        /// Перемещает объекты с цветом на передний план
        /// </summary>
        void MoveToTop(ObjectId[] ids, Database db);
        /// <summary>
        /// Удаляет обыект entity
        /// </summary>
        void DeletEntity(Entity entity);
        /// <summary>
        /// Создает объект DBText с заданными параметрами
        /// </summary>
        DBText CreateText(double positionByX, double positionByY, double positionByZ, string textToWrite, string layerName, double heightOfText, Color color);
    }
}
