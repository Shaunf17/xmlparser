﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Data;

namespace XmlParserThing
{
    class Program
    {
        static void Main(string[] args)
        {
            string aircraftId = "JETAIR-143RTU";

            try
            {
                //Console.WriteLine(aircraftId);
                //Console.WriteLine("");
                //XmlTextReader reader = new XmlTextReader("FlightLogs.xml");
                //ProcessLogs(reader);

                tabDelimit("hello", null);
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

        static void ProcessLogs(XmlTextReader reader)
        {
            string movieTitle;
            EventList el;

            while (reader.Read())
            {
                if (reader.Name.Equals("PlayLog"))
                {
                    //Console.WriteLine(reader.GetAttribute("navigationPath"));
                    movieTitle = reader.GetAttribute("navigationPath");
                    Console.WriteLine(movieTitle);
                }

                if (reader.Name.Equals("playEvent") &&
                    (reader.NodeType == XmlNodeType.Element))
                {
                    if (reader.GetAttribute("type") != null)
                    {
                        Console.WriteLine("---{0}---", reader.GetAttribute("type"));
                        el = new EventList();
                        ProcessPlayEvent(reader, reader.GetAttribute("type"), el);
                    }
                    else
                    {
                        reader.Skip();
                    }
                }
            }
        }

        static void ProcessPlayEvent(XmlTextReader reader, string attr, EventList el)
        {
            TimeSpan ts;
            string startTime = "";
            string endTime = "";

            while (reader.NodeType != XmlNodeType.EndElement &&
                reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    //Compare each element name and extract the ones we are interested in
                    switch (reader.Name)
                    {
                        //case "bitRate":
                        //    Console.WriteLine("Bitrate: " + reader.ReadString());
                        //    reader.Read();
                        //    break;

                        case "startTime":
                            startTime = reader.ReadString().ToString();
                            //Console.WriteLine("Start Time: " + startTime);
                            reader.Read();
                            break;

                        case "endTime":
                            endTime = reader.ReadString().ToString();
                            reader.Read();
                            break;

                        //If reader cannot find a use case it skips to the next one
                        default:
                            reader.Skip();
                            break;

                    } //End switch
                } //End if
            } //End element comparison

            //Displays information about each individual Play Event
            if (endTime == "")
                endTime = startTime;
            //Console.WriteLine("Start Time: {0} \nEnd Time: {1}", startTime, endTime);

            //if (startTime != "" && endTime != "")
            //{
            //    ts = getDurationOfPlayEvent(startTime, endTime);
            //    Console.WriteLine("Total Duration: {0} secs", ts.TotalSeconds);
            //    //Console.WriteLine("Day(s): " + ts.Days);  //Time in days
            //}

            ts = getDurationOfPlayEvent(startTime, endTime);

            //Console.WriteLine("EventID: " + generatePlayEventId(attr));
            //Console.WriteLine("");

            el.add(startTime, endTime, ts.TotalSeconds, generatePlayEventId(attr));
            el.printList();
            Console.WriteLine("");
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

        public static string generatePlayEventId(string attr)
        {
            string id;
            id = attr.Substring(0,3) + "-" + Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 4);

            return id;
        }

        public static void tabDelimit(string title, EventList el)
        {
            var delimiter = "\t";
            //var itemContent = "H\t13\t170000000000001\t20150630D\t1050\t10.0000\tYD\t1050\t5.0000\tN";
            var itemContent = "this\tis\ta\ttest\tof\ttsv";

            FileStream fs = File.Create("C:/Users/sfalconer/Documents/Output.csv");

            Byte[] info = new UTF8Encoding(true).GetBytes(itemContent);
            fs.Write(info, 0, info.Length);

            var line = string.Join(delimiter, itemContent);
            Console.WriteLine(line);
            
        }
    }

    public class Node
    {
        public Node next;
        public object startTime;
        public object endTime;
        public object duration;
        public object eventID;
    }

    public class EventList
    {
        private Node head;
        private Node current;
        public int count;

        public EventList()
        {
            head = new Node();
            current = head;
        }

        public void add(Object startTime, Object endTime, Object duration, Object eventID)
        {
            Node newNode = new Node();
            newNode.startTime = startTime;
            newNode.endTime = endTime;
            newNode.duration = duration;
            newNode.eventID = eventID;
            current.next = newNode;
            current = newNode;
            count++;
        }

        public void printList()
        {
            Node curr = head.next;
            while (curr != null)
            {
                //curr = curr.next;
                //Console.WriteLine("Start Time: {0}\nEnd Time: {1}\nDuration: {2} secs\nEventID: {3}", curr.startTime, curr.endTime, curr.duration, curr.eventID);
                curr = curr.next;
            }
        }

        public void collateList()
        {
            
        }
    }


    
}
