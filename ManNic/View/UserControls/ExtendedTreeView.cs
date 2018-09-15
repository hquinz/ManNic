using System.Windows;
using System.Windows.Controls;

namespace HQ4P.Tools.ManNic.View.UserControls
{
    public class ExtendedTreeView : TreeView
    {
        public ExtendedTreeView() : base()
        {
            this.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(___ICH);
        }

        void ___ICH(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (SelectedItem != null)
            {
                SetValue(SelectedItemBindableProperty, SelectedItem);
            }
        }

        public object SelectedItemBindable
        {
            get => (object)GetValue(SelectedItemBindableProperty);
            set => SetValue(SelectedItemBindableProperty, value);
        }
        public static readonly DependencyProperty SelectedItemBindableProperty = DependencyProperty.Register("SelectedItemBindable"
                                                                                                            , typeof(object)
                                                                                                            , typeof(ExtendedTreeView)
                                                                                                            , new UIPropertyMetadata(null));
    
}
}