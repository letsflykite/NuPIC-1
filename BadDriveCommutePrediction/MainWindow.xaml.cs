using System;
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

using System.IO;
using Microsoft.Maps.MapControl.WPF;
using System.Threading;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace BadDriveCommutePrediction
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int LastMapItem = 0;
        private List<Tuple<MapPolygon, MapPolygon, string>> latLong = new List<Tuple<MapPolygon, MapPolygon, string>>();

        public MainWindow()
        {
            InitializeComponent();
            latLong = GetChords();
            timeLabel.Content = "YOU NEED TO GET YOUR OWN BING MAP DEVELOEPR KEY";
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //TheWorld.Children.Add(latLong[LastMapItem]);
            //LastMapItem++;
            //if (LastMapItem >= latLong.Count)
            //    LastMapItem = latLong.Count - 1;

            //TODO: For demo purposes, all traffic incidents are shown immediately. It woudld be better to interleve them with the training data
            foreach (Tuple<MapPolygon, MapPolygon, string> mp in latLong)
            {
                TheWorld.Children.Add(mp.Item1);
            }

            
        }


        private int predictionCount = 0;

        private void nextPrediction_Click(object sender, RoutedEventArgs e)
        {
            latLong.Reverse();
            
            //TODO: For demo purposes, only a few prediction data points were shown. You can show them all, but it woudld be better to interleve them with the training data
            //foreach (Tuple<MapPolygon, MapPolygon, string> mp in latLong)
            {
                timeLabel.Content = latLong[predictionCount].Item3;
                TheWorld.Children.Add(latLong[predictionCount].Item2);
                ++predictionCount;
                
            }
        }

        

        private List<Tuple<MapPolygon, MapPolygon, string>> GetChords()
        {
            IEnumerable<string> lines = File.ReadLines("FakeTrafficData.csv");
            List<Tuple<MapPolygon, MapPolygon, string>> result = new List<Tuple<MapPolygon, MapPolygon, string>>();
            
            string[] splitty = new string[1200];    //TODO: Hard coded due to hackathon time constraints. Actual set was only 1000 rows
            foreach (string l in lines)
            {
                splitty = l.Split(',');

                MapPolygon newPoint = new MapPolygon()
                {
                    Fill = Brushes.Red,
                    Stroke = Brushes.Red,
                    Locations = new LocationCollection()
                    {   //TODO: This section dictates the shape of the map polygon. It could be refined to show a better shape
                        new Location(Convert.ToDouble(splitty[2]) - .001, Convert.ToDouble(splitty[3]) - .001),
                        new Location(Convert.ToDouble(splitty[2]) + .001, Convert.ToDouble(splitty[3]) + .001),
                        new Location(Convert.ToDouble(splitty[2]) + .001, Convert.ToDouble(splitty[3]) - .001)
                    }
                };

                MapPolygon predictedPoint = new MapPolygon()
                {
                    Fill = Brushes.Green,
                    Stroke = Brushes.Green,
                    Locations = new LocationCollection()
                    {   //TODO: This section dictates the shape of the map polygon. It could be refined to show a better shape
                        new Location(Convert.ToDouble(splitty[9]) - .001, -122.3 - .001),
                        new Location(Convert.ToDouble(splitty[9]) + .001, -122.3 + .001),
                        new Location(Convert.ToDouble(splitty[9]) + .001, -122.3 - .001)
                    }
                };

                result.Add(new Tuple<MapPolygon, MapPolygon, string>(newPoint, predictedPoint, Convert.ToDateTime(splitty[0]).ToLongTimeString()));
            }

            return result;
        }

        
    }
}