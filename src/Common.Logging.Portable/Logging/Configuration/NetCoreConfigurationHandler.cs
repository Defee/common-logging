#if NETSTANDARD || NETCOREAPP || NET451


using Common.Logging.Configuration;
using Common.Logging.Simple;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace Common.Logging.Configuration
{
    /// <summary>
    /// Class is intended to handle the configuration for NetCore Standard libraries
    /// </summary>
    public class NetCoreConfigurationHandler
    {
        private static IList<string> _lookupFiles = new List<string>
        {
            "appsettings.json",
            "appsettings.xml",
            "CommonLoggingCfg.json",
            "CommonLoggingCfg.xml"
        };

        private static string NETCORE_COMMON_LOGGING_SECTION { get { return "common:logging"; } }
        /// <summary>
        /// Gets the LogConfiguration with lookup of standard files.
        /// </summary>
        /// <param name="sectionName"> section to look into the config files common:logging by default.</param>
        /// <returns>LogConfiguration object is everything is ok and null if cfg section is absenct</returns>
        public static LogConfiguration GetDefaultLogConfig(string sectionName = null)
        {
            var cfgObj = InitDefaultCommonLogging();
            var cfg = cfgObj as IConfiguration;
            if (string.IsNullOrEmpty(sectionName)) 
                sectionName = NETCORE_COMMON_LOGGING_SECTION;

            LogConfiguration logConfiguration = new LogConfiguration();
            var loggingSection = cfg.GetSection(sectionName);
            if (loggingSection == null) return null;
            loggingSection.Bind(logConfiguration);
            return logConfiguration;
        }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public static LogConfiguration InitLogConfigurationFromExistingConfig(IConfiguration configuration, string sectionName = null)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (string.IsNullOrEmpty(sectionName)) sectionName = NETCORE_COMMON_LOGGING_SECTION;

            LogConfiguration logConfiguration = new LogConfiguration();
            var loggingSection = configuration.GetSection(sectionName);
            if (loggingSection == null) return null;
            loggingSection.Bind(logConfiguration);
            return logConfiguration;
        }

        /// <summary>
        /// Searches and tries to init default configuration.
        /// </summary>
        /// <returns></returns>
        public static object InitDefaultCommonLogging()
        {
            try
            {
                var dirs = new List<string>
            {
                Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                Directory.GetCurrentDirectory()
            };
                var result = dirs.Select(it => new DirectoryInfo(it))
                    .FirstOrDefault(it =>
                        it.GetFiles()
                            .Any(it2 => _lookupFiles.Any(it3 => it2.Name == it3)));

                if (result == null)
                {
                    throw new FileNotFoundException(
                        $" No files with names:{JsonConvert.SerializeObject(_lookupFiles)} was found in the lookup directories: {JsonConvert.SerializeObject(dirs)}. please ensure it is copied on build.");
                }

                var cfgBuilder = new ConfigurationBuilder()
                    .SetBasePath(result.FullName);

                foreach (var fileName in _lookupFiles)
                {

                    var ext = Path.GetExtension(fileName).Trim('.').ToLower();
                    switch (ext)
                    {
                        case "json":
                            cfgBuilder.AddJsonFile(fileName, optional: true);
                            break;
                        case "xml":
                            cfgBuilder.AddXmlFile(fileName, optional: true);
                            break;
                        default:
                            throw new NotImplementedException("We do not accept files except xml and json");
                    }
                }
                return cfgBuilder.Build();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}

#endif