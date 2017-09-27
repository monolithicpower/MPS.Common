using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace MPS.Common.Model
{
    [Serializable]
    public class Page : ObservableObject
    {

        [XmlAttribute]
        public string PageName { get; set; }
        //[XmlAttribute]
        //public string PageAddress { get; set; }
        private bool _isSelected;
        [XmlIgnore]
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }

        public ObservableCollection<GroupItemInfo> GroupItems { get; set; }
    }
}
