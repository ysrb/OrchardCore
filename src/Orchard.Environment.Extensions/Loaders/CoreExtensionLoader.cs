﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Orchard.DependencyInjection;
using Orchard.Environment.Extensions.Models;
using Microsoft.Extensions.Logging;
using Orchard.FileSystem;
using System.IO;
using Microsoft.DotNet.ProjectModel;
using Microsoft.DotNet.ProjectModel.Loader;

namespace Orchard.Environment.Extensions.Loaders
{
    public class CoreExtensionLoader : IExtensionLoader
    {
        private const string CoreAssemblyName = "Orchard.Core";
        private readonly IHostEnvironment _hostEnvironment;
        private readonly IOrchardFileSystem _fileSystem;
        private readonly ILogger _logger;
        private Assembly _assembly;

        public CoreExtensionLoader(
            IHostEnvironment hostEnvironment,
            IOrchardFileSystem fileSystem,
            ILogger<CoreExtensionLoader> logger)
        {
            _hostEnvironment = hostEnvironment;
            _fileSystem = fileSystem;
            _logger = logger;
        }

        public string Name => GetType().Name;

        public int Order => 10;

        public void ExtensionActivated(ExtensionLoadingContext ctx, ExtensionDescriptor extension)
        {
        }

        public void ExtensionDeactivated(ExtensionLoadingContext ctx, ExtensionDescriptor extension)
        {
        }

        public bool IsCompatibleWithModuleReferences(ExtensionDescriptor extension, IEnumerable<ExtensionProbeEntry> references)
        {
            return true;
        }

        public ExtensionEntry Load(ExtensionDescriptor descriptor)
        {
            if (!descriptor.Location.StartsWith("Core"))
            {
                return null;
            }

            var assembly = LoadCoreAssembly();

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Loaded referenced extension \"{0}\": assembly name=\"{1}\"", descriptor.Name, assembly.FullName);
            }
            return new ExtensionEntry
            {
                Descriptor = descriptor,
                Assembly = assembly,
                ExportedTypes = assembly.ExportedTypes.Where(x => IsTypeFromModule(x, descriptor))
            };
        }

        public ExtensionProbeEntry Probe(ExtensionDescriptor descriptor)
        {
            return null;
        }

        public void ReferenceActivated(ExtensionLoadingContext context, ExtensionReferenceProbeEntry referenceEntry)
        {
        }

        public void ReferenceDeactivated(ExtensionLoadingContext context, ExtensionReferenceProbeEntry referenceEntry)
        {
        }
        private static bool IsTypeFromModule(Type type, ExtensionDescriptor descriptor)
        {
            return (type.Namespace + ".").StartsWith(CoreAssemblyName + "." + descriptor.Id + ".");
        }

        private Assembly LoadCoreAssembly()
        {
            if (_assembly == null)
            {
                _assembly = Assembly.Load(new AssemblyName(CoreAssemblyName));

                //var projectContext = ProjectContext.CreateContextForEachFramework("Core/Orchard.Core").FirstOrDefault();

                //var loadContext = projectContext.CreateLoadContext();

                //_assembly = loadContext.LoadFromAssemblyName(new AssemblyName(CoreAssemblyName));
            }

            return _assembly;
        }
    }
}