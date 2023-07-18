using System;

namespace ChangeDXFLiraToAutoCAD.Infrastructure.Interfaces
{
    internal interface IWorkerWithLayer
    {
        /// <summary>
        /// Удаляет слой с именем layerName.
        /// </summary>
        void RemoveLayer(string layerName);

        /// <summary>
        /// Renames layer with name equal layerName.
        /// Пееименовывает слой с именем layerName. Новое имя newLayerName
        /// </summary>
        void RenameLayer(string layerName, string newLayerName);
    }
}
