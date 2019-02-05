using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MappingConfigSection
{
    public class UniqueConstraintElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new UniqueParameterElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((UniqueParameterElement)element).Name;
        }

        public UniqueParameterElement this[int idx]
        {
            get { return (UniqueParameterElement)BaseGet(idx); }
        }
    }
}
