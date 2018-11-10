# Common.Logging 4x - Breaking Changes 

- [**GITHUBSOURCE**](http://github.com/net-commons/common-logging)

- [**Framework Owner** -> netcommon.sf.net](http://netcommon.sf.net)

___

## BREAKING CHANGES: Release 4.0.0-beta, November 9 , 2018

___

### Platforms Supported

| Component      |  Platform & Version  | Comment |
| ------------- |:-------------:| -----:|
| Common.Logging.Core| Net4+/NETSTANDARD2.0+ | Common Logging abstraction layer |
| Common.Logging      | Net4+/NETSTANDARD2.0+      |   Common Logging Loggers and basic functionality |
| Common.Logging.TestUtils | Net4.5+/NETSTANDARD2.0+      |    Test Utilities which are used in every test assembly we have in solution |
| Common.Logging.Tests | Net4.5+/Netcoreapp2.0+      |   Testing for the Common logging general functionality  |
| Common.Logging.NLog | Net4+/NETSTANDARD2.0+      |   Common Logging and NLog Integration |
| Common.Logging.NLog.Tests | Net4.5+/Netcoreapp2.0+ | Common Logging and NLog Integration tests |
| Common.Logging.Serilog | Net451+/NETSTANDARD2.0+| Common Logging and Serilog Integration |
| Common.Logging.Serilog.Tests | Net451+/Netcoreapp2.0+| Common Logging and Serilog Integration tests |
| Common.Logging.EntLib6 | Net4+      |    Entlib doesn't support netframework by default |
| Common.Logging.ApplicationInsights | Net45+/NETSTANDARD2.0+| Common Logging and application insights integration |
| Common.Logging.ApplicationInsights.Tests | Net45+/Netcoreapp2.0+| Common Logging and application insights integration tests|
You can browse the solution with `Common.Logging.NetCoreSupport.sln`.
___

### Platforms which are not supported based on the Microsoft strategy

- **Silverlight** - use previous version (3.4.1) for the silverlight support. Microsoft is planning to abandon it.
- **Portable** - portable framework was replaced by .NetStandard libraries and should be deleted.[MSDN](https://docs.microsoft.com/en-us/dotnet/standard/net-standard)
- **CLS Compliance** - ignored. Might be added in future releases if deemed required by @sbolen.
- **Migration to VS2018 Solution** - It broke Silverlight project :(.

___

### Loggers which have not being supported yet in Netstandard and 4.0.0

- Log4Net
- ETWLogger
- MultipleLogger

___

### Suggested changes in build system

**The below is just a suggestion which should be confirmed with framework owner. **

As Microsoft build system improved over last two years I suggest two approaches:

#### .csproj Attributes approach

To set the default attributes for the .csproj and change it only in case of need before uploading new version.
**Example (attributes inside .csproj):**
`<AssemblyVersion>4.0.0 </AssemblyVersion>` we did some changes and before upload we change it to the 4.0.1 and do `dotnet build` + `dotnet pack` it will automatically generate the `project_name.nuspec` file for the required version. It might remove the need for maintenance of all the versions in lib folder and maintaining all the different versions in separate projects as it is hard to support.

#### AssemblyInfo.cs approach

As I googled it can be done via `AssemblyInfo.cs` file too, but the attributes couldn't cross in `project_name.csproj` and `AssemblyInfo.cs`.
___

### Major Changes in Framework and Integrations

#### Changes inside configuration

1. If you use NETSTANDARD or NETCOREAPP you should use standard configuration for netstandard: [Microsoft.Extensions.Configuration](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration?view=aspnetcore-2.1).
1. If you use NETFRAMEWORK you should use old fashioned config.
1. It is suggested to move to move to new [Microsoft.Extensions.Configuration](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration?view=aspnetcore-2.1) based configuration.

[Microsoft.Extensions.Configuration](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration?view=aspnetcore-2.1) based configuration in `.json` and `.xml` formats.

```json
"common": {
        "logging": {
            "factoryAdapter": {
                "type": "Common.Logging.Simple.ConsoleOutLoggerFactoryAdapter, Common.Logging",
                "arguments": {
                    "level": "DEBUG",
                    "showLogName": true,
                    "showDateTime": true,
                    "dateTimeFormat": "yyyy/MM/dd HH:mm:ss:fff"
                }
            }
        }
    }
```

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <common>
    <logging>
      <factoryAdapter>
        <type>Common.Logging.Simple.ConsoleOutLoggerFactoryAdapter, Common.Logging</type>
        <arguments>
          <level>
          DEBUG
          </level>
          <showLogName>
          true
          </showLogName>
          <showDateTime>
          true
          </showDateTime>
          <dateTimeFormat>
          yyyy/MM/dd HH\:mm\:ss\:fff
          </dateTimeFormat>
        </arguments>
      </factoryAdapter>
    </logging>
  </common>
</configuration>
```

You can pass configuration directly into the logger via new api or via following files adding the sections outlined above:

- `"appsettings.json"` or
- `"appsettings.xml"` or
- `"CommonLoggingCfg.json"` or
- `"CommonLoggingCfg.xml"`

It might be considered to move to the above configuration for both NETFRAMEWORK and NETSTANDARD implementations to easy out the framework support.

**P.S.** Shortcuts for the types is working with those configs so in the example above it will work the same way if we state `CONSOLE` instead of `Common.Logging.Simple.ConsoleOutLoggerFactoryAdapter, Common.Logging`.

#### General Changes

- **All `.csproj` files were moved to new format, so all the directives are irrelivant right now**
- **Please use `NETSTANDARD` if need to change `NETSTANDARDX.X` and `NETFRAMEWORK` if you want to do directive for `NETX.X.X`**
- **All `.csproj` files dependencies were updated to the latest versions including integrated frameworks. _Exception is Configuraiton dependencies_**
- **All projects were kept in lowerst possible compatability level if possible (net4+)**
- **As compatability for the tests cannot be preserved in net4+ it was updated to net4.5+ and therefore the code was tested in net4.5+ platforms and netcoreapp2+. So use on net4+ with caution and better update.**

#### Changes inside Serilog

1. Unit tests fixed as per logic inside them.
1. Fixed bug with passing the null object for the logging (NullReferenceException).

#### Future Changes

1. Netcore web app logging (integration with Microsfot.Logger framework).
1. Wrapper for the CommonLogger witn NLog with with line.
1. Cleaning out the code when the modifications will be confirmed with @sbollen.
1. CLS - compatablity if required (object wrappers for the IConfiguration interfaces and other uncompitable methods which were added)

#### Suggestions for the changes

1. Clean up old projects and leave only those which we plan to support.
1. Remove permission sets from the tests.
1. Clean up lib folder from the packages which can be retrived from nuget.
1. Remove previous years solutions not to create a mess with support.
1. Abandon Silverlight support.
1. Clean up `directives` inside the code and concentrate only on NETSTANDARD support.
1. Clean up the old irrelivant files and old `.nuspecs`

___

### Major Changes in Tests

1. **Rhino framework**  - Support changes to the latest version but should be dropped in future, as Rhino is not activelly supported right now. Should be moved to **[FakeItEasy](https://fakeiteasy.github.io/)**
2. **[FakeItEasy](https://fakeiteasy.github.io/)** - it is the replacement for the Rhino Mocks, which will work in both **NETSTANDARD** and **NETFRAMEWORK**. It is used for Mocks and testing of the calls.
3. Tests are performed on separate frameworks. Platforms tested: Netframework4.5 and NETCOREAPP2.0. **Command to run the test for the library is _dotnet test_**
4. **NCover** will not be working as I didn't find any mentioning about Netcoreapps and NCover. **If somebody want to investigare further you are the most welcome to do it.** It should still be working with Neframework though.
5. **AppDomain + permission sets** - are depreciated and might be **[not supported](https://github.com/dotnet/standard/blob/master/docs/faq.md#is-appdomain-part-of-net-standard)** in the Netstandard. They are not supported in  NETCOREAPP/NETSTANDARD in non windows platforms such as Linux/iOS. Therefore I suggest to depriciate them and remove this from tests.

___