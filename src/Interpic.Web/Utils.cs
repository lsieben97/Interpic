﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpic.Web
{
    public static class Utils
    {
        public static string GetFriendlyHtmlName(HtmlNode node)
        {
            string name = node.Name;
            if (string.IsNullOrWhiteSpace(node.Id) == false)
            {
                name += "#" + node.Id;
            }

            if (node.GetClasses() != null)
            {
                foreach(string className in node.GetClasses())
                {
                    name += " ." + className;
                }
            }
            return name;
        }
    }
}
