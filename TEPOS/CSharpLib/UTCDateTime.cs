using System;

namespace ERP.CSharpLib
{
    /// <summary>
    /// this is define seales class
    /// this class can't be inheritance by another classes
    /// this class inherite another classes 
    /// </summary>
    public sealed class UTCDateTime
    {

        /// <summary>
        /// this methode private it's use only this class
        /// call by another methode in this class and get bangladesh date & time
        /// </summary>
        /// <returns>bangladesh date time</returns>
        private static DateTime BangladeshDateTime()
        {
            DateTime localTime;
            try
            {
                localTime = DateTime.Now;
                //  localTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, TimeZoneInfo.Local.Id, "Central Asia Standard Time");
            }
            catch (Exception)
            {
                localTime = DateTime.Now;
                //  localTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, TimeZoneInfo.Local.Id, "Bangladesh Standard Time");
            }

            return localTime;
        }

        /// <summary>
        /// this method return bangladesh date & time from UTC date time 
        /// </summary>
        /// <returns>BD date time in datetime formate</returns>
        public static DateTime BDDateTime()
        {
            return BangladeshDateTime();
        }


        /// <summary>
        /// this methode return only Bangladesh time
        /// </summary>
        /// <returns>BD time only</returns>
        public static TimeSpan BDTime()
        {
            return BangladeshDateTime().TimeOfDay;
        }

        /// <summary>
        /// this methode return only Bangladesh date
        /// </summary>
        /// <returns>BD date only</returns>
        public static DateTime BDDate()
        {
            return BangladeshDateTime().Date;
        }

        //static void Main(string[] args)
        //{
        //    //Console.WriteLine(BDDate());
        //    //Console.ReadKey();
        //}
    }


}
