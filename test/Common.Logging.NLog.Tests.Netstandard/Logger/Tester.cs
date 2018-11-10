#if !NETFRAMEWORK
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;

namespace Common.Logging.NLog.Tests.Netstandard.Logger
{

    public class Tester
    {
    [OneTimeSetUp]
        public void Init()
        {
#if NETFRAMEWORK
            Assembly assembly = Assembly.GetCallingAssembly();

            AppDomainManager manager = new AppDomainManager();
            FieldInfo entryAssemblyfield = manager.GetType().GetField("m_entryAssembly", BindingFlags.Instance | BindingFlags.NonPublic);
            entryAssemblyfield.SetValue(manager, assembly);

            AppDomain domain = AppDomain.CurrentDomain;
            FieldInfo domainManagerField = domain.GetType().GetField("_domainManager", BindingFlags.Instance | BindingFlags.NonPublic);
            domainManagerField.SetValue(domain, manager);
#endif
        }
    }
}

#endif