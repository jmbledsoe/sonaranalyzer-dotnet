/*
 * SonarLint for Visual Studio
 * Copyright (C) 2015 SonarSource
 * sonarqube@googlegroups.com
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02
 */

using System;

namespace SonarLint.Common
{
    public sealed class AnalyzerLanguage
    {
        private const string CsLiteral = "cs";
        private const string VbNetLiteral = "vbnet";

        public static readonly AnalyzerLanguage None = new AnalyzerLanguage("none");
        public static readonly AnalyzerLanguage CSharp = new AnalyzerLanguage(CsLiteral);
        public static readonly AnalyzerLanguage VisualBasic = new AnalyzerLanguage(VbNetLiteral);
        public static readonly AnalyzerLanguage Both = new AnalyzerLanguage("both");
        private readonly string language;

        private AnalyzerLanguage(string language)
        {
            this.language = language;
        }

        public override string ToString()
        {
            return language;
        }

        public AnalyzerLanguage AddLanguage(AnalyzerLanguage language)
        {
            if (language == null)
            {
                throw new ArgumentNullException(nameof(language));
            }

            if (this == None ||
                this == language)
            {
                return language;
            }

            return Both;
        }

        public bool IsAlso(AnalyzerLanguage language)
        {
            if (language == null)
            {
                throw new ArgumentNullException(nameof(language));
            }

            if (language == None)
            {
                throw new NotSupportedException("IsAlso doesn't support AnalyzerLanguage.None.");
            }

            return this == language || this == Both;
        }

        public static AnalyzerLanguage Parse(string s)
        {
            if (s == CsLiteral)
            {
                return CSharp;
            }
            if (s == VbNetLiteral)
            {
                return VisualBasic;
            }

            throw new NotSupportedException(string.Format("Supplied language needs to be '{0}' or '{1}', but found: '{2}'.", CsLiteral, VbNetLiteral, s));
        }

        public string GetQualityProfileRepositoryKey()
        {
            if (this == CSharp)
            {
                return "csharpsquid";
            }

            if (this == VisualBasic)
            {
                return "vbnet";
            }

            throw new NotSupportedException(string.Format("Quality profile can only be queried for a single language. But was called on '{0}'.", ToString()));
        }

        public string GetDirectoryName()
        {
            if (this == CSharp)
            {
                return "CSharp";
            }

            if (this == VisualBasic)
            {
                return "VisualBasic";
            }

            throw new NotSupportedException(string.Format("Can't get folder name for '{0}'.", ToString()));
        }
    }
}