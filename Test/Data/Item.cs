using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Mvvm;

namespace Test.Data
{
    public class Item: BindableBase
    {
        private string _Title = default(string);
        public string Title { get { return _Title; } set { Set(ref _Title, value); } }

        private bool? _IsFavorite = default(bool);
        public bool? IsFavorite { get { return _IsFavorite; } set { Set(ref _IsFavorite, value); } }

        Mvvm.Command _ToggleFavorite = default(Mvvm.Command);
        public Mvvm.Command ToggleFavorite { get { return _ToggleFavorite ?? (_ToggleFavorite = new Mvvm.Command(ExecuteToggleFavoriteCommand, CanExecuteToggleFavoriteCommand)); } }
        private bool CanExecuteToggleFavoriteCommand() { return true; }
        private void ExecuteToggleFavoriteCommand()
        {
            IsFavorite = !IsFavorite;
        }
    }
}
