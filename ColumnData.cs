
using System;
using System.Collections.Generic;
using UnityEngine;

namespace USheet
{

    public class IColumnData
    {
        public string title;

        public virtual bool isOutRange(int index) { return false; }
        public virtual int size() { return 0; }
        public virtual void insert(int index, IGridData gridData) { }
        public virtual void deleteRow(int index) { }
        public virtual void modify(int index, IGridData gridData) { }
        public virtual IGridData getValue(int index) { return null; }
    }

    [Serializable]
    public class ColumnData<T> : IColumnData
    {
        [SerializeField]
        private List<T> _data;

        public T defaultValue { get; set; }

        public ColumnData(int count)
        {
            _data = new List<T>();
            for (int i = 0; i < count; i++)
            {
                _data.Add(defaultValue);
            }
        }

        public override bool isOutRange(int index)
        {
            if (_data != null)
                return index < 0 || index >= _data.Count;
            else
                return true;
        }

        public override int size()
        {
            if (_data == null)
                return 0;
            else
                return _data.Count;
        }

        public override void insert(int index, IGridData gridData)
        {
            T value = defaultValue;
            if (gridData != null)
            {
                if (gridData.dataType != typeof(T))
                {
                    Debug.LogError(string.Format("Column insert error, try to insert{0} to {1}", gridData.dataType, typeof(T)));
                    return;
                }

                value = (gridData as GridData<T>).data;
            }

            if (_data == null)
                _data = new List<T>();

            if (isOutRange(index))
                _data.Add(value);
            else
                _data.Insert(index, value);
        }

        public override void deleteRow(int index)
        {
            if (index < 0 || index >= _data.Count)
                index = _data.Count - 1;
            _data.RemoveAt(index);
        }

        public override void modify(int index, IGridData gridData)
        {
            T value = defaultValue;
            if (gridData != null)
            {
                if (gridData.dataType != typeof(T))
                {
                    Debug.LogError(string.Format("Column modify error, try to use {1} to modify {0}", gridData.dataType, typeof(T)));
                    return;
                }

                value = (gridData as GridData<T>).data;
            }
            else
            {
                Debug.LogError("modify error, gridData is null");
            }

            if (!isOutRange(index))
                _data[index] = value;
            else
                Debug.LogError(string.Format("modify error, index{0} out of range", index));
        }

        public T at(int index)
        {
            if (!isOutRange(index))
                return _data[index];
            else
                Debug.LogError(string.Format("get value error, index{0} out of range", index));

            return defaultValue;
        }

        public override IGridData getValue(int index)
        {
            T value = at(index);

            return new GridData<T>(value);
        }

        public int indexOf(T value, int startIndex)
        {
            return _data.IndexOf(value, startIndex);
        }
    }
}

