﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Mnk.Library.Common.Log;
using Mnk.Library.Common.Models;
using Mnk.Library.Common.Tools;

namespace Mnk.Library.Common.Plugins
{
    public sealed class PluginLoadInfo
    {
        public string Path { get; set; }
        public Action Op { get; set; }
    }
    public sealed class Factory<TInterface>
    {
        private readonly ILog log = LogManager.GetLogger<Factory<TInterface>>();
        private readonly ILog infoLog = LogManager.GetInfoLogger<Factory<TInterface>>();
        private const string DllFilter = "*.dll";
        private readonly Dictionary<string, Type> dictionary = new Dictionary<string, Type>();
        public Factory(string pluginsDir, string sharedLibraries)
        {
            var time = Environment.TickCount;
            var targetName = typeof(TInterface).FullName;
            var tasks = GetFiles(sharedLibraries)
                .Select(x => new PluginLoadInfo { Path = x, Op = () => LoadAssembly(x) })
                .Concat(
                GetFiles(pluginsDir)
                    .Select(x => new PluginLoadInfo { Path = x, Op = () => LoadTypes(x, targetName) })
                );
            ;
            Parallel.ForEach(
                    tasks,
                    t => t.Op()
                    );
            infoLog.Write("Load dlls time: {0}", Environment.TickCount - time);
        }

        private static IEnumerable<string> GetFiles(string sharedLibraries)
        {
            return Directory.GetFiles(sharedLibraries, DllFilter, SearchOption.TopDirectoryOnly);
        }

        private void LoadTypes(string filePath, string targetName)
        {
            var assembly = LoadAssembly(filePath);
            if (assembly == null) return;
            foreach (var type in assembly.GetExportedTypes()
                .Where(x => x.GetInterface(targetName) != null &&
                      !(x.IsAbstract || x.IsInterface)))
            {
                try
                {
                    lock (this)
                    {
                        dictionary.Add(type.Name, type);
                    }
                }
                catch (Exception ex)
                {
                    log.Write(ex, "Error loading plugins from: '{0}'", filePath);
                }
            }
        }

        private Assembly LoadAssembly(string filePath)
        {
            try
            {
                return AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(filePath));
            }
            catch (Exception ex)
            {
                log.Write(ex, "Error loading library from: '{0}'", filePath);
            }
            return null;
        }

        public Pair<Type, TInterface> Create(string name)
        {
            var ret = new Pair<Type, TInterface>();
            try
            {
                ret.Key = dictionary[name];
                ret.Value = (TInterface)Activator.CreateInstance(ret.Key);
                return ret;
            }
            catch (Exception ex)
            {
                log.Write(ex, "Error creating plugin: '{0}'", name);
                return null;
            }
        }

        public IEnumerable<string> Names => dictionary.Keys;

        public Type Get(string name)
        {
            return dictionary.Get(name);
        }

        public void Remove(string name)
        {
            dictionary.Remove(name);
        }
    }
}