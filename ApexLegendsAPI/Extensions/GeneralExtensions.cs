﻿using System;
using System.Linq;
using System.Collections.Generic;

namespace ApexLegendsAPI.Extensions
{
    internal static class GeneralExtensions
    {
        internal static string RandomString(int length)
        {
            return new string(RandomChars(length).ToArray());
        }

        internal static IEnumerable<char> RandomChars(int amount)
        {
            var rng = new Random();
            var allowedChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            for (int i = 0; i < amount; i++)
            {
                yield return allowedChars[rng.Next(allowedChars.Length)];
            }
        }
    }
}
