using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Data;
using System.Reflection;

namespace XmlParserThing
{
    class Program
    {
        static void Main(string[] args)
        {
            string aircraftId = "JETAIR-143RTU";

            try
            {
                if (File.Exists("C:/Users/sfalconer/Desktop/generic.txt"))
                    File.Delete("C:/Users/sfalconer/Desktop/generic.txt"); //Temporary deletion of the file to start with clean slate
                Console.WriteLine(aircraftId);
                Console.WriteLine("");
                XmlTextReader reader = new XmlTextReader("FlightLogs.xml");
                //ProcessLogs(reader);
                PlayLog p = new PlayLog(reader);
                p.processPlayEvent();
                p.printList();

            }
            catch (XmlException xe)
            {   
                Console.WriteLine("XML Parsing Error: " + xe);
            }
            catch (IOException ioe)
            {
                Console.WriteLine("File I/O Error: " + ioe);
            }
            Console.ReadKey();
        }

        public static void writeToFile()
        {
        }

        static void ProcessLogs(XmlTextReader reader)
        {
            FileStream fs = null;
            string filePath = Environment.GetFolderPath(
                         System.Environment.SpecialFolder.DesktopDirectory);
            string title = "";
            string playEvent = "";

            List<Content> content = new List<Content>();
            int test = 0;

            while (reader.Read())
            {
                if (reader.Name.Equals("PlayLog"))
                {
                    //GETS TITLE OF MOVIE
                    title = reader.GetAttribute("navigationPath");
                    //Console.WriteLine(title);
                }

                if (reader.Name.Equals("playEvent") &&
                    (reader.NodeType == XmlNodeType.Element))
                {

                    if (reader.GetAttribute("type") != null)
                    {
                        //GETS PLAY EVENT NAME
                       // playEvent = reader.GetAttribute("type");
                        //Console.WriteLine("---{0}---", reader.GetAttribute("type"));
                        ProcessPlayEvent(title, reader, reader.GetAttribute("type"), content, test);    //READ INFO FROM PLAY EVENT 
                    }
                    else
                    {
                        reader.Skip();
                    }
                }
            }

            foreach (Content c in content)
            {
                Console.WriteLine(c);
            }

        }

        static void ProcessPlayEvent(string title, XmlTextReader reader, string attr, List<Content> content, int test)
        {
            TimeSpan ts;
            string[] contentTitle;
            string titleCategory;
            string titleGenre;

            string startTime = "";
            string endTime = "";

            string totalPlayed;
            string totalPaused;
                    
            contentTitle = seperateTitle(title);
            titleCategory = contentTitle[0];
            titleGenre = contentTitle[1];

            while (reader.NodeType != XmlNodeType.EndElement &&
                reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    //Compare each element name and extract the ones we are interested in
                    switch (reader.Name)
                    {
                        case "startTime":
                            startTime = reader.ReadString().ToString();
                            //Console.WriteLine("Start Time: " + startTime);
                            reader.Read();
                            break;

                        case "endTime":
                            endTime = reader.ReadString().ToString();
                            reader.Read();
                            break;

                        default:
                            reader.Skip();
                            break;

                    } //End switch
                } //End if
            } //End element comparison
              //Displays information about each individual Play Event

            if (endTime == "")
                endTime = startTime;
            ts = getDurationOfPlayEvent(startTime, endTime);

            content.Add(new Content { menuCategory = titleCategory, menuGenre = titleGenre, duration = ts.TotalSeconds.ToString(), start = startTime});

            //Console.WriteLine("");
        }


        public static string[] seperateTitle(string title)
        {
            if (title != null)
                return title.Split('/');
            else
                return null;
        }

        public static TimeSpan getDurationOfPlayEvent(string startTime, string endTime)
        {
            //string start = startTime;
            //string end = endTime;

            TimeSpan duration;

            DateTime startDate = DateTime.ParseExact(startTime, "yyyy_MM_dd HH:mm:ss", null);
            DateTime endDate = DateTime.ParseExact(endTime, "yyyy_MM_dd HH:mm:ss", null);

            duration = endDate - startDate;
            return duration;
        }

        public static TimeSpan getTotalTimePlayed(string playtime)
        {
            TimeSpan totalTimePlayed = new TimeSpan();

            return totalTimePlayed;
        }

        public static string generatePlayEventId(string attr)
        {
            string id;
            id = attr.Substring(0, 3) + "-" + Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 4);

            return id;
        }

        public static void tabDelimit(string filePath, string attr, FileStream fs)
        {
            //var path = "C:/Users/Shaun/Desktop/output.txt"; 
            StreamWriter sw;
            var delimiter = "\t";
            string itemContent = "this\tis\ta\ttest\tof\ttsv";
            string fileName = "/generic.txt";

            if (!File.Exists(filePath + fileName))
            {
                Console.WriteLine("File Created");
                var myFile = File.Create(filePath + fileName);
                myFile.Close();
            }

            File.AppendAllText(filePath + fileName, attr + "\r\n");
            File.AppendAllText(filePath + fileName, "\r\n");
        }
    }

    public class PlayLog
    {
        List<Content> content;

        private XmlTextReader reader;
        private PlayEvent playEvent;

        private string title;
        private string UID;
        private string airline;
        private string contentSet;

        public PlayLog(XmlTextReader reader)
        {
            content = new List<Content>();

            this.reader = reader;
            playEvent = new PlayEvent(reader);
        }

        public void processPlayEvent()
        {
            string title;
            string attribute;

            //List<Content> c = new List<Content>();

            Console.WriteLine(sqr(playEvent.returnFive()));

            content.Add(new Content { menuCategory = "Movie", menuGenre = "Horror", duration = "1600" });
            content.Add(new Content { menuCategory = "Music", menuGenre = "R&B", duration = "186" });

            //Console.WriteLine(getContent(content));

        }

        public void printList()
        {
            foreach (Content c in content)
            {
                Console.WriteLine(c);
            }
        }

        public static string[] seperateTitle(string title)
        {
            if (title != null)
                return title.Split('/');
            else
                return null;
        }

        public int sqr(int num)
        {
            return num * num; 
        }

        //public static TimeSpan getDurationOfLog()
        //{ }

        //public static

    }

    public class PlayEvent
    {
        private XmlTextReader reader;

        public PlayEvent(XmlTextReader reader)
        {
            this.reader = reader;
        }

        public void printPlayEvent()
        {
            Console.WriteLine("Working");

            while (reader.Read())
            {
                if (reader.Name.Equals("playEvent"))
                {
                    Console.WriteLine(reader.GetAttribute("type"));
                    reader.Read();
                }
                else
                {
                    reader.Skip();
                }
            }
        }

        public int returnFive()
        {
            return 5;
        }



    }

    public class Content : IEquatable<Content>
    {
        public string menuCategory { get; set; }        
        public string menuGenre { get; set; }           
        public string artistAlbum { get; set; }         
        public string title { get; set; }               
        public string start { get; set; }               
        public string duration { get; set; }            
        public string deviceName { get; set; }          
        public string UDID { get; set; }
        public string airline { get; set; }
        public string contentSet { get; set; }
        public string flightNumber { get; set; }
        public string totalTimePaused { get; set; }
        public string totalTimePlayed { get; set; }
        public string expectedDuration { get; set; }
        public bool flightExpired { get; set; }
        public string marketingSequence { get; set; }
        public string marketingStingerID { get; set; }
        public string sessionID { get; set; }
        public string seriesIdentifier { get; set; }
        public string topPick { get; set; }
        public string paymentType { get; set; }

        public override string ToString()
        {
            return string.Format($"{menuCategory}\t{menuGenre}\t{artistAlbum}\t{title}\t{start}\t{duration}\t{deviceName}\t{UDID}\t{airline}\t{contentSet}\t{flightNumber}\t{totalTimePaused}\t{totalTimePlayed}\t{expectedDuration}\t{flightExpired.ToString()}\t{marketingSequence}\t{marketingStingerID}\t{sessionID}\t{seriesIdentifier}\t{topPick}\t{paymentType}");
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            Content objAsContent = obj as Content;
            if (objAsContent == null)
                return false;
            else
                return Equals(objAsContent);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode(); 
        }

        public bool Equals(Content other)
        {
            if (other == null)
                return false;
            return (this.sessionID.Equals(other.sessionID));
        }
    }
}
