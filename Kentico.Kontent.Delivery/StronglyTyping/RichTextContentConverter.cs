﻿using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json.Linq;
using AngleSharp.Parser.Html;
using Kentico.Kontent.Delivery.Abstractions;
using Kentico.Kontent.Delivery.ContentLinks;

namespace Kentico.Kontent.Delivery.StrongTyping
{
    internal class RichTextContentConverter : IPropertyValueConverter
    {
        public object GetPropertyValue(PropertyInfo property, JToken elementData, ResolvingContext context)
        {
            if (!typeof(IRichTextContent).IsAssignableFrom(property.PropertyType))
            {
                throw new InvalidOperationException($"Type of property {property.Name} must implement {nameof(IRichTextContent)} in order to receive rich text content.");
            }

            var element = ((JObject)elementData);
            if (element == null)
            {
                return null;
            }

            var links = element.Property("links")?.Value;
            var images = element.Property("images").Value;
            var value = element.Property("value")?.Value?.ToObject<string>();

            // Handle rich_text link resolution
            if (links != null && elementData != null && context.ContentLinkUrlResolver != null)
            {
                value = new ContentLinkResolver(context.ContentLinkUrlResolver).ResolveContentLinks(value, links);
            }

            var blocks = new List<IRichTextBlock>();

            var htmlInput = new HtmlParser().Parse(value);
            foreach (var block in htmlInput.Body.Children)
            {
                if (block.TagName?.Equals("object", StringComparison.OrdinalIgnoreCase) == true && block.GetAttribute("type") == "application/kenticocloud" && block.GetAttribute("data-type") == "item")
                {
                    var codename = block.GetAttribute("data-codename");
                    blocks.Add(new InlineContentItem { ContentItem = context.GetLinkedItem(codename) });
                }
                else if (block.TagName?.Equals("figure", StringComparison.OrdinalIgnoreCase) == true)
                {
                    var img = block.Children.FirstOrDefault(child => child.TagName?.Equals("img", StringComparison.OrdinalIgnoreCase) == true);
                    if (img != null)
                    {
                        var assetId = img.GetAttribute("data-asset-id");
                        var asset = images[assetId];
                        blocks.Add(new InlineImage
                        {
                            Url = asset.Value<string>("url"),
                            Description = asset.Value<string>("description"),
                            Height = asset.Value<int>("height"),
                            Width = asset.Value<int>("width")
                        });
                    }
                }
                else
                {
                    blocks.Add(new HtmlContent { Html = block.OuterHtml });
                }
            }

            return new RichTextContent
            {
                Blocks = blocks
            };
        }
    }
}
