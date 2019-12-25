using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace MPS.Common.Model
{
    [Serializable]
    public class Chip : ObservableObject
    {
        private string _chipAddr;
        [XmlAttribute]
        public string ChipAddr
        {
            get
            {
                return this._chipAddr;
            }
            set
            {
                this._chipAddr = value;
                RaisePropertyChanged("ChipAddr");
            }
        }
        [XmlAttribute]
        public string ChipName { get; set; }

        private bool? _isAddrAvalible;
        [XmlIgnore]
        public bool? IsAddrAvalible
        {
            get { return _isAddrAvalible; }
            set
            {
                _isAddrAvalible = value;
                RaisePropertyChanged("IsAddrAvalible");
            }
        }
        public ObservableCollection<Page> Pages { get; set; }
    }
}

