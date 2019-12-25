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

        public string Product { get; set; }

        public string BaseProduct { get; set; }

        public string CreatedTool { get; set; }

        public string CreatedDate { get; set; }

        public string LastModifiedTool { get; set; }
        public string CreatedToolVersion { get; set; }

        public string LastModifiedToolVersion { get; set; }

        public string LastModifiedDate { get; set; }

        public string SpecVersion { get; set; }

    }
}
