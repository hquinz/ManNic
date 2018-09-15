using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using HQ4P.Tools.ManNic.NicManagement.API;

namespace HQ4P.Tools.ManNic.ViewModels
{
    internal class VmNicData : INotifyPropertyChanged
    {
        private const string TxtNicDataOk = "Network Data OK";
        #region Propertys

        private INicConfigurator _nicData;
        public INicConfigurator NicData
        {
            get => _nicData;
            set
            {
                _nicData = value;
                NicDataToLocal();
            }
        }
        public string MacAddress => _nicData?.MacAddress ?? "";

        private bool _ipEnabled;
        public bool IpEnabled
        {
            get => _ipEnabled;
            set
            {
                _ipEnabled = value;
                OnPropertyChanged(nameof(IpEnabled));
                OnPropertyChanged(nameof(EnableIpSetting));
            }
        }
        private bool _dhcpEnabled;
        public bool DhcpEnabled
        {
            get => _dhcpEnabled;
            set
            {
                _dhcpEnabled = value;
                OnPropertyChanged(nameof(DhcpEnabled));
                OnPropertyChanged(nameof(EnableIpSetting));
            }
        }

        public bool EnableIpSetting => _ipEnabled && !_dhcpEnabled;

        private string _ipV4Address;
        public string IpV4Address
        {
            get => _ipV4Address;
            set
            {
                _ipV4Address = value;
                OnPropertyChanged(nameof(IpV4Address));
                OnPropertyChanged(nameof(NicDataOk));
            }
        }
        private string _ipV4SubnetMask;
        public string IpV4SubnetMask
        {
            get => _ipV4SubnetMask;
            set
            {
                _ipV4SubnetMask = value;
                OnPropertyChanged(nameof(IpV4SubnetMask));
                OnPropertyChanged(nameof(NicDataOk));
            }
        }

        private string _ipV6Address;
        public string IpV6Address
        {
            get => _ipV6Address;
            set
            {
                _ipV6Address = value;
                OnPropertyChanged(nameof(IpV6Address));
                OnPropertyChanged(nameof(NicDataOk));
            }
        }
        private string _ipV6Prefix;
        public string IpV6Prefix
        {
            get => _ipV6Prefix;
            set
            {
                _ipV6Prefix = value;
                OnPropertyChanged(nameof(IpV6Prefix));
                OnPropertyChanged(nameof(NicDataOk));
            }
        }

        private string _defaultGateway;
        public string DefaultGateway
        {
            get => _defaultGateway;
            set
            {
                _defaultGateway = value;
                OnPropertyChanged(nameof(DefaultGateway));
                OnPropertyChanged(nameof(NicDataOk));
            }
        }

        public bool NicDataOk => AllNicDataOk();

        private bool _ipv4AddressDataOk;
        public bool IpV4AddressDataOk
        {
            get => _ipv4AddressDataOk;
            set
            {
                _ipv4AddressDataOk = value;
                OnPropertyChanged(nameof(NicDataOk));
            }
        }

        private bool _ipv4SubnetDataOk;
        public bool IpV4SubnetDataOk
        {
            get => _ipv4SubnetDataOk;
            set
            {
                _ipv4SubnetDataOk = value;
                OnPropertyChanged(nameof(NicDataOk));
            }
        }

        private bool _ipv6AddressDataOk;
        public bool IpV6AddressDataOk
        {
            get => _ipv6AddressDataOk;
            set
            {
                _ipv6AddressDataOk = value;
                OnPropertyChanged(nameof(NicDataOk));
            }
        }

        private bool _ipv6PrefixDataOk;
        public bool IpV6PrefixDataOk
        {
            get => _ipv6PrefixDataOk;
            set
            {
                _ipv6PrefixDataOk = value;
                OnPropertyChanged(nameof(NicDataOk));
            }
        }

        private bool _ipv4GatewayDataOk;
        public bool IpV4GatewayDataOk
        {
            get => _ipv4GatewayDataOk;
            set
            {
                _ipv4GatewayDataOk = value;
                OnPropertyChanged(nameof(NicDataOk));
            }
        }

        #endregion


        #region Propertyhelpers

        private bool AllNicDataOk()
        {
            LocalToNicData();
            var nicState = NicDataState();
            _setState(nicState);
            return nicState == TxtNicDataOk;
        }

        #endregion

        #region Commands

        public ICommand ReloadNicParameter => new RelayCommand<object>(ReloadNicData);
        public ICommand SetNicParameter => new RelayCommand<object>(DoNicKonfig);

        #endregion

        private readonly Action<string> _setState;
        public VmNicData(INicConfigurator nicData, Action<string> setState)
        {
            _nicData = nicData;
            NicDataToLocal();
            _setState = setState;
        }

        #region Workers

        private string NicDataState()
        {
            if (!_ipEnabled || _dhcpEnabled) return TxtNicDataOk;
            if (!_ipv4AddressDataOk) return "IPv4 Address unvalid, please check...";
            if (!_ipv4SubnetDataOk) return "IPv4 Subnetmask unvalid, please check...";
            if (!_ipv6AddressDataOk) return "IPv6 Address unvalid, please check...";
            if (!_ipv6PrefixDataOk) return "IPv6 Prefix unvalid, please check...";
            if (!_ipv4GatewayDataOk || !_nicData.DefaultIpGatewayOk(0)) return "IPv4 Gateway - Address unvalid, please check...";

            return TxtNicDataOk;
        }

        private void ReloadNicData(object parameter) {NicDataToLocal();}

        private void NicDataToLocal()
        {
            _ipEnabled = _nicData?.IpEnabled ?? false;
            _dhcpEnabled = _nicData?.DhcpEnabled ?? false;
            _ipV4Address = _nicData?.IpAddresses?.Count > 0 ? _nicData.IpAddresses[0] : "";
            _ipV4SubnetMask = _nicData?.IpSubNetMask?.Count > 0 ? _nicData.IpSubNetMask[0] : "";
            _ipV6Address = _nicData?.IpAddresses?.Count > 1 ? _nicData.IpAddresses[1] : "";
            _ipV6Prefix = _nicData?.IpSubNetMask?.Count > 1 ? _nicData.IpSubNetMask[1] : "";
            _defaultGateway = _nicData?.DefaultIpGateways?.Count > 0 ? _nicData.DefaultIpGateways[0] : "";
            NicDataChanged();
        }

        private void LocalToNicData()
        {
            _nicData.IpEnabled = _ipEnabled;
            _nicData.DhcpEnabled = _dhcpEnabled;
            _nicData.SetIpAddress(_ipV4Address, 0);
            _nicData.SetIpSubNetMask(_ipV4SubnetMask, 0);
            _nicData.SetIpAddress(_ipV6Address, 1);
            _nicData.SetIpSubNetMask(_ipV6Prefix, 1);
            _nicData.SetDefaultIpGateway(_defaultGateway, 0);

        }

        private void DoNicKonfig(object parameter)
        {
            LocalToNicData();
            //note here do deaktivate NicConfiguration
            _nicData.DoNicConfiguration();
            _setState("Nic config done");
        }


        #endregion
        #region Event firing

        public event PropertyChangedEventHandler PropertyChanged;

        private void NicDataChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MacAddress)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IpEnabled)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DhcpEnabled)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EnableIpSetting)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IpV4Address)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IpV4SubnetMask)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IpV6Address)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IpV6Prefix)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DefaultGateway)));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}