using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SC.API.ComInterop;
using Attribute = SC.API.ComInterop.Models.Attribute;

namespace SCDateBuckets
{
    class Program
    {
        static void Main(string[] args)
        {
            var userid = ConfigurationManager.AppSettings["userid"];
            if (string.IsNullOrEmpty(userid))
            {
                Console.WriteLine("Please enter your username");
                userid = Console.ReadLine();
            }
            var passwd = ConfigurationManager.AppSettings["passwd"];
            if (string.IsNullOrEmpty(passwd))
            {
                Console.WriteLine("Please enter your password");
                passwd = Console.ReadLine();
            }
            var URL = ConfigurationManager.AppSettings["URL"];
            if (string.IsNullOrEmpty(URL))
            {
                URL = "https://my.sharpcloud.com";
            }
            var AttributeName = ConfigurationManager.AppSettings["attributeName"];
            if (string.IsNullOrEmpty(AttributeName))
            {
                Console.WriteLine("Please enter the attribute you want to store the dates in");
                AttributeName = Console.ReadLine();
            }
            var storyID = ConfigurationManager.AppSettings["storyID"];
            if (string.IsNullOrEmpty(AttributeName))
            {
                Console.WriteLine("Please enter the ID of the story you want edit");
                storyID = Console.ReadLine();
            }


            var sc = new SharpCloudApi(userid, passwd, URL);

            var story = sc.LoadStory(storyID);
            // delete the attribute
            story.Attribute_DeleteByName(AttributeName);

            var attribute = story.Attribute_Add(AttributeName, Attribute.AttributeType.List);

            foreach (var item in story.Items.OrderBy(i => i.StartDate))
            {
                var s = GetQtr(item.StartDate);
                attribute.Labels_Add(s);
                item.SetAttributeValue(attribute, s);
            }

            story.Save();


        }

        static string GetQtr(DateTime start)
        {
            int quarter = 1; // assume Jan,Feb,Mar unless proven otherwise
            switch (start.Month)
            {
                case 4:
                case 5:
                case 6:
                    quarter = 2;
                    break;
                case 7:
                case 8:
                case 9:
                    quarter = 3;
                    break;
                case 10:
                case 11:
                case 12:
                    quarter = 4;
                    break;
            }
            return $"Q{quarter} {start.Year}";
        }

    }
}
