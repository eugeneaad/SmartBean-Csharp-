using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace SmartBEAM
{
    [Table]
    public class SmartBEAMDB : INotifyPropertyChanged, INotifyPropertyChanging
    {
        private string _time;
        private string _channel;
        private int _id;

        
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        [Column(IsPrimaryKey = true, IsDbGenerated = true,
                DbType = "INT NOT NULL Identity", CanBeNull = false,
                AutoSync = AutoSync.OnInsert)]
        
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                if (_id != value)
                {
                    NotifyPropertyChanging("Id");
                    _id = value;
                    NotifyPropertyChanged("Id");
                }
            }
        }
        
        [Column]
        public string tableTime
        {
            get
            {
                return _time;
            }
            set
            {
                if (_time != value)
                {
                    NotifyPropertyChanging("tableTime");
                    _time = value;
                    NotifyPropertyChanged("tableTime");
                }
            }
        }

        [Column]
        public string tableChannel
        {
            get
            {
                return _channel;
            }
            set
            {
                if (_channel != value)
                {
                    NotifyPropertyChanging("tableChannel");
                    _channel = value;
                    NotifyPropertyChanged("tableChannel");
                }
            }
        }

       
        
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new
                    PropertyChangedEventArgs(propertyName));
            }
        }

        //---used to notify the data context that a data context property is 
        // about to change---
        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new
                    PropertyChangingEventArgs(propertyName));
            }
        }

    }
}


