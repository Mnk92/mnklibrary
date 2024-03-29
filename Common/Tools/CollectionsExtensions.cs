﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Mnk.Library.Common.Models;
using Mnk.Library.Common.UI.Model;
using Mnk.Library.Common.UI.ModelsContainers;

namespace Mnk.Library.Common.Tools
{
    public static class CollectionsExtensions
    {
        public static void Save(this IList<string> collection, string filePath)
        {
            using var s = File.OpenWrite(filePath);
            using var stream = new BinaryWriter(s);
            stream.Write(collection.Count);
            foreach (var item in collection)
            {
                stream.Write(item);
            }
        }

        public static string[] Load(this string[] collection, string filePath)
        {
            using var s = File.OpenRead(filePath);
            using var stream = new BinaryReader(s);
            var result = new string[stream.ReadInt32()];
            for (var i = 0; i < result.Length; ++i)
            {
                result[i] = stream.ReadString();
            }
            return result;
        }

        public static int Insert<T>(this List<T> collection, T item, Func<T, string> nameProvider)
        {
            var id = ~collection.BinarySearch(item, new BinaryComparer<T>(nameProvider));
            if (id < 0) id = 0;
            collection.Insert(id, item);
            return id;
        }

        public static int Insert<T>(this IList<T> collection, T item, Func<T, string> nameProvider)
        {
            var id = collection.IndexOf(nameProvider(item), nameProvider);
            collection.Insert(id, item);
            return id;
        }

        public static int Insert<T>(this IList<T> collection, T item, Func<T, string> nameProvider, int min, int max)
        {
            var id = collection.Skip(min).Take(max - min).IndexOf(nameProvider(item), nameProvider);
            id += min;
            if (id > max) id = max;
            collection.Insert(id, item);
            return id;
        }

        public static int IndexOf<T>(this IEnumerable<T> collection, string name, Func<T, string> nameProvider)
        {
            var i = 0;
            foreach (var it in collection)
            {
                if (string.Compare(nameProvider(it), name, StringComparison.CurrentCultureIgnoreCase) < 0)
                {
                    ++i;
                    continue;
                }
                break;
            }
            return i;
        }

        public static bool Contains<T>(this IEnumerable<T> collection, string name, Func<T, string> nameProvider)
        {
            return collection
                .Any(it => string.Compare(nameProvider(it), name, StringComparison.CurrentCultureIgnoreCase) == 0);
        }

        public static void Insert(this IList<string> collection, string item)
        {
            var id = collection.FindIndex(item);
            collection.Insert(id, item);
        }

        public static void InsertIfNotContains(this IList<string> collection, string item)
        {
            if (collection.Contains(item)) return;
            collection.Insert(item);
        }

        public static void InsertIfNotContains(this List<string> collection, string item)
        {
            if (collection.BinarySearch(item) >= 0) return;
            collection.Insert(item);
        }

        public static int FindIndex(this IEnumerable<string> collection, string item)
        {
            var i = 0;
            foreach (var it in collection)
            {
                if (string.Compare(it, item, StringComparison.CurrentCultureIgnoreCase) < 0)
                {
                    ++i;
                    continue;
                }
                break;
            }
            return i;
        }

        public static void Sort<T>(this IList list, Func<T, T, int> comparer, int end, int begin = 0)
        {
            ArrayList.Adapter(list).Sort(begin, end, new Comparer<T>(comparer));
        }

        public static void Sort(this IList list, int end, int begin = 0)
        {
            ArrayList.Adapter(list).Sort(begin, end, new Comparer<object>((x, y) => string.Compare(x.ToString(), y.ToString(), StringComparison.CurrentCultureIgnoreCase)));
        }

        public static void Swap<T>(this IList<T> list, int left, int right)
        {
            (list[left], list[right]) = (list[right], list[left]);
        }

        public static IDictionary<string, T> ToDictionary<T>(this object[] values)
        {
            if (values.Length % 2 != 0)
                throw new ArgumentException("Can't create dictionary, invalid values count");
            var dictionary = new Dictionary<string, T>();
            for (var i = 0; i < values.Length; i += 2)
            {
                try
                {
                    dictionary[(string)values[i]] = (T)values[i + 1];
                }
                catch (Exception)
                {
                    throw new ArgumentException("Can't convert: " + values[i + 1] + " to " + typeof(T));
                }
            }
            return dictionary;
        }

        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> collection, TKey key)
        {
            TValue val;
            return collection.TryGetValue(key, out val) ? val : default(TValue);
        }

        public static ObservableCollection<T> Clone<T>(this ObservableCollection<T> collection)
    where T : ICloneable
        {
            return new ObservableCollection<T>(collection.Select(x => (T)x.Clone()));
        }

        public static CheckableDataCollection<T> Clone<T>(this CheckableDataCollection<T> collection)
            where T : class, ICloneable, ICheckableData
        {
            return new CheckableDataCollection<T>(collection.Select(x => (T)x.Clone()));
        }

        public static IEnumerable<T> DistinctByKey<T>(this IEnumerable<T> collection)
            where T : UI.Model.Data
        {
            return collection.Distinct(new EqualityDataComparer<T>());
        }

        public static IEnumerable<CheckableData<string>> DistinctByStringValue(this IEnumerable<CheckableData<string>> collection)
        {
            return collection.Distinct(new EqualityDataValueStringComparer());
        }

        public static IEnumerable<CheckableData<Guid>> DistinctByGuidValue(this IEnumerable<CheckableData<Guid>> collection)
        {
            return collection.Distinct(new EqualityDataValueGuidComparer());
        }

        public static IEnumerable<CheckableData<int>> DistinctByIntegerValue(this IEnumerable<CheckableData<int>> collection)
        {
            return collection.Distinct(new EqualityDataValueIntegerComparer());
        }

        public static IEnumerable<CheckableData<double>> DistinctByDoubleValue(this IEnumerable<CheckableData<double>> collection)
        {
            return collection.Distinct(new EqualityDataValueDoubleComparer());
        }

        public static CheckableDataCollection<CheckableData<T>> ToCollection<T>(this IDictionary<string, T> dictionary)
        {
            return new CheckableDataCollection<CheckableData<T>>(
                    dictionary.Select(x => new CheckableData<T>
                    {
                        Key = x.Key,
                        Value = x.Value
                    }));
        }

        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }

        public static void FillCollection<T>(this IList<T> collection, params string[] values)
            where T : IData, new()
        {
            foreach (var s in values)
            {
                collection.Add(new T { Key = s });
            }
        }

        public static IEnumerable<IHasChildren> Collect(IHasChildren result)
        {
            foreach (var r in result.Children.SelectMany(Collect))
            {
                yield return r;
            }
            yield return result;
        }
    }
}
