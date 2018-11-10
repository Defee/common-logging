#if NETSTANDARD
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Xml;

namespace Common.Utils.Extensions.Config.Xml
{
#pragma warning disable CS3009 // Base type is not CLS-compliant
    /// <summary>
    /// Xml String Provider class which parses Json-based config from <see cref="XmlStringSource"/>.
    /// </summary>
    public class XmlStringProvider : ConfigurationProvider, IConfigurationProvider
#pragma warning restore CS3009 // Base type is not CLS-compliant
    {
        private const string NameAttributeKey = "Name";

        internal XmlDocumentDecryptor Decryptor { get; set; } = XmlDocumentDecryptor.Instance;

        /// <summary>
        /// Constructor for <see cref="XmlStringProvider"/>.
        /// </summary>
        /// <param name="source"><see cref="XmlStringSource"/></param>
        public XmlStringProvider(XmlStringSource source)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
        }

        /// <inheritdoc />
        public override void Load()
        {
            var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            var readerSettings = new XmlReaderSettings()
            {
                CloseInput = false, // caller will close the stream
                DtdProcessing = DtdProcessing.Prohibit,
                IgnoreComments = true,
                IgnoreWhitespace = true
            };

            if (string.IsNullOrEmpty(Source.SourceString))
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
                using (var stream = Source.SourceString.ToStream())
                {
                    using (var reader = Decryptor.CreateDecryptingXmlReader(stream, readerSettings))
                    {
                        var prefixStack = new Stack<string>();

                        SkipUntilRootElement(reader);

                        // We process the root element individually since it doesn't contribute to prefix 
                        ProcessAttributes(reader, prefixStack, data, AddNamePrefix);
                        ProcessAttributes(reader, prefixStack, data, AddAttributePair);

                        var preNodeType = reader.NodeType;
                        while (reader.Read())
                        {
                            switch (reader.NodeType)
                            {
                                case XmlNodeType.Element:
                                    prefixStack.Push(reader.LocalName);
                                    ProcessAttributes(reader, prefixStack, data, AddNamePrefix);
                                    ProcessAttributes(reader, prefixStack, data, AddAttributePair);

                                    // If current element is self-closing
                                    if (reader.IsEmptyElement)
                                    {
                                        prefixStack.Pop();
                                    }

                                    break;

                                case XmlNodeType.EndElement:
                                    if (prefixStack.Any())
                                    {
                                        // If this EndElement node comes right after an Element node,
                                        // it means there is no text/CDATA node in current element
                                        if (preNodeType == XmlNodeType.Element)
                                        {
                                            var key = ConfigurationPath.Combine(prefixStack.Reverse());
                                            data[key] = string.Empty;
                                        }

                                        prefixStack.Pop();
                                    }

                                    break;

                                case XmlNodeType.CDATA:
                                case XmlNodeType.Text:
                                    {
                                        var key = ConfigurationPath.Combine(prefixStack.Reverse());
                                        if (data.ContainsKey(key))
                                        {
                                            throw new FormatException($"The key: {key} is duplicated. {GetLineInfo(reader)}");
                                        }

                                        data[key] = reader.Value;
                                        break;
                                    }
                                case XmlNodeType.XmlDeclaration:
                                case XmlNodeType.ProcessingInstruction:
                                case XmlNodeType.Comment:
                                case XmlNodeType.Whitespace:
                                    // Ignore certain types of nodes
                                    break;

                                default:
                                    throw new FormatException($"There is unsupported node type: {reader.NodeType}. {GetLineInfo(reader)}");
                            }

                            preNodeType = reader.NodeType;
                            // If this element is a self-closing element,
                            // we pretend that we just processed an EndElement node
                            // because a self-closing element contains an end within itself
                            if (preNodeType == XmlNodeType.Element &&
                                reader.IsEmptyElement)
                            {
                                preNodeType = XmlNodeType.EndElement;
                            }
                        }
                    }

                    Data = data;
                }
            }
        }

        /// <summary>
        /// The source settings for this provider.
        /// </summary>
        public XmlStringSource Source { get; }


        #region private Method Helpers

        private void SkipUntilRootElement(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.NodeType != XmlNodeType.XmlDeclaration &&
                    reader.NodeType != XmlNodeType.ProcessingInstruction)
                {
                    break;
                }
            }
        }

        private static string GetLineInfo(XmlReader reader)
        {
            var lineInfo = reader as IXmlLineInfo;
            return lineInfo == null
                ? string.Empty
                : $"Error is located in xml in line {lineInfo.LineNumber} & position is {lineInfo.LinePosition}";
        }

        private void ProcessAttributes(XmlReader reader, Stack<string> prefixStack, IDictionary<string, string> data,
            Action<XmlReader, Stack<string>, IDictionary<string, string>, XmlWriter> act, XmlWriter writer = null)
        {
            for (int i = 0; i < reader.AttributeCount; i++)
            {
                reader.MoveToAttribute(i);

                // If there is a namespace attached to current attribute
                if (!string.IsNullOrEmpty(reader.NamespaceURI))
                    throw new FormatException($"Namespace is not supported. {GetLineInfo(reader)}");
                
                act(reader, prefixStack, data, writer);
            }

            // Go back to the element containing the attributes we just processed
            reader.MoveToElement();
        }

        // The special attribute "Name" only contributes to prefix
        // This method adds a prefix if current node in reader represents a "Name" attribute
        private static void AddNamePrefix(XmlReader reader, Stack<string> prefixStack,
            IDictionary<string, string> data, XmlWriter writer)
        {
            if (!string.Equals(reader.LocalName, NameAttributeKey, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // If current element is not root element
            if (prefixStack.Any())
            {
                var lastPrefix = prefixStack.Pop();
                prefixStack.Push(ConfigurationPath.Combine(lastPrefix, reader.Value));
            }
            else
            {
                prefixStack.Push(reader.Value);
            }
        }

        // Common attributes contribute to key-value pairs
        // This method adds a key-value pair if current node in reader represents a common attribute
        private static void AddAttributePair(XmlReader reader, Stack<string> prefixStack,
            IDictionary<string, string> data, XmlWriter writer)
        {
            prefixStack.Push(reader.LocalName);
            var key = ConfigurationPath.Combine(prefixStack.Reverse());
            if (data.ContainsKey(key))
                throw new FormatException($"The key: {key} is not supported. {GetLineInfo(reader)}");

            data[key] = reader.Value;
            prefixStack.Pop();
        }

        #endregion
    }
}
#endif