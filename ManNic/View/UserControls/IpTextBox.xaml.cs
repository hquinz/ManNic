using System;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HQ4P.Tools.ManNic.View.UserControls
{
    /// <summary>
    /// Generates an Textbox for IP - Adresses including the Subnetparameters (IPv4 & Ipv6)
    /// If changigng Value via binding and entering new chars in txtbox happens nearly same time changes my be lost!
    /// ignoring murphis law I keep it that way (servh for "_setValueMyself"
    /// stil to be done:
    /// - Generate Mode from Value - String
    /// - React properly on paste
    /// </summary>
    public partial class IpTextBox : UserControl
    {

        public enum EnumMode
        {
              IPv4Address = 0
            , IPv4SubnetMask = 1
            , IPv6Address = 2       
            , IPv6Prefix = 3        
        };

        #region Propertys

        public EnumMode Mode
        {
            get => (EnumMode)GetValue(ModeProperty);
            set => SetValue(ModeProperty, value);
        }
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register("Mode", typeof(EnumMode), typeof(IpTextBox), new FrameworkPropertyMetadata()
                                                                                            {DefaultValue = EnumMode.IPv4Address
                                                                                            ,AffectsMeasure = true
                                                                                            ,PropertyChangedCallback = ModeChanged}
            );

        public string Value
        {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), typeof(IpTextBox), new FrameworkPropertyMetadata()
                                                                                            {DefaultValue = "192.168.0.1"
                                                                                            ,PropertyChangedCallback = ValueChanged
                                                                                            ,BindsTwoWayByDefault = true}
                                        );

        public bool DataValid
        {
            get => (bool)GetValue(DataValidProperty);
            set => SetValue(DataValidProperty, value);
        }
        public static readonly DependencyProperty DataValidProperty =
                    DependencyProperty.Register("DataValid", typeof(bool), typeof(IpTextBox), new PropertyMetadata(false)
                                                                                                    {CoerceValueCallback = DataValidPreviewChange});
       

        #endregion


        private bool _setValueMyself;
        private bool _setTextboxMyself;


        #region Construct

        public IpTextBox()
        {
            InitializeComponent();
            AddUiElements();
            this.IsEnabledChanged += IpEnabledChanged;
        }

        internal void Redraw()
        {
            IpTextBoxContainer.Children.Clear();
            AddUiElements();
        }

        private void AddUiElements()
        {
            AddTxtBoxAdressPart(1);
            var parts = AddressParts();

            for (var i = 2; i <= parts; i++)
            {
                AddSeparator(i);
                AddTxtBoxAdressPart(i);
            }
        }

        private void AddTxtBoxAdressPart(int number)
        {
            var txtBox = new TextBox
            {
                Name = $"txtBoxPart{number}"
                ,TabIndex = number
                ,BorderThickness = new Thickness(0)
                ,Style = FindResource("BackgroundControll") as Style
                ,VerticalContentAlignment = VerticalAlignment.Center
                ,HorizontalContentAlignment = HorizontalAlignment.Center
                ,MaxLength = AddressLength(Mode)
                ,Width = GetMinWith()
                ,Text = "0"
             
            };

            txtBox.GotFocus += AdressPart_OnGotFocus;
            txtBox.PreviewTextInput += OnPreviewTextInput;
            txtBox.PreviewKeyDown += OnPreviewKeyDown;
            txtBox.TextChanged += OnTextChanged;


            IpTextBoxContainer.Children.Add(txtBox);
           
        }

        private void AddSeparator(int number)
        {
            IpTextBoxContainer.Children.Add(new Label{Name = $"LblDevider{number}",Content = ":"});
        }

        private double GetMinWith()
        {
            var text = GetFormattedSampleText();
            return text.Width + 5;
        }


        private FormattedText GetFormattedSampleText()
        {
            string sampleText;

            switch (Mode)
            {
                case EnumMode.IPv6Address:
                    sampleText = "FFFF";
                    break;
                case EnumMode.IPv6Prefix:
                    sampleText = "/128";
                    break;
                default:
                    sampleText = "255";
                    break;
            }


            return new FormattedText(sampleText
                                    , CultureInfo.InvariantCulture
                                    , FlowDirection.LeftToRight
                                    , new Typeface(FontFamily.ToString())
                                    , FontSize
                                    , Foreground);
        }


        #endregion

        #region eventhandling

        private static void IpEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var me = (IpTextBox)sender;
            me.DataValid = me.AddressIsValid();
        }
        private static void ModeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var me = (IpTextBox)sender;
            me.Redraw();
        }

        private static object DataValidPreviewChange(DependencyObject sender, object newValue)
        {
            var me = (IpTextBox)sender;
            return me.AddressIsValid();
        }

        private static void ValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var me = (IpTextBox)obj;
            me.NewValueInterpretation(e.NewValue.ToString());
        }
        
        private void AdressPart_OnGotFocus(object sender, RoutedEventArgs e)
        {
            var tbSender = (TextBox)sender;
            tbSender.SelectAll();
        }
 
        protected void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter
                && e.Key != Key.OemPeriod //":"
                && e.Key != Key.Tab
            ) return;

            var shiftPressed = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
            e.Handled = ChangeFocus(sender, shiftPressed);   
        }


        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var insertedText = e.Text;
            if (insertedText.Length != 1) return;      //Paste ist not coverd by this method!; an some shortcuts like <ctrl> + s length == 0
            e.Handled =CheckInputKey(Convert.ToChar(insertedText[0]));
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            DataValid = AddressIsValid();

            if (SetTexboxMyself()) return;

            var tbSender = (TextBox)sender;
            var index = int.Parse(tbSender.Name[tbSender.Name.Length - 1].ToString());
            tbSender.Background = (CheckAddressPart(tbSender.Text, index) ? null : Brushes.DarkOrange);
            if (tbSender.Text.Length >= AddressLength(Mode)) ChangeFocus(sender, false);
            if (AddressIsValid()) WriteValue();       //Generate Value for dep property
        }



        #endregion

        #region Value - Handling

        private void NewValueInterpretation(string value)
        {
            if (SetValueMyself()) return;
            _setTextboxMyself = true;
            switch (Mode)
            {
                case EnumMode.IPv4Address:
                case EnumMode.IPv4SubnetMask:
                    GetIpV4FromValue(value, '.');
                    break;
                case EnumMode.IPv6Address:
                    GetIpV6FromValue(value, ':');
                    break;
                case EnumMode.IPv6Prefix:
                    GetIpV6PrefixFromValue(value);
                    break;
            }

        }


        private void GetIpV4FromValue(string address, char splitter)
        {
            if (string.IsNullOrEmpty(address))
            {
                InitializeAddressFields("0");
                return;
            }

            var addressElements = address.Split(splitter);
            
            if (addressElements.Length != AddressParts()) throw new ArgumentOutOfRangeException(nameof(Value),@"Count of adresselements unvalid");

            var index = 0;
            foreach (var addressElement in addressElements)
            {
                SetTextBoxText(index, addressElement);
                index = index + 2;
            }
        }

        private void GetIpV6FromValue(string address, char splitter)
        {
            if (string.IsNullOrEmpty(address))
            {
                InitializeAddressFields("0000");
                return;
            }

            var addressElements = address.Split(splitter);

            if (addressElements.Length > AddressParts()) throw new ArgumentOutOfRangeException(nameof(Value), @"Count of adresselements unvalid");
            var fillElements = AddressParts() - addressElements.Length + 1;

            var index = 0;
            foreach (var addressElement in addressElements)
            {

                if (!string.IsNullOrEmpty(addressElement))
                {
                    SetTextBoxText(index, addressElement);
                    index = index + 2;

                }
                else
                {
                    for (var i = 0; i < fillElements; i++)
                    {
                        SetTextBoxText(index, "000");
                        index = index + 2;
                    }
                }
            }
        }

        private void GetIpV6PrefixFromValue(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                InitializeAddressFields("0");
                return;
            }

            if (AddressParts() != 1) throw new ArgumentOutOfRangeException(nameof(Value), @"Count of adresselements unvalid");

            SetTextBoxText(0, (address[0] == '/') ? address : "/" + address);
        }

        private void InitializeAddressFields(string initString)
        {

            var index = 0;
            for (var i = 0; i < AddressParts(); i++)
            {
                SetTextBoxText(index, initString);
                index = index + 2;

            }

        }

        private void SetTextBoxText(int index, string value)
        {
            _setTextboxMyself = true;
            var txtbox = (TextBox)IpTextBoxContainer.Children[index];
            txtbox.Text = value;

        }

        internal bool SetValueMyself()
        {
            var done = _setValueMyself;
            _setValueMyself = false;
            return done;
        }

        internal bool SetTexboxMyself()
        {
            var done = _setTextboxMyself;
            _setTextboxMyself = false;
            return done;
        }

        private void WriteValue()
        {
            if (IpTextBoxIsEmpty())
            {
                SetPropValue("");
                return;
            };

            switch (Mode)
            {
                case EnumMode.IPv4Address:
                case EnumMode.IPv4SubnetMask:
                    SetPropValue(GenerateIpValue('.'));
                    return;
                case EnumMode.IPv6Address:
                    SetPropValue(GenerateIpValue(':'));
                    return;
                case EnumMode.IPv6Prefix:
                    SetPropValue(GenerateIpV6Prefix());
                    return;
            }
        }

        private string GenerateIpValue(char seperator)
        {
            var txtbox = (TextBox)IpTextBoxContainer.Children[0];
            var newValue = new StringBuilder(txtbox.Text);

            var index = 2;
            for (var i = 1; i < AddressParts(); i++)
            {
                txtbox = (TextBox)IpTextBoxContainer.Children[index];
                newValue.Append(seperator);
                newValue.Append(txtbox.Text);
                index = index + 2;
            }

            return newValue.ToString();
        }

        private string GenerateIpV6Prefix()
        {
            var txtbox = (TextBox)IpTextBoxContainer.Children[0];
            return txtbox.Text.Trim('/');
        }

        private void SetPropValue(string value)
        {
            _setValueMyself = true;
            Value = value;
        }

        public bool IpTextBoxIsEmpty()
        {

            var index = 0;
            for (var i = 0; i < AddressParts(); i++)
            {
                var txtbox = (TextBox)IpTextBoxContainer.Children[index];
                if (!string.IsNullOrEmpty(txtbox.Text)) return false;
                index = index + 2;
            }

            return true;

        }


        #endregion

        #region control
        private int AddressParts()
        {
            switch (Mode)
            {
                case EnumMode.IPv4Address:
                case EnumMode.IPv4SubnetMask:
                    return 4;
                case EnumMode.IPv6Address:
                    return 8;
                case EnumMode.IPv6Prefix:
                    return 1;
                default:
                    return 0;
            }

        }

        private int AddressLength(EnumMode mode)
        {
            switch (mode)
            {
                case EnumMode.IPv4Address:
                case EnumMode.IPv4SubnetMask:
                    return 3;
                case EnumMode.IPv6Prefix:
                case EnumMode.IPv6Address:
                    return 4;
            }
            return 1;
        }

        #endregion

        #region user experience

        private bool ChangeFocus(object sender, bool backwards)
        {
            var tbSender = (TextBox)sender;
            var fieldNumber = int.Parse(tbSender.Name[tbSender.Name.Length - 1].ToString());
            int newFieldIndex;
            switch (Mode)
            {
                case EnumMode.IPv4Address:
                case EnumMode.IPv4SubnetMask:
                    newFieldIndex = NewIndex(fieldNumber, 4, backwards);
                    if (newFieldIndex >= 0)
                    {
                        IpTextBoxContainer.Children[newFieldIndex].Focus();
                        return true;
                    }
                    break;
                case EnumMode.IPv6Prefix:
                    return false;
                case EnumMode.IPv6Address:
                    newFieldIndex = NewIndex(fieldNumber, 8, backwards);
                    if (newFieldIndex >= 0)
                    {
                        IpTextBoxContainer.Children[newFieldIndex].Focus();
                        return true;
                    }
                    break;
            }
            IpTextBoxContainer.Children[0].Focus(); // Index of focused child seems to be static, therfor focus is changed to first child here)
            return false;
        }
        private int NewIndex(int aktFieldNumber, int maxFieldNumber, bool backwards)
        {
            var newFieldNumber = backwards ? aktFieldNumber - 2 : aktFieldNumber;
            return newFieldNumber < maxFieldNumber ? newFieldNumber * 2 : -1;
        }

        #endregion


        #region Validation

        private bool CheckInputKey( char key)
        {
            switch (Mode)
            {
                case EnumMode.IPv4Address:
                case EnumMode.IPv4SubnetMask:
                    if (char.IsNumber(key)) return false;
                    break;
                case EnumMode.IPv6Prefix:
                    if (char.IsNumber(key) || key == '/') return false;
                    break;
                case EnumMode.IPv6Address:
                    if (char.IsNumber(key) || (key >= 'a' && key <='f') || (key >= 'A' && key <= 'F')) return false;
                    break;
                default:
                    return true;
            }
            return true;
        }

        private bool AddressIsValid()
        {
            if (Mode == EnumMode.IPv6Prefix)
            {
                var txtBox = (TextBox)IpTextBoxContainer.Children[0];
                if (ConvertIpV6Prefix(txtBox.Text) == 0) return true;
            }
            else
            {
                if (AddressIsZero()) return true;
            }

            var addressIsOk = true;
            var index = 0;
            for (var i = 0; i < AddressParts(); i++)
            {
                var txtBox = (TextBox) IpTextBoxContainer.Children[index];
                if (!CheckAddressPart(txtBox.Text, i + 1))
                {
                    addressIsOk = false;
                    break;
                }

                index = index + 2;
            }

            return addressIsOk;
        }


        /// <summary>
        /// Checks Addressparts for fitting the Rules.
        /// Returns TRUE if so
        /// </summary>
        /// <param name="text"> Adress - Part</param>
        /// <param name="index"> Index of Addresspart starting with 1!</param>
        /// <returns>TRUE if Part of Address fits the rules</returns>
        private bool CheckAddressPart(string text, int index)
        {
            switch (Mode)
            {
                case EnumMode.IPv4Address:
                    return CheckIpV4AddressPart(text, index);
                case EnumMode.IPv4SubnetMask:
                    return CheckIpv4SubnetPart(text, index);
                case EnumMode.IPv6Address:
                    return CheckIpV6AddressPart(text, index);
                case EnumMode.IPv6Prefix:
                    return CheckIpV6Prefix(text);
                default:
                    return false;
            }
        }

        private int ConvertIpV4Addresspart(string text)
        {
            return int.TryParse(text, out var addressValue) ? addressValue : -1;
        }

        private int ConvertIpV6Prefix(string text)
        {
            if (string.IsNullOrEmpty(text)) return -1;
            var chkText = (text[0] == '/') ? text.Substring(1) : text;
            return int.TryParse(chkText, out var addressValue) ? addressValue : -1;

        }

        private bool CheckIpV4AddressPart(string text, int index)
        {
            var lowAddressLimit = (index < AddressParts()) ? 0 : 1;
            var highAddressLimit = (index < AddressParts()) ? 255 : 254;
            var addressValue = ConvertIpV4Addresspart(text);

            return ((addressValue >= lowAddressLimit && addressValue <= highAddressLimit) || AddressIsZero());
        }
        private bool CheckIpv4SubnetPart(string text, int index)
        {
            var addressValue = ConvertIpV4Addresspart(text);
            if (addressValue < 0 || addressValue > 255) return false;
            return CheckIpV4SubnetMask(index);
        }

        private bool CheckIpV4SubnetMask(int index)
        {
            uint mask = 0;
            for (var i = 0; i < index; i++)
            {
                var txtBox = (TextBox)IpTextBoxContainer.Children[i * 2];
                if (string.IsNullOrEmpty(txtBox.Text)) return false;
                mask = mask << 8 | uint.Parse(txtBox.Text);
            }

            var j = 0;
            var foundTrue = false;
            while (j < (index * 8))
            {
                var isTrue = (mask & 1) != 0;
                if (isTrue) foundTrue = true;
                if (foundTrue && !isTrue) return false;

                mask = mask >> 1;
                j++;
            }
            return foundTrue;
        }
        
        private bool CheckIpV6AddressPart(string text, int index)
        {
            var lowAddressLimit = (index < AddressParts()) ? 0 : 1;
            var highAddressLimit = (index < AddressParts()) ? ushort.MaxValue : ushort.MaxValue - 1;
            uint addressValue;

            try {addressValue = uint.Parse(text, NumberStyles.HexNumber);}
            catch (Exception) {return false;}

            return ((addressValue >= lowAddressLimit && addressValue <= highAddressLimit) ||  AddressIsZero());
        }

        private bool CheckIpV6Prefix(string text)
        {
            return ConvertIpV6Prefix(text) >= 0;
        }

        private bool AddressIsZero()
        {

            var zeroAddress = true;
            var index = 0;

            var typeOfAdressValue = (Mode == EnumMode.IPv6Address) ? NumberStyles.HexNumber : NumberStyles.Integer;
            for (var i = 0; i < AddressParts(); i++)
            {
                var txtbox = (TextBox)IpTextBoxContainer.Children[index];
                if (string.IsNullOrEmpty(txtbox.Text)) continue;
                if (int.Parse(txtbox.Text, typeOfAdressValue) != 0)
                {
                    zeroAddress = false;
                    break;
                }
                index = index + 2;

            }

            return zeroAddress;
        }

        #endregion


    }


}
