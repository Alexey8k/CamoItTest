using System;
using System.Collections.Generic;
using System.Linq;

namespace DataMapping
{
    public class Parameter
    {
        public Parameter(string name, bool isRepeat, bool isRequired, DataType type, bool isAllowEmtyValues)
        {
            Name = name;
            IsRepeat = isRepeat;
            IsRequired = isRequired;
            Type = type;
            IsAllowEmtyValues = isAllowEmtyValues;
        }
        public string Name { get; }

        public bool IsRepeat { get; }

        public bool IsRequired { get; }

        public DataType Type { get; }

        public bool IsAllowEmtyValues { get; }
    }
}