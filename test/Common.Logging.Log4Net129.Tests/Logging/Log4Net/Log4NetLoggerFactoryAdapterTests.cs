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

using Common.Logging.Configuration;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Reflection;

namespace Common.Logging.Log4Net
{
    /// <summary>
    /// </summary>
    /// <author>Erich Eichinger</author>
    [TestFixture]
    public class Log4NetLoggerFactoryAdapterTests
    {
        public class TestLog4NetLoggerFactoryAdapter : Log4NetLoggerFactoryAdapter
        {
            public TestLog4NetLoggerFactoryAdapter(NameValueCollection properties, ILog4NetRuntime runtime)
                : base(properties, runtime)
            { }
        }

        public class TestAppender : AppenderSkeleton
        {
            public LoggingEvent LastLoggingEvent;

            protected override void Append(LoggingEvent loggingEvent)
            {
                loggingEvent.Fix = FixFlags.LocationInfo;
                LastLoggingEvent = loggingEvent;
            }
        }


        public class TestLoggerRepository : AppenderSkeleton
        {
            public LoggingEvent LastLoggingEvent;

            protected override void Append(LoggingEvent loggingEvent)
            {
                loggingEvent.Fix = FixFlags.LocationInfo;
                LastLoggingEvent = loggingEvent;
            }
        }

        [Test]
        public void InitWithProperties()
        {
            MockRepository mocks = new MockRepository();
            Log4NetLoggerFactoryAdapter.ILog4NetRuntime rt = (Log4NetLoggerFactoryAdapter.ILog4NetRuntime)mocks.StrictMock(typeof(Log4NetLoggerFactoryAdapter.ILog4NetRuntime));

            string configFileName = new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;

            using (mocks.Ordered())
            {
                rt.XmlConfiguratorConfigure();
                rt.XmlConfiguratorConfigure(configFileName);
                rt.XmlConfiguratorConfigureAndWatch(configFileName);
                rt.BasicConfiguratorConfigure();
                Expect.Call(rt.GetLogger("testLogger")).Return(mocks.DynamicMock<log4net.ILog>());
            }
            mocks.ReplayAll();

            Log4NetLoggerFactoryAdapter a;
            NameValueCollection props = new NameValueCollection();

            props["configType"] = "inLiNe";
            a = new TestLog4NetLoggerFactoryAdapter(props, rt);

            props["ConfigTYPE"] = "fiLe";
            props["CONFIGFILE"] = configFileName;
            a = new TestLog4NetLoggerFactoryAdapter(props, rt);

            props["ConfigTYPE"] = "fiLe-WATCH";
            props["CONFIGFILE"] = configFileName;
            a = new TestLog4NetLoggerFactoryAdapter(props, rt);

            props["ConfigTYPE"] = "external";
            a = new TestLog4NetLoggerFactoryAdapter(props, rt);

            props["ConfigTYPE"] = "any unknown";
            a = new TestLog4NetLoggerFactoryAdapter(props, rt);

            a.GetLogger("testLogger");

            mocks.VerifyAll();
        }

        [Test]
        public void LogsCorrectLoggerName()
        {
            TestAppender testAppender = new TestAppender();
#if NETCOREAPP
            var repository = log4net.LogManager.CreateRepository("Common.Logging.Repository");
            BasicConfigurator.Configure(repository, testAppender);
#endif
#if !NETCOREAPP
            BasicConfigurator.Configure(testAppender);
#endif
            Log4NetLoggerFactoryAdapter a;
            NameValueCollection props = new NameValueCollection();

            props["configType"] = "external";
            a = new Log4NetLoggerFactoryAdapter(props);

            a.GetLogger(this.GetType()).Debug("TestMessage");

            Assert.AreEqual(this.GetType().FullName, testAppender.LastLoggingEvent.GetLoggingEventData().LoggerName);
            Assert.AreEqual(this.GetType().FullName, testAppender.LastLoggingEvent.LocationInformation.ClassName);
            Assert.AreEqual(MethodBase.GetCurrentMethod().Name, testAppender.LastLoggingEvent.LocationInformation.MethodName);
            Assert.AreEqual("TestMessage", testAppender.LastLoggingEvent.MessageObject);
        }

        [Test]
        public void CachesLoggers()
        {
            NameValueCollection props = new NameValueCollection();

            props["configType"] = "external";
            Log4NetLoggerFactoryAdapter a = new Log4NetLoggerFactoryAdapter(props);

            ILog log = a.GetLogger(this.GetType());
            Assert.AreSame(log, a.GetLogger(this.GetType()));
        }

        [Test]
        public void CheckGlobalVariablesSet()
        {
            NameValueCollection props = new NameValueCollection();

            props["configType"] = "external";
            var a = new Log4NetLoggerFactoryAdapter(props);

            a.GetLogger(this.GetType()).GlobalVariablesContext.Set("TestKey", "TestValue");

            var actualValue = global::log4net.GlobalContext.Properties["TestKey"];

            Assert.AreEqual("TestValue", actualValue);
        }

        [Test]
        public void CheckThreadVariablesSet()
        {
            NameValueCollection props = new NameValueCollection();

            props["configType"] = "external";
            var a = new Log4NetLoggerFactoryAdapter(props);

            a.GetLogger(this.GetType()).ThreadVariablesContext.Set("TestKey", "TestValue");

            var actualValue = global::log4net.ThreadContext.Properties["TestKey"];

            Assert.AreEqual("TestValue", actualValue);
        }


		[Test]
		public void CheckNestedThreadVariablesSet()
		{
			NameValueCollection props = new NameValueCollection();
			props["configType"] = "external";
			var a = new Log4NetLoggerFactoryAdapter(props);

			bool hasItems = a.GetLogger(this.GetType()).NestedThreadVariablesContext.HasItems;
			Assert.AreEqual(false, hasItems);

			a.GetLogger(this.GetType()).NestedThreadVariablesContext.Push("TestValue1");
			a.GetLogger(this.GetType()).NestedThreadVariablesContext.Push("TestValue2");

			hasItems = a.GetLogger(this.GetType()).NestedThreadVariablesContext.HasItems;
			Assert.AreEqual(true, hasItems);

			int depth = global::log4net.NDC.Depth;
			Assert.AreEqual(2, depth);

			var actualValue = global::log4net.NDC.Pop();
			Assert.AreEqual("TestValue2", actualValue);

			actualValue = global::log4net.NDC.Pop();
			Assert.AreEqual("TestValue1", actualValue);

			hasItems = a.GetLogger(this.GetType()).NestedThreadVariablesContext.HasItems;
			Assert.AreEqual(false, hasItems);

			depth = global::log4net.NDC.Depth;
			Assert.AreEqual(0, depth);
		}

	}
}