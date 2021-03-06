﻿using GauntletLeaderboard.Core;
using GauntletLeaderboard.Core.Extensions;
using Humanizer;
using Nancy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Web;

namespace GauntletLeaderboard.Api.Extensions
{
    public static class NancyModuleExtensions
    {

        public static dynamic PrepareResult<T>(this INancyModule module, IPagedResult<T> model, Query query, Func<T, string> linkFactory = null)
        {
            var root = model.GetType().GetInnerTypeName().Pluralize();
            var url = new UriBuilder(module.Request.Url);
            var queryString = HttpUtility.ParseQueryString(module.Request.Url.Query);
            var result = new Dictionary<string, dynamic>();
            dynamic links = new ExpandoObject();
            dynamic meta = new ExpandoObject();

            if (query.PageSize.HasValue)
                queryString.Set("pageSize", model.PageSize.ToString());

            if (model.Next != null)
            {
                queryString.Set("page", model.Next.Value.ToString());
                url.Query = queryString.AllKeys.Select(a => a + "=" + HttpUtility.UrlEncode(queryString[a])).JoinWith("&");
                links.Next = url.ToString();
            }

            if (model.Previous != null)
            {
                queryString.Set("page", model.Previous.Value.ToString());
                url.Query = queryString.AllKeys.Select(a => a + "=" + HttpUtility.UrlEncode(queryString[a])).JoinWith("&");
                links.Previous = url.ToString();
            }

            queryString.Remove("page");
            url.Query = queryString.AllKeys.Select(a => a + "=" + HttpUtility.UrlEncode(queryString[a])).JoinWith("&");
            links.First = url.ToString();

            queryString.Set("page", model.Last.ToString());
            url.Query = queryString.AllKeys.Select(a => a + "=" + HttpUtility.UrlEncode(queryString[a])).JoinWith("&");
            links.Last = url.ToString();

            meta.total = model.TotalItemCount;

            result[root] = model.Page.Select(m => AddLinks<T>(module, m, linkFactory));
            result["links"] = links;
            result["meta"] = meta;

            return result;
        }

        public static dynamic PrepareResult<T>(this INancyModule module, IEnumerable<T> model, Func<T, string> linkFactory = null)
        {
            var root = model.GetType().GetInnerTypeName().Pluralize();
            var result = new Dictionary<string, dynamic>();

            result[root] = model.Select(m => AddLinks(module, m, linkFactory));

            return result;
        }

        public static dynamic PrepareResult<T>(this INancyModule module, T model, Func<T, string> linkFactory = null)
        {
            var typeName = model.GetType().Name;
            var result = new Dictionary<string, dynamic>();

            result[typeName] = AddLinks(module, model, linkFactory);

            return result;
        }

        private static dynamic AddLinks<T>(INancyModule module, T model, Func<T, string> linkFactory)
        {
            IDictionary<string, object> result = new ExpandoObject();
            IDictionary<string, object> links = new ExpandoObject();
            var type = model.GetType();

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(type))
            {
                if (linkFactory != null && property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    var path = linkFactory(model);

                    if (!path.StartsWith("/"))
                        path =  string.Format("{0}/{1}", module.ModulePath, path);

                    links.Add(property.Name, path.ToLower());
                }
                else
                    result.Add(property.Name, property.GetValue(model));
            }

            if (links.Keys.Any())
                result["links"] = links;

            return result;
        }
    }
}