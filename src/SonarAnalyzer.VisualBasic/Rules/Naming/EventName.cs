﻿/*
 * SonarAnalyzer for .NET
 * Copyright (C) 2015-2016 SonarSource SA
 * mailto:contact@sonarsource.com
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

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using SonarAnalyzer.Common;
using SonarAnalyzer.Helpers;

namespace SonarAnalyzer.Rules.VisualBasic
{
    [DiagnosticAnalyzer(LanguageNames.VisualBasic)]
    [Rule(DiagnosticId)]
    public class EventName : ParameterLoadingDiagnosticAnalyzer
    {
        internal const string DiagnosticId = "S2348";
        internal const string MessageFormat = "Rename this event to match the regular expression: \"{0}\".";

        private static readonly DiagnosticDescriptor rule =
            DiagnosticDescriptorBuilder.GetDescriptor(DiagnosticId, MessageFormat, RspecStrings.ResourceManager);

        protected sealed override DiagnosticDescriptor Rule => rule;

        [RuleParameter("format", PropertyType.String,
            "Regular expression used to check the event names against.", FieldNameChecker.PascalCasingPattern)]
        public string Pattern { get; set; } = FieldNameChecker.PascalCasingPattern;

        protected override void Initialize(ParameterLoadingAnalysisContext context)
        {
            context.RegisterSyntaxNodeActionInNonGenerated(
                c =>
                {
                    var eventDeclaration = (EventStatementSyntax)c.Node;
                    if (!FieldNameChecker.IsRegexMatch(eventDeclaration.Identifier.ValueText, Pattern))
                    {
                        c.ReportDiagnostic(Diagnostic.Create(Rule, eventDeclaration.Identifier.GetLocation(), Pattern));
                    }
                },
                SyntaxKind.EventStatement);
        }
    }
}
