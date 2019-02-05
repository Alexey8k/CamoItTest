using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MappingConfigSection
{
    public class ParameterElement : ConfigurationElement
    {

        [ConfigurationProperty("name", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string Name
        {
            get => (string)base["name"];
            set => base["name"] = value;
        }

        [ConfigurationProperty("isRepeat", DefaultValue = true, IsKey = false, IsRequired = false)]
        public bool IsRepeat
        {
            get => (bool)base["isRepeat"];
            set => base["isRepeat"] = value;
        }

        [ConfigurationProperty("isRequired", DefaultValue = false, IsKey = false, IsRequired = false)]
        public bool IsRequired
        {
            get => (bool)base["isRequired"];
            set => base["isRequired"] = value;
        }

        [TypeConverter(typeof(IntConfigConverter))]
        [ConfigurationProperty("dataType", DefaultValue = 0, IsKey = false, IsRequired = false)]
        public int DataType
        {
            get => (int)base["dataType"];
            set => base["dataType"] = value;
        }

        [ConfigurationProperty("isAllowEmtyValues", DefaultValue = true, IsKey = false, IsRequired = false)]
        public bool IsAllowEmtyValues
        {
            get => (bool)base["isAllowEmtyValues"];
            set => base["isAllowEmtyValues"] = value;
        }

        //[ConfigurationProperty("isDefault", DefaultValue = false, IsKey = false, IsRequired = false)]
        //public bool IsDefault
        //{
        //    get => (bool)base["isDefault"];
        //    set => base["isDefault"] = value;
        //}

        //[ConfigurationProperty("isIgnore", DefaultValue = false, IsKey = false, IsRequired = false)]
        //public bool IsIgnore
        //{
        //    get => (bool)base["isIgnore"];
        //    set => base["isIgnore"] = value;
        //}
    }
}
