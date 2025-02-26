﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FlightSimulator.Model;
using FlightSimulator.ViewModel;

namespace FlightSimulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataViewModel vm;
        public MainWindow()
        {
            InitializeComponent();
            DataModel dm = DataModel.Instance;
            this.vm = new DataViewModel(dm);
            DataContext = vm;
        }

        public void UploadButtonHandler(object sender, RoutedEventArgs e)
        {   
            if(sender.Equals(UploadTrainButton))
            {   
                this.vm.getFile("train");
            }
            else
            {
                if (this.vm.VM_TrainFILE == null)
                {
                    MessageBox.Show("please upload train file");
                    return;
                }
                this.vm.getFile("test");

            }
            
        }

        public void StartButtonHandler(object sender, RoutedEventArgs e)
        {
            this.vm.start();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // Begin dragging the window
            this.DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(1);
        }
    }
}
