using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;


namespace Nvelope
{
    public static class XmlNodeListExtensions
    {
        public static IEnumerable<XmlNode> ToList(this XmlNodeList source)
        {
            foreach (XmlNode node in source)
                yield return node;
        }

        /// <summary>
        /// Get the document that owns the supplied node
        /// </summary>
        /// <remarks>Might be node itself if node is an XMLDocument</remarks>
        public static XmlDocument GetOwnerDocument(this XmlNode node)
        {
            return node.OwnerDocument ?? node as XmlDocument;
        }

        #region Formattting / Strings
        /// <summary>
        /// Gets the text of the XML node (or document) in a nice, readable format (or null if the node is null)
        /// </summary>
        public static string ToIndentedXmlText(this XmlNode node)
        {
            if (node != null)
            {
                StringBuilder sb = new StringBuilder();
                XmlWriter xw = XmlWriter.Create(sb, new XmlWriterSettings() { Indent = true, OmitXmlDeclaration = true });
                node.WriteTo(xw);
                xw.Flush();
                return sb.ToString();
            }
            return null;
        }

        /// <summary>
        /// Escapes all the XML reserved characters to make this string safe to put in an XML document
        /// </summary>
        public static string EscapeForXml(this string source)
        {
            // If anyone knows of a better place to find an XML escaping method,
            // feel free to change this
            return SecurityElement.Escape(source);
        }

        public static string UnescapeForXml(this string s)
        {
            if (string.IsNullOrEmpty(s)) return s;

            String answer = s;
            answer = answer.Replace("&apos;", "'");
            answer = answer.Replace("&quot;", "\"");
            answer = answer.Replace("&gt;", ">");
            answer = answer.Replace("&lt;", "<");
            answer = answer.Replace("&amp;", "&");

            return answer;
        }

        #endregion

        #region XPath Functions

        /// <param name="namespacePrefix">A namespace prefix to associate with the Document.NamespaceURI namespace</param>
        public static XmlNamespaceManager GetNamespaceManager(this XmlNode node, string namespacePrefix)
        {
            XmlDocument doc = GetOwnerDocument(node);

            var nsm = new XmlNamespaceManager(doc.NameTable);
            if (doc.DocumentElement != null && doc.DocumentElement.NamespaceURI != null)
                nsm.AddNamespace(namespacePrefix, doc.DocumentElement.NamespaceURI);
            return nsm;
        }

        /// <summary>
        /// Translates a bare xpath expression to a namespaced one, using the supplied prefix,
        /// but only if the namespace manager defines that prefix
        /// </summary>
        public static string NamespaceXPath(this XmlNamespaceManager namespaceManager, string xpathExpression, string prefix)
        {
            if (namespaceManager.HasNamespace(prefix))
                return NamespaceXPath(xpathExpression, prefix);
            else
                return xpathExpression;
        }

        /// <summary>
        /// Translates a bare xpath expression to a namespaced one, using the supplied prefix
        /// </summary>
        /// <example>Converts //Project/@id to //xs:Project/@id</example>
        /// <param name="xpathExpression"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static string NamespaceXPath(this string xpathExpression, string prefix)
        {
            var leadingDot = Regex.IsMatch(xpathExpression, "^\\.");
            if (leadingDot)
                xpathExpression = Regex.Replace(xpathExpression, "^\\.", "");

            var res = Regex.Replace(xpathExpression, "(/{1,2}|\\[ *)([^@0-9])", "$1" + prefix + ":" + "$2");
            // If we specified a path without a leading /, make sure we namespace that too.
            res = Regex.Replace(res, "^([^@/])", prefix + ":" + "$1");

            if (leadingDot)
                res = "." + res;

            return res;
        }

        /// <summary>
        /// Get the namespace of the xpath expression
        /// </summary>
        /// <param name="namespaceManager"></param>
        /// <param name="xpathExpression"></param>
        /// <returns>uri as string or empty string when no namespace is given</returns>
        public static string GetNamespace(this XmlNamespaceManager namespaceManager, string xpathExpression)
        {
            var prefix = xpathExpression.FirstNodePrefix();
            if (namespaceManager.HasNamespace(prefix))
                return namespaceManager.LookupNamespace(prefix);
            else
                return string.Empty;
        }

        /// <summary>
        /// Get the namespace prefix from the first node of the xpath expression
        /// </summary>
        /// <param name="xpathExpression"></param>
        /// <returns></returns>
        public static string FirstNodePrefix(this string xpathExpression)
        {
            var match = Regex.Match(xpathExpression, "^/{0,2}([^:/\\[\\]@]+):");
            if (match.Success)
                return match.Groups[1].Value;
            else
                return string.Empty;
        }

        /// <summary>
        /// Get the name of the first node of the xpath expression (minus namespace prefix)
        /// </summary>
        /// <param name="xpathExpression"></param>
        /// <returns></returns>
        public static string FirstNodeName(this string xpathExpression)
        {
            var match = Regex.Match(xpathExpression, "^/{0,2}([^:@]*[:@])?([^/\\[\\]@]+)");
            if (match.Success)
                return match.Groups[2].Value;
            else
                return string.Empty;
        }

        /// <summary>
        /// Is the result of this xpath expression an attribute?
        /// </summary>
        /// <param name="xpathExpression"></param>
        /// <returns></returns>
        public static bool IsAttribute(this string xpathExpression)
        {
            // Strip out any predicates so they don't confuse us
            xpathExpression = Regex.Replace(xpathExpression, "\\[[^\\]]+\\]", "");

            // it's an attribute if it ends with .../@something or ..@something
            return Regex.IsMatch(xpathExpression, "/{0,2}@[^/@]+$");
        }

        /// <summary>
        /// Does this xpath expression contain a predicate?
        /// </summary>
        /// <param name="xpathExpression"></param>
        /// <returns></returns>
        public static bool HasPredicate(this string xpathExpression)
        {
            return Regex.IsMatch(xpathExpression, "\\[");
        }

        #endregion

        #region Getting values
        /// <summary>
        /// Returns the values of the nodes selected by the xpath expression
        /// </summary>
        /// <param name="document"></param>
        /// <param name="xpathExpresson"></param>
        /// <returns></returns>
        public static IEnumerable<string> Vals(this XmlNode document, string xpathExpresson)
        {
            var namespaceManager = GetNamespaceManager(document, "x");
            return Vals(document, namespaceManager.NamespaceXPath(xpathExpresson, "x"), namespaceManager);
        }

        /// <summary>
        /// Returns the values of the nodes selected by the xpath expression
        /// </summary>
        /// <param name="document"></param>
        /// <param name="xpathExpression"></param>
        /// <param name="namespaceManager"></param>
        /// <returns></returns>
        public static IEnumerable<string> Vals(this XmlNode document, string xpathExpression, XmlNamespaceManager namespaceManager)
        {
            foreach (XmlNode node in document.SelectNodes(xpathExpression, namespaceManager))
                if (node.Value == null && node.InnerText != string.Empty)
                    yield return node.InnerText;
                else
                    yield return node.Value;
        }

        /// <summary>
        /// Returns the value of the node selected by the xpath expression converted to another type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="document"></param>
        /// <param name="xpathExpression"></param>
        /// <returns></returns>
        public static T Val<T>(this XmlNode document, string xpathExpression)
        {
            return (T)Val(document, xpathExpression).ConvertTo<T>();
        }

        /// <summary>
        /// Like Val, but return the equivalent of "" if the node isn't found
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="document"></param>
        /// <param name="xpathExpression"></param>
        /// <returns></returns>
        public static T ValN<T>(this XmlNode document, string xpathExpression)
        {
            return (T)ValN(document, xpathExpression).ConvertTo<T>();
        }

        /// <summary>
        /// Like Val, but returns "" if the node is not found
        /// </summary>
        /// <param name="document"></param>
        /// <param name="xpathExpression"></param>
        /// <returns></returns>
        public static string ValN(this XmlNode document, string xpathExpression)
        {
            var vals = document.Vals(xpathExpression);
            if (vals.Any())
                return vals.First();
            else
                return "";
        }

        /// <summary>
        /// Returns the value of the node selected by the xpath expression
        /// </summary>
        /// <remarks>Throws an exception if more than one node is found, or if none are found</remarks>
        /// <param name="document"></param>
        /// <param name="xpathExpression"></param>
        /// <returns></returns>
        public static string Val(this XmlNode document, string xpathExpression)
        {
            var namespaceManager = GetNamespaceManager(document, "x");
            return Val(document, namespaceManager.NamespaceXPath(xpathExpression, "x"), namespaceManager);
        }

        /// <summary>
        /// Returns the value of the node selected by the xpath expression
        /// </summary>
        /// <remarks>Throws an exception if more than one node is found, or if none are found</remarks>
        /// <param name="document"></param>
        /// <param name="xpathExpression"></param>
        /// <param name="namespaceManager"></param>
        /// <returns></returns>
        public static string Val(this XmlNode document, string xpathExpression, XmlNamespaceManager namespaceManager)
        {
            return Vals(document, xpathExpression, namespaceManager).Single();
        }

        /// <summary>
        /// Returns whether or not the specified node exists
        /// </summary>
        /// <param name="document"></param>
        /// <param name="xpathExpression"></param>
        /// <returns></returns>
        public static bool Exists(this XmlNode document, string xpathExpression)
        {
            var namespaceManager = GetNamespaceManager(document, "x");
            return Exists(document, namespaceManager.NamespaceXPath(xpathExpression, "x"), namespaceManager);
        }

        /// <summary>
        /// Returns whether or not the specified node exists
        /// </summary>
        /// <param name="document"></param>
        /// <param name="xpathExpression"></param>
        /// <param name="namespaceManager"></param>
        /// <returns></returns>
        public static bool Exists(this XmlNode document, string xpathExpression, XmlNamespaceManager namespaceManager)
        {
            var list = document.SelectNodes(xpathExpression, namespaceManager);
            return list.Count > 0;
        }

        /// <summary>
        /// Equivalent to document.SelectNodes, but handles namespaces automatically
        /// </summary>
        /// <param name="document"></param>
        /// <param name="xpathExpression"></param>
        /// <returns></returns>
        public static IEnumerable<XmlNode> Nodes(this XmlNode document, string xpathExpression)
        {
            var namespaceManager = GetNamespaceManager(document, "x");
            var qualifiedExpression = namespaceManager.NamespaceXPath(xpathExpression, "x");
            return Nodes(document, qualifiedExpression, namespaceManager);
        }

        /// <summary>
        /// Equivalent to document.SelectNodes
        /// </summary>
        /// <param name="document"></param>
        /// <param name="xpathExpression"></param>
        /// <param name="namespaceManager"></param>
        /// <returns></returns>
        public static IEnumerable<XmlNode> Nodes(this XmlNode document, string xpathExpression, XmlNamespaceManager namespaceManager)
        {
            foreach (XmlNode node in document.SelectNodes(xpathExpression, namespaceManager))
                yield return node;
        }

        /// <summary>
        /// Equivalent to node.SelectNodes on a collection of elements.
        /// Handles namespaces automatically
        /// </summary>
        /// <param name="documents"></param>
        /// <param name="xpathExpression"></param>
        /// <returns></returns>
        public static IEnumerable<XmlNode> Nodes(this IEnumerable<XmlNode> documents, string xpathExpression)
        {
            if (!documents.Select(d => d.GetOwnerDocument()).AreAllEqual())
                throw new Exception("All the nodes must belong to the same document");

            var namespaceManager = GetNamespaceManager(documents.First(), "x");
            return Nodes(documents, namespaceManager.NamespaceXPath(xpathExpression, "x"), namespaceManager);
        }


        /// <summary>
        /// Equivalent to node.SelectNodes on a collection of elements
        /// </summary>
        /// <param name="documents"></param>
        /// <param name="xpathExpression"></param>
        /// <param name="namespaceManager"></param>
        /// <returns></returns>
        public static IEnumerable<XmlNode> Nodes(this IEnumerable<XmlNode> documents, string xpathExpression, XmlNamespaceManager namespaceManager)
        {
            foreach (XmlNode doc in documents)
                foreach (XmlNode child in doc.Nodes(xpathExpression, namespaceManager))
                    yield return child;
        }

        /// <summary>
        /// Calls Nodes, but returns the single result (or throws an exception)
        /// </summary>
        /// <param name="document"></param>
        /// <param name="xpathExpression"></param>
        /// <returns></returns>
        public static XmlNode Node(this XmlNode document, string xpathExpression)
        {
            return Nodes(document, xpathExpression).Single();
        }

        #endregion

        #region Setting values
        /// <summary>
        /// Creates a new node as a direct child of the node and returns it
        /// </summary>
        /// <param name="node"></param>
        /// <param name="nodeName"></param>
        /// <returns>The new node</returns>
        public static XmlNode AddChild(this XmlNode node, string nodeName)
        {
            return AddChild(node, nodeName, "");
        }

        /// <summary>
        /// Creates a new node as a direct child of the node and returns it
        /// </summary>
        /// <param name="node"></param>
        /// <param name="nodeName"></param>
        /// <param name="value"></param>
        /// <returns>The new node</returns>
        public static XmlNode AddChild(this XmlNode node, string nodeName, string value)
        {
            var namespaceManager = GetNamespaceManager(node, "x");
            return AddChild(node, nodeName, value, namespaceManager);
        }

        /// <summary>
        /// Creates a new node as a direct child of the node and returns it
        /// </summary>
        /// <param name="node"></param>
        /// <param name="nodeName"></param>
        /// <param name="namespaceManager"></param>
        /// <returns>The new node</returns>
        public static XmlNode AddChild(this XmlNode node, string nodeName, string value, XmlNamespaceManager namespaceManager)
        {
            return _createNode(node, nodeName, value, namespaceManager);
        }

        /// <summary>
        /// Sets the value of multiple nodes
        /// </summary>
        /// <param name="document"></param>
        /// <param name="xpathExpression"></param>
        /// <param name="value"></param>
        /// <returns>The original node, so calls to SetVals can be chained</returns>
        public static XmlNode SetVals(this XmlNode document, string xpathExpression, string value)
        {
            var namespaceManager = GetNamespaceManager(document, "x");
            return SetVals(document, namespaceManager.NamespaceXPath(xpathExpression, "x"), value, namespaceManager);
        }

        /// <summary>
        /// Sets the value of multiple nodes
        /// </summary>
        /// <param name="document"></param>
        /// <param name="xpathExpression"></param>
        /// <param name="value"></param>
        /// <param name="namespaceManager"></param>
        /// <returns>The original node, so calls to SetVals can be chained</returns>
        public static XmlNode SetVals(this XmlNode document, string xpathExpression, string value, XmlNamespaceManager namespaceManager)
        {
            return _setVals(document, xpathExpression, value, namespaceManager, true);
        }

        /// <summary>
        /// Sets the value of a single node - creates nodes if they don't exist
        /// </summary>
        /// <param name="document"></param>
        /// <param name="xpathExpression"></param>
        /// <param name="value"></param>
        /// <returns>The original node, so calls to SetVal can be chained</returns>
        public static XmlNode SetVal(this XmlNode document, string xpathExpression, string value)
        {
            var namespaceManager = GetNamespaceManager(document, "x");
            return SetVal(document, namespaceManager.NamespaceXPath(xpathExpression, "x"), value, namespaceManager);
        }

        /// <summary>
        /// Sets the value of a single node - creates nodes if they don't exist
        /// </summary>
        /// <param name="document"></param>
        /// <param name="xpathExpression"></param>
        /// <param name="value"></param>
        /// <param name="namespaceManager"></param>
        /// <returns>The original node, so calls to SetVal can be chained</returns>
        public static XmlNode SetVal(this XmlNode document, string xpathExpression, string value, XmlNamespaceManager namespaceManager)
        {
            _ensurePathExists(document, xpathExpression, namespaceManager);
            return _setVals(document, xpathExpression, value, namespaceManager, false);
        }

        /// <summary>
        /// Creates any nodes needed to make the path valid
        /// </summary>
        /// <param name="document"></param>
        /// <param name="xpathExpression"></param>
        /// <param name="namespaceManager"></param>
        private static void _ensurePathExists(XmlNode document, string xpathExpression, XmlNamespaceManager namespaceManager)
        {
            // Break up the path into nodes, and validate each node            
            var paths = xpathExpression.Tokenize("^(\\.|(/{1,2}[^/]+))");
            IEnumerable<XmlNode> currentElements = new List<XmlNode>() { document };

            foreach (var path in paths)
            {
                // Don't do anything for . - it just selects the current node
                if (path == ".")
                    continue;

                var foundElements = currentElements.Nodes("." + path, namespaceManager);
                if (!foundElements.Any())
                {
                    // We can chop .// off the front of the path, because we just want the name of the node to create
                    // and if we haven't found it anywhere, we'll just create it under the current node
                    _createNode(currentElements.First(), path.ChopStart(".", "//"), "", namespaceManager);
                    foundElements = currentElements.Nodes("." + path, namespaceManager);
                    if (!foundElements.Any())
                        throw new Exception("We couldn't find the node we just created! Tried to create '" + path + "' in '" + xpathExpression + "'");
                }
                currentElements = foundElements;
            }
        }

        /// <summary>
        /// Creates a new node, gives it the supplied value, and returns it
        /// </summary>
        /// <param name="node"></param>
        /// <param name="nodeName"></param>
        /// <param name="value"></param>
        /// <param name="namespaceManager"></param>
        /// <returns></returns>
        private static XmlNode _createNode(XmlNode node, string nodeName, string value, XmlNamespaceManager namespaceManager)
        {
            if (nodeName.IsAttribute()) // it's an attribute
            {
                XmlAttribute att = node.GetOwnerDocument().CreateAttribute(
                    nodeName.FirstNodeName(),
                    namespaceManager.GetNamespace(nodeName));
                att.Value = value;
                node.Attributes.Append(att);

                return att;
            }
            else
            {
                // it's an element
                var atts = new Dictionary<string, string>();
                if (nodeName.HasPredicate())
                    atts = getAttsFromPredicate(nodeName);
                
                XmlElement elem = node.GetOwnerDocument().CreateElement(
                    nodeName.FirstNodeName(),
                    namespaceManager.GetNamespace(nodeName));
                elem.InnerText = value;
                // Set the attributes
                atts.Each(kv => elem.SetVal("/@" + kv.Key, kv.Value));
                node.AppendChild(elem);

                return elem;
            }
        }

        /// <summary>
        /// Gets the attributes that are in the supplied pathFragment. Throws an exception if the
        /// predicate is not parsable
        /// </summary>
        /// <example>"/add[@key = 'foobar']" returns (["key","foobar"])</example>        
        /// <param name="pathFragment"></param>
        /// <returns></returns>
        private static Dictionary<string, string> getAttsFromPredicate(string nodeName)
        {
            // Grab just the predicate part of the path (the part in []
            var match = Regex.Match(nodeName, "\\[([^\\]]+)\\]");
            if (!match.Success)
                throw new Exception("I don't know how to create a node from this xpath fragment: '" + nodeName + "'");

            var predicate = match.Groups[1].Value;
            var simplePredMatch = Regex.Match(predicate, "\\s*@([^\\s]+)\\s*=\\s*([^\\s]+)\\s*");
            if (simplePredMatch.Success)
            {
                var res = new Dictionary<string, string>();
                res.Add(simplePredMatch.Groups[1].Value, simplePredMatch.Groups[2].Value.ChopStart("'").ChopEnd("'"));
                return res;
            }
            else
                throw new Exception("I don't know how to create a node from this predicate: '" + predicate + "'");

        }


        /// <summary>
        /// Sets the values of nodes conforming to the xpathExpression. NOTE: Returns the document element originally passed in,
        /// so that things can be chained easily
        /// </summary>
        /// <param name="document"></param>
        /// <param name="xpathExpression"></param>
        /// <param name="value"></param>
        /// <param name="namespaceManager"></param>
        /// <param name="allowUpdateMultiple"></param>
        /// <returns></returns>
        private static XmlNode _setVals(XmlNode document, string xpathExpression, string value, XmlNamespaceManager namespaceManager, bool allowUpdateMultiple)
        {
            var nodes = document.SelectNodes(xpathExpression, namespaceManager);
            if (!allowUpdateMultiple && nodes.Count > 1)
                throw new Exception("Not allowed to update multiple rows");
            if (nodes.Count == 0)
                throw new Exception("No nodes found that matched the expression '" + xpathExpression + "'");

            foreach (XmlNode node in nodes)
                if (node is XmlElement)
                    node.InnerText = value;
                else if (node is XmlAttribute)
                    node.Value = value;

            return document;
        }

        #endregion

        #region As Dictionary Functions
        /// <summary>
        /// Get the names of the child elements of this node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetElementNames(this XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
                yield return child.Name;
        }

        /// <summary>
        /// Get the names of the child elements of this document
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetElementNames(this XmlDocument doc)
        {
            return GetElementNames(doc.FirstChild);

        }

        /// <summary>
        /// Are the documents identical when comparing the supplied child elements?
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="other"></param>
        /// <param name="elementNames"></param>
        /// <returns></returns>
        public static bool XmlIdenticalOnFields(this XmlDocument doc, XmlDocument other, IEnumerable<string> elementNames)
        {
            return XmlIdenticalOnFields(doc.FirstChild, other.FirstChild, elementNames);
        }

        /// <summary>
        /// Are the nodes identical when comparing the supplied child elements?
        /// </summary>
        /// <param name="node"></param>
        /// <param name="other"></param>
        /// <param name="elementNames"></param>
        /// <returns></returns>
        public static bool XmlIdenticalOnFields(this XmlNode node, XmlNode other, IEnumerable<string> elementNames)
        {
            foreach (var field in elementNames)
                if (node.Val(field) != other.Val(field))
                    return false;

            return true;

        }


        #endregion
    }
}
