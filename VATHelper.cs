/*
   Copyright 2011 Dorin Huzum.

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the spevatic language governing permissions and
   limitations under the License.
*/
/** This software was based on work from http://www.braemoor.co.uk/software/vat.shtml */

using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace vat.validation
{
    public class VATHelper
    {
        public static bool CheckVat(string countryCode, string vatNumber, out string name, out string address)
        {
            var inValue = new CheckVatRequest { countryCode = countryCode, vatNumber = vatNumber };
            var retVal = ((CheckVatPortType)(new CheckVatPortTypeClient())).CheckVat(inValue);

            name = retVal.name;
            address = retVal.address;
            return retVal.valid;
        }

        public static bool ValidateVAT(String taxAuthId, String vat)
        {
            if (taxAuthId == null) taxAuthId = "RO";

            switch (taxAuthId)
            {
                case "AT":
                    return ValidateVAT_AT(vat);
                case "BE":
                    return ValidateVAT_BE(vat);
                case "BG":
                    return ValidateVAT_BG(vat);
                case "CY":
                    return ValidateVAT_CY(vat);
                case "CZ":
                    return ValidateVAT_CZ(vat);
                case "DE":
                    return ValidateVAT_DE(vat);
                case "DK":
                    return ValidateVAT_DK(vat);
                case "EE":
                    return ValidateVAT_EE(vat);
                case "EL":
                    return ValidateVAT_EL(vat);
                case "FI":
                    return ValidateVAT_FI(vat);
                case "FR":
                    return ValidateVAT_FR(vat);
                case "HU":
                    return ValidateVAT_HU(vat);
                case "IE":
                    return ValidateVAT_IE(vat);
                case "IT":
                    return ValidateVAT_IT(vat);
                case "LT":
                    return ValidateVAT_LT(vat);
                case "LU":
                    return ValidateVAT_LU(vat);
                case "LV":
                    return ValidateVAT_LV(vat);
                case "MT":
                    return ValidateVAT_MT(vat);
                case "NL":
                    return ValidateVAT_NL(vat);
                case "PL":
                    return ValidateVAT_PL(vat);
                case "PT":
                    return ValidateVAT_PT(vat);
                case "SE":
                    return ValidateVAT_SE(vat);
                case "SK":
                    return ValidateVAT_SK(vat);
                case "SI":
                    return ValidateVAT_SI(vat);
                case "RO":
                    return ValidateVAT_RO(vat);

                default:
                    return true;
            }
        }

        // Checks the check digits of a Romanian VAT number.
        private static bool ValidateVAT_RO(String vat)
        {
            if (String.IsNullOrEmpty(vat) || vat.Length > 10 || vat.Length < 2)
            {
                return false;
            }

            Int32 nr;
            Int32.TryParse(vat, out nr);
            var control = nr % 10;

            vat = vat.Substring(0, vat.Length - 1);
            while (vat.Length < 9) vat = "0" + vat;

            var i = 0;
            var sum = new[] { 7, 5, 3, 2, 1, 7, 5, 3, 2 }.Sum(m => GetInt(vat[i++]) * m);

            var rest = (sum * 10) % 11;
            if (rest == 10) rest = 0;

            return (rest == control);
        }

        // Checks the check digits of a Greek VAT number.
        private static bool ValidateVAT_EL(String vat)
        {
            //eight character numbers should be prefixed with an 0.
            if (vat.Length == 8) vat = "0" + vat;

            // Extract the next digit and multiply by the counter.
            var i = 0;
            var total = new[] { 256, 128, 64, 32, 16, 8, 4, 2 }.Sum(m => GetInt(vat[i++]) * m);

            // Establish check digit.
            total = total % 11;
            if (total > 9) { total = 0; }

            // Compare it with the last character of the VAT number. If it is the same, 
            // then it's a valid check digit.
            return total == Convert.ToInt32(vat[8]);
        }

        // Checks the check digits of an Austrian VAT number.
        private static bool ValidateVAT_AT(String vat)
        {
            var i = 1;
            var total = new[] { 1, 2, 1, 2, 1, 2, 1 }.Select(m => GetInt(vat[i++]) * m).Select(temp => temp > 9 ? (int)Math.Floor(temp / 10D) + temp % 10 : temp).Sum();

            // Establish check digit.
            total = 10 - (total + 4) % 10;
            if (total == 10) total = 0;

            // Compare it with the last character of the VAT number. If it is the same, 
            // then it's a valid check digit.
            return total == GetInt(vat[8]);
        }

        /// <summary>
        /// Checks the check digits of a Belgium VAT number.
        /// </summary>
        /// <param name="vat">The vat.</param>
        /// <returns></returns>
        private static bool ValidateVAT_BE(String vat)
        {
            // First character of 10 digit numbers should be 0
            if (vat.Length == 10 && vat[0] != '0') return false;

            // Nine digit numbers have a 0 inserted at the front.
            if (vat.Length == 9) vat = "0" + vat;

            // Modulus 97 check on last nine digits
            return (97 - int.Parse(vat.Substring(0, 8)) % 97) == int.Parse(vat.Substring(8, 2));
        }

        /// <summary>
        /// Check the check digit of 10 digit Bulgarian VAT numbers.
        /// </summary>
        /// <param name="vat">The vat.</param>
        /// <returns></returns>
        private static bool ValidateVAT_BG(String vat)
        {
            if (vat.Length != 9) return true;

            var i = 0;
            var total = new[] { 4, 3, 2, 7, 6, 5, 4, 3, 2 }.Sum(m => GetInt(vat[i++]) * m) % 11;

            if (total == 10)
            {
                i = 0;
                total = new[] { 3, 4, 5, 6, 7, 8, 9, 10 }.Sum(m => GetInt(vat[i++]) * m) % 11;
                if (total == 10) total = 0;
            }

            // Compare it with the last character of the VAT number. If it is the same, 
            // then it's a valid check digit.
            return total == GetInt(vat[9]);
        }

        /// <summary>
        /// Checks the check digits of a Cypriot VAT number.
        /// </summary>
        /// <param name="vat">The vat.</param>
        /// <returns></returns>
        private static bool ValidateVAT_CY(String vat)
        {
            if (int.Parse(vat.Substring(0, 2)) == 12) return false;

            var total = 0;
            for (var i = 0; i < 8; i++)
            {
                var temp = GetInt(vat[i]);
                if (i % 2 == 0)
                {
                    switch (temp)
                    {
                        case 0: temp = 1; break;
                        case 1: temp = 0; break;
                        case 2: temp = 5; break;
                        case 3: temp = 7; break;
                        case 4: temp = 9; break;
                        default:
                            temp = temp * 2 + 3;
                            break;
                    }
                }
                total = total + temp;
            }

            // Establish check digit using modulus 26, and translate to char. equivalent.
            total = total % 26;
            return vat[8] == (char)(total + 65);
        }

        /// <summary>
        /// Checks the check digits of a Czech Republic VAT number.
        /// </summary>
        /// <param name="vat">The vat.</param>
        /// <returns></returns>
        private static bool ValidateVAT_CZ(String vat)
        {
            // Only do check digit validation for standard VAT numbers
            if (vat.Length != 8) return true;

            var i = 0;
            var total = new[] { 8, 7, 6, 5, 4, 3, 2 }.Sum(m => GetInt(vat[i++]) * m);
            // Extract the next digit and multiply by the counter.

            // Establish check digit.
            total = 11 - total % 11;
            if (total == 10) total = 0;
            if (total == 11) total = 1;

            // Compare it with the last character of the VAT number. If it is the same, 
            // then it's a valid check digit.
            return (total == GetInt(vat[7]));
        }

        /// <summary>
        /// Checks the check digits of a German VAT number.
        /// </summary>
        /// <param name="vat">The vat.</param>
        /// <returns></returns>
        private static bool ValidateVAT_DE(String vat)
        {
            var product = 10;
            for (var i = 0; i < 8; i++)
            {
                // Extract the next digit and implement perculiar algorithm!.
                var sum = (GetInt(vat[i]) + product) % 10;
                if (sum == 0)
                {
                    sum = 10;
                }
                product = (2 * sum) % 11;
            }

            // Establish check digit.  
            var checkdigit = 11 - product == 10 ? 0 : 11 - product;

            // Compare it with the last two characters of the VAT number. If the same, 
            // then it is a valid check digit.
            return checkdigit == GetInt(vat[8]);
        }

        /// <summary>
        /// Checks the check digits of a Danish VAT number.
        /// </summary>
        /// <param name="vat">The vat.</param>
        /// <returns></returns>
        private static bool ValidateVAT_DK(String vat)
        {
            var i = 0;
            var total = new[] { 2, 7, 6, 5, 4, 3, 2, 1 }.Sum(m => GetInt(vat[i++]) * m);

            // The remainder should be 0 for it to be valid..
            return total % 11 == 0;
        }

        /// <summary>
        /// Checks the check digits of an Estonian VAT number.
        /// </summary>
        /// <param name="vat">The vat.</param>
        /// <returns></returns>
        private static bool ValidateVAT_EE(String vat)
        {
            // Checks the check digits of an Estonian VAT number.
            var i = 0;
            var total = new[] { 3, 7, 1, 3, 7, 1, 3, 7 }.Sum(m => GetInt(vat[i++]) * m);

            // Establish check digits using modulus 10.
            total = 10 - total % 10;
            if (total == 10) total = 0;

            // Compare it with the last character of the VAT number. If it is the same, 
            // then it's a valid check digit.
            return total == GetInt(vat[8]);
        }

        /// <summary>
        /// Checks the check digits of a Finnish VAT number.
        /// </summary>
        /// <param name="vat">The vat.</param>
        /// <returns></returns>
        private static bool ValidateVAT_FI(String vat)
        {
            var i = 0;
            var total = new[] { 7, 9, 10, 5, 8, 4, 2 }.Sum(m => GetInt(vat[i++]) * m);

            // Establish check digit.
            total = 11 - total % 11;
            if (total > 9) { total = 0; }

            // Compare it with the last character of the VAT number. If it is the same, 
            // then it's a valid check digit.
            return total == GetInt(vat[7]);
        }

        /// <summary>
        /// Checks the check digits of a French VAT number.
        /// </summary>
        /// <param name="vat">The vat.</param>
        /// <returns></returns>
        private static bool ValidateVAT_FR(String vat)
        {
            //TODO: new style.

            int temp;
            if (!int.TryParse(vat.Substring(0, 2), out temp)) return true;

            // Extract the last nine digits as an integer.
            var total = int.Parse(vat.Substring(2));

            // Establish check digit.
            total = (total * 100 + 12) % 97;

            // Compare it with the last character of the VAT number. If it is the same, 
            // then it's a valid check digit.
            return total == temp;
        }

        /// <summary>
        /// Checks the check digits of a Hungarian VAT number.
        /// </summary>
        /// <param name="vat">The vat.</param>
        /// <returns></returns>
        private static bool ValidateVAT_HU(String vat)
        {
            var i = 0;
            var total = new[] { 9, 7, 3, 1, 9, 7, 3 }.Sum(m => GetInt(vat[i++]) * m);

            // Establish check digit.
            total = 10 - total % 10;
            if (total == 10) total = 0;

            // Compare it with the last character of the VAT number. If it is the same, 
            // then it's a valid check digit.
            return total == GetInt(vat[7]);
        }

        /// <summary>
        /// Checks the check digits of an Irish VAT number.
        /// </summary>
        /// <param name="vat">The vat.</param>
        /// <returns></returns>
        private static bool ValidateVAT_IE(String vat)
        {
            if (Regex.IsMatch(vat, @"/^\d[A-Z\*\+]/"))
            {
                vat = "0" + vat.Substring(2, 7) + vat.Substring(0, 1) + vat.Substring(7, 8);
            }

            var i = 0;
            var total = new[] { 8, 7, 6, 5, 4, 3, 2 }.Sum(m => GetInt(vat[i++]) * m);

            // Establish check digit using modulus 23, and translate to char. equivalent.
            total = total % 23;
            return vat[7] == (total == 0 ? 'W' : (char)(total + 64));
        }

        /// <summary>
        /// Checks the check digits of an Italian VAT number.
        /// </summary>
        /// <param name="vat">The vat.</param>
        /// <returns></returns>
        private static bool ValidateVAT_IT(String vat)
        {
            // The last three digits are the issuing office, and cannot exceed more 201
            if (int.Parse(vat.Substring(0, 7)) == 0) return false;

            var temp = int.Parse(vat.Substring(7, 3));

            if ((temp < 1) || (temp > 201)) return false;

            // Extract the next digit and multiply by the appropriate  
            var i = 0;
            var total = 0;
            foreach (var m in new[] { 1, 2, 1, 2, 1, 2, 1, 2, 1, 2 })
            {
                temp = GetInt(vat[i++]) * m;
                total += temp > 9 ? (int)Math.Floor(temp / 10D) + temp % 10 : temp;
            }

            // Establish check digit.
            total = 10 - total % 10;
            if (total > 9) { total = 0; }

            // Compare it with the last character of the VAT number. If it is the same, 
            // then it's a valid check digit.
            return total == GetInt(vat[10]);
        }

        /// <summary>
        /// Checks the check digits of a Lithuanian VAT number.
        /// </summary>
        /// <param name="vat">The vat.</param>
        /// <returns></returns>
        private static bool ValidateVAT_LT(String vat)
        {
            // Only do check digit validation for standard VAT numbers
            if (vat.Length != 9) return true;

            // Extract the next digit and multiply by the counter+1.
            var total = 0;
            for (var i = 0; i < 8; i++) total += GetInt(vat[i]) * (i + 1);

            // Can have a double check digit calculation!
            if (total % 11 == 10)
            {
                var i = 0;
                total = new[] { 3, 4, 5, 6, 7, 8, 9, 1 }.Sum(m => GetInt(vat[i++]) * m);
            }

            // Establish check digit.
            total = total % 11;
            if (total == 10) { total = 0; }

            // Compare it with the last character of the VAT number. If it is the same, 
            // then it's a valid check digit.
            return total == GetInt(vat[8]);
        }

        /// <summary>
        /// Checks the check digits of a Luxembourg VAT number.
        /// </summary>
        /// <param name="vat">The vat.</param>
        /// <returns></returns>
        private static bool ValidateVAT_LU(String vat)
        {
            return int.Parse(vat.Substring(0, 6)) % 89 == int.Parse(vat.Substring(6, 2));
        }

        /// <summary>
        /// Checks the check digits of a Latvian VAT number.
        /// </summary>
        /// <param name="vat">The vat.</param>
        /// <returns></returns>
        private static bool ValidateVAT_LV(String vat)
        {
            // Only check the legal bodies
            if (Regex.IsMatch(vat, "/^[0-3]/")) return true;

            var i = 0;
            var total = new[] { 9, 1, 4, 8, 3, 10, 2, 5, 7, 6 }.Sum(m => GetInt(vat[i++]) * m);

            // Establish check digits by getting modulus 11.
            if (total % 11 == 4 && vat[0] == '9') total = total - 45;
            if (total % 11 == 4)
            {
                total = 4 - total % 11;
            }
            else if (total % 11 > 4)
            {
                total = 14 - total % 11;
            }
            else if (total % 11 < 4)
            {
                total = 3 - total % 11;
            }

            // Compare it with the last character of the VAT number. If it is the same, 
            // then it's a valid check digit.
            return total == GetInt(vat[10]);
        }

        /// <summary>
        /// Checks the check digits of a Maltese VAT number.
        /// </summary>
        /// <param name="vat">The vat.</param>
        /// <returns></returns>
        private static bool ValidateVAT_MT(String vat)
        {
            var i = 0;
            var total = new[] { 3, 4, 6, 7, 8, 9 }.Sum(m => GetInt(vat[i++]) * m);

            // Establish check digits by getting modulus 37.
            total = 37 - total % 37;

            // Compare it with the last character of the VAT number. If it is the same, 
            // then it's a valid check digit.
            return total == int.Parse(vat.Substring(6, 2));
        }

        /// <summary>
        /// Checks the check digits of a Dutch VAT number.
        /// </summary>
        /// <param name="vat">The vat.</param>
        /// <returns></returns>
        private static bool ValidateVAT_NL(String vat)
        {
            var i = 0;
            var total = new[] { 9, 8, 7, 6, 5, 4, 3, 2 }.Sum(m => GetInt(vat[i++]) * m);

            // Establish check digits by getting modulus 11.
            total = total % 11;
            if (total > 9) { total = 0; }

            // Compare it with the last character of the VAT number. If it is the same, 
            // then it's a valid check digit.
            return total == GetInt(vat[8]);
        }

        /// <summary>
        /// Checks the check digits of a Polish VAT number.
        /// </summary>
        /// <param name="vat">The vat.</param>
        /// <returns></returns>
        private static bool ValidateVAT_PL(String vat)
        {
            var i = 0;
            var total = new[] { 6, 5, 7, 2, 3, 4, 5, 6, 7 }.Sum(m => GetInt(vat[i++]) * m);

            // Establish check digits subtracting modulus 11 from 11.
            total = total % 11;
            if (total > 9) { total = 0; }

            // Compare it with the last character of the VAT number. If it is the same, then it's a valid 
            // check digit.
            return total == GetInt(vat[9]);
        }

        /// <summary>
        /// Checks the check digits of a Portugese VAT number.
        /// </summary>
        /// <param name="vat">The vat.</param>
        /// <returns></returns>
        private static bool ValidateVAT_PT(String vat)
        {
            var i = 0;
            var total = new[] { 9, 8, 7, 6, 5, 4, 3, 2 }.Sum(m => GetInt(vat[i++]) * m);

            // Establish check digits subtracting modulus 11 from 11.
            total = 11 - total % 11;
            if (total > 9) { total = 0; }

            // Compare it with the last character of the VAT number. If it is the same, then it's a valid 
            // check digit.
            return total == GetInt(vat[8]);
        }

        /// <summary>
        /// Checks the check digits of a Swedish VAT number.
        /// </summary>
        /// <param name="vat">The vat.</param>
        /// <returns></returns>
        private static bool ValidateVAT_SE(String vat)
        {
            var i = 0;
            var total = new[] { 2, 1, 2, 1, 2, 1, 2, 1, 2 }.Select(m => GetInt(vat[i++]) * m).Select(temp => temp > 9 ? (int)Math.Floor(temp / 10D) + temp % 10 : temp).Sum();

            // Establish check digits by subtracting mod 10 of total from 10.
            total = 10 - (total % 10);
            if (total == 10) total = 0;

            // Compare it with the 10th character of the VAT number. If it is the same, 
            // then it's a valid check digit.
            return total == GetInt(vat[9]);
        }

        /// <summary>
        /// Checks the check digits of a Slovak VAT number.
        /// </summary>
        /// <param name="vat">The vat.</param>
        /// <returns></returns>
        private static bool ValidateVAT_SK(String vat)
        {
            var i = 3;
            var total = new[] { 8, 7, 6, 5, 4, 3, 2 }.Sum(m => GetInt(vat[i++]) * m);

            // Establish check digits by getting modulus 11.
            total = 11 - total % 11;
            if (total > 9) total = total - 10;

            // Compare it with the last character of the VAT number. If it is the same, 
            // then it's a valid check digit.
            return total == GetInt(vat[9]);
        }

        /// <summary>
        /// Checks the check digits of a Slovenian VAT number.
        /// </summary>
        /// <param name="vat">The vat.</param>
        /// <returns></returns>
        private static bool ValidateVAT_SI(String vat)
        {
            var i = 0;
            var total = new[] { 8, 7, 6, 5, 4, 3, 2 }.Sum(m => GetInt(vat[i++]) * m);

            // Establish check digits by subtracting 97 from total until negative.
            total = 11 - total % 11;
            if (total > 9) { total = 0; }

            // Compare the number with the last character of the VAT number. If it is the 
            // same, then it's a valid check digit.
            return total == GetInt(vat[7]);
        }

        /// <summary>
        /// Checks the check digits of a UK VAT number.
        /// </summary>
        /// <param name="vat">The vat.</param>
        /// <returns></returns>
        private static bool ValidateVAT_UK(String vat)
        {
            return true;
        }

        private static int GetInt(char c)
        {
            return (Convert.ToInt32(c) - Convert.ToInt32('0'));
        }
    }
}
