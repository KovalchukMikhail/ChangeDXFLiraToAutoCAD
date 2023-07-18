using Autodesk.AutoCAD.EditorInput;

namespace ChangeDXFLiraToAutoCAD.Infrastructure.Interfaces
{
    internal interface IFilterCreator
    {
        /// <summary>
        /// Возвращает филтр соответсвующих переданным параметрам
        /// </summary>
        SelectionFilter GetFilterByParameters(int dxfCodeOfParameter, string parameterName);
    }
}
