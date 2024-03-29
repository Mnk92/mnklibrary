﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Mnk.Library.Common.Tools
{
    public static class CommonExtensions
    {
        public static T Parse<T>(string value)
            where T : struct
        {
            if (string.IsNullOrWhiteSpace(value)) return default(T);
            T tmp;
            if (Enum.TryParse(value, true, out tmp))
            {
                return tmp;
            }
            throw new ArgumentException("Can't parse enumeration: " + value);
        }

        public static void AddIfNotExist(this IList<string> list, string value)
        {
            if (!string.IsNullOrEmpty(value) && list.IndexOf(value) == -1)
            {
                list.Add(value);
            }
        }

        public static void AddIfNotExist(this ICollection<string> list, string value)
        {
            if (!string.IsNullOrEmpty(value) && !list.Contains(value))
            {
                list.Add(value);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> items, Action<T> command)
        {
            foreach (var item in items)
            {
                command(item);
            }
        }

        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunkSize)
        {
            var result = new List<T>(chunkSize);
            foreach (var x in source)
            {
                if (result.Count > chunkSize)
                {
                    yield return result;
                    result = new List<T>(chunkSize);
                }
                result.Add(x);
            }
            yield return result;
        }

        public static bool HasMethod(this object sender, string name)
        {
            return sender.GetType()
                .GetMethod(name, BindingFlags.Public | BindingFlags.Instance) != null;
        }
    }
}
