using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeDXFLiraToAutoCAD.Infrastructure.Interfaces
{
    internal interface IWorkerWithLayer
    {
        /// <summary>
        /// Удаляет слой с именем layerName.
        /// </summary>
        void RemoveLayer(String layerName);

        /// <summary>
        /// Renames layer with name equal layerName.
        /// Пееименовывает слой с именем layerName. Новое имя newLayerName
        /// </summary>
        void RenameLayer(String layerName, string newLayerName);
    }
}
