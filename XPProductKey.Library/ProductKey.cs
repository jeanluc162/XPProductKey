using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.Win32;

namespace XPProductKey.Library
{
    public static class ProductKey
    {
        private const Int32 _DecodeLength = 29;
        private const Int32 _DecodeStringLength = 15;
        private const Int32 _KeyStartIndex = 52;
        private const Int32 _KeyEndIndex = _KeyStartIndex + _DecodeStringLength;
        private static readonly Char[] digits = new Char[] {'B', 'C', 'D', 'F', 'G', 'H', 'J', 'K', 'M', 'P', 'Q', 'R', 'T', 'V', 'W', 'X', 'Y', '2', '3', '4', '6', '7', '8', '9'};
        
        public static Boolean Write(String ProductKey)
        {
            ProductKey = ProductKey.ToUpper();

            RegistryKey registry = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", true);
            if (registry == null) return false;
            
            byte[] digitalProductId = registry.GetValue("DigitalProductId") as byte[];
            byte[] LicKey = Encode(ProductKey);

            for (int i = _KeyStartIndex; i < _KeyEndIndex; i++)
            {
                digitalProductId[i] = LicKey[i-_KeyStartIndex];
            }

            try
            {
                registry.SetValue("DigitalProductId", digitalProductId, RegistryValueKind.Binary);
            }
            catch (Exception ex)
            {
                registry.Close();
                return false;
            }
            registry.Close();
            return true;
        }
        public static String Read()
        {
            byte[] digitalProductId = GetRegistryDigitalProductId();

            ArrayList hexPid = new ArrayList();
            for (int i = _KeyStartIndex; i < _KeyEndIndex; i++)
            {
                hexPid.Add(digitalProductId[i]);
            }
            return Decode(hexPid);
        }
        private static byte[] GetRegistryDigitalProductId()
        {
            byte[] digitalProductId = null;
            RegistryKey registry = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", false);
            if (registry != null)
            {
                digitalProductId = registry.GetValue("DigitalProductId") as byte[];
                registry.Close();
            }
            return digitalProductId;
        }
        private static Byte[] Encode(String ToEncode)
        {
            ToEncode = ToEncode.Replace("-", "").ToUpper();
            byte[] hexPid = new byte[15];
            for (int i = 0; i < 25; i++)
            {
                int LookupTableIndex = Array.IndexOf(digits,ToEncode[i]);
                for (int j = 0; j < hexPid.Length; j++)
                {
                    int byteValue = (hexPid[j] * 24) + LookupTableIndex;
                    hexPid[j] = (byte)(byteValue);
                    LookupTableIndex = (byteValue >> 8);
                }
            }
            return hexPid;
        }
        private static String Decode(ArrayList ToDecode)
        {
            char[] decodedChars = new char[_DecodeLength];
            for (int i = _DecodeLength - 1; i >= 0; i--)
            {
                if ((i + 1) % 6 == 0)
                {
                    decodedChars[i] = '-';
                }
                else
                {
                    int digitMapIndex = 0;
                    for (int j = _DecodeStringLength - 1; j >= 0; j--)
                    {
                        int byteValue = (digitMapIndex << 8) | (byte)ToDecode[j];
                        ToDecode[j] = (byte)(byteValue / 24);
                        digitMapIndex = byteValue % 24;
                        decodedChars[i] = digits[digitMapIndex];
                    }
                }
            }
            return new string(decodedChars);
        }
    }
}
