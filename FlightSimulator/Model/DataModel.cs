﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;

namespace FlightSimulator.Model
{
    class DataModel : INotifyPropertyChanged
    {
      private Boolean stop = false;
      private string ip = "127.0.0.1";
      private int in_port = 5006;
      private int out_port = 5004;
      private string file;
      private float alieron;
      private float elevator;
      private float rudder;
      private float throttle;
      private SocketModel in_socket;
      private SocketModel out_socket;
      public event PropertyChangedEventHandler PropertyChanged;

        public DataModel()
        {
            this.in_socket = new SocketModel(ip, in_port);
            this.out_socket = new SocketModel(ip, out_port);
        }

   
        // File Property
        public string File {
           get { return this.file; }
           set
            {
                if (this.file != value)
                {
                    this.file = value;
                    this.start();
                    NotifyPropertyChanged("File");
                }
            }
        }

        public float Alieron
        {
           get { return this.alieron; }
           set
            {
                if (this.alieron != value)
                {
                    this.alieron = value;
                    NotifyPropertyChanged("Alieron");
                }
            }
        }

        public float Elevator
        {
            get { return this.elevator; }
            set
            {
                if (this.elevator != value)
                {
                    this.elevator = value;
                    NotifyPropertyChanged("Elevator");
                }
            }
        }


        public float Rudder
        {
            get { return this.rudder; }
            set
            {
                if (this.rudder != value)
                {
                    this.rudder = value;
                    NotifyPropertyChanged("Rudder");
                }
            }
        }

        public float Throttle
        {
            get { return this.throttle; }
            set
            {
                if (this.throttle != value)
                {
                    this.throttle = value;
                    NotifyPropertyChanged("Throttle");
                }
            }
        }




        // parse the line from the csv, update needed properties
        public void parseLine(string line)
        {
            if (line == null)
            {
                return;
            }

            string[] parsedLine = line.Split(',');
            Console.WriteLine("Alieron: {0}, Elevator: {1}, Rudder: {2}, Throttle: {3}", parsedLine[0], parsedLine[1], parsedLine[2], parsedLine[6]);
            this.Alieron = float.Parse(parsedLine[0]);
            this.Elevator = float.Parse(parsedLine[1]);
            this.Rudder = float.Parse(parsedLine[2]);
            this.Throttle = float.Parse(parsedLine[6]);


        }

        public void start()
        {
            try
            {
                out_socket.disconnect();
                out_socket.connect();
                new Thread(delegate ()
                {
                    string[] lines = System.IO.File.ReadAllLines(this.file);
                    int i = 0;
                    while (!stop || i < lines.Length)
                    {
                        this.parseLine(lines[i]);
                        out_socket.send(lines[i] + "\n");

                        /**
                         * recieving data?
                         *  string data = out_socket.recieve();
                         *  Console.WriteLine("{0}", data);
                        **/
                        i++;

                        Thread.Sleep(100);
                    }
                }).Start();
            } 
            catch (Exception e)
            {
                Console.Write("Error while starting connection");
            }
            
        }


        // Notify handler
        public void NotifyPropertyChanged(string propName)
        {
            if(this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

    }

}
