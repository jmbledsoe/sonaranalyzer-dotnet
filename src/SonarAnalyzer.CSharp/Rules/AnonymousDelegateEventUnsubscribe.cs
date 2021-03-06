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
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using SonarAnalyzer.Common;
using SonarAnalyzer.Helpers;

namespace SonarAnalyzer.Rules.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [Rule(DiagnosticId)]
    public class AnonymousDelegateEventUnsubscribe : SonarDiagnosticAnalyzer
    {
        internal const string DiagnosticId = "S3244";
        internal const string MessageFormat = "Unsubscribe with the same delegate that was used for the subscription.";

        private static readonly DiagnosticDescriptor rule =
            DiagnosticDescriptorBuilder.GetDescriptor(DiagnosticId, MessageFormat, RspecStrings.ResourceManager);

        protected sealed override DiagnosticDescriptor Rule => rule;

        protected override void Initialize(SonarAnalysisContext context)
        {
            context.RegisterSyntaxNodeActionInNonGenerated(
                c =>
                {
                    var assignment = (AssignmentExpressionSyntax)c.Node;

                    var @event = c.SemanticModel.GetSymbolInfo(assignment.Left).Symbol as IEventSymbol;

                    if (@event != null &&
                        assignment.Right is AnonymousFunctionExpressionSyntax)
                    {
                        var location = Location.Create(c.Node.SyntaxTree,
                            new TextSpan(assignment.OperatorToken.SpanStart, assignment.Span.End - assignment.OperatorToken.SpanStart));

                        c.ReportDiagnostic(Diagnostic.Create(Rule, location));
                    }
                },
                SyntaxKind.SubtractAssignmentExpression);
        }
    }
}
