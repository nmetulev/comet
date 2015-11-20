using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sample.Mvvm;

namespace Sample.Data
{
    public class Email: BindableBase
    {
        private string subject = default(string);
        public string Subject { get { return subject; } set { Set(ref subject, value); } }

        private bool? read = default(bool);
        public bool? Read { get { return read; } set { Set(ref read, value); } }

        private string from = default(string);
        public string From { get { return from; } set { Set(ref from, value); } }

        private string content = default(string);
        public string Content { get { return content; } set { Set(ref content, value); } }

        private DateTime time = default(DateTime);
        public DateTime Time { get { return time; } set { Set(ref time, value); } }

        Mvvm.Command _toggleRead = default(Mvvm.Command);
        public Mvvm.Command ToggleRead { get { return _toggleRead ?? (_toggleRead = new Mvvm.Command(ExecuteToggleReadCommand, CanExecuteToggleReadCommand)); } }
        private bool CanExecuteToggleReadCommand() { return true; }

        private void ExecuteToggleReadCommand()
        {
            Read = !Read;
        }
    }
}
