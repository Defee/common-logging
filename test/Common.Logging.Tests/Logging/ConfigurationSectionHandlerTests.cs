#region License

/*
 * Copyright © 2002-2007 the original author or authors.
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
using Common.Logging.Configuration;
using Common.Logging.Simple;
using NUnit.Framework;
using System.Configuration;

namespace Common.Logging
{
    [TestFixture]
    public class ConfigurationSectionHandlerTests
    {
#if NETFRAMEWORK
        [Test]
        public void NoParentSectionsAllowed()
        {
            //TODO: I should figure out what is it and why it is needed. for now commented.
            IConfigurationSectionHandler handler = new ConfigurationSectionHandler();
            Assert.Throws(Is.TypeOf<ConfigurationException>().And.Message.EqualTo("parent configuration sections are not allowed")
                         , delegate
                         {
                             handler.Create(new LogSetting(typeof(ConsoleOutLoggerFactoryAdapter), null),
                                            null,
                                            null);
                         });
        }
#endif
        [Test]
        public void TooManyAdapterElements()
        {
#if NETCOREAPP
            const string xml = @"<?xml version='1.0' encoding='utf-8' ?>
                                <configuration>
                                  <common>
                                    <logging>
                                      <factoryAdapter>
                                        <type>Common.Logging.Simple.ConsoleOutLoggerFactoryAdapter, Common.Logging</type>
                                      </factoryAdapter>
                                      <factoryAdapter>
                                        <type>Common.Logging.Simple.ConsoleOutLoggerFactoryAdapter, Common.Logging</type>
                                      </factoryAdapter>
                                    </logging>
                                  </common>
                                </configuration>";
            StandaloneConfigurationReader reader = new StandaloneConfigurationReader(xml);
            Assert.Throws(Is.TypeOf<FormatException>(), () => reader.GetSection(null));
#endif

#if NETFRAMEWORK
          const string xml =
                        @"<?xml version='1.0' encoding='UTF-8' ?>
                        <logging>
                          <factoryAdapter type='Common.Logging.Simple.ConsoleOutLoggerFactoryAdapter, Common.Logging'>
                          </factoryAdapter>
                          <factoryAdapter type='Common.Logging.Simple.ConsoleOutLoggerFactoryAdapter, Common.Logging'>
                          </factoryAdapter>
                        </logging>";   
            StandaloneConfigurationReader reader = new StandaloneConfigurationReader(xml);
            Assert.Throws(Is.TypeOf<ConfigurationException>()
                            .And.Message.EqualTo("Only one <factoryAdapter> element allowed")
                            , delegate
                            {
                                reader.GetSection(null);
                            });
#endif
        }

        [Test]
        public void NoTypeElementForAdapterDeclaration()
        {
#if NETCOREAPP
            const string xml = @"<?xml version='1.0' encoding='utf-8' ?>
                                <configuration>
                                  <common>
                                    <logging>
                                      <factoryAdapter>
                                        <clazz>CONSOLE</clazz>
                                        <arg>
                                            <kez>level</kez>
                                            <value>DEBUG></value>
                                        </arg>
                                      </factoryAdapter>
                                    </logging>
                                  </common>
                                </configuration>";
#endif
#if NETFRAMEWORK
          const string xml =
                    @"<?xml version='1.0' encoding='UTF-8' ?>
                    <logging>
                      <factoryAdapter clazz='Common.Logging.Simple.ConsoleOutLoggerFactoryAdapter, Common.Logging'>
                        <arg kez='level' value='DEBUG' />
                      </factoryAdapter>
                    </logging>";  
#endif
            
            StandaloneConfigurationReader reader = new StandaloneConfigurationReader(xml);
            Assert.Throws<ConfigurationException>(() => reader.GetSection(null));
        }

        [Test]
        public void NoKeyElementForAdapterArguments()
        {
#if NETCOREAPP
            const string xml = @"<?xml version='1.0' encoding='utf-8' ?>
                                <configuration>
                                  <common>
                                    <logging>
                                      <factoryAdapter>
                                        <type>CONSOLE</type>
                                        <arg>
                                            <kez>level</kez>
                                            <value>DEBUG></value>
                                        </arg>
                                      </factoryAdapter>
                                    </logging>
                                  </common>
                                </configuration>";
            StandaloneConfigurationReader reader = new StandaloneConfigurationReader(xml);
            LogSetting setting = null;
            Assert.DoesNotThrow(() => setting = reader.GetSection(null) as LogSetting);
            Assert.True(setting.Properties==null);
#endif
#if NETFRAMEWORK
           
            const string xml =
                        @"<?xml version='1.0' encoding='UTF-8' ?>
                        <logging>
                          <factoryAdapter type='Common.Logging.Simple.ConsoleOutLoggerFactoryAdapter, Common.Logging'>
                            <arg kez='level' value='DEBUG' />
                          </factoryAdapter>
                        </logging>";
            StandaloneConfigurationReader reader = new StandaloneConfigurationReader(xml);
            Assert.Throws<ConfigurationException>(() => reader.GetSection(null));
#endif
            
        }

        [Test]
        public void ConsoleShortCut()
        {
#if NETCOREAPP
            const string xml = @"<?xml version='1.0' encoding='utf-8' ?>
                                <configuration>
                                  <common>
                                    <logging>
                                      <factoryAdapter>
                                        <type>CONSOLE</type>
                                      </factoryAdapter>
                                    </logging>
                                  </common>
                                </configuration>";
#endif
#if NETFRAMEWORK
            const string xml =
                                @"<?xml version='1.0' encoding='UTF-8' ?>
                                <logging>
                                  <factoryAdapter type='CONSOLE'/>
                                </logging>";
#endif
            StandaloneConfigurationReader reader = new StandaloneConfigurationReader(xml);
            LogSetting setting = reader.GetSection(null) as LogSetting;
            Assert.IsNotNull(setting);
            Assert.AreEqual(typeof(ConsoleOutLoggerFactoryAdapter), setting.FactoryAdapterType);

        }

        [Test]
        public void TraceShortCut()
        {
#if NETCOREAPP
            const string xml = @"<?xml version='1.0' encoding='utf-8' ?>
                                <configuration>
                                  <common>
                                    <logging>
                                      <factoryAdapter>
                                        <type>TRACE</type>
                                      </factoryAdapter>
                                    </logging>
                                  </common>
                                </configuration>";
#endif
#if NETFRAMEWORK
            const string xml =
    @"<?xml version='1.0' encoding='UTF-8' ?>
    <logging>
      <factoryAdapter type='TRACE'/>
    </logging>";
#endif
            StandaloneConfigurationReader reader = new StandaloneConfigurationReader(xml);
            LogSetting setting = reader.GetSection(null) as LogSetting;
            Assert.IsNotNull(setting);
            Assert.AreEqual(typeof(TraceLoggerFactoryAdapter), setting.FactoryAdapterType);

        }

        [Test]
        public void NoOpShortCut()
        {
#if NETCOREAPP
            const string xml = @"<?xml version='1.0' encoding='utf-8' ?>
                                <configuration>
                                  <common>
                                    <logging>
                                      <factoryAdapter>
                                        <type>NOOP</type>
                                      </factoryAdapter>
                                    </logging>
                                  </common>
                                </configuration>";
#endif
#if NETFRAMEWORK
            

            const string xml =
    @"<?xml version='1.0' encoding='UTF-8' ?>
    <logging>
      <factoryAdapter type='NOOP'/>
    </logging>";
#endif
            StandaloneConfigurationReader reader = new StandaloneConfigurationReader(xml);
            LogSetting setting = reader.GetSection(null) as LogSetting;
            Assert.IsNotNull(setting);
            Assert.AreEqual(typeof(NoOpLoggerFactoryAdapter), setting.FactoryAdapterType);

        }

        [Test]
        public void ArgumentKeysCaseInsensitive()
        {
#if NETCOREAPP
            const string xml = @"<?xml version='1.0' encoding='utf-8' ?>
                                <configuration>
                                  <common>
                                    <logging>
                                      <factoryAdapter>
                                        <type>CONSOLE</type>
                                        <arguments>
                                          <leVel1>DEBUG</leVel1>
                                          <LEVEL2>DEBUG</LEVEL2>
                                          <level3>DEBUG</level3>
                                        </arguments>
                                      </factoryAdapter>
                                    </logging>
                                  </common>
                                </configuration>";
#endif
#if NETFRAMEWORK
            const string xml =
                            @"<?xml version='1.0' encoding='UTF-8' ?>
                            <logging>
                              <factoryAdapter type='CONSOLE'>
                                <arg key='LeVel1' value='DEBUG' />
                                <arg key='LEVEL2' value='DEBUG' />
                                <arg key='level3' value='DEBUG' />
                              </factoryAdapter>
                            </logging>";
#endif
            
            StandaloneConfigurationReader reader = new StandaloneConfigurationReader(xml);
            LogSetting setting = reader.GetSection(null) as LogSetting;
            Assert.IsNotNull(setting);

            Assert.AreEqual(3, setting.Properties.Count);
            var expectedValue = new[] { "DEBUG" };
            CollectionAssert.AreEqual(expectedValue, setting.Properties.GetValues("level1"));
            CollectionAssert.AreEqual(expectedValue, setting.Properties.GetValues("level2"));
            CollectionAssert.AreEqual(expectedValue, setting.Properties.GetValues("LEVEL3"));

            //Assert.AreEqual( 1, setting.Properties.Count );
            //Assert.AreEqual( 3, setting.Properties.GetValues("LeVeL").Length );
        }
    }
}
