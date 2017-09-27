using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace MPS.Common.Model
{
    [Serializable]
    public class ProjectConfig
    {
        [XmlArrayItem]
        public ObservableCollection<Chip> Chips { get; set; }
    }
}
