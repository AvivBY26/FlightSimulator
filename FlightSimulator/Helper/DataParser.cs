﻿using LiveCharts;
using LiveCharts.Defaults;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace FlightSimulator.Helper
{

    class FlightParamater
    {
        private string name;
        private float[] data;
        private string corrFeature;

        public FlightParamater(string name, float[] data)
        {
            this.name = name;
            this.data = data;
        }

        public string getName() { return this.name; }
        public float[] getData() { return data; }
        public string getCorrFeature() { return this.corrFeature; }
        public void setCorrFeature(string name) { this.corrFeature = name; }

    }

    class DataParser : ICSVDataParser
    {

        private List<string> flightParamatersNames;
        private string trainFilePath;
        private string testFilePath;
        private string[] trainFileCsvRows;
        private string[] testFileCsvRows;
        private List<FlightParamater> trainData;
        private List<FlightParamater> testData;
        private bool isTestFlightLoaded = false;
        private Dictionary<string, int> paramatersIndicesDict;
        private int numberOfStampsIn30Sec = 200;


        public DataParser()
        {

        }

        public void learnFlight(string trainFile, List<string> flightParamaters)
        {

            this.trainFilePath = trainFile;
            this.trainFileCsvRows = System.IO.File.ReadAllLines(trainFile);
            this.flightParamatersNames = flightParamaters;
            this.trainData = new List<FlightParamater>();
            this.paramatersIndicesDict = new Dictionary<string, int>();
            extractData(trainFileCsvRows,trainFilePath,ref trainData);

            calcCorrFeatures();
        }

       

        public void extractDataFromTestFlight(string testFile)
        {
            this.testFilePath = testFile;
            this.testFileCsvRows = System.IO.File.ReadAllLines(testFile);
            this.testData = new List<FlightParamater>();
            extractData(testFileCsvRows, testFilePath, ref testData);
            isTestFlightLoaded = true;

        }

        public bool getIsTestFlightLoaded() { return this.isTestFlightLoaded; }

        public void integrateCorFeatures()
        {
            for (int i = 0; i < this.testData.Count; i++)
            {
                testData[i].setCorrFeature(trainData[i].getCorrFeature());
            }
        }

        public float getDataInTime(string feat, int timeStamp)
        {
            var featData = getFeatureData(feat);
            return featData[timeStamp];
        }

        public string getFeatMostCorrFeature(string feat)
        {
            int index = this.paramatersIndicesDict[feat];
            return this.trainData[index].getCorrFeature();
        }

        public float[] getFeatureData(string feat)
        {
            int index = this.paramatersIndicesDict[feat];
            return this.trainData[index].getData();
        }

        public float[] getFeatureDataInRange(string name, int endingTimeStamp)
        {
            float[] paramData = getFeatureData(name);
            Array.Resize(ref paramData, endingTimeStamp);
            return paramData;
        }

        public List<string> getFeatures()
        {
            return this.flightParamatersNames;
        }

        private void extractData(string[] csvRows, string filePath, ref List<FlightParamater> list)
        {
            TextFieldParser parser;
            int numOfTimeStamps = csvRows.Length;

            //creating the Flight Paramater data list.
            for (int index = 0; index < this.flightParamatersNames.Count; index++)
            {
                parser = new TextFieldParser(filePath);
                parser.SetDelimiters(",");

                string[] row;
                float[] paramData = new float[numOfTimeStamps];
                
                for (int j = 0; j < numOfTimeStamps; j++)
                {
                    row = parser.ReadFields();
                    paramData[j] = float.Parse(row[index]);
                }

                list.Add(new FlightParamater(this.flightParamatersNames[index], paramData));
                //updating dictionary.
                if(!this.paramatersIndicesDict.ContainsKey(this.flightParamatersNames[index]))
                {
                    this.paramatersIndicesDict.Add(this.flightParamatersNames[index], index);
                }
            }
        }

        public ChartValues<ScatterPoint> getLast30SecRegLinePoints(string reaserchedFeat, string correlataedFeat, int currentTimeStamp)
        {

            var values = new ChartValues<ScatterPoint>() { };
            float[] resFeat = getFeatureData(reaserchedFeat);
            float[] corFeat = getFeatureData(correlataedFeat);
            int timeStampIn30Seconds = calc30SecondsNumberOfTimeStamp();

            if (currentTimeStamp == 0)
            {
                return new ChartValues<ScatterPoint>() { };
            }
            else if(currentTimeStamp > timeStampIn30Seconds)
            {
                for (int i = (currentTimeStamp - timeStampIn30Seconds)-1; i < currentTimeStamp -1; i++)
                {
                    values.Add(new ScatterPoint(resFeat[i], corFeat[i]));
                }
            } 
            else
            {
                for (int i = 0; i < currentTimeStamp; i++)
                {
                    values.Add(new ScatterPoint(resFeat[i], corFeat[i]));
                }
            }        
            return values;
        }

        private int calc30SecondsNumberOfTimeStamp()
        {
            return this.numberOfStampsIn30Sec;
        }

        private void calcCorrFeatures()
        {
            int len = this.trainFileCsvRows.Length;

            for (int i = 0; i < this.trainData.Count; i++)
            {
                string f1 = this.trainData[i].getName();
                float max = 0;
                int jmax = 0;
                float p = 0;
                for (int j = i + 1; j < this.trainData.Count; j++)
                {
                    p = Math.Abs(Calculation.pearson(getFeatureData(f1), getFeatureData(this.trainData[j].getName()), len));
                    if (p > max)
                    {
                        max = p;
                        jmax = j;
                    }
                }
                string f2 = this.trainData[jmax].getName();
                this.trainData[i].setCorrFeature(f2);
            }
        }

        public ChartValues<ObservablePoint> getRegLineValues(string feat, string corFeat,int timeStamp)
        {
            //X value of reg line.
            float[] featData = getFeatureData(feat);
            //converting to points and getting the reg line function.
            Point[] points = Calculation.toPoints(getFeatureData(feat), getFeatureData(corFeat));
            Line l = Calculation.linear_reg(points, points.Length);

            //returning the edges of the reg line. (min and max)
            return new ChartValues<ObservablePoint> {
                new ObservablePoint(featData.Min(),l.f(featData.Min())),
                new ObservablePoint(featData.Max(),l.f(featData.Max()))
                };
        }
    }
}
