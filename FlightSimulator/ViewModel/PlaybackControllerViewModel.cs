﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using FlightSimulator.Model;

namespace FlightSimulator.ViewModel
{
    class PlaybackControllerViewModel : INotifyPropertyChanged
    {
        private DataModel model;
        public event PropertyChangedEventHandler PropertyChanged;

        public PlaybackControllerViewModel(DataModel model)
        {
            this.model = model;
            model.PropertyChanged += delegate (Object sender, PropertyChangedEventArgs e)
            {
                NotifyPropertyChanged("VM_" + e.PropertyName);
            };
        }

        // Notify handler
        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        // Methods
        public void ResetVideo()
        {
            this.model.Timestamp = 0;
            this.model.Pause = true;
        }

        public void ResumeVideo()
        {
            if (this.model.Pause == true)
            {
                this.model.Pause = false;
            }
        }

        public void PauseVideo()
        {
            if (this.model.Pause == false)
            {
                this.model.Pause = true;
            }
        }

        public void FinishVideo()
        {
            this.model.Timestamp = this.model.MaximumLength - 1;
            this.model.Pause = true;
        }

        public void ReduceSpeed()
        {
            this.model.PlaybackMultiplier = Math.Max(0.25, this.model.PlaybackMultiplier - 0.25);
        }

        public void IncreaseSpeed()
        {
            this.model.PlaybackMultiplier = Math.Min(2, this.model.PlaybackMultiplier + 0.25);
        }


        // Properties
        public int VM_Timestamp
        { // Setting timestamp to 0 resets the video
            get { return model.Timestamp; }
            set { model.Timestamp = value; }
        }

        public Boolean VM_Pause
        {
            get { return model.Pause; }
            set { model.Pause = value; }
        }

        public double VM_PlaybackMultiplier
        {
            get { return model.PlaybackMultiplier; }
            set { model.PlaybackMultiplier = value; }
        }
    }
}
