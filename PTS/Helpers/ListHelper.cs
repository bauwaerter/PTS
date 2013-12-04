using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Core;
using Core.Domains;
using PTS.Infrastructure;
using Service;


namespace PTS.Helpers {
    public static class ListHelper
    {
        #region Enum Helpers
        /// <summary>
        /// Gets the list of Roles based off current user
        /// </summary>
        /// <returns>List of Roles</returns>
        private static string DisplayName(this Enum value) {
            var enumType = value.GetType();
            var enumValue = Enum.GetName(enumType, value);
            var member = enumType.GetMember(enumValue)[0];

            var attrs = member.GetCustomAttributes(typeof(DisplayAttribute), false);
            var outString = ((DisplayAttribute)attrs[0]).Name;

            if (((DisplayAttribute)attrs[0]).ResourceType != null) {
                outString = ((DisplayAttribute)attrs[0]).GetName();
            }

            return outString;
        }

        public static Dictionary<CardType, string> GetAllCardType() {
            return new Dictionary<CardType, string>
                {
                    {CardType.AmericanExpress, CardType.AmericanExpress.DisplayName()},
                    {CardType.Discover, CardType.Discover.DisplayName()},
                    {CardType.MasterCard, CardType.MasterCard.DisplayName()},
                    {CardType.Visa, CardType.Visa.DisplayName()},
                };
        }

        public static Dictionary<Country, string> GetListCountries() {
            return Enum.GetValues(typeof(Country)).Cast<Country>().ToDictionary(val => val, val => val.DisplayName());
        }
        #endregion

        public static List<Subject> GetListOfSubjects(){
            var baseService = new BaseService<Subject>();
            return baseService.GetTableQuery().OrderBy(x => x.Name).ToList();
        }

         public static bool CheckIndexOf(string fieldString, string textSearch)
	     {	
            if (fieldString != null)	
            {
                return (fieldString.IndexOf(textSearch, StringComparison.OrdinalIgnoreCase) >= 0);
            }
	        return false;
        }
	
        public static bool CheckIndexOf(bool fieldBool, string textSearch)
        {
            return CheckIndexOf(fieldBool ? "Yes" : "No", textSearch);
        }

        public static bool CalculateDistance(double lat1, double lon1, double lat2, double lon2, int miles)
        {
            var R = 6371; // Radius of the earth in km
            var dLat = deg2rad(lat2 - lat1);  // deg2rad below
            var dLon = deg2rad(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = R * c; // Distance in km
            var distance = d * .621371;

            if (distance <= miles)
                return true;
            else
                return false;
        
        }
                
        public static double deg2rad(double deg) {
            return deg * (Math.PI / 180);
        }
    }
}

