using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using ChangeDXFLiraToAutoCAD.Infrastructure.Interfaces;


namespace ChangeDXFLiraToAutoCAD.Infrastructure
{
    internal class FilterCreator : IFilterCreator
    {
        TypedValue[] filterList;
        public FilterCreator()
        {
            filterList = new TypedValue[1];
        }
        public SelectionFilter GetFilterByParameters(int dxfCodeOfParameter, string parameterName)
        {
            filterList[0] = new TypedValue(dxfCodeOfParameter, parameterName);
            // создаем фильтр
            return new SelectionFilter(filterList);
        }
    }
}
