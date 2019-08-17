﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainSimulator
{
    public class Module2DSmell : ModuleBase
    {
        float average = 0;
        bool increasing = false;
        float last0, last1;
        public override void Fire()
        {
            Init();  //be sure to leave this here to enable use of the na variable
            float a0 = na.GetNeuronAt(0, 0).CurrentCharge;
            float a1 = na.GetNeuronAt(1, 0).CurrentCharge;

            if (a0 == last0 && a1 == last1) return;

            last0 = a0;
            last1 = a1;
            float ave = (a0 + a1) / 2;
            float diff = a0 - a1;
            if (ave > average)
            {
                increasing = true; na.GetNeuronAt(2, 0).SetValue(1); na.GetNeuronAt(3, 0).SetValue(diff);
            }
            else 
            {
                increasing = false; na.GetNeuronAt(2, 0).SetValue(0); na.GetNeuronAt(3, 0).SetValue(diff);
            }

            average = ave;
            increasing = !increasing; //we might use this soon
        }
        public override void Initialize()
        {
            na.GetNeuronAt(0, 0).Range = 2;
            na.GetNeuronAt(1, 0).Range = 2;
            na.GetNeuronAt(3, 0).Range = 2;
        }
        public override void ShowDialog() //delete this function if it isn't needed
        {
            base.ShowDialog();

        }
    }


}