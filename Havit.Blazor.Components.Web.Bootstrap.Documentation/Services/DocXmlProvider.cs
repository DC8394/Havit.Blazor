﻿using System.Reflection;
using System.Xml.XPath;
using LoxSmoke.DocXml;

namespace Havit.Blazor.Components.Web.Bootstrap.Documentation.Services;
public class DocXmlProvider : IDocXmlProvider
{
	private readonly Dictionary<string, DocXmlReader> docXmlReaders = new();

	public DocXmlReader GetDocXmlReaderFor(string docXmlResourceName)
	{
		if (docXmlReaders.TryGetValue(docXmlResourceName, out var result))
		{
			return result;
		}

		var result2 = LoadDocXmlReader(docXmlResourceName);
		docXmlReaders.Add(docXmlResourceName, result2);

		return result2;
	}

	private DocXmlReader LoadDocXmlReader(string resourceName)
	{
		var assembly = Assembly.GetExecutingAssembly();

		resourceName = assembly.GetManifestResourceNames()
			.Single(str => str.EndsWith(resourceName));

		using (Stream stream = assembly.GetManifestResourceStream(resourceName))
		using (StreamReader reader = new StreamReader(stream))
		{
			TextReader textReader = new StringReader(reader.ReadToEnd());   // TODO async?
			XPathDocument xPathDocument = new(textReader);

			return new(xPathDocument);
		}
	}
}
