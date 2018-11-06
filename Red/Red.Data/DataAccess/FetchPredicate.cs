using System;
using System.Collections.Generic;
using System.Xml;
using Red.Core;

namespace Red.Data.DataAccess
{
    public class FetchPredicate : ILoadable<XmlElement>, ISavable<XmlElement>
    {       
        public virtual string Text { get; private set; }
        public virtual object[] Arguments { get; private set; }

        public FetchPredicate()
        {
        }

        public FetchPredicate(string text, params object[] arguments) : this()
        {
            Text = text;
            Arguments = arguments;
        }

        public void Load(XmlElement source)
        {
            Sanity.Enforce<ArgumentException>(
                source.LocalName == nameof(FetchPredicate), 
                $"Element must be named {nameof(FetchPredicate)}");

            Text = source.SelectSingleNode(nameof(Text)).InnerText;
            var arguments = new List<object>();
            foreach (XmlElement element in source.SelectNodes("Arguments/Value"))
            {
                arguments.Add(element.InnerText);
            }

            Arguments = arguments.ToArray();
        }

        public void Save(XmlElement target)
        {
            var document = target.OwnerDocument;
            var element = document.CreateElement(nameof(FetchPredicate));
            var text = document.CreateElement(nameof(Text));
            element.AppendChild(text);
            var argumentsElement = document.CreateElement("Arguments");
            foreach (var argument in Arguments)
            {
                var value = document.CreateElement("Value");
                value.AppendChild(document.CreateTextNode(Convert.ToString(argument)));
                argumentsElement.AppendChild(value);
            }

            element.AppendChild(argumentsElement);
            
            target.AppendChild(element);
        }
    }
}