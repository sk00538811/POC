﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileZipUnzip
{
    public class Generate
    {
        public static string AlphabateNumber(int GroupIdLength = 10, bool IsAllwAlphabet = false)
        {
            string alphabets = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string small_alphabets = "abcdefghijklmnopqrstuvwxyz";
            string numbers = "1234567890";
            string characters = numbers;
            int length = GroupIdLength;
            string otp = string.Empty;

            if (IsAllwAlphabet)
            {
                characters += alphabets + small_alphabets + numbers;
            }

            for (int i = 0; i < length; i++)
            {
                string character = string.Empty;
                do
                {
                    int index = new Random().Next(0, characters.Length);
                    character = characters.ToCharArray()[index].ToString();
                }
                while (otp.IndexOf(character) != -1);

                otp += character;
            }
            return otp;
        }
    }
}
