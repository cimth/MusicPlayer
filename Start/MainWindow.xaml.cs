﻿using System.Windows;
using System.Windows.Input;
using Common;
using ViewModel;

namespace Start
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            // Add language resources
            this.Resources.MergedDictionaries.Add(LanguageUtil.LocalizedResourceDictionary);
            
            // Create main view model for DataContext
            // => ATTENTION: The MainWindow is the only view where the view model is created like this.
            //               For all other views, the view model is set via binding.
            DataContext = new MainWindowViewModel();
        }
    }
}