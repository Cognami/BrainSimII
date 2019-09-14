﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Xml.Serialization;


namespace BrainSimulator
{


    public class NeuronArea : IComparable<NeuronArea>
    {
        int firstNeuron = 0;//, lastNeuron = 0;
        string label;
        string commandLine;
        int color;
        ModuleBase theModule = null;
        int width = 0;
        int height = 0;
        public int CompareTo (NeuronArea na)
        { 
            //two bizarre cases
            if (na == null) return 1;
            if (na.firstNeuron == firstNeuron) return 0;
            if (na.firstNeuron < firstNeuron) return 1;
            return -1;
        }

        public NeuronArea(int firstNeuron1, int width, int height, string theLabel, string theCommandLine, int theColor)
        {
            FirstNeuron = firstNeuron1;
            Width = width;
            Height = height;
            Label = theLabel;
            CommandLine = theCommandLine;
            color = theColor;
            string[] Params = CommandLine.Split(' ');
            if (commandLine.IndexOf("Module") == 0) //take this out when all modules are ported to new class structure
            {
                Type t = Type.GetType("BrainSimulator." + Params[0]);
                theModule = (ModuleBase)Activator.CreateInstance(t);
            }
        }

        public NeuronArea() { }
        public int FirstNeuron { get => firstNeuron; set => firstNeuron = value; }
        //public int LastNeuron { get => lastNeuron; set => lastNeuron = value; }
        public string CommandLine { get => commandLine; set => commandLine = value; }
        public string Label { get => label; set => label = value; }
        public int Rows { get { return MainWindow.theNeuronArray.rows; } }


        public int NeuronCount { get { return Width * Height; } }
        public int Color { get => color; set => color = value; }
        public ModuleBase TheModule { get => theModule; set => theModule = value; }
        public int Height { get => height; set => height = value; }
        public int Width { get => width; set => width = value; }
        public int LastNeuron { get { return firstNeuron + (height - 1) + Rows * (Width - 1); } }

        //these two emulate a foreach which might be implemented some day
        int currentNeuronInArea = 0;
        public void BeginEnum()
        { currentNeuronInArea = 0; }
        public Neuron GetNextNeuron()
        {
            int neuronIndex = (currentNeuronInArea % Height) + (currentNeuronInArea / Height) * Rows + firstNeuron;
            if (currentNeuronInArea >= Height * Width) return null;
            currentNeuronInArea++;
            return MainWindow.theNeuronArray.neuronArray[neuronIndex];
        }

        public Rectangle GetRectangle(DisplayParams dp)
        {
            Rectangle r = new Rectangle();
            Point p1 = dp.pointFromNeuron(firstNeuron);
            Point p2 = dp.pointFromNeuron(LastNeuron);
            p2.X += dp.NeuronDisplaySize;
            p2.Y += dp.NeuronDisplaySize;
            r.Width = p2.X - p1.X;
            r.Height = p2.Y - p1.Y;
            Canvas.SetTop(r, p1.Y);
            Canvas.SetLeft(r, p1.X);
            return r;
        }


        public Neuron GetFreeNeuron()
        {
            BeginEnum();
            for (Neuron n = GetNextNeuron(); n != null; n = GetNextNeuron())
                if (!n.InUse())
                    return n;
            return null;
        }
        //this gets the neuron with an index relative to the area itself
        public Neuron GetNeuronAt(int index)
        {
            int neuronIndex = (index % Height) + (index / Height) * Rows + firstNeuron;
            return MainWindow.theNeuronArray.neuronArray[neuronIndex];
        }
        public Neuron GetNeuronAt(int X, int Y)
        {
            int index = firstNeuron + Y + X * Rows;
            return MainWindow.theNeuronArray.neuronArray[index];
        }
        public Neuron GetNeuronAt(string label)
        {
            for (int i = 0; i < NeuronCount; i++)
            {
                Neuron n = GetNeuronAt(i);
                if (n.Label.ToLower() == label.ToLower())
                    return n;
            }
            return null;
        }

        public void GetBounds(out int X1, out int Y1, out int X2, out int Y2)
        {
            NeuronSelectionRectangle nsr = new NeuronSelectionRectangle(firstNeuron, Width,Height);
            nsr.GetSelectedArea(out X1, out Y1, out X2, out Y2);
        }
        public void GetAbsNeuronLocation(int index, out int X, out int Y)
        {
            X = index / Rows;
            Y = index % Rows;
        }

        public void GetNeuronLocation(Neuron n, out int X, out int Y)
        {
            GetAbsNeuronLocation(n.Id, out int X1, out int Y1);
            GetAbsNeuronLocation(FirstNeuron, out int X2, out int Y2);
            X = X1 - X2;
            Y = Y1 - Y2;
        }
        public void GetNeuronLocation(int nIndex, out int X, out int Y)
        {
            GetAbsNeuronLocation(nIndex, out int X1, out int Y1);
            GetAbsNeuronLocation(FirstNeuron, out int X2, out int Y2);
            X = X1 - X2;
            Y = Y1 - Y2;
        }

        public void ClearAllNeurons()
        {
            BeginEnum();
            for (Neuron n = GetNextNeuron(); n != null; n = GetNextNeuron())
                n.LastCharge = n.CurrentCharge = 0;
        }

        public void ClearNeuronArea()
        {
            BeginEnum();
            for (Neuron n = GetNextNeuron(); n != null; n = GetNextNeuron())
                n.DeleteAllSynapes();
        }
        public int NeuronsInUseInArea()
        {
            int count = 0;
            BeginEnum();
            for (Neuron n = GetNextNeuron(); n != null; n = GetNextNeuron())
                if (n.InUse())
                    count++;
            return count;
        }
        public int NeuronsFiredInArea()
        {
            int count = 0;
            BeginEnum();
            for (Neuron n = GetNextNeuron(); n != null; n = GetNextNeuron())
                if (n.LastCharge > .9)
                    count++;
            return count;
        }
        public void ClearNeuronChargeInArea(bool CurrentToo = true)
        {
            BeginEnum();
            for (Neuron n = GetNextNeuron(); n != null; n = GetNextNeuron())
            {
                if (CurrentToo) n.CurrentCharge = 0;
                n.LastCharge = 0;
            }
        }
        public string GetParam(string key)
        {
            string[] parameters = CommandLine.Split(' ');
            for (int i = 0; i < parameters.Length - 1; i++)
            {
                string param = parameters[i];
                if (param == key)
                {
                    return parameters[i + 1];
                }
            }
            return "";
        }
    }

}