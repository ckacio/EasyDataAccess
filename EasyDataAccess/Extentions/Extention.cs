using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace DataAccess.Extentions
{
    public static class Extention
    {
        public static DataTable ToDataTable<T>(this IList<T> list)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            DataTable dt = new DataTable();
            for(int i = 0; i < props.Count; i++) 
            { 
                PropertyDescriptor prop = props[i];
                dt.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach(T item in list) 
            { 
                for(int i = 0; i < values.Length; i++) 
                {
                    values[i] = props[i].GetValue(item);
                }
                dt.Rows.Add(values);
            }
            return dt;
        }

        public static decimal ToDecimal(this object v)=> Convert.ToDecimal(v);

        public static DateTime ToDatetime(this object v) => Convert.ToDateTime(v);

        public static Double ToDouble(this object v) => Convert.ToDouble(v);

        public static bool ToBoolean(this object v) => Convert.ToBoolean(v);

        public static int ToInt(this object v) => Convert.ToInt32(v);

        public static int ToChar(this object v) => Convert.ToChar(v);
    }
}
