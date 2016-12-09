/*
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

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using SonarAnalyzer.Common;
using SonarAnalyzer.Rules.Common;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SonarAnalyzer.Helpers;
using System;

namespace SonarAnalyzer.Rules.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [Rule(DiagnosticId)]
    public class OptionalParameter : OptionalParameterBase<SyntaxKind, BaseMethodDeclarationSyntax, ParameterSyntax>
    {        
        protected static readonly DiagnosticDescriptor rule =
            DiagnosticDescriptorBuilder.GetDescriptor(DiagnosticId, MessageFormat, RspecStrings.ResourceManager);

        protected override DiagnosticDescriptor Rule => rule;

        private static readonly ImmutableArray<SyntaxKind> kindsOfInterest = ImmutableArray.Create(SyntaxKind.MethodDeclaration, SyntaxKind.ConstructorDeclaration);
        public override ImmutableArray<SyntaxKind> SyntaxKindsOfInterest => kindsOfInterest;

        protected override IEnumerable<ParameterSyntax> GetParameters(BaseMethodDeclarationSyntax method) =>
            method.ParameterList?.Parameters ?? Enumerable.Empty<ParameterSyntax>();

        protected override bool IsOptional(ParameterSyntax parameter, SemanticModel semanticModel)
        {
            var parameterTypeSymbol = semanticModel.GetSymbolInfo(parameter.Type).Symbol as ITypeSymbol;
            
            var literalValueSyntax = parameter.Default?.Value as LiteralExpressionSyntax;
            if (literalValueSyntax != null)
            {
                var literalValue = literalValueSyntax.Token.Value;

                // Always pass optional values set to null (which is always the default for reference types).
                if (literalValue == null)
                {
                    return false; 
                }

                // Always fail if the parameter and default value have different types.
                var literalValueTypeSymbol = semanticModel.Compilation.GetTypeByMetadataName(literalValue.GetType().FullName);
                if (!literalValueTypeSymbol.Equals(parameterTypeSymbol))
                {
                    return true;
                }

                var literalValueType = literalValue.GetType();
                if (literalValueType.IsValueType)
                {
                    var literalValueTypeDefault = Activator.CreateInstance(literalValue.GetType());

                    // Only fail on optional values that are not the default value for the type.
                    return !object.Equals(literalValue, literalValueTypeDefault);
                }
                else
                {
                    // Fail because the parameter value is non-null, but the type has a null default.
                    return true;
                }
            }

            var defaultValueSyntax = parameter.Default?.Value as DefaultExpressionSyntax;
            if (defaultValueSyntax != null && defaultValueSyntax.Type != null)
            {
                var defaultTypeSymbol = semanticModel.GetSymbolInfo(defaultValueSyntax.Type).Symbol as ITypeSymbol;
                
                // Fail on optional values that don't have the same type as the parameter.
                return !defaultTypeSymbol.Equals(parameterTypeSymbol);
            }

            // Parameter doesn't have a literal default value, so use the base implementation.
            return parameter.Default != null && parameter.Default.Value != null;
        }


        protected override Location GetReportLocation(ParameterSyntax parameter) =>
            parameter.Default.GetLocation();

        protected sealed override GeneratedCodeRecognizer GeneratedCodeRecognizer => Helpers.CSharp.GeneratedCodeRecognizer.Instance;
    }
}
