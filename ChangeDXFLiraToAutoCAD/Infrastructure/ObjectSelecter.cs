using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using ChangeDXFLiraToAutoCAD.Infrastructure.Interfaces;
using System;


namespace ChangeDXFLiraToAutoCAD.Infrastructure
{
    internal class ObjectSelecter : IObjectSelecter
    {
        Editor editor;
        IFilterCreator filterCreator;
        public ObjectSelecter(Editor editor)
        {
            this.editor = editor;
            filterCreator = new FilterCreator();
        }
        public ObjectId[] SelectObjectIds(int dxfCodeOfParameter, string parameterName)
        {
            // Получаем фильтр
            SelectionFilter filter = filterCreator.GetFilterByParameters(dxfCodeOfParameter, parameterName);
            // пытаемся получить ссылки на объекты с учетом фильтра
            PromptSelectionResult selRes = editor.SelectAll(filter);

            // если произошла ошибка - сообщаем о ней
            if (selRes.Status != PromptStatus.OK)
            {
                throw new Exception("\nError!\n");
            }
            // Возвращаем массив ID объектов
            return selRes.Value.GetObjectIds();
        }
    }
}
