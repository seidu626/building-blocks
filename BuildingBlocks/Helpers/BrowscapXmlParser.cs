using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using BuildingBlocks.Infrastructure;

namespace BuildingBlocks.Helpers;

public class BrowscapXmlHelper
{
    private readonly IFileProvider _fileProvider;
    private Regex _crawlerUserAgentsRegexp;

    public BrowscapXmlHelper(string userAgentStringsPath, string crawlerOnlyUserAgentStringsPath, IFileProvider fileProvider)
    {
        _fileProvider = fileProvider;
        Initialize(userAgentStringsPath, crawlerOnlyUserAgentStringsPath);
    }

    private static bool IsBrowscapItemCrawler(XElement browscapItem)
    {
        var crawlerElement = browscapItem.Elements("item").FirstOrDefault(e => e.Attribute("name")?.Value == "Crawler");
        return crawlerElement != null && crawlerElement.Attribute("value")?.Value.ToLower() == "true";
    }

    private static string ToRegexp(string str)
    {
        var sb = new StringBuilder(Regex.Escape(str));
        sb.Replace("&amp;", "&").Replace("\\?", ".").Replace("\\*", ".*?");
        return $"^{sb}$";
    }

    private void Initialize(string userAgentStringsPath, string crawlerOnlyUserAgentStringsPath)
    {
        List<XElement> crawlerItems = null;
        bool loadedFromFullFile = false;

        if (!string.IsNullOrEmpty(crawlerOnlyUserAgentStringsPath) && _fileProvider.FileExists(crawlerOnlyUserAgentStringsPath))
        {
            using (var reader = new StreamReader(crawlerOnlyUserAgentStringsPath))
            {
                var root = XDocument.Load(reader).Root;
                crawlerItems = root?.Elements("browscapitem").ToList();
            }
        }

        if (crawlerItems == null || !crawlerItems.Any())
        {
            using (var reader = new StreamReader(userAgentStringsPath))
            {
                var root = XDocument.Load(reader).Root;
                var browserCapItems = root?.Element("browsercapitems");
                crawlerItems = browserCapItems?.Elements("browscapitem").Where(IsBrowscapItemCrawler).ToList();
                loadedFromFullFile = true;
            }
        }

        if (crawlerItems == null || !crawlerItems.Any())
        {
            throw new Exception("Incorrect file format");
        }

        _crawlerUserAgentsRegexp = new Regex(string.Join("|",
            crawlerItems.Select(e => e.Attribute("name"))
                .Where(attr => !string.IsNullOrEmpty(attr?.Value))
                .Select(attr => ToRegexp(attr.Value))));

        if (!string.IsNullOrEmpty(crawlerOnlyUserAgentStringsPath) && !_fileProvider.FileExists(crawlerOnlyUserAgentStringsPath) && loadedFromFullFile)
        {
            using (var writer = new StreamWriter(crawlerOnlyUserAgentStringsPath))
            {
                var crawlerItemsRoot = new XElement("browsercapitems");
                foreach (var item in crawlerItems)
                {
                    foreach (var element in item.Elements().ToList())
                    {
                        if (!string.Equals(element.Attribute("name")?.Value, "crawler", StringComparison.OrdinalIgnoreCase))
                        {
                            element.Remove();
                        }
                    }
                    crawlerItemsRoot.Add(item);
                }
                crawlerItemsRoot.Save(writer);
            }
        }
    }

    public bool IsCrawler(string userAgent)
    {
        return _crawlerUserAgentsRegexp.IsMatch(userAgent);
    }
}