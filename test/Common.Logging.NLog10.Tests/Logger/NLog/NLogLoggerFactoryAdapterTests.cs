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
using System.Reflection;
using System.Security;
using Common.Logging;
using Common.Logging.Configuration;
using Common.Logging.NLog;
using Common.TestUtil;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
#if NLOG2
using NLog.Targets;
#endif

using NUnit.Framework;
using LogLevel = NLog.LogLevel;
using LogManager = NLog.LogManager;

namespace Common.Logger.NLog
{
    [TestFixture]
    public class NLogLoggerFactoryAdapterTests : ILogTestsBase
    {
        public NLogLoggerFactoryAdapterTests()
        {

        }
#if NETFRAMEWORK
        /// <summary>
        /// NLog lacks <see cref="AllowPartiallyTrustedCallersAttribute"/> 
        /// and therefore needs full trust enviroments.
        /// </summary>
        protected override string CompliantTrustLevelName
        {
            get
            {
                return SecurityTemplate.PERMISSIONSET_FULLTRUST;
            }
        }
#endif


        private class TestLoggingConfiguration : LoggingConfiguration
        {
            public readonly TestTarget Target;

            public TestLoggingConfiguration()
            {
                Target = new TestTarget();
                LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, Target));
            }
        }

        private class TestTarget : TargetWithLayout
        {
            public LogEventInfo LastLogEvent;
            protected override void Write(LogEventInfo logEvent)
            {
                LastLogEvent = logEvent;
            }

            #region Overrides of TargetWithLayout


            #endregion

//#if NLOG1
//            //protected override int NeedsStackTrace()
//            //{
//            //    return 1;
//            //}
//#endif

            // NLog2 does not have NeedsStackTrace anymore, it looks for stacktrace token in log message layout.

            public TestTarget()
            {
                Layout = "${message}|${stacktrace}";
            }

        }

        [SetUp]
        public override void SetUp()
        {
            TestLoggingConfiguration cfg = new TestLoggingConfiguration();
            LogManager.Configuration = cfg;
            base.SetUp();
        }



        protected override ILoggerFactoryAdapter GetLoggerFactoryAdapter()
        {
            return new NLogLoggerFactoryAdapter((Common.Logging.Configuration.NameValueCollection)null);
        }

        [Test]
        public void LogsUserStackFrame()
        {
            TestLoggingConfiguration cfg = new TestLoggingConfiguration();
            LogManager.Configuration = cfg;

            var nLogAdapter = new NLogLoggerFactoryAdapter((Common.Logging.Configuration.NameValueCollection)null);
            Logging.LogManager.Adapter = nLogAdapter;

            var logger = Logging.LogManager.GetLogger("myLogger");

            logger.Debug("TestMessage");

            Assert.IsNotNull(cfg.Target.LastLogEvent);
            string stackTrace = string.Empty;
            Assert.DoesNotThrow(()=> stackTrace = cfg.Target.LastLogEvent.StackTrace.ToString());
            Assert.True(!string.IsNullOrEmpty(stackTrace));
            Assert.AreSame(MethodBase.GetCurrentMethod(), cfg.Target.LastLogEvent.UserStackFrame.GetMethod());
        }


    }
}
