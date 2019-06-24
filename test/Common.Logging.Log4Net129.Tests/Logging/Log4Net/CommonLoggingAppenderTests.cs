#region License

/*
 * Copyright 2002-2009 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion

using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Xml;
using Common.Logging.Simple;
using log4net.Config;
using log4net.Core;
using log4net.Repository;
using log4net.Util;
using NUnit.Framework;

namespace Common.Logging.Log4Net
{
    /// <summary>
    /// </summary>
    /// <author>Erich Eichinger</author>
    [TestFixture]
    public class CommonLoggingAppenderTests
    {
        [Test]
        public void RoutesToCommonLogging()
        {
            //            CommonLoggingAppender appender = new CommonLoggingAppender();
            //            appender.Layout = new PatternLayout("%level - %class.%method: %message");
            //            BasicConfigurator.Configure(stm);

            Stream stm = this.GetType().Assembly.GetManifestResourceStream(this.GetType().FullName + "_log4net.config.xml");
#if NETCOREAPP
            MissingApiExtensions.Configure(stm);
#endif
#if NETFRAMEWORK
            XmlConfigurator.Configure(stm);
#endif


            CapturingLoggerFactoryAdapter adapter = new CapturingLoggerFactoryAdapter();
            LogManager.Adapter = adapter;

            string message = "testmessage";
            Exception exception = new Exception("testexception");

            adapter.ClearLastEvent();
            var logger = log4net.LogManager.GetLogger(this.GetType());
            logger.Debug(message, exception);
            Assert.AreEqual(this.GetType().FullName, adapter.LastEvent.Source.Name);
            Assert.AreEqual(string.Format("{0} - {1}.{2}: {3}", Level.Debug, this.GetType().FullName, MethodBase.GetCurrentMethod().Name, message), adapter.LastEvent.MessageObject.ToString());
            Assert.AreSame(exception, adapter.LastEvent.Exception);

            adapter.ClearLastEvent();
            log4net.LogManager.GetLogger(this.GetType()).Warn(message, exception);
            Assert.AreEqual(this.GetType().FullName, adapter.LastEvent.Source.Name);
            Assert.AreEqual(string.Format("{0} - {1}.{2}: {3}", Level.Warn, this.GetType().FullName, MethodBase.GetCurrentMethod().Name, message), adapter.LastEvent.MessageObject.ToString());
            Assert.AreSame(exception, adapter.LastEvent.Exception);
        }


    }

    public static class MissingApiExtensions
    {
        private static readonly Type declaringType = typeof(XmlConfigurator);
        /// <summary>
        /// Configures log4net using the specified configuration data stream.
        /// </summary>
        /// <param name="configStream">A stream to load the XML configuration from.</param>
        /// <remarks>
        /// <para>
        /// The configuration data must be valid XML. It must contain
        /// at least one element called <c>log4net</c> that holds
        /// the log4net configuration data.
        /// </para>
        /// <para>
        /// Note that this method will NOT close the stream parameter.
        /// </para>
        /// </remarks>
        public static ICollection Configure(Stream configStream)
        {
            ArrayList arrayList = new ArrayList();
            ILoggerRepository repository = log4net.LogManager.GetRepository(Assembly.GetCallingAssembly());
            using (new LogLog.LogReceivedAdapter((IList)arrayList))
                InternalConfigure(repository, configStream);
            repository.ConfigurationMessages = (ICollection)arrayList;
            return (ICollection)arrayList;
        }

        private static void InternalConfigure(ILoggerRepository repository, Stream configStream)
        {
            LogLog.Debug(declaringType, "configuring repository [" + repository.Name + "] using stream");
            if (configStream == null)
            {
                LogLog.Error(declaringType, "Configure called with null 'configStream' parameter");
            }
            else
            {
                XmlDocument xmlDocument = new XmlDocument();
                try
                {
                    XmlReader reader = XmlReader.Create(configStream, new XmlReaderSettings()
                    {
                        DtdProcessing = DtdProcessing.Parse
                    });
                    xmlDocument.Load(reader);
                }
                catch (Exception ex)
                {
                    LogLog.Error(declaringType, "Error while loading XML configuration", ex);
                    xmlDocument = (XmlDocument)null;
                }
                if (xmlDocument == null)
                    return;
                LogLog.Debug(declaringType, "loading XML configuration");
                XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("log4net");
                if (elementsByTagName.Count == 0)
                    LogLog.Debug(declaringType, "XML configuration does not contain a <log4net> element. Configuration Aborted.");
                else if (elementsByTagName.Count > 1)
                    LogLog.Error(declaringType, "XML configuration contains [" + (object)elementsByTagName.Count + "] <log4net> elements. Only one is allowed. Configuration Aborted.");
                else
                    InternalConfigureFromXml(repository, elementsByTagName[0] as XmlElement);
            }
        }

        /// <summary>
        /// Configures the specified repository using a <c>log4net</c> element.
        /// </summary>
        /// <param name="repository">The hierarchy to configure.</param>
        /// <param name="element">The element to parse.</param>
        /// <remarks>
        /// <para>
        /// Loads the log4net configuration from the XML element
        /// supplied as <paramref name="element" />.
        /// </para>
        /// <para>
        /// This method is ultimately called by one of the Configure methods
        /// to load the configuration from an <see cref="T:System.Xml.XmlElement" />.
        /// </para>
        /// </remarks>
        private static void InternalConfigureFromXml(ILoggerRepository repository, XmlElement element)
        {
            if (element == null)
                LogLog.Error(declaringType, "ConfigureFromXml called with null 'element' parameter");
            else if (repository == null)
            {
                LogLog.Error(declaringType, "ConfigureFromXml called with null 'repository' parameter");
            }
            else
            {
                LogLog.Debug(declaringType, "Configuring Repository [" + repository.Name + "]");
                IXmlRepositoryConfigurator repositoryConfigurator = repository as IXmlRepositoryConfigurator;
                if (repositoryConfigurator == null)
                {
                    LogLog.Warn(declaringType, "Repository [" + (object)repository + "] does not support the XmlConfigurator");
                }
                else
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    XmlElement element1 = (XmlElement)xmlDocument.AppendChild(xmlDocument.ImportNode((XmlNode)element, true));
                    repositoryConfigurator.Configure(element1);
                }
            }
        }
    }
}