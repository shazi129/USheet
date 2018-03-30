
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace USheet
{
    public enum E_DATA_TYPE
    {
        INT,
        STRING,
        SPRITE,
    }

    #region column data class
    [Serializable]
    public class IntColumnData : ColumnData<int>
    {
        public IntColumnData(int count) : base(count)
        {
        }
    }

    [Serializable]
    public class StringColumnData : ColumnData<string>
    {
        public StringColumnData(int count) : base(count)
        {
        }
    }

    [Serializable]
    public class SpriteColumnData : ColumnData<Sprite>
    {
        public SpriteColumnData(int count) : base(count)
        {
        }
    }
    #endregion

    #region column data entry

    [Serializable]
    public class ColumnDataEntry
    {
        //数据区
        public List<IntColumnData> intColumns = new List<IntColumnData>();
        public List<StringColumnData> stringColumns = new List<StringColumnData>();
        public List<SpriteColumnData> spriteColumns = new List<SpriteColumnData>();

        //索引
        public Dictionary<E_DATA_TYPE, IList> typeEntryMap = new Dictionary<E_DATA_TYPE, IList>();

        public void reloadDataMap()
        {
            typeEntryMap.Clear();
            typeEntryMap.Add(E_DATA_TYPE.INT, intColumns);
            typeEntryMap.Add(E_DATA_TYPE.STRING, stringColumns);
            typeEntryMap.Add(E_DATA_TYPE.SPRITE, spriteColumns);
        }
    }
    #endregion

    public class SheetDataSet
    {
        public E_DATA_TYPE eType;
        public Type dataType;
        public Type columnType;

        public SheetDataSet(E_DATA_TYPE eType, Type dataType, Type columnType)
        {
            this.eType = eType;
            this.dataType = dataType;
            this.columnType = columnType;
        }
    }

    public class DataTypeManager
    {

        private static DataTypeManager _instance = null;
        private List<SheetDataSet> _sheetDataSet = new List<SheetDataSet>();

        public static DataTypeManager instance
        {
            get
            {
                if (_instance == null) _instance = new DataTypeManager();
                return _instance;
            }
        }

        public List<SheetDataSet> sheetDataSet
        {
            get { return _sheetDataSet; }
        }

        private DataTypeManager()
        {
            sheetDataSet.Clear();
            sheetDataSet.Add(new SheetDataSet(E_DATA_TYPE.INT, typeof(int), typeof(IntColumnData)));
            sheetDataSet.Add(new SheetDataSet(E_DATA_TYPE.STRING, typeof(string), typeof(StringColumnData)));
            sheetDataSet.Add(new SheetDataSet(E_DATA_TYPE.SPRITE, typeof(Sprite), typeof(SpriteColumnData)));
        }

        public Type getColumnType(E_DATA_TYPE dataType)
        {
            for (int i = 0; i < sheetDataSet.Count; i++)
            {
                if (sheetDataSet[i].eType == dataType)
                {
                    return sheetDataSet[i].columnType;
                }
            }
            return null;
        }
    }
}
