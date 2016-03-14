/*
3-clause BSD license

Copyright (c) 2013-2016 Michael Krause (krause@tum.de) Institute of Ergonomics, Technische Universität München

All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
      notice, this list of conditions and the following disclaimer in the
      documentation and/or other materials provided with the distribution.
    * Neither the name of the <organization> nor the
      names of its contributors may be used to endorse or promote products
      derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 
*/

//-------------------------------
//history
//
// 1.0 initial [Michael Krause]
// 1.1 convert marker if >= 48 [Michael Krause]
// 1.2 adjusted header check [Michael Krause]
// 1.3 adjusted header check dt/engl [Michael Krause]
// 1.4 prepared open source [Michael Krause]
//
//TODO open topic. count/asses missed/correct lane changes
//TODO open topic. calculate reaction time


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;

namespace convert
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string[] filePaths; // global array of silab data files
        private string convFolder; // global var for access by conv-thread
        public delegate void UpdateTextCallback(string message);
        private Thread convertThread;


        private static string reportLineEnd = "\r\n";
        private static string lctHeader = "Zeit in ms(GMT),Xpos,Ypos,Geschw in KmH,Lenk in Grad,Strecke,Marker";
        private static char lctDelimiter = '\t';
        private static string lctFileEnd = "\r\n";
        private static char silabDelimiter = ',';
        /*
         * //old
        private static string[] silabHeader = {"Spalte1=Messzeitpunkt", 
                                   "Spalte2=Messzeitpunktfehler",
                                   "Spalte3=Streckennummer",
                                   "Spalte4=Abstand zum rechten Fahrbahnrand",
                                   "Spalte5=Streckenmeter",
                                   "Spalte6=Geschwindigkeit in km/h",
                                   "Spalte7=Lenkradwinkel",
                                   "Spalte8=Marker"};
        */
        //new
        private static string[] silabHeader = {"Messzeitpunkt", 
                                   "Messzeitpunktfehler",
                                   "Streckennummer",
                                   "Abstand zum rechten Fahrbahnrand",
                                   "Streckenmeter",
                                   "Geschwindigkeit in km/h",
                                   "Lenkradwinkel",
                                   "Marker"};

        private static string[] silabHeaderEngl = {"MeasurementTime", 
                                   "MeasurementTimeError",
                                   "Streckennummer",
                                   "Abstand zum rechten Fahrbahnrand",
                                   "Streckenmeter",
                                   "Geschwindigkeit in km/h",
                                   "Lenkradwinkel",
                                   "Marker"};

        private static int[] trackStarts = { 0, 3335, 6510, 9783, 12985, 16279, 19525, 22843, 26113, 29383 };
        private static double[] signPos = { 150, 304.26, 445.96, 588.72, 738.52, 878.71, 1021.25, 1171.75, 1322, 1464.4, 1611.25, 1763.29, 1915.87, 2059.67, 2228.43, 2416.43, 2560, 2701.51, 2849.52, 150, 323.32, 480.65, 622.01, 768.24, 910.21, 1069.44, 1221.86, 1364.28, 1511.03, 1653.27, 1796.39, 1949.58, 2094.48, 2254.03, 2395.06, 2545.32, 2692.92, 2840.66, 150, 290.96, 435.36, 583.42, 726.67, 868.01, 1042.19, 1184.93, 1349.24, 1489.57, 1635.85, 1783.97, 1957.49, 2115.67, 2256.66, 2396.89, 2546.08, 2690.82, 2840.3, 150, 300.44, 451.36, 591.49, 735.09, 888.3, 1028.86, 1171.61, 1317.78, 1473.58, 1625.52, 1773.62, 1942.84, 2088.43, 2257.04, 2409.34, 2549.9, 2698.75, 2845.62, 150, 293.52, 439.49, 581.62, 721.97, 869.12, 1018.68, 1160.27, 1314.15, 1457.4, 1619.01, 1773, 1922.18, 2074.08, 2214.47, 2358.07, 2503.61, 2673.1, 2848.8, 150, 305.87, 448.57, 597.02, 754.32, 899.73, 1042.42, 1196.44, 1338.61, 1489.59, 1653.53, 1796.21, 1943.54, 2084.27, 2224.74, 2373.65, 2515.79, 2691.64, 2849.12, 150, 298.11, 443.79, 591.17, 737.4, 883.29, 1026.62, 1174.66, 1319.06, 1480.73, 1659.25, 1802.42, 1956.21, 2099.33, 2264.08, 2409.94, 2562.37, 2706.81, 2847.66, 150, 292.04, 452.94, 596.98, 737.73, 883.24, 1049.08, 1200.07, 1346.2, 1503.07, 1651.05, 1830.96, 1985.21, 2129.49, 2273.63, 2416.66, 2561.13, 2701.93, 2843.74, 150, 290.05, 438.67, 587.71, 740.21, 895.78, 1039.13, 1182.32, 1327.83, 1472, 1636.82, 1777.47, 1921.31, 2106.35, 2249, 2398.01, 2555.66, 2702.74, 2847.1, 150, 318.49, 467.19, 621.82, 765.22, 928.42, 1070.7, 1222.22, 1364.93, 1521.99, 1674.55, 1828.81, 1969.7, 2113.33, 2255.79, 2403.74, 2552.74, 2701.96, 2842.16 };
        private static int[] signMeaning = { 3, 2, 1, 0, 2, 0, 2, 1, 2, 0, 2, 0, 1, 2, 1, 0, 1, 0, 1, 3, 0, 1, 0, 2, 0, 2, 1, 0, 2, 1, 2, 1, 2, 0, 1, 2, 0, 1, 3, 2, 0, 2, 0, 1, 0, 1, 2, 0, 2, 1, 2, 1, 0, 1, 0, 2, 1, 3, 0, 1, 0, 1, 2, 0, 2, 1, 2, 0, 2, 1, 2, 1, 0, 2, 0, 1, 3, 2, 0, 1, 0, 1, 0, 2, 1, 2, 1, 2, 0, 2, 1, 0, 2, 0, 1, 3, 0, 2, 0, 1, 2, 0, 1, 0, 1, 2, 1, 2, 1, 0, 2, 0, 2, 1, 3, 0, 2, 0, 1, 2, 1, 0, 1, 0, 2, 0, 2, 0, 1, 2, 1, 2, 1, 3, 2, 0, 1, 0, 2, 0, 1, 2, 1, 2, 1, 0, 2, 0, 2, 1, 0, 1, 3, 2, 0, 2, 1, 0, 2, 1, 2, 1, 0, 1, 2, 0, 1, 0, 2, 0, 1, 3, 2, 1, 2, 1, 0, 1, 0, 2, 0, 1, 2, 0, 1, 0, 2, 0, 2, 1};
        //3 start, 2 left, 1 mid, 0 right
        private static double laneWidth = 3.85;
 

        public MainWindow()
        {
            InitializeComponent();
            folderTB.Text = System.AppDomain.CurrentDomain.BaseDirectory;
            initThread(); 
            refreshFileScan();
        }

        private void initThread(){
            convertThread = new Thread(new ThreadStart(convertFiles));//init global thread var
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();



            dialog.Description = "select the folder with the SILAB LCT data...";
            dialog.SelectedPath = folderTB.Text;

            DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                folderTB.Text = dialog.SelectedPath +System.IO.Path.DirectorySeparatorChar;
                refreshFileScan();
            }
        }

        private void refreshFileScan() {
            try
            {
                filePaths = System.IO.Directory.GetFiles(folderTB.Text, "*.asc");
                fileScanTB.Text = "found " + filePaths.Count().ToString() +" asc-files in the folder";
                if ((filePaths.Count() > 0) && (!convertThread.IsAlive))
                {
                    convertB.IsEnabled = true;
                }
                else {
                    convertB.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                fileScanTB.Text = "Error: "+ ex.Message;
            }

        }

        private static int getTargetAreaFromXpos(double xPos){//helper function see LCT ISO Figure D.1

            if (xPos > (laneWidth + 1d)) { return(7); }// right beside lane 0
            if (xPos > (laneWidth - 1d)) { return(6); }//within lane 0
            if (xPos > (1d)) { return(5); }//between lane 1 and lane 0
            if (xPos > (-1d)) { return(4); }//within lane 1
            if (xPos > (-laneWidth + 1d)) { return(3); }//between lane 2 and lane 1
            if (xPos > (-laneWidth - 1d)) { return(2); }//within lane 2
            return (1); //left of lane 2

        }

        private static int convertLaneToTargetArea(int lane){//helper function see LCT ISO Figure D.1
 
            if (lane == 0) return 6;
            if (lane == 1) return 4;
            if (lane == 2) return 2;
            return 0;

        }


        private static void saveAndShift(double[] array, double newValue){//shift reg helper

            array[0] = array[1];
            array[1] = array[2];
            array[2] = newValue;

        }

        //calc() returns array [experiment, dev, length]
        //   "experiment" experiment is "!=0" when we are on the track and between start sign and before end of track 
        //   "dev" is deviation from desired 
        //   "length" is length 
        //   dev and length are summed up by the caller on every call. caller calculates later mDev = sumOfDev / sumOfLength

        //TODO (future)
        //   "targetArea" ISO defines 7 target areas (1 to 7) 40m to 140m behind a sign. to detect if the participant changed the lane Figure D.1. (0 means not within 40m to 140m)
        //   "lastTargetLane" is the lastTagetLane before the passed sign (2 left, 1 mid, 0 right)
        //   "targetLane" is the desiredLane after the laneChange(2 left, 1 mid, 0 right)
        //    caller has to sum up targetArea values and make a decision based upon "lastTargetLane" and "targetLane"
        //   "currentYPosOnTrack"
        //
        private static double[] calc(double[] trackA, double[] xPosA, double[] yPosA)
        {
            double[] returnVal = new double[3];
            int trackInt = Convert.ToInt32(trackA[2]-1);
            double trackStartMeter = trackStarts[trackInt];
            double currentYPosOnTrack = yPosA[1] - trackStartMeter; // use last y =>[1]. so [0] is y-1 and [2] is y+1
            int passedSignIndex = -1;
            double passedSignPos = -1;
            double nextSignPos = -1;
            int nextSignMeaning = -1;

            //asserts---------------------------------------
            if ((trackA[0] != trackA[1]) || (trackA[1] != trackA[2])) { return returnVal; }//values are not from the same track. return [0]array
            if (currentYPosOnTrack < signPos[trackInt * 19]) { return returnVal; }//yPos is lower than start sign. return [0] array
            if (currentYPosOnTrack > signPos[trackInt * 19+18]+ 50d) { return returnVal; }//yPos is beyond 50 meters of last sign. return [0] array

            returnVal[0] = 1;//"experiment" running. we are on the track and passed the start and are not beyond the end of the track


            for(int i= 0; i< 19; i++){//find last passed sign
                if (currentYPosOnTrack <= signPos[(trackInt * 19) + i])
                {
                    break;
                }else {
                    passedSignIndex = (trackInt * 19) + i;
                }
            }
            passedSignPos = signPos[passedSignIndex];

            int passedSignMeaning = signMeaning[passedSignIndex];

            if (passedSignIndex % 19 < 18)
            {//if this is not the last sign on track, get the position of the next sign
                nextSignPos = signPos[passedSignIndex + 1];
                nextSignMeaning = signMeaning[passedSignIndex+1];
            }
            else {
                nextSignPos = -1;
                nextSignMeaning = -1;
            }


            int desiredLane = passedSignMeaning;
            if (passedSignMeaning == 3){ //start sign means mid position
                desiredLane = 1;
            }

            //if this is not the start sign on the track, get the meaning of the sign before the lastPassed sign (to detect if participant missed a change)
            int formerlyDesiredLane = -1;
            if ((passedSignIndex % 19) != 0){
                formerlyDesiredLane = signMeaning[passedSignIndex-1];
                if (formerlyDesiredLane == 3){ //start sign means mid position
                    formerlyDesiredLane = 1;
                }
            }

            //--------------------------------------------------------------------------------------------
            //calc values for mdev------------------------------------------------------------------------
            //--------------------------------------------------------------------------------------------
            double desiredXpos = 0;
            double distanceToNextSign = nextSignPos - currentYPosOnTrack;//only meaningful if (nextSignPos != -1)

            if (//we are more than 30 meters away from next sign or behind the last sign
                ((nextSignPos != -1) && (distanceToNextSign >= 30)) || 
                 (nextSignPos == -1)
                ) {
                    desiredXpos = laneWidth - (desiredLane * laneWidth);
            }
            if (//we are closer than 20 meters from next sign
                ((nextSignPos != -1) && (distanceToNextSign <= 20))
                )
            {
                desiredXpos = laneWidth - (nextSignMeaning * laneWidth);
            }
            if (//we are closer than 20 to 30 meters from next sign. this should be a lane change
                (nextSignPos != -1) && (distanceToNextSign > 20) && (distanceToNextSign < 30)
                )
            {
                double desiredXposBeforeLaneChange = laneWidth - (desiredLane * laneWidth);
                double desiredXposAfterLaneChange = laneWidth - (nextSignMeaning * laneWidth);

                desiredXpos = desiredXposAfterLaneChange + ((desiredXposBeforeLaneChange - desiredXposAfterLaneChange) / 10d) * (nextSignPos - 20 - currentYPosOnTrack);
            }

            double xDev = Math.Abs(xPosA[1] - desiredXpos);
            double yDiff = (yPosA[2] - yPosA[0]) / 2;//mid
            returnVal[1] = xDev * yDiff;
            returnVal[2] = yDiff;


            /*
             
            //TODO
            //--------------------------------------------------------------------------------------------
            //calc values for correct lane change---------------------------------------------------------
            //--------------------------------------------------------------------------------------------
            double travelledSinceLastSign = currentYPosOnTrack - passedSignPos;
            // we are 40 to 140 meters behind the last sign and it was not a start sign
            if ((travelledSinceLastSign > 40d) && (travelledSinceLastSign < 140d) && (signMeaning[passedSignIndex] != 3))
            {
                returnVal[3] = getTargetAreaFromXpos(xPosA[1]);
            }
            else {
                returnVal[3] = 0;
            }

                returnVal[4] = formerlyDesiredLane;//"lastTargetLane"
                returnVal[5] = desiredLane;//current "targetLane"
                returnVal[6] = currentYPosOnTrack;
                returnVal[7] = passedSignPos;
            */
            //Console.WriteLine(passedSignIndex);



            return returnVal;
        }

        private void convertFiles(){

            StringBuilder reportString = new StringBuilder();

            reportString.Append("report " + reportLineEnd);


            System.IO.Directory.CreateDirectory(convFolder);

            reportString.Append("created folder: " + convFolder + reportLineEnd);

            for (int i = 0; i < filePaths.Count(); i++)
            {

                    string inFile = filePaths[i];
                    string outFile = convFolder + System.IO.Path.GetFileNameWithoutExtension(inFile) + ".txt";

                    try
                    {

                        long timestamp = System.IO.File.GetCreationTime(inFile).Ticks - DateTime.Parse("01/01/1970 00:00:00").Ticks;
                        timestamp /= 10000; //Convert windows ticks to milliseconds

                        reportString.Append("READ [" + (i+1).ToString() + " file][file=" + inFile + "]" + reportLineEnd);
                        reportString.Append("CONVERT TO [" + (i + 1).ToString() + " file][file=" + outFile + "]" + reportLineEnd);


                        reportTB.Dispatcher.Invoke(
                                        new UpdateTextCallback(this.updateReportText),
                                        new object[] { reportString.ToString() }
                        );



                        using (StreamReader reader = new StreamReader(inFile))
                        {
                            using (StreamWriter writer = new StreamWriter(outFile))
                            {

                                string line = reader.ReadLine();
                                int lineCount = 0;
                                int statTime = 1;//avoid div 0
                                long[,] lowSpeedCount = new long[10, 10];
                                int currentMarker = 0;
                                double trackStart = 0;
                                double trackAdjustment = 0;
                                double[,] devSum = new double[10,10];
                                double[,] devLength = new double[10,10];
                                long[] targetAreaCounter = new long[8];//in which targetArea the participant drove 40m to 140m after the sign (ISO Figure D.1)

                                //some arrays and for lct and other calculations 
                                double[] timeA = new double[3];
                                double[] xPosA = new double[3];
                                double[] yPosA = new double[3];
                                double[] markerA = new double[3];
                                double[] trackA = new double[3];
                                double[] speedA = new double[3];

                                while (line != null)
                                {
                                    //textBox1.Text += line;
                                    if (lineCount < silabHeader.Count())
                                    {//read header lines and compare
                                        string sHeader = silabHeader[lineCount];
                                        string sHeaderEngl = silabHeaderEngl[lineCount];
                                        if ( (line.Contains(sHeader)) || (line.Contains(sHeaderEngl)) )
                                        {//contains dt or engl
                                            //its ok do nothing
                                        }
                                        else
                                        {//does not contain the correct dt or engl header line
                                            throw new System.InvalidOperationException("header check failed [lineCount=" + lineCount.ToString() + "] maybe not a LCT file");
                                        }
                                    }
                                    else
                                    {//read and write data


                                        //after headercheck write new header to outputfile
                                        if (lineCount ==  silabHeader.Count()-1)
                                        {
                                           writer.WriteLine(lctHeader);
                                        }

                                        string[] values = line.Split(silabDelimiter);

                                        double time = double.Parse(values[0], System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                                        double timeError = double.Parse(values[1], System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                                        double track = double.Parse(values[2], System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                                        double xPos = double.Parse(values[3], System.Globalization.CultureInfo.InvariantCulture.NumberFormat) + 5.5775; // convert silab coordinate to LCT
                                        double yPos = double.Parse(values[4], System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                                        double speed = double.Parse(values[5], System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                                        double steeringWheel = double.Parse(values[6], System.Globalization.CultureInfo.InvariantCulture.NumberFormat) * 180 / Math.PI; //convert silab rad to deg
                                        double marker = double.Parse(values[7], System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

                                        if (timeError > 10) {
                                            reportString.Append("    WARNING: time error more than 10 ms [lineCount=" + lineCount.ToString() + "]" + reportLineEnd);
                                        }


                                        if ((marker != markerA[2]) && (marker >= 48)) {
                                            currentMarker = Convert.ToInt32(marker) - 48;
                                            reportString.Append("    INFO MARKER: detected keypress value " + Convert.ToInt32(marker) + " in line " + lineCount.ToString() + " and converted it to marker " + currentMarker.ToString() + reportLineEnd);
                                        }

                                        if ((track != trackA[2]) && (Convert.ToInt32(track) != 77)){//77 curve
                                            trackStart = yPos;//save
                                            trackAdjustment = trackStarts[Convert.ToInt32(track)-1]-yPos;
                                            reportString.Append("    INFO TRACK_START: detected start of track " + Convert.ToInt32(track) + " in line " + lineCount.ToString() + " and will adjust the yPos of " + yPos.ToString("0.00") + "m by " + trackAdjustment.ToString("0.00") + "m" + reportLineEnd);
                                        }

                                        if  ((Convert.ToInt32(track) == 0)||(Convert.ToInt32(track) == 77)){//0 we passed no hedgehog and stand at the start or we driving one of the curves 77 curve
                                            yPos += 33491; //we make sure that this is out of analysis range
                                        }else{
                                            yPos += trackAdjustment;
                                        }
                                        saveAndShift(timeA, time);
                                        saveAndShift(xPosA, xPos);
                                        saveAndShift(yPosA, yPos);
                                        saveAndShift(markerA, marker);
                                        saveAndShift(trackA, track);
                                        saveAndShift(speedA, speed);

                                        int temp_marker = 0;
                                        if ((marker > 0) && (marker < 10)){
                                            temp_marker = Convert.ToInt32(marker);
                                        }

                                        if ( (track > 0) && (track < 11) ){
                                            double[] calcValue = calc(trackA, xPosA, yPosA);
                                            if ((speed < 59) && (calcValue[0] != 0)) {//low speed and experiment running
                                                lowSpeedCount[Convert.ToInt32(track - 1), temp_marker]++;
                                            }
                                            devSum[Convert.ToInt32(track - 1), temp_marker] += calcValue[1];
                                            devLength[Convert.ToInt32(track - 1), temp_marker] += calcValue[2];
                                            targetAreaCounter[Convert.ToInt32(calcValue[2])]++;//increment targetAreaCounter
                                        }


                                        writer.Write(Convert.ToInt64(timestamp + time));
                                        writer.Write(lctDelimiter);
                                        writer.Write(xPos.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture));
                                        writer.Write(lctDelimiter);
                                        writer.Write((yPos).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture));
                                        writer.Write(lctDelimiter);
                                        writer.Write(speed.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture));
                                        writer.Write(lctDelimiter);
                                        writer.Write(steeringWheel.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture));
                                        writer.Write(lctDelimiter);
                                        writer.Write(Convert.ToInt32(track));
                                        writer.Write(lctDelimiter);
                                        writer.Write(currentMarker);
                                        writer.Write(lctFileEnd);

                                        statTime = Convert.ToInt32(time);//save time for statistics

                                    }

                                    line = reader.ReadLine();
                                    lineCount++;
                                    
                                    
                                }//while line

                                //some stats
                                long dataLines = lineCount - silabHeader.Count();
                                double dataRate = dataLines *1000.0d/ statTime;
                                reportString.Append("    INFO GENERAL: file contained " + dataLines.ToString() + " data lines and a recording time of " + (statTime/1000.0d).ToString("0.") + "s. That's an average data rate of "+dataRate.ToString("0.00") +"Hz"+reportLineEnd);

                                for (int t = 0; t < 10; t++) {
                                    double tDevSum = 0;
                                    double tDevLength = 0;
                                    long tSlowSum = 0;
                                    for (int m = 0; m < 10; m++)
                                    {
                                        tDevSum += devSum[t, m];
                                        tDevLength += devLength[t, m];
                                        tSlowSum += lowSpeedCount[t, m];
                                    }
                                    if (tDevLength > 0)
                                    {
                                        double mDev = tDevSum / tDevLength;
                                        reportString.Append("    INFO MDEV TRACK: found for track " + (t + 1).ToString() + " on a length of " + tDevLength.ToString("0.0") + "m a MDEV of " + mDev.ToString("0.0000") + "m" + reportLineEnd);
                                    }
                                    if (tSlowSum > 0)
                                    {
                                        reportString.Append("    INFO LOW SPEED: track " + (t + 1).ToString() + " contained " + tSlowSum.ToString() + " data lines with speed lower 59km/h. So about " + (tSlowSum * statTime / dataLines / 1000.0d).ToString("0.0") + "s." + reportLineEnd);
                                    }

                                }
                                for (int m = 0; m < 10; m++)
                                {
                                    double tDevSum = 0;
                                    double tDevLength = 0;
                                    long tSlowSum = 0;

                                    for (int t = 0; t < 10; t++)
                                    {
                                        tDevSum += devSum[t, m];
                                        tDevLength += devLength[t, m];
                                        tSlowSum += lowSpeedCount[t, m];
                                    }
                                    if (tDevLength > 0)
                                    {
                                        double mDev = tDevSum / tDevLength;
                                        reportString.Append("    INFO MDEV MARKER: found for marker " + m.ToString() + " on a length of " + tDevLength.ToString("0.0") + "m a MDEV of " + mDev.ToString("0.0000") + "m" + reportLineEnd);
                                    }
                                    if (tSlowSum > 0)
                                    {
                                        reportString.Append("    INFO LOW SPEED: marker " + m.ToString() + " contained " + tSlowSum.ToString() + " data lines with speed lower 59km/h. So about " + (tSlowSum * statTime / dataLines / 1000.0d).ToString("0.0") + "s." + reportLineEnd);
                                    }

                                }

                                /*
                                for (int m = 0; m < 10; m++)
                                {
                                    for (int t = 0; t < 10; t++)
                                    {
                                        if (devLength[t, m] > 0)
                                        {
                                            double mDev = devSum[t, m] / devLength[t, m];
                                            reportString.Append("    INFO MDEV TRACK|MARKER:  found for track " + (t + 1).ToString() + " marker " + m.ToString() + " on a length of " + devLength[t, m].ToString("0.0") + "m a MDEV of " + mDev.ToString("0.00") + "m" + reportLineEnd);
                                        }

                                    }
                                }
                                */

                                reader.Close();
                                reportString.Append("CLOSE [" + (i + 1).ToString() + " file][file=" + inFile + "]" + reportLineEnd);
                            }//using outFile
                        }//using inFile

                        //the dates of the experiments are often usefull; copy file times to new file
                        File.SetCreationTime(outFile, System.IO.File.GetCreationTime(inFile));
                        File.SetLastWriteTime(outFile, System.IO.File.GetCreationTime(inFile));
                        File.SetLastAccessTime(outFile, System.IO.File.GetCreationTime(inFile));

                    }//try
                
                    catch (Exception ex)
                    {
                        reportString.Append("ERR [" + (i+1).ToString() + " file][file=" + inFile + "] msg:" + ex.Message + reportLineEnd);
                    }
                
            }//for every file


            //log reportString to report window and report/log file
            string reportFile = convFolder + "report.log";



            try{
                    reportString.Append("WRITE REPORT [file=" + reportFile + "]" + reportLineEnd);
                    using (StreamWriter writer = new StreamWriter(reportFile))
                    {
                            writer.Write(reportString.ToString());
                    }//using report file

                }//try
            catch (Exception ex)
                {
                    reportString.Append("ERR failed to save report. msg:" + ex.Message + reportLineEnd);
                }

            reportString.Append("THREAD END");

            reportTB.Dispatcher.Invoke(
                            new UpdateTextCallback(this.updateReportText),
                            new object[] { reportString.ToString() }
            );

        }



        private void refreshB(object sender, RoutedEventArgs e)
        {
            refreshFileScan();
        }

        private void convertB_Click(object sender, RoutedEventArgs e)
        {
            convertB.IsEnabled = false;

            //convFolder to globalVar
            DateTime now = DateTime.Now;
            string timeString = now.ToString("yyyy_MM_dd_HH_mm_ss");
            convFolder = folderTB.Text + timeString + System.IO.Path.DirectorySeparatorChar;

            initThread();
            convertThread.Start();
        }


        private void updateReportText(string message)
        {
            reportTB.Text = message;
            reportTB.ScrollToEnd();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {

            System.Windows.MessageBox.Show("3-clause BSD License\r\nCopyright (c) 2013-2016 Michael Krause (krause@tum.de) Institute of Ergonomics, Technische Universität München." +
            "All rights reserved.\r\n" +
            "Redistribution and use in source and binary forms, with or without\r\n" +
            "modification, are permitted provided that the following conditions are met:\r\n" +
            "    * Redistributions of source code must retain the above copyright\r\n" +
            "      notice, this list of conditions and the following disclaimer.\r\n" +
            "    * Redistributions in binary form must reproduce the above copyright\r\n" +
            "      notice, this list of conditions and the following disclaimer in the\r\n" +
            "      documentation and/or other materials provided with the distribution.\r\n" +
            "    * Neither the name of the <organization> nor the\r\n" +
            "      names of its contributors may be used to endorse or promote products\r\n" +
            "      derived from this software without specific prior written permission.\r\n\r\n" +

            "THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS 'AS IS' AND" +
            "ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED" +
            "WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE" +
            "DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY" +
            "DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES" +
            "(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;" +
            "LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND" +
            "ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT" +
            "(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS" +
            "SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.\r\n");
        }
    }
}
