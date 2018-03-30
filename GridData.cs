
using System;

namespace USheet
{
    [Serializable]
    public class IGridData
    {
        public Type dataType;

        public override string ToString() { return ""; }
    }

    [Serializable]
    public class GridData<T> : IGridData
    {
        public T data;
        public GridData(T value)
        {
            data = value;
            dataType = typeof(T);
        }

        public override string ToString()
        {
            object objData = data as object;
            return string.Format("{0}|{1}", (objData??"null").ToString(), dataType.ToString());
        }
    }
}