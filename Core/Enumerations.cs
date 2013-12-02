using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Core.Helpers;

namespace Core
{
    #region Users
	
    public enum UserRoles
    {
        Admin = 3,
	    Tutor = 2,
        Student = 1,
        None = 0
    }

    public enum CardType {
        [Display(Name = "Visa")]
        Visa,

        [Display(Name = "Master Card")]
        MasterCard,

        [Display(Name = "American Express")]
        AmericanExpress,

        [Display(Name = "Discover")]
        Discover
    }

    public enum StateCode {
        AL, AK, AZ, AR, CA, CO, CT, DE, DC, FL, GA,
        HI, ID, IL, IN, IA, KS, KY, LA, ME, MT, NE, NV,
        NH, NJ, NM, NY, NC, ND, OH, OK, OR, MD, MA,
        MI, MN, MS, MO, PA, RI, SC, SD, TN, TX, UT, VT,
        VA, WA, WV, WI, WY
    }

    #region countries
    public enum Country {
        [Display(Name = "United States")]
        UnitedStates,
        [Display(Name = "Afghanistan")]
        Afghanistan,
        [Display(Name = "Albania")]
        Albania,
        [Display(Name = "Algeria")]
        Algeria,
        [Display(Name = "Andorra")]
        Andorra,
        [Display(Name = "Angola")]
        Angola,
        [Display(Name = "Antigua & Deps")]
        AntiguaDeps,
        [Display(Name = "Argentina")]
        Argentina,
        [Display(Name = "Armenia")]
        Armenia,
        [Display(Name = "Australia")]
        Australia,
        [Display(Name = "Austria")]
        Austria,
        [Display(Name = "Azerbaijan")]
        Azerbaijan,
        [Display(Name = "Bahamas")]
        Bahamas,
        [Display(Name = "Bahrain")]
        Bahrain,
        [Display(Name = "Bangladesh")]
        Bangladesh,
        [Display(Name = "Barbados")]
        Barbados,
        [Display(Name = "Belarus")]
        Belarus,
        [Display(Name = "Belgium")]
        Belgium,
        [Display(Name = "Belize")]
        Belize,
        [Display(Name = "Benin")]
        Benin,
        [Display(Name = "Bhutan")]
        Bhutan,
        [Display(Name = "Bolivia")]
        Bolivia,
        [Display(Name = "Bosnia Herzegovina")]
        BosniaHerzegovina,
        [Display(Name = "Botswana")]
        Botswana,
        [Display(Name = "Brazil")]
        Brazil,
        [Display(Name = "Brunei")]
        Brunei,
        [Display(Name = "Bulgaria")]
        Bulgaria,
        [Display(Name = "Burkina")]
        Burkina,
        [Display(Name = "Burundi")]
        Burundi,
        [Display(Name = "Cambodia")]
        Cambodia,
        [Display(Name = "Cameroon")]
        Cameroon,
        [Display(Name = "Canada")]
        Canada,
        [Display(Name = "Cape Verde")]
        CapeVerde,
        [Display(Name = "Central African Rep")]
        CentralAfricanRep,
        [Display(Name = "Chad")]
        Chad,
        [Display(Name = "Chile")]
        Chile,
        [Display(Name = "China")]
        China,
        [Display(Name = "Colombia")]
        Colombia,
        [Display(Name = "Comoros")]
        Comoros,
        [Display(Name = "Congo")]
        Congo,
        [Display(Name = "Congo {Democratic Rep}")]
        CongoDemocraticRep,
        [Display(Name = "Costa Rica")]
        CostaRica,
        [Display(Name = "Croatia")]
        Croatia,
        [Display(Name = "Cuba")]
        Cuba,
        [Display(Name = "Cyprus")]
        Cyprus,
        [Display(Name = "Czech Republic")]
        CzechRepublic,
        [Display(Name = "Denmark")]
        Denmark,
        [Display(Name = "Djibouti")]
        Djibouti,
        [Display(Name = "Dominica")]
        Dominica,
        [Display(Name = "Dominican Republic")]
        DominicanRepublic,
        [Display(Name = "East Timor")]
        EastTimor,
        [Display(Name = "Ecuador")]
        Ecuador,
        [Display(Name = "Egypt")]
        Egypt,
        [Display(Name = "El Salvador")]
        ElSalvador,
        [Display(Name = "Equatorial Guinea")]
        EquatorialGuinea,
        [Display(Name = "Eritrea")]
        Eritrea,
        [Display(Name = "Estonia")]
        Estonia,
        [Display(Name = "Ethiopia")]
        Ethiopia,
        [Display(Name = "Fiji")]
        Fiji,
        [Display(Name = "Finland")]
        Finland,
        [Display(Name = "France")]
        France,
        [Display(Name = "Gabon")]
        Gabon,
        [Display(Name = "Gambia")]
        Gambia,
        [Display(Name = "Georgia")]
        Georgia,
        [Display(Name = "Germany")]
        Germany,
        [Display(Name = "Ghana")]
        Ghana,
        [Display(Name = "Greece")]
        Greece,
        [Display(Name = "Grenada")]
        Grenada,
        [Display(Name = "Guatemala")]
        Guatemala,
        [Display(Name = "Guinea")]
        Guinea,
        [Display(Name = "Guinea-Bissau")]
        GuineaBissau,
        [Display(Name = "Guyana")]
        Guyana,
        [Display(Name = "Haiti")]
        Haiti,
        [Display(Name = "Honduras")]
        Honduras,
        [Display(Name = "Hungary")]
        Hungary,
        [Display(Name = "Iceland")]
        Iceland,
        [Display(Name = "India")]
        India,
        [Display(Name = "Indonesia")]
        Indonesia,
        [Display(Name = "Iran")]
        Iran,
        [Display(Name = "Iraq")]
        Iraq,
        [Display(Name = "Ireland {Republic}")]
        IrelandRepublic,
        [Display(Name = "Israel")]
        Israel,
        [Display(Name = "Italy")]
        Italy,
        [Display(Name = "Ivory Coast")]
        IvoryCoast,
        [Display(Name = "Jamaica")]
        Jamaica,
        [Display(Name = "Japan")]
        Japan,
        [Display(Name = "Jordan")]
        Jordan,
        [Display(Name = "Kazakhstan")]
        Kazakhstan,
        [Display(Name = "Kenya")]
        Kenya,
        [Display(Name = "Kiribati")]
        Kiribati,
        [Display(Name = "Korea North")]
        KoreaNorth,
        [Display(Name = "Korea South")]
        KoreaSouth,
        [Display(Name = "Kosovo")]
        Kosovo,
        [Display(Name = "Kuwait")]
        Kuwait,
        [Display(Name = "Kyrgyzstan")]
        Kyrgyzstan,
        [Display(Name = "Laos")]
        Laos,
        [Display(Name = "Latvia")]
        Latvia,
        [Display(Name = "Lebanon")]
        Lebanon,
        [Display(Name = "Lesotho")]
        Lesotho,
        [Display(Name = "Liberia")]
        Liberia,
        [Display(Name = "Libya")]
        Libya,
        [Display(Name = "Liechtenstein")]
        Liechtenstein,
        [Display(Name = "Lithuania")]
        Lithuania,
        [Display(Name = "Luxembourg")]
        Luxembourg,
        [Display(Name = "Macedonia")]
        Macedonia,
        [Display(Name = "Madagascar")]
        Madagascar,
        [Display(Name = "Malawi")]
        Malawi,
        [Display(Name = "Malaysia")]
        Malaysia,
        [Display(Name = "Maldives")]
        Maldives,
        [Display(Name = "Mali")]
        Mali,
        [Display(Name = "Malta")]
        Malta,
        [Display(Name = "Marshall Islands")]
        MarshallIslands,
        [Display(Name = "Mauritania")]
        Mauritania,
        [Display(Name = "Mauritius")]
        Mauritius,
        [Display(Name = "Mexico")]
        Mexico,
        [Display(Name = "Micronesia")]
        Micronesia,
        [Display(Name = "Moldova")]
        Moldova,
        [Display(Name = "Monaco")]
        Monaco,
        [Display(Name = "Mongolia")]
        Mongolia,
        [Display(Name = "Montenegro")]
        Montenegro,
        [Display(Name = "Morocco")]
        Morocco,
        [Display(Name = "Mozambique")]
        Mozambique,
        [Display(Name = "Myanmar, {Burma}")]
        MyanmarBurma,
        [Display(Name = "Namibia")]
        Namibia,
        [Display(Name = "Nauru")]
        Nauru,
        [Display(Name = "Nepal")]
        Nepal,
        [Display(Name = "Netherlands")]
        Netherlands,
        [Display(Name = "New Zealand")]
        NewZealand,
        [Display(Name = "Nicaragua")]
        Nicaragua,
        [Display(Name = "Niger")]
        Niger,
        [Display(Name = "Nigeria")]
        Nigeria,
        [Display(Name = "Norway")]
        Norway,
        [Display(Name = "Oman")]
        Oman,
        [Display(Name = "Pakistan")]
        Pakistan,
        [Display(Name = "Palau")]
        Palau,
        [Display(Name = "Panama")]
        Panama,
        [Display(Name = "Papua New Guinea")]
        PapuaNewGuinea,
        [Display(Name = "Paraguay")]
        Paraguay,
        [Display(Name = "Peru")]
        Peru,
        [Display(Name = "Philippines")]
        Philippines,
        [Display(Name = "Poland")]
        Poland,
        [Display(Name = "Portugal")]
        Portugal,
        [Display(Name = "Qatar")]
        Qatar,
        [Display(Name = "Romania")]
        Romania,
        [Display(Name = "Russian Federation")]
        RussianFederation,
        [Display(Name = "Rwanda")]
        Rwanda,
        [Display(Name = "St Kitts & Nevis")]
        StKittsNevis,
        [Display(Name = "St Lucia")]
        StLucia,
        [Display(Name = "Saint Vincent & the Grenadines")]
        SaintVincentGrenadines,
        [Display(Name = "Samoa")]
        Samoa,
        [Display(Name = "San Marino")]
        SanMarino,
        [Display(Name = "Sao Tome & Principe")]
        SaoTomePrincipe,
        [Display(Name = "Saudi Arabia")]
        SaudiArabia,
        [Display(Name = "Senegal")]
        Senegal,
        [Display(Name = "Serbia")]
        Serbia,
        [Display(Name = "Seychelles")]
        Seychelles,
        [Display(Name = "Sierra Leone")]
        SierraLeone,
        [Display(Name = "Singapore")]
        Singapore,
        [Display(Name = "Slovakia")]
        Slovakia,
        [Display(Name = "Slovenia")]
        Slovenia,
        [Display(Name = "Solomon Islands")]
        SolomonIslands,
        [Display(Name = "Somalia")]
        Somalia,
        [Display(Name = "South Africa")]
        SouthAfrica,
        [Display(Name = "South Sudan")]
        SouthSudan,
        [Display(Name = "Spain")]
        Spain,
        [Display(Name = "Sri Lanka")]
        SriLanka,
        [Display(Name = "Sudan")]
        Sudan,
        [Display(Name = "Suriname")]
        Suriname,
        [Display(Name = "Swaziland")]
        Swaziland,
        [Display(Name = "Sweden")]
        Sweden,
        [Display(Name = "Switzerland")]
        Switzerland,
        [Display(Name = "Syria")]
        Syria,
        [Display(Name = "Taiwan")]
        Taiwan,
        [Display(Name = "Tajikistan")]
        Tajikistan,
        [Display(Name = "Tanzania")]
        Tanzania,
        [Display(Name = "Thailand")]
        Thailand,
        [Display(Name = "Togo")]
        Togo,
        [Display(Name = "Tonga")]
        Tonga,
        [Display(Name = "Trinidad & Tobago")]
        TrinidadTobago,
        [Display(Name = "Tunisia")]
        Tunisia,
        [Display(Name = "Turkey")]
        Turkey,
        [Display(Name = "Turkmenistan")]
        Turkmenistan,
        [Display(Name = "Tuvalu")]
        Tuvalu,
        [Display(Name = "Uganda")]
        Uganda,
        [Display(Name = "Ukraine")]
        Ukraine,
        [Display(Name = "United Arab Emirates")]
        UnitedArabEmirates,
        [Display(Name = "United Kingdom")]
        UnitedKingdom,
        [Display(Name = "Uruguay")]
        Uruguay,
        [Display(Name = "Uzbekistan")]
        Uzbekistan,
        [Display(Name = "Vanuatu")]
        Vanuatu,
        [Display(Name = "Vatican City")]
        VaticanCity,
        [Display(Name = "Venezuela")]
        Venezuela,
        [Display(Name = "Vietnam")]
        Vietnam,
        [Display(Name = "Yemen")]
        Yemen,
        [Display(Name = "Zambia")]
        Zambia,
        [Display(Name = "Zimbabwe")]
        Zimbabwe,
    }
    #endregion

    #endregion
}
