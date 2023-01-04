using Newtonsoft.Json;
using System;

namespace Core.Models
{
    public class Filter
    {
        public string Key { get; set; }
        public object Value { get; set; }
        public FilterType FilterType { get; set; }
        public bool IsDateTime { get; private set; }

        public Filter(string key, object value)
        {
            Key = key;
            Value = value;
            IsDateTime = value.GetType() == typeof(DateTime) || value.GetType() == typeof(DateTime?);
            FilterType = FilterType.Equal;
        }
        [JsonConstructor]
        public Filter(string key, FilterType filterType, object value, bool? isDatetime = null) : this(key, value)
        {
            FilterType = filterType;
            if (isDatetime != null) 
                IsDateTime = (bool)isDatetime;
        }
        public Filter(string key, string value)
        {
            Key = key;
            Value = value;
            FilterType = FilterType.Equal;
        }
        public Filter(string key, DateTime dateTime)
        {
            Key = key;
            Value = dateTime;
            IsDateTime = true;
            FilterType = FilterType.Equal;
        }
        public static Filter ByUserId(string userId)
        {
            return new Filter("UserId", FilterType.Equal, userId);
        }
        public static Filter ById(int id)
        {
            return new Filter("Id", FilterType.Equal, id.ToString());
        }
        public static Filter NotNull(string key)
        {
            return new Filter(key, FilterType.NotNull, "_");
        }
        public static Filter Null(string key)
        {
            return new Filter(key, FilterType.Null, "_");
        }
    }

    public enum FilterType
    {
        Contains,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,
        Equal,
        Null,
        NotNull
    }


}
