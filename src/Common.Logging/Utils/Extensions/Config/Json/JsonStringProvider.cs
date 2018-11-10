#if NETSTANDARD
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Common.Utils.Extensions.Config.Json
{
#pragma warning disable CS3009 // Base type is not CLS-compliant
    /// <summary>
    /// Json String Provider class which parses Json-based config from <see cref="JsonStringSource"/>.
    /// </summary>
    public class JsonStringProvider : ConfigurationProvider, IConfigurationProvider
#pragma warning restore CS3009 // Base type is not CLS-compliant
    {
        /// <summary>
        /// Constructor for <see cref="JsonStringProvider"/>.
        /// </summary>
        /// <param name="source"><see cref="JsonStringSource"/></param>
        public JsonStringProvider(JsonStringSource source)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
        }

        /// <inheritdoc />
        public override void Load()
        {
            var str = Source.SourceString;
            if (string.IsNullOrEmpty(str))
            {
                if (Source.Optional) // Always optional on reload
                {
                    Data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }
                else
                {
                    var error = new StringBuilder($"The configuration string '{Source.SourceString}' was not found and is not optional.");
                    throw new Exception(error.ToString());
                }
            }
            else
            {
                Data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                using (var sr = new StringReader(str))
                {
                    try
                    {

                        Data = JsonConfigurationStringParser.Parse(sr);
                    }
                    catch (JsonReaderException e)
                    {
                        throw e;
                        //string errorLine = string.Empty;
                        //if (sr.CanSeek)
                        //{
                        //    stream.Seek(0, SeekOrigin.Begin);

                        //    IEnumerable<string> fileContent;
                        //    using (var streamReader = new StreamReader(stream))
                        //    {
                        //        fileContent = ReadLines(streamReader);
                        //        errorLine = RetrieveErrorContext(e, fileContent);
                        //    }
                        //}

                        //throw new FormatException(Resources.FormatError_JSONParseError(e.LineNumber, errorLine), e);
                    }
                }
            }

        }

        /// <summary>
        /// The source settings for this provider.
        /// </summary>
        public JsonStringSource Source { get; }

        private static IEnumerable<string> ReadLines(StreamReader streamReader)
        {
            string line;
            do
            {
                line = streamReader.ReadLine();
                yield return line;
            } while (line != null);
        }
    }
}
#endif