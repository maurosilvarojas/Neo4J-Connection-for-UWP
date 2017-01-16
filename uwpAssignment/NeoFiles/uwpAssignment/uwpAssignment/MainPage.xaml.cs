using Neo4j.Driver.V1;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace uwpAssignment
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        List<Ratings> ratings = new List<Ratings>();
        List<Ratings> CompareRateA = new List<Ratings>();
        List<Ratings> CompareRateB = new List<Ratings>();
        double[] vectorA = new double[160];
        double[] vectorB = new double[160];
        int counter = 0;
        public MainPage()
        {

            this.InitializeComponent();
            var driver = GraphDatabase.Driver("bolt://localhost");
            var session = driver.Session();
            var result = session.Run(" MATCH(n: Customer) -[r: RATES] - (p:Product) RETURN n.customerID,r.score,p.productID ");
                        foreach (var record in result)
            {
                int customer = Convert.ToInt32(record["n.customerID"].As<string>());
                int product = Convert.ToInt32(record["p.productID"].As<string>());
                double score = Convert.ToDouble(record["r.score"].As<string>());
                ratings.Add(myInstance(customer, product, score));
                counter++;
            }
            //Debug.WriteLine("Total Customer: " +countCustomer+ "Name: "+ name);
            //session.Run("CREATE (a:Person {name:'Arthur', title:'King'})");
            //var result = session.Run("MATCH (a:Person) WHERE a.name = 'Arthur' RETURN a.name AS name, a.title AS title");
            //var result = session.Run("MATCH (c:Customer {customerID:'19'} )-[r:RATES]->(p) RETURN c.customerID,r.score,p.productID");
            //var result = session.Run("MATCH (js:Customer)-[b1:BOUGHT]-(p:Product)-[b2:BOUGHT]-(jk:Customer)WHERE jk.customerID ='99' RETURN DISTINCT js.customerID, p.productID");working
            //var result = session.Run("MATCH (n:Customer) RETURN n.customerID");
            // Debug.WriteLine($"{record["title"].As<string>()} {record["name"].As<string>()}");
            //Debug.WriteLine($"{record["n.customerID"].As<string>()} {record["p.productID"].As<string>()} {record["r.score"].As<string>()} ");
            //foreach (Ratings r in ratings)
            {
                //Debug.WriteLine("Customer ID={0}, Product ID = {1} , Score = {2}", r.customerID, r.productID, r.score);
            }
            checkSimilarity.Click += (object sender, RoutedEventArgs e) =>
            {
                int m = Convert.ToInt32(custATextBox.Text);
                int n = Convert.ToInt32(custBTextBox.Text);
                vectorMakerA(m);
                vectorMakerB(n);
                double varSimilarity = CosineSimilarity(vectorA, vectorB);
                similarityText.Text = varSimilarity.ToString();

                var postSimilarity = session.Run(" MATCH(m: Customer{customerID:'" + m + "'}) -[r: RATES] - (p:Product) - [r1:RATES]- (n:Customer{customerID:'" + n + "'})  MERGE (m)-[s:SIMILARITY]-(n) SET s.similarity ='" + varSimilarity + "'");
            };
            locationButton.Click += (object sender, RoutedEventArgs e) =>
            {
                var locationResult = session.Run("MATCH x=(p)<-[b:BOUGHT]-(a)-[r:ACCESS_FROM]->(point:ipLocation{country:'China'}) RETURN p.productID, count(*) as recomendationLoc ORDER BY  recomendationLoc DESC ");
                List<string> location = new List<string>();
                foreach (var record in locationResult)
                {
                    ListViewItem item = new ListViewItem();
                    locationList.Items.Add(record["p.productID"].As<string>())
;                    
                }
                
            };
            ageButton.Click += (object sender, RoutedEventArgs e) =>
            {
                var agesResult = session.Run("MATCH x=(p)<-[b:BOUGHT]-(a) WHERE TOINT(a.age) > 50 AND TOINT(a.age) < 55 RETURN p.productID, count(*) as recomendationAge ORDER BY  recomendationAge DESC");
                List<string> ages = new List<string>();
                foreach (var record in agesResult)
                {
                    ListViewItem item = new ListViewItem();
                    ageListView.Items.Add(record["p.productID"].As<string>())
;
                }

            };
            maleButton.Click += (object sender, RoutedEventArgs e) =>
            {
                var maleResult = session.Run("MATCH x=(p)<-[b:BOUGHT]-(a) WHERE a.gender = 'Male'RETURN p.productID, count(*) as recomendationGender ORDER BY  recomendationGender DESC");
                List<string> males = new List<string>();
                foreach (var record in maleResult)
                {
                    ListViewItem item = new ListViewItem();
                    maleListView.Items.Add(record["p.productID"].As<string>())
;
                }

            };
            femaleButton.Click += (object sender, RoutedEventArgs e) =>
            {
                var femaleResult = session.Run("MATCH x=(p)<-[b:BOUGHT]-(a) WHERE a.gender = 'Female' RETURN p.productID, count(*) as recomendationGender ORDER BY  recomendationGender DESC");
                List<string> females = new List<string>();
                foreach (var record in femaleResult)
                {
                    ListViewItem item = new ListViewItem();
                    femaleListView.Items.Add(record["p.productID"].As<string>())
;
                }

            };
            collaborativeButton.Click += (object sender, RoutedEventArgs e) =>
            {
                var collaborativeResult = session.Run("MATCH (x:Customer)-[b1:BOUGHT]-(p1:Product)-[b2:BOUGHT]-(y:Customer) MATCH(x: Customer) -[r1: RATES] - (p2:Product)-[r2: RATES] - (y:Customer) WHERE y.customerID ='172' AND TOINT(r1.score) > 4 and TOINT(r2.score) > 4 RETURN x.customerID, count(*) as recomendationFilter ORDER BY  recomendationFilter  DESC");
                List<string> filter = new List<string>();
                foreach (var record in collaborativeResult)
                {
                    ListViewItem item = new ListViewItem();
                    collaborativeListView.Items.Add(record["x.customerID"].As<string>())
;
                }

            };
        }
            


        public Array vectorMakerA(int custID)
        {
            int m = 1;
            //double[] vectorA = new double[160];
            foreach (Ratings r in ratings)
            {

                if (r.customerID == custID)
                {
                    vectorA[r.productID] = r.score;

                    //Debug.WriteLine("Vector A: product id " +r.productID+"  Pointer:"+ r.productID + ", value:" + vectorA[r.productID]);

                }

            }

            for (int x = 0; x < vectorA.Length; x++)
            {
                Debug.WriteLine("Vector A: product " + x + ":" + vectorA[x]);
            }

            return vectorA;
        }

        public Array vectorMakerB(int custID)
        {
            int m = 1;

            foreach (Ratings r in ratings)
            {

                if (r.customerID == custID)
                {
                    vectorB[r.productID] = r.score;

                    //Debug.WriteLine("Vector B: product id " + r.productID + "  Pointer:" + r.productID + ", value:" + vectorB[r.productID]);

                }

            }

            for (int x = 0; x < vectorB.Length; x++)
            {
                Debug.WriteLine("Vector B: product " + x + ":" + vectorA[x]);
            }

            return vectorB;
        }

        private double CosineSimilarity(double[] arrayA, double[] arrayB)
        {
            double sumA = 0;
            double sumB = 0;
            double sumC = 0;
            double multip = 0;

            double similarity = 0;
            int j = 5;

            // double[] arrayA = new double[5] { 2, 3, 4, 5, 4};
            //double[] arrayB = new double[5] { 2, 3, 4, 5, 0 };

            for (int i = 0; i < arrayA.Length; i++)
            {
                sumA = Math.Pow(arrayA[i], 2) + sumA;
            }
            for (int i = 0; i < arrayB.Length; i++)
            {
                sumB = Math.Pow(arrayB[i], 2) + sumB;
            }
            for (int i = 0; i < arrayA.Length; i++)
            {
                multip = arrayA[i] * arrayB[i];
                sumC = sumC + multip;
            }
            similarity = (sumC) / ((Math.Sqrt(sumA)) * (Math.Sqrt(sumA)));
            Debug.WriteLine(similarity);
            return similarity;
        }

        public Ratings myInstance(int customer, int product, double score)
        {
            Ratings newRating = new Ratings();
            newRating.customerID = customer;
            newRating.productID = product;
            newRating.score = score;
            return newRating;
        }
        public Ratings myInstance(int product, double score)
        {
            Ratings newRating = new Ratings();
            newRating.productID = product;
            newRating.score = score;
            return newRating;
        }

        private void checkSimilarity_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void locationButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void textBlock6_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }
    }

    public class Ratings
    {
        public int customerID { get; set; }
        public int productID { get; set; }
        public double score { get; set; }


    }

    
}

      







