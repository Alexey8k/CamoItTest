using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using MappingConfigSection;

namespace DataMapping
{
    public class ParameterManager
    {
        public ParameterManager()
        {
            ReadFromConfig();
        }

        private readonly List<Parameter> _parameters = new List<Parameter>();

        internal Parameter this[string name] => _parameters.Find(p => p.Name == name);

        public string Default { get; private set; }

        public string Ignore { get; private set; }

        public IEnumerable<string> UniqueConstraint { get; private set; }

        public bool IsDefault(string name) => Default == name;

        public bool IsIgnore(string name) => Ignore == name;

        public void Update()
        {
            ReadFromConfig();
        }

        public IEnumerable<string> GetNames()
        {
            var names = _parameters.Select(p => p.Name).ToList();
            names.Insert(0, Default);
            names.Insert(names.Count, Ignore);
            return names;
        }

        public string[] ValidationParameters(string[] parameters)
        {
            List<string> errors = new List<string>();
            var isAnyDefault = false;
            foreach (var required in _parameters.Where(p => p.IsRequired).Select(p => p.Name))
            {
                if (!parameters.Contains(required))
                    errors.Add($"Обязательный параметер \"{required}\" не был установлен.");
            }
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = this[parameters[i]];
                isAnyDefault = parameters[i] == Default;
                if (!isAnyDefault && (!parameter?.IsRepeat ?? false) && parameters.Count(p => p == parameters[i]) > 1)
                {
                    errors.Add($"Параметер \"{parameters[i]}\" должен быть установлен в единственном числе.");
                    for (var j = i; j < parameters.Length; j++)
                    {
                        if (parameters[j] == parameter.Name)
                            parameters[j] = string.Empty;
                    }
                }
            }
            if (isAnyDefault)
                errors.Add($"Не все параметры были назначены, есть \"{Default}\".");
            
            return errors.ToArray();
        }

        private void ReadFromConfig()
        {
            Default = ((MappingSettingSection)ConfigurationManager.GetSection("mappingSetting")).ParameterDefault.Name;
            Ignore = ((MappingSettingSection)ConfigurationManager.GetSection("mappingSetting")).ParameterIgnore.Name;
            var uniqueParameters = ((MappingSettingSection)ConfigurationManager.GetSection("mappingSetting")).UniqueConstraint;
            var uniqueConstraint = new List<string>();
            foreach (ParameterElement up in uniqueParameters)
            {
                uniqueConstraint.Add(up.Name);
            }
            UniqueConstraint = uniqueConstraint;
            var parameters = ((MappingSettingSection)ConfigurationManager.GetSection("mappingSetting")).ParameterCollection;
            foreach (ParameterElement p in parameters)
            {
                _parameters.Add(new Parameter(
                    p.Name,
                    p.IsRepeat,
                    p.IsRequired,
                    (DataType)p.DataType,
                    p.IsAllowEmtyValues));
            }
        }
    }
}