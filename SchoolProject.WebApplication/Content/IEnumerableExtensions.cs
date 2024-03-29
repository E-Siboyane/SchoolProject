﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCapital.RF.DataAccess {
   public static class IEnumerableExtensions {
      public static DataTable AsDataTable<T>(this IEnumerable<T> data) {
         PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
         var table = new DataTable();


         foreach(PropertyDescriptor prop in properties) {
            if(string.Compare(prop.Name, "Id", true) != 0) {
               table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
         }
         foreach(T item in data) {
            DataRow row = table.NewRow();
            foreach(PropertyDescriptor prop in properties) {
               if(string.Compare(prop.Name, "Id", true) != 0) {
                  row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
               }
            }
            table.Rows.Add(row);
         }
         return table;
      }

      public static DataTable AsDataTableWithId<T>(this IEnumerable<T> data) {
         PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
         var table = new DataTable();

         foreach(PropertyDescriptor prop in properties) {
            table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
         }
         foreach(T item in data) {
            DataRow row = table.NewRow();
            foreach(PropertyDescriptor prop in properties) {
               row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
            }
            table.Rows.Add(row);
         }
         return table;
      }
   }
}
