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
    }
}