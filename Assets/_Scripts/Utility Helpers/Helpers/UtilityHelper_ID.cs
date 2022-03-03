using System.Collections.Generic;

namespace Utilities
{
    public static partial class UtilityHelper
    {
        /// <summary>
        /// Returns a random script that can be used to id
        /// </summary>
        /// <param name="complexity">How many characters the returned ID string will contain</param>
        public static string GetIdString(int complexity = 10)
        {
            string alphabet = "0123456789abcdefghijklmnopqrstuvxywzABCDEFGHIJKLMNOPQRSTUVXYWZ";
            string ret = string.Empty;
            for (int i = 0; i < complexity; i++)
                ret += alphabet[UnityEngine.Random.Range(0, alphabet.Length)];
            return ret;
        }
        
        /// <summary>
        /// Get a random male name and optionally single letter surname
        /// </summary>
        public static string GetRandomMaleName(bool withSurname = false)
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
                return $"{firstNameList[UnityEngine.Random.Range(0, firstNameList.Count)]} {alphabet[UnityEngine.Random.Range(0, alphabet.Length)]}.";
            }
        }

        /// <summary>
        /// Get a random city/area name
        /// </summary>
        public static string GetRandomCityName()
        {
            List<string> cityNameList = new List<string>() {"Alabama","New York","Old York","Bangkok","Lisbon","Vee","Agen","Agon","Ardok","Arbok",
                            "Kobra","Houser","Noun","Hayar","Salma","Chancellor","Dascomb","Payn","Inglo","Lorr","Ringu","Brot","Mount Loom","Kip",
                            "Chicago","Madrid","London","Gam","Greenvile","Franklin","Clinton","Springfield","Salem","Fairview","Fairfax","Washington","Madison",
                            "Georgetown","Arlington","Marion","Oxford","Harvard","Vallencia","Ashland","Burlington","Manchester","Clayton",
                            "Milton","Auburn","Dayton","Lexington","Milford","Riverside","Cleveland","Dover","Hudson","Kingston","Mount Vernon",
                            "Newport","Oakland","Centerville","Winchester","Rotary","Bailey","Saint Mary","Three Waters","Veritas","Chaos","Center",
                            "Millbury","Stockland","Deerstead Hills","Plaintown","Fairchester","Milaire View","Bradton","Glenfield","Kirkmore",
                            "Fortdell","Sharonford","Inglewood","Englecamp","Harrisvania","Bosstead","Brookopolis","Metropolis","Colewood","Willowbury",
                            "Hearthdale","Weelworth","Donnelsfield","Greenline","Greenwich","Clarkswich","Bridgeworth","Normont",
                            "Lynchbrook","Ashbridge","Garfort","Wolfpain","Waterstead","Glenburgh","Fortcroft","Kingsbank","Adamstead","Mistead",
                            "Old Crossing","Crosten","New Agon","New Agen","Old Agon","New Valley","Old Valley","New Kingsbank","Old Kingsbank",
                            "New Dover","Old Dover","New Burlington","Shawshank","Old Shawshank","New Shawshank","New Bradton", "Old Bradton",
                            "New Metropolis","Old Clayton","New Clayton"
            };
            return cityNameList[UnityEngine.Random.Range(0, cityNameList.Count)];
        }
    }
}