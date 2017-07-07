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
            try
            {
                XmlTextReader reader = new XmlTextReader("FlightLog3.xml");

                List<Content> content = new List<Content>();
                //PlayLog p = new PlayLog(reader, content);

                while (reader.Read())//&& reader.NodeType != XmlNodeType.EndElement)
                {
                    if (reader.Name.Equals("PlayLog"))//&& reader.NodeType != XmlNodeType.EndElement)
                    {
                        if (reader.NodeType != XmlNodeType.EndElement)
                        {
                            PlayLog p = new PlayLog(reader, content);
                            p.processPlayLog();
                        }
                    }
                }

                if (File.Exists("C:/Users/sfalconer/Desktop/parsed.txt"))
                    File.Delete("C:/Users/sfalconer/Desktop/parsed.txt");

                foreach (Content c in content)
                {
                    Console.WriteLine(c);
                    Console.WriteLine("");
                    writeToFile(c);
                }
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

        public static void writeToFile(Content c)
        {
            string path = "C:/Users/sfalconer/Desktop/parsed.txt";
            //FileStream fs = new FileStream(path, FileMode.Append);

            //File.WriteAllText(path, c.ToString());

            File.AppendAllText(path, c.ToString());
            File.AppendAllText(path, "\r\n");
        }

    }

    public class PlayLog
    {
        List<Content> content;
        private XmlTextReader reader;
        private PlayEvent playEvent;

        private string title;
        private string [] titleSplit;
        private string menuCategory;
        private string menuTitle;

        private string UID;
        private string airline;
        private string contentSet;

        private string attribute;

        public PlayLog(XmlTextReader reader, List<Content> content)
        {
            this.content = content;
            this.reader = reader;
            playEvent = new PlayEvent(reader);

            airline = "JAF";
            contentSet = "TB123";
        }

        public void processPlayLog()
        {
            title = reader.GetAttribute("navigationPath");
            if (title != "")
            {
                titleSplit = seperateTitle(title);
                menuCategory = titleSplit[0];
                menuTitle = titleSplit[1];
            }
            else
            {
                menuCategory = "N/A";
                menuTitle = "N/A";
            }

            Console.WriteLine("Log");

            while (reader.Read()) //ADD CONDITIONAL HERE)
            {
                if (reader.Name.Equals("playEvent"))
                {
                    if (reader.NodeType != XmlNodeType.EndElement)
                    {
                        Console.WriteLine(reader.GetAttribute("type"));
                        playEvent.processPlayEvent();
                    }
                }
                else if (reader.Name.Equals("playEvents") && reader.NodeType == XmlNodeType.EndElement)
                {
                    Console.WriteLine("End of PlayLog\r\n");
                    break;
                }
            }
            content.Add(new Content
            { menuCategory = menuCategory, title = menuTitle, start = playEvent.startTime.ToString(), duration = getDurationOfLog(playEvent.startTime, playEvent.endTime).TotalSeconds.ToString(), totalTimePaused = playEvent.totalTimePaused.TotalSeconds.ToString(), totalTimePlayed = playEvent.totalTimePlayed.TotalSeconds.ToString(), UDID = generateUID().ToString(), marketingSequence = "1", marketingStingerID = "1", sessionID = generateUID().ToString(), airline = airline, contentSet = contentSet, seriesIdentifier = "0", topPick = "0", paymentType = "0", expectedDuration = "0", flightExpired = "0", flightNumber = "-1" });
        }

        public string[] seperateTitle(string title)
        {
            if (title != null)
                return title.Split('/');
            else
                return null;
        }
        public TimeSpan getDurationOfLog(DateTime startTime, DateTime endTime)
        {
            return endTime - startTime;
        }
        public Guid generateUID()
        {
            return Guid.NewGuid();
        }
    }

    public class PlayEvent
    {
        private XmlTextReader reader;

        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }

        public TimeSpan totalTimePlayed { get; set; }
        public TimeSpan totalTimePaused { get; set; }

        public DateTime startHolder;
        public DateTime endHolder;
        int flag; //for determining if total times should be incremented; 0 = no, 1 = played, 2 = paused

        public PlayEvent(XmlTextReader reader)
        {
            this.reader = reader;
        }

        public void processPlayEvent()
        {
            if (reader.GetAttribute("type") == "play" || reader.GetAttribute("type") == "Play")
                flag = 1;
            else if (reader.GetAttribute("type") == "pause" || reader.GetAttribute("type") == "Pause")
                flag = 2;
            else
                flag = 0;

            while (reader.NodeType != XmlNodeType.EndElement && reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "startTime":
                            if (!reader.IsEmptyElement)
                            {
                                startHolder = (DateTime.ParseExact(reader.ReadString(), "yyyy_MM_dd HH:mm:ss", null));
                                compareTimes(startHolder);
                                Console.WriteLine(startHolder);
                                reader.Read();
                            }
                            break;

                        case "endTime":
                            if (!reader.IsEmptyElement)
                            {
                                endHolder = (DateTime.ParseExact(reader.ReadString(), "yyyy_MM_dd HH:mm:ss", null));
                                compareTimes(endHolder);
                                Console.WriteLine(endHolder);
                                reader.Read();
                            }
                            break;

                        default:
                            reader.Skip();
                            break;
                    }
                }
            }
            playEventDuration(startHolder, endHolder, flag);
            //Console.WriteLine("Time Played: " + totalTimePlayed);
            //Console.WriteLine("Time Paused: " + totalTimePaused);
            Console.WriteLine("");
        }

        public void compareTimes(DateTime newTime)
        {
            if (startTime.Equals(DateTime.MinValue))
            {
                startTime = newTime;
            }

            if (DateTime.Compare(newTime, startTime) == 0)
            {
                startTime = newTime;
            }

            if (endTime.Equals(DateTime.MinValue))
            {
                endTime = newTime; 
            }

            if (DateTime.Compare(newTime, endTime) == 1)
            {
                endTime = newTime;
            }
            //Console.WriteLine(DateTime.Compare(newTime, startTime));
        }
        public void playEventDuration(DateTime startTime, DateTime endTime, int flag)
        {
            if (flag == 1)
            {
                totalTimePlayed += endTime - startTime;
            }
            else if (flag == 2)
            {
                totalTimePaused += endTime - startTime;
            }
            else if (flag == 0)
            { }//Do nothing
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
        public string flightExpired { get; set; }
        public string marketingSequence { get; set; }
        public string marketingStingerID { get; set; }
        public string sessionID { get; set; }
        public string seriesIdentifier { get; set; }
        public string topPick { get; set; }
        public string paymentType { get; set; }

        public override string ToString()
        {
            return string.Format($"{menuCategory}\t{menuGenre}\t{artistAlbum}\t{title}\t{start}\t{duration}\t{deviceName}\t{UDID}\t{airline}\t{contentSet}\t{flightNumber}\t{totalTimePaused}\t{totalTimePlayed}\t{expectedDuration}\t{flightExpired}\t{marketingSequence}\t{marketingStingerID}\t{sessionID}\t{seriesIdentifier}\t{topPick}\t{paymentType}");
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
