using System.Collections.Generic;

namespace Utilities
{
    public static partial class UtilityHelper
    {
        // Returns a random script that can be used to id
        public static string GetIdString()
        {
            string alphabet = "0123456789abcdefghijklmnopqrstuvxywz";
            string ret = "";
            for (int i = 0; i < 8; i++)
                ret += alphabet[UnityEngine.Random.Range(0, alphabet.Length)];
            return ret;
        }

        // Returns a random script that can be used to id (bigger alphabet)
        public static string GetIdStringLong() => GetIdStringLong(10);

        // Returns a random script that can be used to id (bigger alphabet)
        public static string GetIdStringLong(int chars)
        {
            string alphabet = "0123456789abcdefghijklmnopqrstuvxywzABCDEFGHIJKLMNOPQRSTUVXYWZ";
            string ret = "";
            for (int i = 0; i < chars; i++)
                ret += alphabet[UnityEngine.Random.Range(0, alphabet.Length)];
            return ret;
        }


        // Get a random male name and optionally single letter surname
        public static string GetRandomName(bool withSurname = false)
        {
            List<string> firstNameList = new List<string>() {"Gabe","Cliff","Tim","Ron","Jon","John","Mike","Seth","Alex","Steve","Chris","Will","Bill","James","Jim",
                                        "Ahmed","Omar","Peter","Pierre","George","Lewis","Lewie","Adam","William","Ali","Eddie","Ed","Dick","Robert","Bob","Rob",
                                        "Neil","Tyson","Carl","Chris","Christopher","Jensen","Gordon","Morgan","Richard","Wen","Wei","Luke","Lucas","Noah","Ivan","Yusuf",
                                        "Ezio","Connor","Milan","Nathan","Victor","Harry","Ben","Charles","Charlie","Jack","Leo","Leonardo","Dylan","Steven","Jeff",
                                        "Alex","Mark","Leon","Oliver","Danny","Liam","Joe","Tom","Thomas","Bruce","Clark","Tyler","Jared","Brad","Jason"};

            if (!withSurname)
                return firstNameList[UnityEngine.Random.Range(0, firstNameList.Count)];
            else
            {
                string alphabet = "ABCDEFGHIJKLMNOPQRSTUVXYWZ";
                return firstNameList[UnityEngine.Random.Range(0, firstNameList.Count)] + " " + alphabet[UnityEngine.Random.Range(0, alphabet.Length)] + ".";
            }
        }

        public static string GetRandomCityName()
        {
            List<string> cityNameList = new List<string>() {"Alabama","New York","Old York","Bangkok","Lisbon","Vee","Agen","Agon","Ardok","Arbok",
                            "Kobra","House","Noun","Hayar","Salma","Chancellor","Dascomb","Payn","Inglo","Lorr","Ringu",
                            "Brot","Mount Loom","Kip","Chicago","Madrid","London","Gam",
                            "Greenvile","Franklin","Clinton","Springfield","Salem","Fairview","Fairfax","Washington","Madison",
                            "Georgetown","Arlington","Marion","Oxford","Harvard","Valley","Ashland","Burlington","Manchester","Clayton",
                            "Milton","Auburn","Dayton","Lexington","Milford","Riverside","Cleveland","Dover","Hudson","Kingston","Mount Vernon",
                            "Newport","Oakland","Centerville","Winchester","Rotary","Bailey","Saint Mary","Three Waters","Veritas","Chaos","Center",
                            "Millbury","Stockland","Deerstead Hills","Plaintown","Fairchester","Milaire View","Bradton","Glenfield","Kirkmore",
                            "Fortdell","Sharonford","Inglewood","Englecamp","Harrisvania","Bosstead","Brookopolis","Metropolis","Colewood","Willowbury",
                            "Hearthdale","Weelworth","Donnelsfield","Greenline","Greenwich","Clarkswich","Bridgeworth","Normont",
                            "Lynchbrook","Ashbridge","Garfort","Wolfpain","Waterstead","Glenburgh","Fortcroft","Kingsbank","Adamstead","Mistead",
                            "Old Crossing","Crossing","New Agon","New Agen","Old Agon","New Valley","Old Valley","New Kingsbank","Old Kingsbank",
                            "New Dover","Old Dover","New Burlington","Shawshank","Old Shawshank","New Shawshank","New Bradton", "Old Bradton",
                            "New Metropolis","Old Clayton","New Clayton"
            };
            return cityNameList[UnityEngine.Random.Range(0, cityNameList.Count)];
        }
    }
}