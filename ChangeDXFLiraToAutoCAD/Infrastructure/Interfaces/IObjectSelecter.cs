using Autodesk.AutoCAD.DatabaseServices;

namespace ChangeDXFLiraToAutoCAD.Infrastructure.Interfaces
{
    internal interface IObjectSelecter
    {
        /// <summary>
        /// Возвращает список объектов соответсвующих переданным параметрам
        /// </summary>
        ObjectId[] SelectObjectIds(int dxfCodeOfParameter, string parameterName);
    }
}
