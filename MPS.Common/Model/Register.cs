using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Xml.Serialization;

namespace MPS.Common.Model
{
    public class Register : ObservableObject
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string Right { get; set; }
        [XmlAttribute]
        public string DataType { get; set; }
     
        private int _value;
        [XmlAttribute]
        public int Value
        {
            get
            {
                return this._value;
            }
            set
            {
                this._value = value;
                RaisePropertyChanged("Value");
            }
        }
        [XmlAttribute]
        public byte Address { get; set; }

        //[XmlAttribute]
        //public byte Order { get; set; }

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

        [XmlArrayItem]
        public List<BitControl> BitControls { get; set; }

    }
}
