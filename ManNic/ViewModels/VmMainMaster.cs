using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using HQ4P.Tools.ManNic.NicManagement;

namespace HQ4P.Tools.ManNic.ViewModels
{
    internal class VmMainMaster : INotifyPropertyChanged
    {
        private readonly NicCollector _nicCollector;

        public VmNetworkAdapterList VmAdapterList { get; private set; }
        public VmNicData VmNicData { get; private set; }
        public VmTreeView VmTreeView { get; private set; }

        private string _actState;
        public string ActState
        {
            get => _actState;
            set
            {
                _actState = value;
                OnPropertyChanged(nameof(ActState));
            }
        }

        public ICommand OnNicChangedCommand => new RelayCommand<int>(NicChanged);


        internal VmMainMaster(NicCollector nicCollector)
        {
            _nicCollector = nicCollector;
            VmAdapterList = new VmNetworkAdapterList(nicCollector);
            VmNicData = new VmNicData(nicCollector.NetworkAdapter[0], SetState);
            VmTreeView = new VmTreeView(SetState);

            SetState("Starting up...");

        }


        private void SetState(string state)
        {
            _actState = state;
            OnPropertyChanged(nameof(ActState));

        }

        private void NicChanged(int index)
        {
            VmNicData.NicData = _nicCollector.NetworkAdapter[index];
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChangedEvent([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}