using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yesolm.DevOps.Utils
{
    /// <summary>
    /// Some random extension methods
    /// </summary>
    public static class MiscExtensions
    {
        /// <summary>
        /// Checks if the enumerable is null or has no items.
        /// </summary>
        /// <typeparam name="T"><see cref="Type"/> of the enumerable</typeparam>
        /// <param name="input">Value</param>
        /// <returns></returns>
        public static bool IsEmpty<T>(this IEnumerable<T> input) => input == null || !input.Any()
            || !input.GetEnumerator().MoveNext();
        public static int CountSafely<T>(this IEnumerable<T> input) => input.IsEmpty() ? 0 : input.Count();

        /// <summary>
        /// Checks if <paramref name="parentObject"/> in <paramref name="parentObject"/> 
        /// is null or has no items.
        /// </summary>
        /// <param name="parentObject"><see cref="object"/></param>
        /// <param name="property">The property to check.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsEmpty(this object parentObject, System.Reflection.PropertyInfo property)
        {
            if (parentObject == null)
                throw new ArgumentNullException("parentObject");

            if (parentObject == null)
                throw new ArgumentNullException("property");

            var value = property.GetValue(parentObject);

            return value == null
                || (property is ICollection
                && (value as ICollection).Count == 0);
        }
    
        /// <summary>
        /// Short extension for a forach block.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">Enumerable of type <see cref="{T}"/></param>
        /// <param name="action"><see cref="Action<typeparamref name="T"/>"/> delegate.</param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null)
                throw new ArgumentNullException("enumerable");

            if (action == null)
                throw new ArgumentNullException("action");

            foreach (T item in source)
            {
                action(item);
            }
        }
        /// <summary>
        /// Extends <see cref="List{T}.Add(T)"/> method to create new <see cref="List{T}"/>
        /// </summary>
        /// <typeparam name="T">Type of list.</typeparam>
        /// <param name="list">List to add to.</param>
        /// <param name="item">Item to add.</param>
        public static void Push<T>(this List<T> list, T item)
        {
            if (list == null)
                list = new List<T>();

            list.Add(item);
        }
        /// <summary>
        /// Converts <see cref="Enum"/> names of <paramref name="type"/> to a comma separated string
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string EnumToCsv(this Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            var list = Enum.GetNames(type);

            if (list is null)
                throw new ArgumentException("Input ca not be converted to csv.");

            return string.Join(", ", Enum.GetNames(type));
        }

        /// <summary>
        /// Adds a line made up of <paramref name="lineChar"/> after <paramref name="str"/>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="lineChar">The <see cref="char"/> making the line</param>
        /// <param name="charCount">Number of repetitions of the <see cref="char"/> '_'</param>
        /// <returns></returns>
        public static string AppendHorizontalLine(this string str, char lineChar ='_', int charCount = 100) =>
            string.Format("\n{0}\n{1}\n", str, new string(lineChar, charCount));
        /// <summary>
        ///  Adds a line made up of <paramref name="lineChar"/> before <paramref name="str"/>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="lineChar">The <see cref="char"/> making the line</param>
        /// <param name="charCount"></param>
        /// <returns></returns>
        public static string PrepemdHorizontalLine(this string str, char lineChar = '_',  int charCount = 100) =>
            string.Format("\n{1}\n{0}\n", str, new string(lineChar, charCount));
    }
}

