using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MySerialization
{
    public class SerializatorToCSV<T> where T : class, new()
    {
        private List<PropertyInfo> Properties;
        private char _separator = ';';
        private char _replacement = ' ';
        private string Filename { get; set; }
        public char Separator
        { 
            get { return  _separator; }
            set { _separator = value; }
        }
        public char Replacement
        {
            get { return _replacement; }
            set { _replacement = value; }
        }

        public SerializatorToCSV(String filename)
        {
            Filename = filename;
            var properties = typeof(T).GetProperties( BindingFlags.Public | BindingFlags.Instance |
                                                      BindingFlags.GetProperty | BindingFlags.SetProperty);
            var pp = properties.AsQueryable()
                    .Where(p => p.PropertyType.IsValueType || p.PropertyType.Name == "String");

            var pl = from a in pp
                     orderby a.Name
                     select a;

            Properties = pl.ToList();
        }

        public void Serialize(IList<T> data)
        {
            var values = new List<String>();
            var header = string.Join(Separator.ToString(), Properties.Select(s => s.Name).ToList());

            var str = new StringBuilder();
            str = str.AppendLine(header);

            foreach (var d in data)
            {
                values.Clear();

                foreach (var p in Properties)
                {
                    var raw = p.GetValue(d);
                    values.Add(raw == null ? "" : raw.ToString().Replace(Separator, Replacement));
                }

                str.AppendLine(string.Join(Separator.ToString(), values.ToArray()));
            }

            using (var stream = new FileStream(Filename, FileMode.Create, FileAccess.Write))
            using (var s = new StreamWriter(stream))
            {
                s.Write(str.ToString().Trim());
            }
        }

        public IList<T> Deserialize()
        {
            string[] columns = new string[0];
            string[] rows = new string[0];

            try
            {
                using (var stream = new FileStream(Filename, FileMode.Open, FileAccess.Read))
                using (var sr = new StreamReader(stream))
                {
                    columns = sr.ReadLine().Split(Separator);
                    rows = sr.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"The file is invalid. {ex.Message}");
            }

            var data = new List<T>();

            for (int row = 0; row < rows.Length; row++)
            {
                var line = rows[row];
                var parts = line.Split(Separator);

                var T_Object = new T();

                for (int i = 0; i < parts.Length; i++)
                {
                    var column = columns[i];
                    var value = parts[i];

                    var p = Properties.FirstOrDefault(x => x.Name.Equals(column));

                    var converter = TypeDescriptor.GetConverter(p.PropertyType);
                    var convertedvalue = converter.ConvertFrom(value);

                    p.SetValue(T_Object, convertedvalue);
                }
                data.Add(T_Object);
            }
            return data;
        }
    }
}
