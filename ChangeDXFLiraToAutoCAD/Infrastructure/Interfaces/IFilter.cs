using Autodesk.AutoCAD.EditorInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
