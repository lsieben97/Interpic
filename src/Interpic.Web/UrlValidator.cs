﻿using Interpic.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Interpic.Settings
{
    public class UrlValidator : ISettingValidator<string>
    {
        private static readonly string REGEX = @"(https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|www\.[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9]\.[^\s]{2,}|www\.[a-zA-Z0-9]\.[^\s]{2,})";
        public SettingValidationResult Validate(string value)
        {
            SettingValidationResult result = new SettingValidationResult();
            result.IsValid = Regex.IsMatch(value, REGEX);
            if (!result.IsValid)
            {
                result.ErrorMessage = "Not a valid URL";
            }
            return result;
        }
    }
}
