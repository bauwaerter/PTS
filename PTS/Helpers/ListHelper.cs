using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Core;


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
        #endregion



    }
}