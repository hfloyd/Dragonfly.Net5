namespace Dragonfly.NetHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    //using System.Web;
    //using System.Web.Script;
    //using System.Web.Script.Serialization;


    public static class Extensions
    {
        private const string ThisClassName = "Dragonfly.NetHelpers.Extensions";

        //Custom Extension Methods

        #region ======= IEnumerable<T> 

        /// <summary>
        /// Find the index of an item in the IEnumerable collection similar to List&lt;T&gt;.FindIndex()
        /// </summary>
        /// <param name="finder">The Item to locate</param>
        /// <returns>Integer representing the Index position</returns>
        public static int FindIndex<T>(this IEnumerable<T> list, Predicate<T> finder)
        {
            return list.ToList().FindIndex(finder);
        }
        #endregion

        #region ======= Dictionary 
        public static NameValueCollection ToNameValueCollection(this Dictionary<string, string> Dict)
        {
            var nvc = new NameValueCollection();
            foreach (var item in Dict)
            {
                nvc.Add(item.Key, item.Value);
            }

            return nvc;
        }

        #endregion
    }
}