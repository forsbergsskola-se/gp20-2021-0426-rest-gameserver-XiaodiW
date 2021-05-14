using System.Collections.Generic;
using System.Linq;

namespace MMORPG.APIs {

    public static class ComApi {
        public static List<T> RestrictedData<T>(List<T> dataArray, string[] properties) where T : new() {
            var result = new List<T>();
            var allFields = dataArray[^1].GetType().GetProperties();
            foreach(var data in dataArray) {
                var subResult = new T();
                foreach(var field in allFields) {
                    var value = data.GetType().GetProperty(field.Name)?.GetValue(data);
                    if(properties.Contains(field.Name))
                        subResult?.GetType().GetProperty(field.Name)?.SetValue(subResult, value, null);
                }
                result.Add(subResult);
            }
            return result;
        }
    }

}