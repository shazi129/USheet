
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace USheet
{
    public class SheetData : ScriptableObject
    {
        #region Serialized Data
        //ordered titles
        [SerializeField]
        private List<string> _titles = new List<string>(); //用来确定列的顺序

        //sheet data
        [SerializeField]
        private ColumnDataEntry _columnData = new ColumnDataEntry();
        #endregion


        //索引和列顺序， 只有在loadData之后才可以使用
        private Dictionary<string, IColumnData> _table = new Dictionary<string, IColumnData>();

        public SheetData()
        {
            _columnData.reloadDataMap();
        }

        public void OnEnable()
        {
            Debug.Log("SheetData enable");
            loadData();
        }

        public string getTitle(int index)
        {
            if (index >= 0 && index < _titles.Count)
            {
                return _titles[index];
            }
            else
            {
                Debug.LogError(string.Format("SheetData get title error: invalid index {0}", index));
                return "";
            } 
        }

        //将序列化的数据载入索引
        public void loadData()
        {
            _table.Clear();

            foreach (var item in _columnData.typeEntryMap)
            {
                for (int columnIndex = 0; columnIndex < item.Value.Count; columnIndex++)
                {
                    IColumnData columnData = item.Value[columnIndex] as IColumnData;
                    if (_table.ContainsKey(columnData.title))
                    {
                        Debug.LogError("LoadData error: find duplicate title:" + columnData.title);
                    }
                    else
                    {
                        _table.Add(columnData.title, columnData);
                    }
                }
            }
        }

        public int columnCount { get { return _titles.Count; } }

        //get rows of this sheet
        public int rowCount
        {
            get
            {
                foreach (var item in _table)
                {
                    return item.Value.size();
                }
                return 0;
            }
        }

        /// <summary>
        /// add a new column
        /// </summary>
        public void insertColumn(string name, E_DATA_TYPE dataType, int index = -1)
        {
            if (String.IsNullOrEmpty(name))
            {
                Debug.LogError("Invalid title name!");
                return;
            }

            if (_table.ContainsKey(name))
            {
                Debug.LogError("Add Column Error: title name exists");
                return;
            }

            //反射创建一个IColumnData
            Type columnType = DataTypeManager.instance.getColumnType(dataType);
            if (columnType != null)
            {
                IColumnData newColumn = (IColumnData)Activator.CreateInstance(columnType, new object[] { rowCount });
                newColumn.title = name;

                _table.Add(name, newColumn);
                _columnData.typeEntryMap[dataType].Add(newColumn);

                //插入title
                if (index >= 0 && index < _titles.Count)
                    _titles.Insert(index, name);
                else
                    _titles.Add(name);
            }
        }


        public void insert(int index = -1)
        {
            List<object> values = new List<object>();
            for (int i = 0; i < _titles.Count; i++)
            {
                values.Add(null);
            }
            insert(_titles, values, index);
        }

        public void insert(List<String> titles, List<object> values, int index = -1)
        {
            if (titles.Count != values.Count)
            {
                Debug.LogError("SheetData insert error, param error");
                return;
            }
            foreach (var item in _table)
            {
                if (titles.Contains(item.Key))
                {
                    object value = values[titles.IndexOf(item.Key)];
                    if (value == null)
                    {
                        item.Value.insert(index, null);
                    }
                    else
                    {
                        Type gridClassType = typeof(GridData<>);
                        gridClassType = gridClassType.MakeGenericType(value.GetType());
                        IGridData gridData = (IGridData)Activator.CreateInstance(gridClassType, new object[] { value });

                        item.Value.insert(index, gridData);
                    }
                }
                else
                {
                    item.Value.insert(index, null);
                }
            }
        }

        public void deleteRow(int index = -1)
        {
            foreach (var item in _table)
            {
                item.Value.deleteRow(index);
            }
        }

        public void modify(string title, int rowIndex, IGridData iData)
        {
            if (_table.ContainsKey(title))
            {
                IColumnData columnData = _table[title];
                columnData.modify(rowIndex, iData);
            }
            else
            {
                Debug.LogError("Modify SheetData error: no that title:" + title);
            }
        }

        public void modifyColumnName(string oldName, string newName)
        {
            int index = _titles.IndexOf(oldName);
            if (index >= 0 && _table.ContainsKey(oldName) && !_titles.Contains(newName) && !_table.ContainsKey(newName))
            {
                _titles[index] = newName;
                _table[oldName].title = newName;
            }
        }

        public void deleteColumn(string columnName)
        {
            //删除表头
            _titles.Remove(columnName);

            //删除索引
            _table.Remove(columnName);

            //删除数据
            foreach (var item in _columnData.typeEntryMap)
            {
                bool hasDeleted = false;

                for (int i = 0; i < item.Value.Count; i++)
                {
                    IColumnData colomndata = item.Value[i] as IColumnData;
                    if (colomndata != null)
                    {
                        if (colomndata.title == columnName)
                        {
                            hasDeleted = true;
                            item.Value.RemoveAt(i);
                            break;
                        }
                    }
                }
                if (hasDeleted)
                {
                    break;
                }
            }
        }

        public Dictionary<string, IGridData> getRow(int index)
        {
            if (index >= rowCount || index < 0)
            {
                return null;
            }
            Dictionary<string, IGridData> result = new Dictionary<string, IGridData>();
            foreach (var item in _table)
            {
                result.Add(item.Key, item.Value.getValue(index));
            }
            return result;
        }

        public List<Dictionary<string, IGridData>> getRows<T>(string keyName, T keyValue)
        {
            if (!_table.ContainsKey(keyName)) return null;

            ColumnData<T> columnData = _table[keyName] as ColumnData<T>;

            if (columnData == null)
            {
                Debug.LogError("SheetData get rows error: type error:" + typeof(T).ToString());
                return null;
            }

            List < Dictionary < string, IGridData >> result = new List<Dictionary<string, IGridData>>();

            int startIndex = 0;
            while (startIndex >= 0)
            {
                startIndex = columnData.indexOf(keyValue, startIndex);
                if (startIndex >= 0)
                {
                    result.Add(getRow(startIndex));
                    startIndex++;
                }
            }

            if (result.Count > 0)
            {
                return result;
            }
            return null;
        }

        public IGridData getValue<T>(string keyName, T keyValue, string title, int index = 0)
        {
            if (!_table.ContainsKey(keyName) || !_table.ContainsKey(title))
            {
                Debug.LogError(string.Format("SheetData getValue error: title[{0} or {1}] does not exist", keyName, title));
                return null;
            }
            ColumnData<T> columnData = _table[keyName] as ColumnData<T>;
            if (columnData == null)
            {
                Debug.LogError(string.Format("SheetData getValue error: column data type error: {0} does not match the type of column {1}", typeof(T).ToString(), keyName));
                return null;
            }
            int firstIndex = columnData.indexOf(keyValue, index);
            return _table[title].getValue(firstIndex);
        }
    }
}
