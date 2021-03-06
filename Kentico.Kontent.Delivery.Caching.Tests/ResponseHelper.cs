﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kentico.Kontent.Delivery.Caching.Tests
{
    public static class ResponseHelper
    {
        internal static object CreateItemResponse(object item, IEnumerable<(string codename, object item)> modularContent = null) => new
        {
            item,
            modular_content = modularContent?.ToDictionary(x => x.codename, x => x.item) ?? new Dictionary<string, object>()
        };

        internal static object CreateItemsResponse(ICollection<object> items, IEnumerable<(string codename, object item)> modularContent = null) => new
        {
            items,
            modular_content = modularContent?.ToDictionary(x => x.codename, x => x.item) ?? new Dictionary<string, object>(),
            pagination = new
            {
                skip = 0,
                limit = 0,
                count = items.Count,
                next_page = ""
            }
        };

        internal static object CreateItemsFeedResponse(ICollection<object> items, IEnumerable<(string codename, object item)> modularContent = null) => new
        {
            items,
            modular_content = modularContent?.ToDictionary(x => x.codename, x => x.item) ?? new Dictionary<string, object>()
        };

        internal static object CreateTypesResponse(ICollection<object> types) => new
        {
            types,
            pagination = new
            {
                skip = 0,
                limit = 0,
                count = types.Count,
                next_page = ""
            }
        };

        internal static object CreateTaxonomiesResponse(ICollection<object> taxonomies) => new
        {
            taxonomies,
            pagination = new
            {
                skip = 0,
                limit = 0,
                count = taxonomies.Count,
                next_page = ""
            }
        };

        internal static object CreateItem(string codename, string value = null) => new
        {
            elements = new Dictionary<string, object>
            {
                {
                    "title",
                    new
                    {
                        type = "text",
                        name= "Title",
                        value= value ?? string.Empty
                    }
                }
            },
            system = new
            {
                id = Guid.NewGuid().ToString(),
                codename,
                type = "test_item"
            }
        };

        internal static (string codename, object item) CreateComponent()
        {
            // Components have substring 01 in its id starting at position 14.
            // xxxxxxxx-xxxx-01xx-xxxx-xxxxxxxxxxxx
            var id = Guid.NewGuid().ToString();
            id = $"{id.Substring(0, 14)}01{id.Substring(16)}";
            var codename = $"n{id}";
            return (
                codename,
                new
                {
                    elements = new Dictionary<string, object>(),
                    system = new
                    {
                        id,
                        codename
                    }
                });
        }

        internal static object CreateType(string codename, string elementName = "Title") => new
        {
            elements = new Dictionary<string, object>
            {
                {
                    elementName.ToLowerInvariant(),
                    new
                    {
                        type = "text",
                        name= elementName
                    }
                }
            },
            system = new
            {
                id = Guid.NewGuid().ToString(),
                codename,
            }
        };

        internal static object CreateTaxonomy(string codename, IEnumerable<string> terms) => new
        {
            terms = (terms ?? Enumerable.Empty<string>()).Select(t => new
            {
                codename = t,
                name = t,
                terms = Enumerable.Empty<object>()
            }),
            system = new
            {
                id = Guid.NewGuid().ToString(),
                codename
            }
        };

        internal static object CreateContentElement(string codename, string name) => new
        {
            type = "text",
            name,
            codename
        };
    }
}
