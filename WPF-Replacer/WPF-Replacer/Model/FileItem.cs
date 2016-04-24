using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Replacer.Model
{
    public class FileItem : ObservableObject
    {
        public string Name { get; set; }

        public string FullName { get; set; }

        public string Extension { get; set; }

        private int _count;

        public int Count
        {
            get { return _count; }
            set {
                _count = value;
                RaisePropertyChanged("Count");
            }
        }


        private DealStatus _status;

        public DealStatus Status
        {
            get { return _status; }
            set
            {
                _status = value;
                RaisePropertyChanged("Status");
            }
        }

        public FileItem()
        {

        }

        public FileItem(string file)
        {
            Extension = Path.GetExtension(file);
            Name = Path.GetFileNameWithoutExtension(file);
            FullName = file;
        }
    }
}
