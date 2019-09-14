﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BrainSimulator
{
    /// <summary>
    /// Interaction logic for Module2DSimDlg.xaml
    /// </summary>
    public partial class ModuleCameraDlg : ModuleBaseDlg
    {
        public ModuleCameraDlg()
        {
            InitializeComponent();
        }

        //this is here so the last change will cause a screen update after 1 second
        DispatcherTimer dt = null;
        private void Dt_Tick(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate { Draw(); });
        }

        public override bool Draw()
        {
            base.Draw();
            ModuleCamera parent = (ModuleCamera)base.Parent1;
            if (parent.theDlgBitMap != null)
            {
                image.Source = parent.theDlgBitMap;
            }
            //dt = new DispatcherTimer() { Interval = new TimeSpan(0,0, 0,  1) };
            //dt.Tick += Dt_Tick;
            //dt.Start();
            return true;
        }

        private void TheCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Draw();
        }

    }
}
