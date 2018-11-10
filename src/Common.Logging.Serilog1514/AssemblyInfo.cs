using System.Reflection;
using System.Security;

[assembly: AssemblyProduct("Common Logging Framework Serilog 1.5.14 Adapter")]
[assembly: SecurityTransparent]

#if NETFRAMEWORK
[assembly: SecurityRules(SecurityRuleSet.Level1)]
#endif