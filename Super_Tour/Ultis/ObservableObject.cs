using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Super_Tour.Ultis
{
    public class ObservableObject :INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void  OnPropertyChanged(string name)
        {
            if(PropertyChanged!=null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));  
            }    
        }
    }
}
