/*
   Copyright 2011 Dorin Huzum.

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
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
        public static bool ValidateCif(String taxAuthId, String cif)
        {
            if (taxAuthId == null) taxAuthId = "RO";

            switch (taxAuthId)
            {
                case "AT":
                    return ValidateCif_AT(cif);
                case "BE":
                    return ValidateCif_BE(cif);
                case "BG":
                    return ValidateCif_BG(cif);
                case "CY":
                    return ValidateCif_CY(cif);
                case "CZ":
                    return ValidateCif_CZ(cif);
                case "DE":
                    return ValidateCif_DE(cif);
                case "DK":
                    return ValidateCif_DK(cif);
                case "EE":
                    return ValidateCif_EE(cif);
                case "EL":
                    return ValidateCif_EL(cif);
                case "FI":
                    return ValidateCif_FI(cif);
                case "FR":
                    return ValidateCif_FR(cif);
                case "HU":
                    return ValidateCif_HU(cif);
                case "IE":
                    return ValidateCif_IE(cif);
                case "IT":
                    return ValidateCif_IT(cif);
                case "LT":
                    return ValidateCif_LT(cif);
                case "LU":
                    return ValidateCif_LU(cif);
                case "LV":
                    return ValidateCif_LV(cif);
                case "MT":
                    return ValidateCif_MT(cif);
                case "NL":
                    return ValidateCif_NL(cif);
                case "PL":
                    return ValidateCif_PL(cif);
                case "PT":
                    return ValidateCif_PT(cif);
                case "SE":
                    return ValidateCif_SE(cif);
                case "SK":
                    return ValidateCif_SK(cif);
                case "SI":
                    return ValidateCif_SI(cif);
                case "RO":
                    return ValidateCif_RO(cif);

                default:
                    return true;
            }
        }

        // Checks the check digits of a Romanian VAT number.
        private static bool ValidateCif_RO(String cif)
        {
            if (String.IsNullOrEmpty(cif) || cif.Length > 10 || cif.Length < 2)
            {
                return false;
            }

            Int32 nr;
            Int32.TryParse(cif, out nr);
            var control = nr % 10;

            cif = cif.Substring(0, cif.Length - 1);
            while (cif.Length < 9) cif = "0" + cif;

            var i = 0;
            var sum = new[] { 7, 5, 3, 2, 1, 7, 5, 3, 2 }.Sum(m => GetInt(cif[i++]) * m);

            var rest = (sum * 10) % 11;
            if (rest == 10) rest = 0;

            return (rest == control);
        }

        // Checks the check digits of a Greek VAT number.
        private static bool ValidateCif_EL(String cif)
        {
            //eight character numbers should be prefixed with an 0.
            if (cif.Length == 8) cif = "0" + cif;

            // Extract the next digit and multiply by the counter.
            var i = 0;
            var total = new[] { 256, 128, 64, 32, 16, 8, 4, 2 }.Sum(m => GetInt(cif[i++]) * m);

            // Establish check digit.
            total = total % 11;
            if (total > 9) { total = 0; }

            // Compare it with the last character of the VAT number. If it is the same, 
            // then it's a valid check digit.
            return total == Convert.ToInt32(cif[8]);
        }

        // Checks the check digits of an Austrian VAT number.
        private static bool ValidateCif_AT(String cif)
        {
            var i = 1;
            var total = new[] { 1, 2, 1, 2, 1, 2, 1 }.Select(m => GetInt(cif[i++]) * m).Select(temp => temp > 9 ? (int)Math.Floor(temp / 10D) + temp % 10 : temp).Sum();

            // Establish check digit.
            total = 10 - (total + 4) % 10;
            if (total == 10) total = 0;

            // Compare it with the last character of the VAT number. If it is the same, 
            // then it's a valid check digit.
            return total == GetInt(cif[8]);
        }

        /// <summary>
        /// Checks the check digits of a Belgium VAT number.
        /// </summary>
        /// <param name="cif">The cif.</param>
        /// <returns></returns>
        private static bool ValidateCif_BE(String cif)
        {
            // First character of 10 digit numbers should be 0
            if (cif.Length == 10 && cif[0] != '0') return false;

            // Nine digit numbers have a 0 inserted at the front.
            if (cif.Length == 9) cif = "0" + cif;

            // Modulus 97 check on last nine digits
            return (97 - int.Parse(cif.Substring(0, 8)) % 97) == int.Parse(cif.Substring(8, 2));
        }

        /// <summary>
        /// Check the check digit of 10 digit Bulgarian VAT numbers.
        /// </summary>
        /// <param name="cif">The cif.</param>
        /// <returns></returns>
        private static bool ValidateCif_BG(String cif)
        {
            if (cif.Length != 9) return true;

            var i = 0;
            var total = new[] { 4, 3, 2, 7, 6, 5, 4, 3, 2 }.Sum(m => GetInt(cif[i++]) * m) % 11;

            if (total == 10)
            {
                i = 0;
                total = new[] { 3, 4, 5, 6, 7, 8, 9, 10 }.Sum(m => GetInt(cif[i++]) * m) % 11;
                if (total == 10) total = 0;
            }

            // Compare it with the last character of the VAT number. If it is the same, 
            // then it's a valid check digit.
            return total == GetInt(cif[9]);
        }

        /// <summary>
        /// Checks the check digits of a Cypriot VAT number.
        /// </summary>
        /// <param name="cif">The cif.</param>
        /// <returns></returns>
        private static bool ValidateCif_CY(String cif)
        {
            if (int.Parse(cif.Substring(0, 2)) == 12) return false;

            var total = 0;
            for (var i = 0; i < 8; i++)
            {
                var temp = GetInt(cif[i]);
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
            return cif[8] == (char)(total + 65);
        }

        /// <summary>
        /// Checks the check digits of a Czech Republic VAT number.
        /// </summary>
        /// <param name="cif">The cif.</param>
        /// <returns></returns>
        private static bool ValidateCif_CZ(String cif)
        {
            // Only do check digit validation for standard VAT numbers
            if (cif.Length != 8) return true;

            var i = 0;
            var total = new[] { 8, 7, 6, 5, 4, 3, 2 }.Sum(m => GetInt(cif[i++]) * m);
            // Extract the next digit and multiply by the counter.

            // Establish check digit.
            total = 11 - total % 11;
            if (total == 10) total = 0;
            if (total == 11) total = 1;

            // Compare it with the last character of the VAT number. If it is the same, 
            // then it's a valid check digit.
            return (total == GetInt(cif[7]));
        }

        /// <summary>
        /// Checks the check digits of a German VAT number.
        /// </summary>
        /// <param name="cif">The cif.</param>
        /// <returns></returns>
        private static bool ValidateCif_DE(String cif)
        {
            var product = 10;
            for (var i = 0; i < 8; i++)
            {
                // Extract the next digit and implement perculiar algorithm!.
                var sum = (GetInt(cif[i]) + product) % 10;
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
            return checkdigit == GetInt(cif[8]);
        }

        /// <summary>
        /// Checks the check digits of a Danish VAT number.
        /// </summary>
        /// <param name="cif">The cif.</param>
        /// <returns></returns>
        private static bool ValidateCif_DK(String cif)
        {
            var i = 0;
            var total = new[] { 2, 7, 6, 5, 4, 3, 2, 1 }.Sum(m => GetInt(cif[i++]) * m);

            // The remainder should be 0 for it to be valid..
            return total % 11 == 0;
        }

        /// <summary>
        /// Checks the check digits of an Estonian VAT number.
        /// </summary>
        /// <param name="cif">The cif.</param>
        /// <returns></returns>
        private static bool ValidateCif_EE(String cif)
        {
            // Checks the check digits of an Estonian VAT number.
            var i = 0;
            var total = new[] { 3, 7, 1, 3, 7, 1, 3, 7 }.Sum(m => GetInt(cif[i++]) * m);

            // Establish check digits using modulus 10.
            total = 10 - total % 10;
            if (total == 10) total = 0;

            // Compare it with the last character of the VAT number. If it is the same, 
            // then it's a valid check digit.
            return total == GetInt(cif[8]);
        }

        /// <summary>
        /// Checks the check digits of a Finnish VAT number.
        /// </summary>
        /// <param name="cif">The cif.</param>
        /// <returns></returns>
        private static bool ValidateCif_FI(String cif)
        {
            var i = 0;
            var total = new[] { 7, 9, 10, 5, 8, 4, 2 }.Sum(m => GetInt(cif[i++]) * m);

            // Establish check digit.
            total = 11 - total % 11;
            if (total > 9) { total = 0; }

            // Compare it with the last character of the VAT number. If it is the same, 
            // then it's a valid check digit.
            return total == GetInt(cif[7]);
        }

        /// <summary>
        /// Checks the check digits of a French VAT number.
        /// </summary>
        /// <param name="cif">The cif.</param>
        /// <returns></returns>
        private static bool ValidateCif_FR(String cif)
        {
            //TODO: new style.

            int temp;
            if (!int.TryParse(cif.Substring(0, 2), out temp)) return true;

            // Extract the last nine digits as an integer.
            var total = int.Parse(cif.Substring(2));

            // Establish check digit.
            total = (total * 100 + 12) % 97;

            // Compare it with the last character of the VAT number. If it is the same, 
            // then it's a valid check digit.
            return total == temp;
        }

        /// <summary>
        /// Checks the check digits of a Hungarian VAT number.
        /// </summary>
        /// <param name="cif">The cif.</param>
        /// <returns></returns>
        private static bool ValidateCif_HU(String cif)
        {
            var i = 0;
            var total = new[] { 9, 7, 3, 1, 9, 7, 3 }.Sum(m => GetInt(cif[i++]) * m);

            // Establish check digit.
            total = 10 - total % 10;
            if (total == 10) total = 0;

            // Compare it with the last character of the VAT number. If it is the same, 
            // then it's a valid check digit.
            return total == GetInt(cif[7]);
        }

        /// <summary>
        /// Checks the check digits of an Irish VAT number.
        /// </summary>
        /// <param name="cif">The cif.</param>
        /// <returns></returns>
        private static bool ValidateCif_IE(String cif)
        {
            if (Regex.IsMatch(cif, @"/^\d[A-Z\*\+]/"))
            {
                cif = "0" + cif.Substring(2, 7) + cif.Substring(0, 1) + cif.Substring(7, 8);
            }

            var i = 0;
            var total = new[] { 8, 7, 6, 5, 4, 3, 2 }.Sum(m => GetInt(cif[i++]) * m);

            // Establish check digit using modulus 23, and translate to char. equivalent.
            total = total % 23;
            return cif[7] == (total == 0 ? 'W' : (char)(total + 64));
        }

        /// <summary>
        /// Checks the check digits of an Italian VAT number.
        /// </summary>
        /// <param name="cif">The cif.</param>
        /// <returns></returns>
        private static bool ValidateCif_IT(String cif)
        {
            // The last three digits are the issuing office, and cannot exceed more 201
            if (int.Parse(cif.Substring(0, 7)) == 0) return false;

            var temp = int.Parse(cif.Substring(7, 3));

            if ((temp < 1) || (temp > 201)) return false;

            // Extract the next digit and multiply by the appropriate  
            var i = 0;
            var total = 0;
            foreach (var m in new[] { 1, 2, 1, 2, 1, 2, 1, 2, 1, 2 })
            {
                temp = GetInt(cif[i++]) * m;
                total += temp > 9 ? (int)Math.Floor(temp / 10D) + temp % 10 : temp;
            }

            // Establish check digit.
            total = 10 - total % 10;
            if (total > 9) { total = 0; }

            // Compare it with the last character of the VAT number. If it is the same, 
            // then it's a valid check digit.
            return total == GetInt(cif[10]);
        }

        /// <summary>
        /// Checks the check digits of a Lithuanian VAT number.
        /// </summary>
        /// <param name="cif">The cif.</param>
        /// <returns></returns>
        private static bool ValidateCif_LT(String cif)
        {
            // Only do check digit validation for standard VAT numbers
            if (cif.Length != 9) return true;

            // Extract the next digit and multiply by the counter+1.
            var total = 0;
            for (var i = 0; i < 8; i++) total += GetInt(cif[i]) * (i + 1);

            // Can have a double check digit calculation!
            if (total % 11 == 10)
            {
                var i = 0;
                total = new[] { 3, 4, 5, 6, 7, 8, 9, 1 }.Sum(m => GetInt(cif[i++]) * m);
            }

            // Establish check digit.
            total = total % 11;
            if (total == 10) { total = 0; }

            // Compare it with the last character of the VAT number. If it is the same, 
            // then it's a valid check digit.
            return total == GetInt(cif[8]);
        }

        /// <summary>
        /// Checks the check digits of a Luxembourg VAT number.
        /// </summary>
        /// <param name="cif">The cif.</param>
        /// <returns></returns>
        private static bool ValidateCif_LU(String cif)
        {
            return int.Parse(cif.Substring(0, 6)) % 89 == int.Parse(cif.Substring(6, 2));
        }

        /// <summary>
        /// Checks the check digits of a Latvian VAT number.
        /// </summary>
        /// <param name="cif">The cif.</param>
        /// <returns></returns>
        private static bool ValidateCif_LV(String cif)
        {
            // Only check the legal bodies
            if (Regex.IsMatch(cif, "/^[0-3]/")) return true;

            var i = 0;
            var total = new[] { 9, 1, 4, 8, 3, 10, 2, 5, 7, 6 }.Sum(m => GetInt(cif[i++]) * m);

            // Establish check digits by getting modulus 11.
            if (total % 11 == 4 && cif[0] == '9') total = total - 45;
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
            return total == GetInt(cif[10]);
        }

        /// <summary>
        /// Checks the check digits of a Maltese VAT number.
        /// </summary>
        /// <param name="cif">The cif.</param>
        /// <returns></returns>
        private static bool ValidateCif_MT(String cif)
        {
            var i = 0;
            var total = new[] { 3, 4, 6, 7, 8, 9 }.Sum(m => GetInt(cif[i++]) * m);

            // Establish check digits by getting modulus 37.
            total = 37 - total % 37;

            // Compare it with the last character of the VAT number. If it is the same, 
            // then it's a valid check digit.
            return total == int.Parse(cif.Substring(6, 2));
        }

        /// <summary>
        /// Checks the check digits of a Dutch VAT number.
        /// </summary>
        /// <param name="cif">The cif.</param>
        /// <returns></returns>
        private static bool ValidateCif_NL(String cif)
        {
            var i = 0;
            var total = new[] { 9, 8, 7, 6, 5, 4, 3, 2 }.Sum(m => GetInt(cif[i++]) * m);

            // Establish check digits by getting modulus 11.
            total = total % 11;
            if (total > 9) { total = 0; }

            // Compare it with the last character of the VAT number. If it is the same, 
            // then it's a valid check digit.
            return total == GetInt(cif[8]);
        }

        /// <summary>
        /// Checks the check digits of a Polish VAT number.
        /// </summary>
        /// <param name="cif">The cif.</param>
        /// <returns></returns>
        private static bool ValidateCif_PL(String cif)
        {
            var i = 0;
            var total = new[] { 6, 5, 7, 2, 3, 4, 5, 6, 7 }.Sum(m => GetInt(cif[i++]) * m);

            // Establish check digits subtracting modulus 11 from 11.
            total = total % 11;
            if (total > 9) { total = 0; }

            // Compare it with the last character of the VAT number. If it is the same, then it's a valid 
            // check digit.
            return total == GetInt(cif[9]);
        }

        /// <summary>
        /// Checks the check digits of a Portugese VAT number.
        /// </summary>
        /// <param name="cif">The cif.</param>
        /// <returns></returns>
        private static bool ValidateCif_PT(String cif)
        {
            var i = 0;
            var total = new[] { 9, 8, 7, 6, 5, 4, 3, 2 }.Sum(m => GetInt(cif[i++]) * m);

            // Establish check digits subtracting modulus 11 from 11.
            total = 11 - total % 11;
            if (total > 9) { total = 0; }

            // Compare it with the last character of the VAT number. If it is the same, then it's a valid 
            // check digit.
            return total == GetInt(cif[8]);
        }

        /// <summary>
        /// Checks the check digits of a Swedish VAT number.
        /// </summary>
        /// <param name="cif">The cif.</param>
        /// <returns></returns>
        private static bool ValidateCif_SE(String cif)
        {
            var i = 0;
            var total = new[] { 2, 1, 2, 1, 2, 1, 2, 1, 2 }.Select(m => GetInt(cif[i++]) * m).Select(temp => temp > 9 ? (int)Math.Floor(temp / 10D) + temp % 10 : temp).Sum();

            // Establish check digits by subtracting mod 10 of total from 10.
            total = 10 - (total % 10);
            if (total == 10) total = 0;

            // Compare it with the 10th character of the VAT number. If it is the same, 
            // then it's a valid check digit.
            return total == GetInt(cif[9]);
        }

        /// <summary>
        /// Checks the check digits of a Slovak VAT number.
        /// </summary>
        /// <param name="cif">The cif.</param>
        /// <returns></returns>
        private static bool ValidateCif_SK(String cif)
        {
            var i = 3;
            var total = new[] { 8, 7, 6, 5, 4, 3, 2 }.Sum(m => GetInt(cif[i++]) * m);

            // Establish check digits by getting modulus 11.
            total = 11 - total % 11;
            if (total > 9) total = total - 10;

            // Compare it with the last character of the VAT number. If it is the same, 
            // then it's a valid check digit.
            return total == GetInt(cif[9]);
        }

        /// <summary>
        /// Checks the check digits of a Slovenian VAT number.
        /// </summary>
        /// <param name="cif">The cif.</param>
        /// <returns></returns>
        private static bool ValidateCif_SI(String cif)
        {
            var i = 0;
            var total = new[] { 8, 7, 6, 5, 4, 3, 2 }.Sum(m => GetInt(cif[i++]) * m);

            // Establish check digits by subtracting 97 from total until negative.
            total = 11 - total % 11;
            if (total > 9) { total = 0; }

            // Compare the number with the last character of the VAT number. If it is the 
            // same, then it's a valid check digit.
            return total == GetInt(cif[7]);
        }

        /// <summary>
        /// Checks the check digits of a UK VAT number.
        /// </summary>
        /// <param name="cif">The cif.</param>
        /// <returns></returns>
        private static bool ValidateCif_UK(String cif)
        {
            return true;
        }

        private static int GetInt(char c)
        {
            return (Convert.ToInt32(c) - Convert.ToInt32('0'));
        }
    }
}
