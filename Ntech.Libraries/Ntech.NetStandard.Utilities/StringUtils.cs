using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;

namespace Ntech.NetStandard.Utilities
{
    /// <summary>
    /// Provide utilities on string processing and dump data structure to string
    /// </summary>
    public static class StringUtils
    {
        public static string GetFriendlyTypeName(Type type)
        {
            var strTypeName = string.Empty;
            if (type == null)
            {
                strTypeName = "Void";
            }
            else
            {
                strTypeName = type.Name;
            }

            return strTypeName;
        }

        public static object GetFriendlyObjectValue(object valueObj)
        {
            if (valueObj == null)
            {
                return valueObj;
            }

            if (valueObj is DateTime)
            {
                return ((DateTime)valueObj).ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            }

            var objType = valueObj.GetType();
            if (objType.IsPrimitive)
            {
                return valueObj;
            }

            var enumerable = valueObj as IEnumerable<object>;
            if (enumerable != null)
            {
                return string.Join(string.Empty, objType.Name, "[Count: ", enumerable.Count(), "]");
            }

            var collection = valueObj as System.Collections.ICollection;
            if (collection != null)
            {
                return string.Join(string.Empty, objType.Name, "[Count: ", collection.Count, "]");
            }

            return valueObj;
        }

        public static string DumpToString<T>(T value, string name)
        {
            using (var writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                var idGenerator = new ObjectIDGenerator();
                InternalDump(0, name, value, writer, idGenerator, true);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Internals the dump.
        /// </summary>
        /// <param name="indentationLevel">The indentation level.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="writer">The writer.</param>
        /// <param name="idGenerator">The identifier generator.</param>
        /// <param name="recursiveDump">if set to <c>true</c> [recursive dump].</param>
        private static void InternalDump(int indentationLevel, string name, object value,
            TextWriter writer, ObjectIDGenerator idGenerator, bool recursiveDump)
        {
            var indentation = new string(' ', indentationLevel * 3);

            if (value == null)
            {
                writer.WriteLine("{0}{1} = <null>", indentation, name);
                return;
            }

            Type type = value.GetType();

            // figure out if this is an object that has already been dumped, or is currently being dumped
            string keyRef = string.Empty;
            string keyPrefix = string.Empty;
            if (!type.IsValueType)
            {
                bool firstTime;
                long key = idGenerator.GetId(value, out firstTime);
                if (!firstTime)
                {
                    keyRef = string.Format(CultureInfo.InvariantCulture, " (see #{0})", key);
                }
                else
                {
                    keyPrefix = string.Format(CultureInfo.InvariantCulture, "#{0}: ", key);
                }
            }

            // work out how a simple dump of the value should be done
            bool isString = value is string;
            string typeName = value.GetType().FullName;
            string formattedValue = value.ToString();

            var exception = value as Exception;
            if (exception != null)
            {
                // formattedValue = exception.GetType().Name + ": " + exception.Message;
                formattedValue = string.Concat(exception.GetType().Name, ": ", exception.Message);
            }

            if (formattedValue == typeName)
            {
                formattedValue = string.Empty;
            }
            else
            {
                // escape tabs and line feeds
                formattedValue = formattedValue.Replace("\t", "\\t").Replace("\n", "\\n").Replace("\r", "\\r");

                // chop at 80 characters
                int length = formattedValue.Length;
                if (length > 80)
                {
                    formattedValue = formattedValue.Substring(0, 80);
                }

                if (isString)
                {
                    formattedValue = string.Format(CultureInfo.InvariantCulture, "\"{0}\"", formattedValue);
                }

                if (length > 80)
                {
                    // formattedValue += " (+" + (length - 80) + " chars)";
                    formattedValue = string.Concat(" (+", (length - 80), " chars)");
                }

                // formattedValue = " = " + formattedValue;
                formattedValue = string.Concat(" = ", formattedValue);
            }

            writer.WriteLine("{0}{1}{2}{3} [{4}]{5}", indentation, keyPrefix, name, formattedValue, value.GetType(), keyRef);

            // Avoid dumping objects we've already dumped, or is already in the process of dumping
            if (keyRef.Length > 0)
            {
                return;
            }

            // don't dump strings, we already got at around 80 characters of those dumped
            if (isString)
            {
                return;
            }

            // don't dump value-types in the System namespace
            if (type.IsValueType && type.FullName == "System." + type.Name)
            {
                return;
            }

            // Avoid certain types that will result in endless recursion
            if (type.FullName == "System.Reflection." + type.Name)
            {
                return;
            }

            if (value is System.Security.Principal.SecurityIdentifier)
            {
                return;
            }

            if (!recursiveDump)
            {
                return;
            }

            PropertyInfo[] properties =
                (from property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                 where property.GetIndexParameters().Length == 0
                       && property.CanRead
                 select property).ToArray();
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).ToArray();

            if (properties.Length == 0 && fields.Length == 0)
            {
                return;
            }

            writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}{{", indentation));
            if (properties.Length > 0)
            {
                writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}   properties {{", indentation));
                foreach (PropertyInfo pi in properties)
                {
                    try
                    {
                        object propertyValue = pi.GetValue(value, null);
                        InternalDump(indentationLevel + 2, pi.Name, propertyValue, writer, idGenerator, true);
                    }
                    catch (TargetInvocationException ex)
                    {
                        InternalDump(indentationLevel + 2, pi.Name, ex, writer, idGenerator, false);
                    }
                    catch (ArgumentException ex)
                    {
                        InternalDump(indentationLevel + 2, pi.Name, ex, writer, idGenerator, false);
                    }
                    catch (Exception)
                    {
                        writer.WriteLine("{0}:{1}", pi.Name, "UnknowProperty");
                    }
                }

                writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}   }}", indentation));
            }
            if (fields.Length > 0)
            {
                writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}   fields {{", indentation));
                foreach (FieldInfo field in fields)
                {
                    try
                    {
                        object fieldValue = field.GetValue(value);
                        InternalDump(indentationLevel + 2, field.Name, fieldValue, writer, idGenerator, true);
                    }
                    catch (TargetInvocationException ex)
                    {
                        InternalDump(indentationLevel + 2, field.Name, ex, writer, idGenerator, false);
                    }
                    catch (Exception)
                    {
                        writer.WriteLine("{0}:{1}", field.Name, "UnknowValue");
                    }
                }
                writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}   }}", indentation));
            }
            writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}}}", indentation));
        }

        public static string TypesToString(this ICollection<Type> types)
        {
            var typeNames = Array.ConvertAll(types.ToArray(), t => t.FullName);
            return string.Format("{0}: [{1}]", types.GetType().Name, string.Join(",", typeNames));
        }

        public static string AssembliesToString(this ICollection<Assembly> assemblies)
        {
            var assNames = Array.ConvertAll(assemblies.ToArray(), ass => ass.GetName().Name);
            return string.Format("{0}: [{1}]", assemblies.GetType().Name, string.Join(",", assNames));
        }

        public static SecureString ToSecureString(string value)
        {
            var result = new SecureString();
            var chars = value.ToCharArray();

            foreach (var c in chars)
            {
                result.AppendChar(c);
            }

            return result;
        }
    }
}
