using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Interpic.Extensions
{
    public interface IHtmlNodeSelector
    {
        XmlNode SelectedNode { get; set; }

        void LoadXmlNodes(XmlNode baseNode);
        void ShowSelector();
    }
}
