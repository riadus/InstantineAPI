using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace InstantineAPI.Formatter
{
    public class CsvInputFormatter : InputFormatter
    {
        private readonly CsvFormatterOptions _options;

        public CsvInputFormatter(CsvFormatterOptions csvFormatterOptions)
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/csv"));
            _options = csvFormatterOptions ?? throw new ArgumentNullException(nameof(csvFormatterOptions));
        }

        public override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            var type = context.ModelType;
            var request = context.HttpContext.Request;
            MediaTypeHeaderValue requestContentType = null;
            MediaTypeHeaderValue.TryParse(request.ContentType, out requestContentType);


            var result = ReadStream(type, request.Body);
            return InputFormatterResult.SuccessAsync(result);
        }

        public override bool CanRead(InputFormatterContext context)
        {
            var type = context.ModelType;
            if (type == null)
                throw new ArgumentNullException(nameof(context.ModelType));

            return IsTypeOfIEnumerable(type);
        }

        private bool IsTypeOfIEnumerable(Type type)
        {

            foreach (Type interfaceType in type.GetInterfaces())
            {

                if (interfaceType == typeof(IList))
                    return true;
            }

            return false;
        }

        private object ReadStream(Type type, Stream stream)
        {
            Type itemType;
            var typeIsArray = false;
            IList list;
            if (type.GetGenericArguments().Length > 0)
            {
                itemType = type.GetGenericArguments()[0];
                list = (IList)Activator.CreateInstance(type);
            }
            else
            {
                typeIsArray = true;
                itemType = type.GetElementType();

                var listType = typeof(List<>);
                var constructedListType = listType.MakeGenericType(itemType);

                list = (IList)Activator.CreateInstance(constructedListType);
            }

            var reader = new StreamReader(stream, Encoding.GetEncoding(_options.Encoding));

            bool skipFirstLine = _options.UseSingleLineHeaderInCsv;
            var lineProcessed = 1;
            var numberOfColumns = 0;
            List<string> mapping = new List<string>();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.StartsWith(_options.EscapeLineCharacters, StringComparison.InvariantCultureIgnoreCase))
                    continue;
                line = line.Replace("\\n", "\n");
                var values = Regex.Split(line, @"(?<!\" + _options.EscapeChar + ")" + _options.CsvDelimiter);
                if (skipFirstLine)
                {
                    skipFirstLine = false;
                    numberOfColumns = values.Length;
                    mapping = values.ToList();
                }
                else
                {
                    var itemTypeInGeneric = list.GetType().GetTypeInfo().GenericTypeArguments[0];
                    var item = Activator.CreateInstance(itemTypeInGeneric);
                    var properties = item.GetType().GetProperties();
                    for (int i = 0; i < values.Length && i < numberOfColumns; i++)
                    {
                        try
                        {
                            var value = values[i];
                            value = value.Replace(_options.EscapeChar + _options.CsvDelimiter, _options.CsvDelimiter);
                            var property = properties.FirstOrDefault(x => x.Name.ToLower() == mapping[i].ToLower());
                            property.SetValue(item, Convert.ChangeType(value, property.PropertyType), null);
                        }
                        catch (Exception exception)
                        {
                            throw new Exception($"An error happened because of the line #{lineProcessed}", exception);
                        }
                    }

                    list.Add(item);
                }
                lineProcessed++;
            }

            if (typeIsArray)
            {
                Array array = Array.CreateInstance(itemType, list.Count);

                for (int t = 0; t < list.Count; t++)
                {
                    array.SetValue(list[t], t);
                }
                return array;
            }

            return list;
        }
    }
}
