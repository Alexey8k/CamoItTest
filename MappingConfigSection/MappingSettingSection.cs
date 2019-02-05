using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MappingConfigSection
{
    public class MappingSettingSection : ConfigurationSection
    {
        [ConfigurationProperty("parameterCollection")]
        public ParameterElementCollection ParameterCollection => (ParameterElementCollection) base["parameterCollection"];

        [ConfigurationProperty("parameterDefault")]
        public ParameterDefaultElement ParameterDefault => (ParameterDefaultElement)base["parameterDefault"];

        [ConfigurationProperty("parameterIgnore")]
        public ParameterIgnoreElement ParameterIgnore => (ParameterIgnoreElement)base["parameterIgnore"];

        [ConfigurationProperty("uniqueConstraint")]
        public ParameterElementCollection UniqueConstraint => (ParameterElementCollection)base["uniqueConstraint"];
    }
}
