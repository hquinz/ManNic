using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using HQ4P.Tools.ManNic.NicManagement;
using HQ4P.Tools.ManNic.View.Tools;

namespace HQ4P.Tools.ManNic.View.Models
{
    internal class VmNetworkAdapterList : INotifyPropertyChanged
    {
        private readonly NicCollector _nicCollector;
        private int _index;
        public ObservableCollection<string> NetworkAdapter { get; private set; }

        public int Index
        {
            get => _index;
            set
            {
                _index = value;
                OnPropertyChanged(nameof(Index));
            }
        }

        public ICommand Refresh => new RelayCommand<object>(GetNetworkAdapter);

        public VmNetworkAdapterList(NicCollector nicCollector)
        {
            _nicCollector = nicCollector;
            GetNetworkAdapter(true);

        }


        private void GetNetworkAdapter(object parameter)
        {
            if (parameter == null) _nicCollector.GetNetworkAdapters();

            NetworkAdapter = new ObservableCollection<string>();
            foreach (var nic in _nicCollector.NetworkAdapter) NetworkAdapter.Add(nic.Id);
            OnPropertyChanged(nameof(NetworkAdapter));
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
