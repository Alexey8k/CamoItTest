using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MappingConfigSection
{
    internal class IntConfigConverter : ConfigurationConverterBase
    {
        public override object ConvertFrom(ITypeDescriptorContext ctx, CultureInfo ci, object data)
        {
            var str = ((string)data)?.ToLower() ?? "none";
            switch (str)
            {
                case "text": return 1;
                case "decimal": return 2;
                default: return 0;
            }
        }
    }
}
